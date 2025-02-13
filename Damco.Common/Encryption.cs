//Utilities to encrypt or decrypt data.
//Basically wrappers over the MD5CryptoServiceProvider and TripleDESCryptoServiceProvider
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Common
 {  /// <summary>
    /// Utilities to encrypt or decrypt data.
    /// <para>Basically wrappers over the MD5CryptoServiceProvider and TripleDESCryptoServiceProvider</para>
    /// </summary>
    public static class Encryption
    {
        /// <summary>
        /// Computes the MD5 hash of the string.
        /// </summary>
        /// <param name="value">The string to be Encrypted.</param>
        /// <returns>Base64 computed hash string. </returns>
        public static string ToMD5String(this string value)
        {
            return ToMD5String(Encoding.Unicode.GetBytes(value));
        }

        /// <summary>
        /// Computes the MD5 hash of the string.
        /// </summary>
        /// <param name="value">The string to be Encrypted.</param>
        /// <returns>Computed hash Byte Array. </returns>
        public static byte[] ToMD5Bytes(this string value)
        {
            return ToMD5Bytes(Encoding.Unicode.GetBytes(value));
        }

        /// <summary>
        /// Computes the MD5 hash of current instance Byte Array.
        /// </summary>
        /// <param name="value">The input to be Encrypted.</param>
        /// <returns>Base64 computed hash string. </returns>
        public static string ToMD5String(this byte[] value)
        {
            return Convert.ToBase64String(ToMD5Bytes(value));
        }

        /// <summary>
        /// Computes the MD5 hash of current instance Byte Array.
        /// </summary>
        /// <param name="value">The input to be Encrypted.</param>
        /// <returns>Computed hash Byte Array.</returns>
        public static byte[] ToMD5Bytes(this byte[] value)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            return md5.ComputeHash(value);
        }

        /// <summary>
        /// Encrypt the value & key of string type.
        /// </summary>
        /// <param name="value">String value to encrypted.</param>
        /// <param name="key">String key reference for input Value.</param>
        /// <returns>Base64 representation of encrypted value and Key. </returns>
        public static string EncryptToString(this string value, string key)
        {
            return EncryptToString(Encoding.Unicode.GetBytes(value), key);
        }

        /// <summary>
        /// Encrypt the curretn instance Byte Array & key of string type.
        /// </summary>
        /// <param name="value">Value to encrypted</param>
        /// <param name="key">String key reference for input Value</param>
        /// <returns>Base64 representation of encrypted value and Key </returns>
        public static string EncryptToString(this byte[] value, string key)
        {
            return Convert.ToBase64String(EncryptToBytes(value, key));
        }


        /// <summary>
        /// Encrypt the current instance Byte Array & key of string type.
        /// </summary>
        /// <remarks>
        /// <para>TripleDES and MD5 to encrypt the Key.</para>
        /// <para>CryptoStream that links data streams of Value and it's Key.</para>
        /// </remarks>
        /// <param name="value">Value to encrypted.</param>
        /// <param name="key">String key reference for input Value.</param>
        /// <returns>Byte array representation of stream of encrypted value and Key. </returns>
        public static byte[] EncryptToBytes(this byte[] value, string key)
        {
            TripleDES des = new TripleDESCryptoServiceProvider();
            des.Key = new MD5CryptoServiceProvider().ComputeHash(Encoding.Unicode.GetBytes(key));
            des.IV = new byte[des.BlockSize / 8];
            MemoryStream stream = new MemoryStream();
            CryptoStream cryptStream = new CryptoStream(stream, des.CreateEncryptor(), CryptoStreamMode.Write);
            cryptStream.Write(value, 0, value.Length);
            cryptStream.FlushFinalBlock();
            return stream.ToArray();
        }
        /// <summary>
        /// Decrypt the encrypted Value to string using the Key 
        /// </summary>
        /// <param name="value">Value to decrypted</param>
        /// <param name="key">String key reference for input Value</param>
        /// <returns>Decrypted string value</returns>
        public static string DecryptToString(this string value, string key)
        {
            return DecryptToString(Convert.FromBase64String(value), key);
        }

        /// <summary>
        /// Decrypt the encrypted Byte Array value to string using the Key. 
        /// </summary>
        /// <param name="value">Value to decrypted.</param>
        /// <param name="key">String key reference for input Value.</param>
        /// <returns>Decrypted string value.</returns>
        public static string DecryptToString(this byte[] value, string key)
        {
            return Encoding.Unicode.GetString(DecryptToBytes(value, key));
        }


        /// <summary>
        /// Decrypt the encrypted Byte Array value to Byte Array using the Key. 
        /// </summary>
        /// <param name="value">Value to decrypted</param>
        /// <param name="key">String key reference for input Value.</param>
        /// <returns>Byte array representation of stream of decrypted value and Key.</returns>
        public static byte[] DecryptToBytes(byte[] value, string key)
        {
            TripleDES des = new TripleDESCryptoServiceProvider();
            des.Key = new MD5CryptoServiceProvider().ComputeHash(Encoding.Unicode.GetBytes(key));
            des.IV = new byte[des.BlockSize / 8];
            MemoryStream stream = new MemoryStream();
            CryptoStream cryptStream = new CryptoStream(stream, des.CreateDecryptor(), CryptoStreamMode.Write);
            cryptStream.Write(value, 0, value.Length);
            cryptStream.FlushFinalBlock();
            return stream.ToArray();
        }
    }
}
