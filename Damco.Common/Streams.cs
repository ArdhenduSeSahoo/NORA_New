
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Damco.Common
{
    /// <summary>
    /// Utilities to work with streams.
    /// </summary>
    public static class Streams
    {
        /// <summary>
        /// Converts the Stream into Byte Array.
        /// </summary>
        /// <param name="stream">Stream to be converted to Byte Array.</param>
        /// <returns>A byte array of corresponding stream length. </returns>
        public static byte[] ToByteArray(this Stream stream)
        {
            using (stream)
            {
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                return buffer;
            }
        }

        /// <summary>
        /// Converts the Byte Array into Stream.
        /// </summary>
        /// <param name="bytes">Byte array to be converted to Stream. </param>
        /// <returns>Stream of corresponding Byte Array.</returns>
        public static Stream ToStream(this byte[] bytes)
        {
            return new MemoryStream(bytes);
        }
    }
}
