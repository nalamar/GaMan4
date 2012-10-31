using System;
using System.Collections.Generic;
using System.Text;

namespace Protocol.Packet
{
    /// <summary>
    /// Stores all possible packet types in the protocol.
    /// </summary>
    public enum PacketType
    {
        /// <summary>
        /// Client asked for a partial contact list. 
        /// </summary>
        ContactListRequest = 1,
        /// <summary>
        /// Client asked for a specific contact. 
        /// </summary>
        ContactRequest,
        /// <summary>
        /// Acknowledged client reply for contact list. 
        /// </summary>
        ContactListReply,
        /// <summary>
        /// Acknowledged client reply for specific contact. 
        /// </summary>
        ContactReply,
        /// <summary>
        /// Perform logout.
        /// </summary>
        ClientExit,
        /// <summary>
        /// A simple text message.
        /// </summary>
        Message,
        /// <summary>
        /// Broadcast a message to all clients.
        /// </summary>
        Broadcast,
        /// <summary>
        /// A login message.
        /// </summary>
        ClientLoginInformation,
        /// <summary>
        /// Client gone.
        /// </summary>
        ClientLogOffInformation,
        /// <summary>
        /// Request a full client list.
        /// </summary>
        SendClientList,
        /// <summary>
        /// Routes a message to another address.
        /// </summary>
        Route,
        /// <summary>
        /// A simple ping.
        /// </summary>
        Ping
    }
}
