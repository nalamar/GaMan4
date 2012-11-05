using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ProtocolLibrary;
using ProtocolLibrary.Packet;
using ProtocolLibrary.Event;
using ProtocolLibrary.Message;
using System.Net.Sockets;

namespace GaMan4Client
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            IPAddress serverAddr = null;
            int serverPort = -1;            
            serverPort = Convert.ToInt16(4850);
            serverAddr = IPAddress.Parse("::1");
            
            _client = new Client(new IPEndPoint(serverAddr, serverPort)); // New client
            _client.ConnectingSuccessEvent += new ConnectingSuccessEventHandler(ConnectingSucceeded);
            _client.ConnectingFailedEvent += new ConnectingFailedEventHandler(ConnectingFailed);
            _client.ServerDisconnectedEvent += new ServerDisconnectedEventHandler(ServerDisconnected);
            _client.ConnectToServer();            
        }

        private Client _client;

        private void ConnectingFailed(object sender, EventArgs e)
        {
            MessageBox.Show("Connection failed!");
        }


        private void ServerDisconnected(object sender, EventArgs e)
        {
            MessageBox.Show("Lost connection to the server!");

            if (_client != null)
            {
                bool b = _client.Disconnect();
                //buttonConnect.Enabled = b;
                //buttonGetData.Enabled = buttonDisconnect.Enabled = !b;
            }
        }

        private void ConnectingSucceeded(object sender, EventArgs e)
        {
            if (_client.Connected)
            {
                //buttonConnect.Enabled = false;
                //buttonGetData.Enabled = buttonDisconnect.Enabled = true;
                //_client.ClientInfo.Name = textBoxName.Text;
                // Send login info packet
                IPacket packet = Protocol.CreatePacket(SocketType.Stream);
                packet.Type = PacketType.ClientLoginInformation;
                packet.Source = _client.LocalIPEndPoint;
                packet.Destination = _client.RemoteIPEndPoint;
                packet.Data = MessageSerializer.Serialize(_client.ClientInfo);
                _client.SendPacket(packet);
                MessageBox.Show("Connection established!");
            }
        }
    }
}
