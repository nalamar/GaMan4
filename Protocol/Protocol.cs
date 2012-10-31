using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using Protocol.Packet;
using Protocol.Attributes;

namespace Protocol
{
    // Protocol based on Codeplanets CLient-Server-Model Tutorial
    [Author("Jan Rothe", "info@nalamar.de", "GaMan Protocol", "http://www.nalamar.de/")]
    class Protocol
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
