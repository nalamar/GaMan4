
using System.Net.Sockets;
using System.Text;
using System.Linq;
using System.Xml.Linq;
using ProtocolLibrary;
using ProtocolLibrary.Packet;


namespace GaMan4Server
{
    public partial class Server
    {
        public void SendContactList(IPacket packet)
        {
            XElement xelement = XElement.Load("xml/demo.xml");            
            foreach (var a in xelement.Elements("Location"))
                LogConsole("Testnamen : " + a.Element("Name").Value, packet.Source);
            //Console.WriteLine("No of Employees living in CA State are {0}", stCnt.Count());
            var stores = xelement.Descendants("Store");
            LogConsole("asked for store list. Sending " + stores.Count() + " store(s).", packet.Source);
            Encoding enc = Encoding.Unicode;
            IPacket replyPacket = Protocol.CreatePacket(SocketType.Stream);
            replyPacket.Type = PacketType.ContactListReply;
            replyPacket.Encoding = enc;
            replyPacket.Source = ServerIPEndPoint;
            replyPacket.Destination = packet.Source;
            replyPacket.Data = SerializeContacts(enc);
            Send(replyPacket);
        }
    }

}