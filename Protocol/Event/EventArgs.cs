using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using ProtocolLibrary;

namespace ProtocolLibrary.Event
{
    /// <summary>
    /// Called delegate, when a packet was sent successfully.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void PacketSentEventHandler(object sender, EventArgs e);

    /// <summary>
    /// Called, when packet delivery failed.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void PacketSendingFailedEventHandler(object sender, EventArgs e);

    /// <summary>
    /// Called, when connection was successfully established.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void ConnectingSuccessEventHandler(object sender, EventArgs e);

    /// <summary>
    /// Called, when connection attempt failed.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void ConnectingFailedEventHandler(object sender, EventArgs e);

    /// <summary>
    /// Called on a network failure.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void NetworkDeadEventHandler(object sender, EventArgs e);

    /// <summary>
    /// Called, when network is alive.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void NetworkAliveEventHandler(object sender, EventArgs e);
}
