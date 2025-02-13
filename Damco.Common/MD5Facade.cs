using System.Security.Cryptography;
using System.Text;

namespace Damco.Common
{
    public static class MD5Facade
    {
        public static string CalculateHash(byte[] inputBytes)
        {
            MD5 md5 = MD5.Create();
            byte[] hash = md5.ComputeHash(inputBytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }

            return sb.ToString();
        }
    }
}
