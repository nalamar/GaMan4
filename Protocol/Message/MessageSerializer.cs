using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Protocol.Message
{
    /// <summary>
    /// Serializes and Deserializes a serializable object.
    /// </summary>
    public class MessageSerializer
    {
        /// <summary>
        /// Serializes a message.
        /// </summary>
        /// <param name="obj">The object to serialize</param>
        /// <returns>A serialized array or null</returns>
        public static byte[] Serialize(object obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            byte[] serialized = null;

            try
            {
                bf.Serialize(ms, obj);
                serialized = ms.ToArray();
            }
            catch (SerializationException)
            {
            }

            return serialized;
        }

        /// <summary>
        /// Deserializes a message.
        /// </summary>
        /// <param name="array">The array to deserialize</param>
        /// <returns>The deserialized object or null</returns>
        public static object Deserialize(byte[] array)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream(array);
            object deserialized = null;

            try
            {
                deserialized = bf.Deserialize(ms);
            }
            catch (SerializationException)
            {
            }

            return deserialized;
        }
    }
}
