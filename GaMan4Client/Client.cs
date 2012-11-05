using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.NetworkInformation;
using System.ComponentModel;
using System.Net.Sockets;
using System.IO;
using ProtocolLibrary;
using ProtocolLibrary.Message;
using System.Diagnostics;
using ProtocolLibrary.Event;
using ProtocolLibrary.Packet;
using System.Windows.Forms;


namespace GaMan4Client
{
    /// <summary>
    /// Represents a single client.
    /// </summary>
    public class Client
    {
        /// <summary>
        /// Creates a new client with a name and connects to the specified
        /// server endpoint.
        /// </summary>
        /// <param name="serverIPEndPoint"></param>
        public Client(IPEndPoint serverIPEndPoint)
        {
            RemoteIPEndPoint = serverIPEndPoint;
            NetworkChange.NetworkAvailabilityChanged += new NetworkAvailabilityChangedEventHandler(NetworkChange_NetworkAvailabilityChanged);
        }

        /// <summary>
        /// Connect to a server.
        /// </summary>
        public void ConnectToServer()
        {
            BackgroundWorker bwConn = new BackgroundWorker();
            bwConn.DoWork += new DoWorkEventHandler(BWConnect_DoWork);
            bwConn.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BWConnect_RunWorkerCompleted);
            bwConn.RunWorkerAsync();
        }

        /// <summary>
        /// Performs the client work.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BWConnect_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                _socket = new Socket(RemoteIPEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                _socket.Connect(RemoteIPEndPoint);
                LocalIPEndPoint = (IPEndPoint)_socket.LocalEndPoint;
                ClientInfo = new ClientInformation(LocalIPEndPoint);
                e.Result = true;
                _bwReceiver = new BackgroundWorker();
                _bwReceiver.WorkerSupportsCancellation = true;
                _bwReceiver.DoWork += new DoWorkEventHandler(ReceivePacketFromServer);
                _bwReceiver.RunWorkerAsync();
                // Client is online...
            }
            catch
            {
                e.Result = false;
            }
        }

        /// <summary>
        /// Called, when the connection worker completes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BWConnect_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!((bool)e.Result))
            {
                OnConnectingFailed(new EventArgs());
            }
            else
            {
                OnConnectingSuccessed(new EventArgs());
            }

            ((BackgroundWorker)sender).Dispose();
        }

        /// <summary>
        /// Sends a packet to the server.
        /// </summary>
        /// <param name="packet"></param>
        public void SendPacket(IPacket packet)
        {
            if (_socket != null && _socket.Connected && !packet.Corrupted)
            {
                BackgroundWorker bwSender = new BackgroundWorker();
                bwSender.DoWork += new DoWorkEventHandler(BWSender_DoWork);
                bwSender.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BWSender_RunWorkerCompleted);
                bwSender.WorkerSupportsCancellation = true;
                bwSender.RunWorkerAsync(packet);
            }
            else
            {
                OnPacketFailed(new EventArgs());
            }
        }

        /// <summary>
        /// Disconnects the client from the server.
        /// </summary>
        /// <returns></returns>
        public bool Disconnect()
        {
            if (_socket != null)
            {
                try
                {
                    OnClientDisconnected(new ClientEventArgs(_socket));
                    _socket.Shutdown(SocketShutdown.Both);
                    _socket.Close();
                    _bwReceiver.CancelAsync();
                    return true;
                }
                catch (ObjectDisposedException)
                {
                    return true;    // Server went offline
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
        /// Called, when the network state changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NetworkChange_NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            if (!e.IsAvailable)
            {
                OnNetworkDead(new EventArgs());
                OnClientDisconnected(new ClientEventArgs(_socket));
            }
            else
            {
                OnNetworkAlived(new EventArgs());
            }
        }

        /// <summary>
        /// This method triggers an event, when we receive a new packet. On
        /// Windows Forms we maybe have to invoke.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnPacketReceived(PacketEventArgs e)
        {
            if (PacketReceivedEvent != null)
            {
                Control target = PacketReceivedEvent.Target as Control;
                if (target != null && target.InvokeRequired)
                {
                    target.Invoke(PacketReceivedEvent, new object[] { this, e });
                
                }
                else
                {
                    PacketReceivedEvent(this, e);
                }
            }
        }

        /// <summary>
        /// This method triggers an event, when we sent a packet. On
        /// Windows Forms we maybe have to invoke.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnPacketSent(EventArgs e)
        {
            if (PacketSentEvent != null)
            {
                Control target = PacketSentEvent.Target as Control;
                if (target != null && target.InvokeRequired)
                {
                    target.Invoke(PacketSentEvent, new object[] { this, e });
                }
                else
                {
                    PacketSentEvent(this, e);
                }
            }
        }

        /// <summary>
        /// This method triggers an event, when we receive a corrupt packet. On
        /// Windows Forms we maybe have to invoke.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnPacketFailed(EventArgs e)
        {
            if (MessageSendingFailedEvent != null)
            {
                Control target = MessageSendingFailedEvent.Target as Control;
                if (target != null && target.InvokeRequired)
                {
                    target.Invoke(MessageSendingFailedEvent, new object[] { this, e });
                }
                else
                {
                    MessageSendingFailedEvent(this, e);
                }
            }
        }

        /// <summary>
        /// This method triggers an event, when the server disconnects. On
        /// Windows Forms we maybe have to invoke.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnServerDisconnected(ServerEventArgs e)
        {
            if (ServerDisconnectedEvent != null)
            {
                Control target = ServerDisconnectedEvent.Target as Control;
                if (target != null && target.InvokeRequired)
                {
                    target.Invoke(ServerDisconnectedEvent, new object[] { this, e });
                }
                else
                {
                    ServerDisconnectedEvent(this, e);
                }
            }
        }

        /// <summary>
        /// This method triggers an event, when a client disconnects. On
        /// Windows Forms we maybe have to invoke.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnClientDisconnected(ClientEventArgs e)
        {
            if (ClientDisconnectedEvent != null)
            {
                Control target = ClientDisconnectedEvent.Target as Control;
                if (target != null && target.InvokeRequired)
                {
                    target.Invoke(ClientDisconnectedEvent, new object[] { this, e });
                }
                else
                {
                    ClientDisconnectedEvent(this, e);
                }
            }
        }

        /// <summary>
        /// This method triggers an event, when a client connects. On
        /// Windows Forms we maybe have to invoke.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnConnectingSuccessed(EventArgs e)
        {
            if (ConnectingSuccessEvent != null)
            {
                Control target = ConnectingSuccessEvent.Target as Control;
                if (target != null && target.InvokeRequired)
                {
                    target.Invoke(ConnectingSuccessEvent, new object[] { this, e });
                }
                else
                {
                    ConnectingSuccessEvent(this, e);
                }
            }
        }

        /// <summary>
        /// This method triggers an event, when a client fails on connection. On
        /// Windows Forms we maybe have to invoke.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnConnectingFailed(EventArgs e)
        {
            if (ConnectingFailedEvent != null)
            {
                Control target = ConnectingFailedEvent.Target as Control;
                if (target != null && target.InvokeRequired)
                {
                    target.Invoke(ConnectingFailedEvent, new object[] { this, e });
                }
                else
                {
                    ConnectingFailedEvent(this, e);
                }
            }
        }

        /// <summary>
        /// This method triggers an event, when the network is dead. On
        /// Windows Forms we maybe have to invoke.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnNetworkDead(EventArgs e)
        {
            if (NetworkDeadEvent != null)
            {
                /*Control target = NetworkDeadEvent.Target as Control;
                if (target != null && target.InvokeRequired)
                {
                    target.Invoke(NetworkDeadEvent, new object[] { this, e });
                }
                else
                {
                    NetworkDeadEvent(this, e);
                }*/
            }
        }

        /// <summary>
        /// This method triggers an event, when the network is alive. On
        /// Windows Forms we maybe have to invoke.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnNetworkAlived(EventArgs e)
        {
            if (NetworkAliveEvent != null)
            {
                /*Control target = NetworkAliveEvent.Target as Control;
                if (target != null && target.InvokeRequired)
                {
                    target.Invoke(NetworkAliveEvent, new object[] { this, e });
                }
                else
                {
                    NetworkAliveEvent(this, e);
                }*/
            }
        }

        /// <summary>
        /// Receives a packet from server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReceivePacketFromServer(object sender, DoWorkEventArgs e)
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
            OnServerDisconnected(new ServerEventArgs(_socket));
            Disconnect();
        }

        /// <summary>
        /// Sends a packet to the server.
        /// </summary>
        /// <param name="msg"></param>
        /// <returns>True, if the packet was successfully sent</returns>
        private bool SendPacketToServer(IPacket packet)
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
        /// Work method of the sender.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BWSender_DoWork(object sender, DoWorkEventArgs e)
        {
            IPacket packet = (IPacket)e.Argument;
            e.Result = SendPacketToServer(packet);
        }

        /// <summary>
        /// Completion routine of the sender.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BWSender_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled && e.Error == null && ((bool)e.Result))
            {
                OnPacketSent(new EventArgs());
            }
            else
            {
                OnPacketFailed(new EventArgs());
            }

            ((BackgroundWorker)sender).Dispose();
            GC.Collect();
        }

        #region Getter/Setter

        /// <summary>
        /// Check if we are connected to the server.
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
        /// Sets or gets the client host info.
        /// </summary>
        public ClientInformation ClientInfo { get; set; }

        /// <summary>
        /// Sets or gets the server endpoint.
        /// </summary>
        public IPEndPoint RemoteIPEndPoint { get; set; }

        /// <summary>
        /// Sets or gets the local endpoint.
        /// </summary>
        public IPEndPoint LocalIPEndPoint { get; set; }

        #endregion

        #region Fields

        /// <summary>
        /// The client socket.
        /// </summary>
        private Socket _socket;

        /// <summary>
        /// A background worker, which handles received packets.
        /// </summary>
        private BackgroundWorker _bwReceiver;

        /// <summary>
        /// Limits the number of threads that can access a resource or pool of resources concurrently. 
        /// </summary>
        private static Mutex _mutex = new Mutex();

        #endregion

        #region Events

        /// <summary>
        /// Called, when a packet was retrieved.
        /// </summary>      
        public event PacketSentEventHandler PacketSentEvent;

        /// <summary>
        /// Called, when message delivery failed.
        /// </summary>   
        public event PacketSendingFailedEventHandler MessageSendingFailedEvent;

        /// <summary>
        /// Called, when a packet was retrieved.
        /// </summary>  
        public event PacketReceivedEventHandler PacketReceivedEvent;

        /// <summary>
        /// Called on a network failure.
        /// </summary>
        public event NetworkDeadEventHandler NetworkDeadEvent;

        /// <summary>
        /// Called, when network is alive.
        /// </summary>   
        public event NetworkAliveEventHandler NetworkAliveEvent;

        /// <summary>
        /// Called, when connection attempt failed.
        /// </summary>  
        public event ConnectingFailedEventHandler ConnectingFailedEvent;

        /// <summary>
        /// Called, when connection was successfully established.
        /// </summary>     
        public event ConnectingSuccessEventHandler ConnectingSuccessEvent;

        /// <summary>
        /// Called on disconnection from server.
        /// </summary>  
        public event ServerDisconnectedEventHandler ServerDisconnectedEvent;

        /// <summary>
        /// Called, when this client disconnects.
        /// </summary>     
        public event ClientDisconnectedEventHandler ClientDisconnectedEvent;

        #endregion
    }
}

