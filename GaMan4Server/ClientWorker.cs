using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.ComponentModel;
using System.IO;
using ProtocolLibrary;
using ProtocolLibrary.Message;
using System.Diagnostics;
using ProtocolLibrary.Event;
using ProtocolLibrary.Packet;

namespace GaMan4Server
{
    /// <summary>
    /// A ClientWorker continuously receives packets and sends
    /// new packets to the recipients. This is done asynchronously
    /// with two seperate threads (BackgroundWorker).
    /// </summary>
    public class ClientWorker
    {
        /// <summary>
        /// Constructs a new client.
        /// </summary>
        /// <param name="clientSocket"></param>
        public ClientWorker(Socket clientSocket)
        {
            _socket = clientSocket;
            LocalIPEndPoint = (IPEndPoint)clientSocket.RemoteEndPoint;
            ClientInfo = new ClientInformation(LocalIPEndPoint);
            _bwReceiver = new BackgroundWorker();
            _bwReceiver.DoWork += new DoWorkEventHandler(ReceivePacketFromClient);
            _bwReceiver.RunWorkerAsync();
        }

        /// <summary>
        /// Receives a packet from a client.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReceivePacketFromClient(object sender, DoWorkEventArgs e)
        {
            bool blockingState = _socket.Blocking;

            try
            {
                while (true)
                {
                    IPacket packet = Protocol.CreatePacket(SocketType.Stream);

                    packet.Deserialize(new NetworkStream(_socket));

                    if (packet.Corrupted)
                    {
                        // If the packet is corrupted, check if we still are connected,
                        // make a nonblocking, zero-byte Send call. If we aren't 
                        // connected, it will throw a Exception.
                        _socket.Blocking = false;
                        _socket.Send(new byte[1], 0, 0);
                    }
                    else
                    {
                        // Packet successfully received
                        OnPacketReceived(new PacketEventArgs(packet));
                    }
                }
            }
            catch (SocketException ex)
            {
                Debug.WriteLine(ex.Message);
            }
            catch (ArgumentException ex)
            {
                Debug.WriteLine(ex.Message);
            }
            catch (IOException ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                _socket.Blocking = blockingState;
            }

            // Socket was closed
            OnClientDisconnected(new ClientEventArgs(_socket));
            Disconnect();
        }

        /// <summary>
        /// Sends asynchronously a packet to a host.
        /// </summary>
        /// <param name="packet">The packet to send</param>
        public void SendMessage(IPacket packet)
        {
            if (_socket != null && _socket.Connected && !packet.Corrupted)
            {
                BackgroundWorker bwSender = new BackgroundWorker();
                bwSender.DoWork += new DoWorkEventHandler(BWSender_DoWork);
                bwSender.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BWSender_RunWorkerCompleted);
                bwSender.RunWorkerAsync(packet);
            }
            else
            {
                OnPacketFailed(new EventArgs());
            }
        }

        /// <summary>
        /// Does the work of the sender.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BWSender_DoWork(object sender, DoWorkEventArgs e)
        {
            IPacket packet = (IPacket)e.Argument;
            e.Result = SendPacketToClient(packet);
        }

        /// <summary>
        /// Sends a packet to a client.
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        private bool SendPacketToClient(IPacket packet)
        {
            try
            {
                // Allow only one thread to operate on the socket
                _mutex.WaitOne();
                NetworkStream networkStream = new NetworkStream(_socket);
                byte[] packed = packet.Serialize();
                networkStream.Write(packed, 0, packed.Length);
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// Called, when the BackgroundWorker for sending packets completes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BWSender_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled && e.Error == null && ((bool)e.Result))
            {
                OnPacketSent(new EventArgs());  // Trigger the sent event handler.
            }
            else
            {
                OnPacketFailed(new EventArgs());
            }

            ((BackgroundWorker)sender).Dispose();
            GC.Collect();
        }

        /// <summary>
        /// Disconnects the client.
        /// </summary>
        /// <returns></returns>
        public bool Disconnect()
        {
            if (_socket != null)
            {
                try
                {
                    _socket.Shutdown(SocketShutdown.Both);
                    _socket.Close();
                    return true;
                }
                catch (ObjectDisposedException)
                {
                    return true;    // Offline
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// This method triggers an event, when we receive a new packet.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnPacketReceived(PacketEventArgs e)
        {
            // Invoke the event; called whenever we receive a packet
            if (PacketReceivedEvent != null)
                PacketReceivedEvent(this, e);
        }

        /// <summary>
        /// This method triggers an event, when we sent a packet.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnPacketSent(EventArgs e)
        {
            if (PacketSentEvent != null)
                PacketSentEvent(this, e);
        }

        /// <summary>
        /// This method triggers an event, when we receive a corrupt packet.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnPacketFailed(EventArgs e)
        {
            if (PacketSendingFailedEvent != null)
                PacketSendingFailedEvent(this, e);
        }

        /// <summary>
        /// This method triggers an event, when a client gets disconnected.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnClientDisconnected(ClientEventArgs e)
        {
            if (ClientDisconnectedEvent != null)
                ClientDisconnectedEvent(this, e);
        }

        /// <summary>
        /// Get the ip address of this client.
        /// </summary>
        public IPAddress Address
        {
            get
            {
                if (_socket != null)
                    return ((IPEndPoint)_socket.RemoteEndPoint).Address;
                else
                    return IPAddress.None;
            }
        }

        /// <summary>
        /// Get the port number of this client. 
        /// </summary>
        public int Port
        {
            get
            {
                if (_socket != null)
                    return ((IPEndPoint)_socket.RemoteEndPoint).Port;
                else
                    return -1;
            }
        }

        /// <summary>
        /// Check if we are connected to this client.
        /// </summary>
        public bool Connected
        {
            get
            {
                if (_socket != null)
                    return _socket.Connected;
                else
                    return false;
            }
        }

        /// <summary>
        /// Get the client information.
        /// </summary>
        public ClientInformation ClientInfo { get; set; }

        /// <summary>
        /// Sets or gets the local endpoint.
        /// </summary>
        public IPEndPoint LocalIPEndPoint { get; set; }

        /// <summary>
        /// A background worker, which handles received packets.
        /// </summary>
        private BackgroundWorker _bwReceiver;

        /// <summary>
        /// The client socket.
        /// </summary>
        private Socket _socket;

        /// <summary>
        /// Limits the number of threads that can access a resource or pool of resources concurrently. 
        /// </summary>
        private static Mutex _mutex = new Mutex();

        /// <summary>
        /// Called, when a packet was retrieved.
        /// </summary>
        public event PacketReceivedEventHandler PacketReceivedEvent;

        /// <summary>
        /// Called delegate, when a packet was sent successfully.
        /// </summary>
        public event PacketSentEventHandler PacketSentEvent;

        /// <summary>
        /// Called, when packet delivery failed.
        /// </summary>
        public event PacketSendingFailedEventHandler PacketSendingFailedEvent;

        /// <summary>
        /// Called on disconnection.
        /// </summary>
        public event ClientDisconnectedEventHandler ClientDisconnectedEvent;
    }
}