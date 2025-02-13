//A set of utilitiy methods (extension methods mostly) to compress and decompress data 
//Basically wrappers for GZipStream and ZipArchive objects to take away most of the coding
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Common
{
    /// <summary>
    /// Class helps to Compress the objects prior to transmission which improves the 
    /// the performance and later Decompresses the same values passed to it. 
    /// Specifically System.IO.Compression class is used for such operation.
    /// 
    /// Both GZipStream and ZipArchive (".zip" files) are supported, 
    /// including zip archives with multiple files in the archive.
    /// </summary>
    public static class Compression
    {
        /// <summary>
        /// Compresses a string to a string.
        /// </summary>
        /// <param name="value">The string to compress.</param>
        /// <returns>Base64 representation of the compressed string.</returns>
        public static string CompressToString(this string value)
        {
            return CompressToString(Encoding.Unicode.GetBytes(value));
        }

        public static string CompressToString(this byte[] value)
        {
            return Convert.ToBase64String(CompressToBytes(value));
        }

        public static byte[] CompressToBytes(this byte[] value)
        {
            byte[] result;
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            using (System.IO.Compression.GZipStream gzipStream = new System.IO.Compression.GZipStream(stream, System.IO.Compression.CompressionMode.Compress))
            {
                gzipStream.Write(value, 0, value.Length);
                gzipStream.Close();
                result = stream.ToArray();
                stream.Close();
            }
            return result;
        }

        public static string DeCompressToString(this string value)
        {
            return DeCompressToString(Convert.FromBase64String(value));
        }

        public static string DeCompressToString(this byte[] value)
        {
            return Encoding.Unicode.GetString(DeCompressToBytes(value));
        }

        public static byte[] DeCompressToBytes(this byte[] value)
        {
            byte[] result;
            using (var inputStream = new MemoryStream(value))
            using (var outputStream = new MemoryStream())
            using (var gzip = new GZipStream(inputStream, CompressionMode.Decompress))
            {
                byte[] bytes = new byte[4096];
                int n;
                while ((n = gzip.Read(bytes, 0, bytes.Length)) != 0)
                {
                    outputStream.Write(bytes, 0, n);
                }
                gzip.Close();
                result = outputStream.ToArray();
            }
            return result;
        }

        public static byte[] CompressAsZipToBytes(this byte[] value, string fileNameInZip)
        {
            Dictionary<string, byte[]> dict = new Dictionary<string, byte[]>();
            dict.Add(fileNameInZip, value);
            return dict.CompressAsZipToBytes();
        }


        public static byte[] CompressAsZipToBytes(this IEnumerable<KeyValuePair<string, byte[]>> value)
        {
            byte[] result;
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                using (var zipArchive = new ZipArchive(stream, ZipArchiveMode.Create))
                {
                    foreach (var sourceEntry in value)
                    {
                        var entry = zipArchive.CreateEntry(sourceEntry.Key);
                        using (var entryStream = entry.Open())
                        {
                            entryStream.Write(sourceEntry.Value, 0, sourceEntry.Value.Length);
                        }
                    }
                }
                result = stream.ToArray();
            }
            return result;
        }

        public static byte[] DeCompressFirstFromZipToBytes(this byte[] value)
        {
            byte[] result;
            using (var inputStream = new MemoryStream(value))
            using (var zipArchive = new ZipArchive(inputStream, ZipArchiveMode.Read))
            {
                var zipEntry = zipArchive.Entries.First();
                using (var entryStream = zipEntry.Open())
                using (var outputStream = new MemoryStream())
                {
                    byte[] bytes = new byte[4096];
                    int n;
                    while ((n = entryStream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        outputStream.Write(bytes, 0, n);
                    }
                    result = outputStream.ToArray();
                }
            }
            return result;
        }

        public static Dictionary<string, byte[]> DeCompressFromZipToBytes(this byte[] value)
        {
            Dictionary<string, byte[]> result = new Dictionary<string, byte[]>();
            using (var inputStream = new MemoryStream(value))
            using (var zipArchive = new ZipArchive(inputStream, ZipArchiveMode.Read))
            {
                foreach (var zipEntry in zipArchive.Entries)
                {
                    using (var entryStream = zipEntry.Open())
                    using (var outputStream = new MemoryStream())
                    {
                        byte[] bytes = new byte[4096];
                        int n;
                        while ((n = entryStream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            outputStream.Write(bytes, 0, n);
                        }
                        result.Add(zipEntry.FullName, outputStream.ToArray());
                    }
                }
            }
            return result;
        }



    }
}
