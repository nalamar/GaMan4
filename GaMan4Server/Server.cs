using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Runtime.Serialization;
using System.Diagnostics;
using ProtocolLibrary;
using ProtocolLibrary.Event;
using ProtocolLibrary.Packet;
using ProtocolLibrary.Message;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml.Linq;


namespace GaMan4Server
{
    /// <summary>
    /// The main server application. The server accepts client connections
    /// on a thred-per-client base. It will send data to the clients.
    /// </summary>
    public class Server
    {
        /// <summary>
        /// Init a new server.
        /// </summary>
        /// <param name="ip">The ip address</param>
        /// <param name="port">The port number</param>
        public Server(string ip, int port)
        {
            //_contacts = new Contacts();
            _clients = new List<ClientWorker>();
            ServerIPEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        }

        /// <summary>
        /// Start the server.
        /// </summary>
        /// <returns>True if server was started</returns>
        public bool Start()
        {
            if (CanReadXML())
            {
                // Start worker
                _bwListener = new BackgroundWorker();
                _bwListener.WorkerSupportsCancellation = true;
                _bwListener.DoWork += new DoWorkEventHandler(StartToListen);
                _bwListener.RunWorkerAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Stop server from listening.
        /// </summary>
        public void Stop()
        {
            if (_clients != null)
            {
                foreach (ClientWorker client in _clients)
                {
                    client.Disconnect();
                }

                _bwListener.CancelAsync();
                _bwListener.Dispose();
                _socket.Close();
                GC.Collect();
            }
        }

        /// <summary>
        /// Switches server to listen mode.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartToListen(object sender, DoWorkEventArgs e)
        {
            // IPv4
            _socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
            _socket.Bind(ServerIPEndPoint);
            _socket.Listen(200);

            // Infinite listening
            while (true)
            {
                try
                {
                    CreateNewClient(_socket.Accept());
                }
                catch (SocketException ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                catch (ObjectDisposedException ex)
                {
                    // Socket was closed
                    Debug.WriteLine(ex.Message);
                }
            }
        }

        /// <summary>
        /// Creates a new client.
        /// </summary>
        /// <param name="socket"></param>
        public void CreateNewClient(Socket socket)
        {
            ClientWorker newClient = new ClientWorker(socket);
            newClient.PacketReceivedEvent += new PacketReceivedEventHandler(PacketReceived);
            newClient.ClientDisconnectedEvent += new ClientDisconnectedEventHandler(ClientDisconnected);
            RemoveDuplicates(newClient);
            _clients.Add(newClient);            
            LogConsole("connected.", newClient.LocalIPEndPoint);
            
        }

        /// <summary>
        /// Removes mysterious duplicates in our list.
        /// </summary>
        /// <param name="client"></param>
        public void RemoveDuplicates(ClientWorker client)
        {
            if (RemoveClient(client))
            {
                LogConsole("removed mysterious duplicate.", client.LocalIPEndPoint);
            }
        }

        /// <summary>
        /// Client disconnected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ClientDisconnected(object sender, ClientEventArgs e)
        {
            if (RemoveClient(e.EndPoint))
            {
                LogConsole("disconnected.", e.EndPoint);
            }
        }

        /// <summary>
        /// Remove a client.
        /// </summary>
        /// <param name="ipEndPoint"></param>
        /// <returns></returns>
        public bool RemoveClient(IPEndPoint ipEndPoint)
        {
            return RemoveClient(GetClient(ipEndPoint));
        }

        /// <summary>
        /// Remove a client.
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public bool RemoveClient(ClientWorker client)
        {
            // Collection is not thread-safe. Lock it...
            lock (this)
            {
                return _clients.Remove(client);
            }
        }

        /// <summary>
        /// Get the client object in the list with the specified endpoint.
        /// </summary>
        /// <param name="ipEndPoint"></param>
        /// <returns></returns>
        public ClientWorker GetClient(IPEndPoint ipEndPoint)
        {
            foreach (ClientWorker client in _clients)
            {
                if (client.LocalIPEndPoint.Equals(ipEndPoint))
                    return client;
            }
            return null;
        }

        /// <summary>
        /// Always called, when the server reveives a new packet.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void PacketReceived(object sender, PacketEventArgs e)
        {
            // Switch on message types
            switch (e.Packet.Type)
            {
                case PacketType.ClientLoginInformation:
                    // On login
                    SetClientData(e.Packet);
                    break;
                case PacketType.ContactListRequest:
                    // Send a partial contact list
                    SendContactList(e.Packet);
                    break;
                case PacketType.ContactRequest:
                    // Send detailled contact information
                    SendContact(e.Packet);
                    break;
                case PacketType.Route:
                    // Route a message
                    Send(e.Packet);
                    break;
                case PacketType.Broadcast:
                    // Broadcast to all
                    BroadCast(e.Packet);
                    break;
                default:
                    // Unknown type
                    LogConsole("unknown message type.", e.Packet.Source);
                    break;
            }
        }

        /// <summary>
        /// Sends a partial contact list to the client, who requested 
        /// the list. 
        /// </summary>
        /// <param name="packet"></param>
        public void SendContactList(IPacket packet)
        {
            //LogConsole("asked for contact list. Sending " + _contacts.Tables["Kontakte"].Rows.Count + " contact(s).", packet.Source);
            Encoding enc = Encoding.Unicode;
            IPacket replyPacket = Protocol.CreatePacket(SocketType.Stream);
            replyPacket.Type = PacketType.ContactListReply;
            replyPacket.Encoding = enc;
            replyPacket.Source = ServerIPEndPoint;
            replyPacket.Destination = packet.Source;
            replyPacket.Data = SerializeContacts(enc);
            Send(replyPacket);
        }

        /// <summary>
        /// Send the specific contact.
        /// </summary>
        /// <param name="packet"></param>
        public void SendContact(IPacket packet)
        {
            // Read the requested contact number from the packet
            int requestedContactId = BitConverter.ToInt32(packet.Data, 0);
            LogConsole("asked for contact " + requestedContactId + ".", packet.Source);
            Encoding enc = Encoding.Unicode;
            byte[] bytes = SerializeSingleContact(requestedContactId, enc, packet.Delimiter);

            if (bytes.Length == 0)
            {
                LogConsole("no such id in contact list.", packet.Source);
            }
            else
            {
                IPacket replyPacket = Protocol.CreatePacket(SocketType.Stream);
                replyPacket.Type = PacketType.ContactReply;
                replyPacket.Encoding = enc;
                replyPacket.Source = ServerIPEndPoint;
                replyPacket.Destination = packet.Source;
                replyPacket.Data = bytes;
                Send(replyPacket);
            }
        }

        /// <summary>
        /// Set the client data, like the name, after login.
        /// </summary>
        /// <param name="packet">The packet with the received client data</param>
        public void SetClientData(IPacket packet)
        {
            ClientWorker client = GetClient(packet.Source);

            if (client != null)
            {
                client.ClientInfo = (ClientInformation)MessageSerializer.Deserialize(packet.Data);
                LogConsole("switched name to " + client.ClientInfo.Name + ".", client.LocalIPEndPoint);
            }
        }

        /// <summary>
        /// Broadcast a packet to all clients.
        /// </summary>
        /// <param name="packet">The packet to broadcast</param>
        public void BroadCast(IPacket packet)
        {
            foreach (ClientWorker client in _clients)
                if (!client.LocalIPEndPoint.Equals(packet.Source))
                    client.SendMessage(packet);
        }

        /// <summary>
        /// Send a packet to a client.
        /// </summary>
        /// <param name="packet">The packet to send</param>
        public void Send(IPacket packet)
        {
            foreach (ClientWorker client in _clients)
            {
                if (client.LocalIPEndPoint.Equals(packet.Destination))
                {
                    client.SendMessage(packet);
                    return;
                }
            }
        }


        /// <summary>
        /// Log a mesage to the Textbox via an Eventhandler.
        /// </summary>
        /// <param name="status"></param>
        /// <param name="endPoint"></param>
        public void LogConsole(string status, IPEndPoint endPoint)
        {            
            string text = DateTime.Now.ToString() + ": Client " + endPoint.Address.ToString() + ":" + endPoint.Port + " " + status;           
            LogText log = new LogText();
            log.SetText = text;
            Log(this, log);    
            
        }

        public event LogHandler Log;
        public EventArgs e = null;
        public delegate void LogHandler(Server s, LogText e);        
        
        

        /// <summary>
        /// Serializes the contact list with the specified encoding.
        /// </summary>
        /// <param name="encoding">The encoding</param>
        /// <returns>The serialized partial contact list</returns>
        private byte[] SerializeContacts(Encoding encoding)
        {
            /*DataSet copy = _contacts.Copy();

            foreach (DataRow Row in copy.Tables["Kontakte"].Rows)
            {
                Row["Firma"] = "?";
                Row["Adresse"] = "?";
                Row["Email"] = "?";
                Row["Telefon"] = "?";
                Row["Handy"] = "?";
                Row["Fax"] = "?";
                Row["Anrede"] = "?";
                Row["Titel"] = "?";
                Row["Position"] = "?";
            }

            return encoding.GetBytes(copy.GetXml());
             */
            byte[] tmp = null;
            return tmp;
        }

        /// <summary>
        /// Serializes a single contact with the specified encoding.
        /// </summary>
        /// <param name="id">Contact id, which should be serialized</param>
        /// <param name="encoding">The encoding</param>
        /// <param name="delimiter">A delimiter</param>
        /// <returns>The serialized contact</returns>
        private byte[] SerializeSingleContact(int contactId, Encoding encoding, string delimiter)
        {
            /*string data = "";

            foreach (DataRow Row in _contacts.Tables["Kontakte"].Rows)
            {
                int num = Convert.ToInt32(Row["Id"]);
                if (num == contactId)
                {
                    data = Row["Id"] + delimiter +
                           Row["Name"] + delimiter +
                           Row["Firma"] + delimiter +
                           Row["Adresse"] + delimiter +
                           Row["Email"] + delimiter +
                           Row["Telefon"] + delimiter +
                           Row["Handy"] + delimiter +
                           Row["Fax"] + delimiter +
                           Row["Anrede"] + delimiter +
                           Row["Titel"] + delimiter +
                           Row["Position"];
                }
            }

            return encoding.GetBytes(data);
            */
            byte[] tmp = null;
            return tmp;
        }

        /// <summary>
        /// Creates a demo xml file, if no file exists.
        /// </summary>
         public void CreateDemoXML()
        {
             XNamespace locNM = "urn:lst-loc:loc";
             XDocument xDoc = new XDocument(
                 new XDeclaration("1.0", "UTF-16", null),
                 new XElement(locNM + "Locations",
                     new XElement("Location",
                         new XElement("LocId", "1"),
                         new XElement("Name", "Bunker"),
                         new XElement("Store",
                            new XElement("StoreId", "1"),
                            new XElement("StoreName", "Bar 2"),
                            new XElement("Products",
                                new XElement("PID", "1"),
                                new XElement("PName", "Becks Pils"),
                                new XElement("PVK", "2,50"),
                                new XElement("PVKg", "0,33"),
                                new XElement("PEK", "0,69"),
                                new XElement("PEKg", "0,33"),
                                new XElement("PID", "2"),
                                new XElement("PName", "Becks Gold"),
                                new XElement("PVK", "2,50"),
                                new XElement("PVKg", "0,33"),
                                new XElement("PEK", "0,69"),
                                new XElement("PEKg", "0,33")),
                            new XElement("StoreId", "2"),
                            new XElement("StoreName", "Bar 3"),
                            new XElement("Products",
                                new XElement("PID", "1"),
                                new XElement("PName", "Becks Pils"),
                                new XElement("PVK", "2,50"),
                                new XElement("PVKg", "0,33"),
                                new XElement("PEK", "0,69"),
                                new XElement("PEKg", "0,33"),
                                new XElement("PID", "2"),
                                new XElement("PName", "Becks Gold"),
                                new XElement("PVK", "2,50"),
                                new XElement("PVKg", "0,33"),
                                new XElement("PEK", "0,69"),
                                new XElement("PEKg", "0,33"))),
                         new XElement("LocId", "2"),
                         new XElement("Name", "Reithalle"),
                            new XElement("Store",
                            new XElement("StoreId", "3"),
                            new XElement("StoreName", "Bar 12"),
                            new XElement("Products",
                                new XElement("PID", "3"),
                                new XElement("PName", "Hasseröder Pils"),
                                new XElement("PVK", "3,00"),
                                new XElement("PVKg", "0,4"),
                                new XElement("PEK", "0,71"),
                                new XElement("PEKg", "50,00"),
                                new XElement("PID", "4"),
                                new XElement("PName", "Köstritzer Schwarz"),
                                new XElement("PVK", "3,00"),
                                new XElement("PVKg", "0,4"),
                                new XElement("PEK", "0,71"),
                                new XElement("PEKg", "30,00")),
                            new XElement("StoreId", "4"),
                            new XElement("StoreName", "Bar 13"),
                            new XElement("Products",
                                new XElement("PID", "3"),
                                new XElement("PName", "Hasseröder Pils"),
                                new XElement("PVK", "3,00"),
                                new XElement("PVKg", "0,4"),
                                new XElement("PEK", "0,71"),
                                new XElement("PEKg", "50,00"),
                                new XElement("PID", "4"),
                                new XElement("PName", "Köstritzer Schwarz"),
                                new XElement("PVK", "3,00"),
                                new XElement("PVKg", "0,4"),
                                new XElement("PEK", "0,71"),
                                new XElement("PEKg", "30,00"))))));

             StringWriter sw = new StringWriter();
             XmlWriter xWrite = XmlWriter.Create(sw);
             xDoc.Save(xWrite);
             xWrite.Close();

             xDoc.Save("xml/demo.xml");
                         

        }

        /// <summary>
        /// Reads the local contact list.
        /// </summary>
        /// <returns>True if contact list readable</returns>
        public bool CanReadXML()
        {
            FileStream fsReadXml = null;
            XmlTextReader myXmlReader = null;

            try
            {
                // Create new FileStream to read schema with.
                fsReadXml = new FileStream(Properties.Resources.XMLFileName, FileMode.Open);
                // Create an XmlTextReader to read the file.
                myXmlReader = new XmlTextReader(fsReadXml);
                // Read the XML document into the DataSet.
                //_contacts.ReadXml(myXmlReader, XmlReadMode.IgnoreSchema);
                //PrintValues(contacts_, "New DataSet");
                return true;
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                if (myXmlReader != null)
                {
                    myXmlReader.Close();
                }
            }

            return false;
        }

        /// <summary>
        /// Prints all values from the table to console.
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="label"></param>
        public void PrintValues(string ds, string label)
        {
            /*Console.WriteLine("\n" + label);
            foreach (DataTable t in ds.Tables)
            {
                Console.WriteLine("TableName: " + t.TableName);
                foreach (DataRow r in t.Rows)
                {
                    foreach (DataColumn c in t.Columns)
                    {
                        Console.Write("\t " + r[c]);
                    }
                    Console.WriteLine();
                }
            }*/
        }

        /// <summary>
        /// Sets or gets the endpoint of the server.
        /// </summary>
        public IPEndPoint ServerIPEndPoint { get; set; }

        /// <summary>
        /// A generic list with client workers.
        /// </summary>
        private List<ClientWorker> _clients;

        /// <summary>
        /// Listen to all connections-
        /// </summary>
        private BackgroundWorker _bwListener;

        /// <summary>
        /// The server listening socket.
        /// </summary>
        private Socket _socket;

        /// <summary>
        /// The contact table as a dataset.
        /// </summary>
        //private Contacts _contacts;
    }

    public class LogText : EventArgs
    {
        private string Text;
        public string SetText
        {
            set { Text = value; }
            get { return this.Text; }
        }
    }
}