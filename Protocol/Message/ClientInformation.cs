using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Protocol.Message
{
    /// <summary>
    /// A class with client information. This is primarily a data storing
    /// object. It is easily extensible with new fields.
    /// </summary>
    [Serializable]
    public class ClientInformation : IComparable
    {
        /// <summary>
        /// Default-Constructor. Only initializes the client, creating
        /// a unique identifier from the IPEndPoint.
        /// </summary>
        public ClientInformation(IPEndPoint ipEndPoint)
        {
            CalculateIdentifier(ipEndPoint);
        }

        /// <summary>
        /// Internal function to calculate a unique identifier. The id is normally a
        /// combination of the host and the port number, seperated through a "|"
        /// like "host|port". The host is from type IPAddress. Valid
        /// host identifier are i.e. "196.120.16.23|385" in IPv4 
        /// and "2001:0db8:3c4d:0015:0000:0000:abcd:ef12|5390" in IPv6.
        /// </summary>
        /// <param name="ipEndPoint">The ipEndPoint from which the identifier will be calculated</param>
        /// <returns></returns>
        public void CalculateIdentifier(IPEndPoint ipEndPoint)
        {
            if (ipEndPoint != null)
            {
                Identifier = ipEndPoint.Address.ToString() + "|" + ipEndPoint.Port;
            }
            else
            {
                Identifier = "Not initialized!";
            }
        }

        /// <summary>
        /// Compares two client informations.
        /// </summary>
        /// <param name="obj">The object to compare with this</param>
        /// <returns>0 if equal</returns>
        public int CompareTo(object obj)
        {
            ClientInformation otherClientInfo = obj as ClientInformation;

            if (otherClientInfo != null)
            {
                return Identifier.CompareTo(otherClientInfo.Identifier);
            }
            else
            {
                throw new ArgumentException("Object is not a ClientInfo");
            }
        }

        /// <summary>
        /// Overrides ToString method.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.ToString()
                    + "[Identifier=" + Identifier
                    + ", Name=" + Name
                    + "]";
        }

        /// <summary>
        /// Gets the unique network identifier.
        /// </summary>
        public string Identifier { get; private set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }
    }
}
