using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtocolLibrary.Packet;

namespace ProtocolLibrary.Event
{
    /// <summary>
    /// Packet events.
    /// </summary>
    public class PacketEventArgs : EventArgs
    {
        /// <summary>
        /// A packet event.
        /// </summary>
        /// <param name="packet"></param>
        public PacketEventArgs(IPacket packet)
        {
            Packet = packet;
        }

        /// <summary>
        /// Gets or sets the packet.
        /// </summary>
        public IPacket Packet { get; private set; }
    }

    /// <summary>
    /// Called, when a packet was retrieved.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void PacketReceivedEventHandler(object sender, PacketEventArgs e);
}