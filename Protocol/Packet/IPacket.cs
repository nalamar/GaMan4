using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace ProtocolLibrary.Packet
{
    /// <summary>
    /// The interface for all network packets.
    /// </summary>
    public interface IPacket
    {
        /// <summary>
        /// Serializes the packet and returns the packed information
        /// to the caller.
        /// </summary>
        /// <exception cref="System.ArgumentException">Thrown when the packet was corrupt</exception>
        /// <returns>The serialized packet</returns>
        byte[] Serialize();

        /// <summary>
        /// Deserialize a new message from a given stream. Rethrows an IOException or 
        /// ArgumentException to the caller.
        /// </summary>
        /// <exception cref="System.ArgumentException">Thrown when the argument was invalid</exception>
        /// <exception cref="System.IO.IOException">Thrown when the I/O failed</exception>
        /// <param name="stream"></param>
        void Deserialize(Stream stream);

        /// <summary>
        /// Gets or sets the packet data.
        /// </summary>
        byte[] Data { get; set; }

        /// <summary>
        /// Sets or gets the compression of the data. If compression
        /// is set to true, the data section will be automatically compressed.
        /// This is done within the packet class.
        /// </summary>
        bool Compressed { get; set; }

        /// <summary>
        /// Gets or sets the encryption of the data. If encryption
        /// is set to true and supported by the packet class, the data 
        /// section will be automatically encrypted. For the symmetric-key
        /// encryption a non-zero length password must be set.
        /// </summary>
        bool Encrypted { get; set; }

        /// <summary>
        /// Indicates, if this packet is corrupted.
        /// </summary>
        bool Corrupted { get; }

        /// <summary>
        /// Gets or sets a password for the encryption of the packet 
        /// data, if encryption is supported.
        /// </summary>
        string Password { get; set; }

        /// <summary>
        /// Gets the packet data delimiter.
        /// </summary>
        string Delimiter { get; }

        /// <summary>
        /// Creates a formatted packet and returns it as a string
        /// representation.
        /// </summary>
        /// <returns>The packet as a string</returns>
        string ToString();

        /// <summary>
        /// Gets the protocol version.
        /// </summary>
        byte ProtocolVersion { get; }

        /// <summary>
        /// Sets or gets the encoding of the payload (data). Returns null
        /// if no character encoding was set. The caller is
        /// responsible for the right encoding.
        /// </summary>
        Encoding Encoding { get; set; }

        /// <summary>
        /// Gets or sets the mesage type.
        /// </summary>
        PacketType Type { get; set; }

        /// <summary>
        /// Gets or sets the source address.
        /// </summary>
        IPEndPoint Source { get; set; }

        /// <summary>
        /// Gets or sets the destination address.
        /// </summary>
        IPEndPoint Destination { get; set; }
    }
}
