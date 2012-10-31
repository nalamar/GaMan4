using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace Protocol.Event
{
    /// <summary>
    /// All server events.
    /// </summary>
    public class ServerEventArgs : EventArgs
    {
        /// <summary>
        /// A server event.
        /// </summary>
        /// <param name="clientSocket">The client socket</param>
        public ServerEventArgs(Socket clientSocket)
        {
            EndPoint = (IPEndPoint)clientSocket.RemoteEndPoint;
        }

        /// <summary>
        /// The endpoint of the server.
        /// </summary>
        public IPEndPoint EndPoint { get; private set; }
    }

    /// <summary>
    /// Called on disconnection from the server.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void ServerDisconnectedEventHandler(object sender, ServerEventArgs e);
}