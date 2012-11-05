using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using ProtocolLibrary.Packet;
using ProtocolLibrary.Attributes;

namespace ProtocolLibrary
{
    [Author("Jan Rothe", "info@nalamar.de", "GaMan4", "http://www.nalamar.de/")]
    public class Protocol
    {
        public static IPacket CreatePacket(SocketType type)
        {
            switch (type)
            {
                case SocketType.Stream:
                    return new StreamPacket();
                case SocketType.Dgram:
                    return null;    // Not yet implemented
                default:
                    return null;
            }
        }
    }
}
