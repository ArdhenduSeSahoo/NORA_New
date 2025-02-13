using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Common
{
    public static class EncodingExtensions
    {
        public static string GetStringWithoutPreamble(this Encoding encoding, byte[] bytes)
        {
            var preamble = encoding.GetPreamble();
            if (bytes.Length >= preamble.Length && preamble.Select((b, i) => b == bytes[i]).All(x => x)) //Has preamble
                return encoding.GetString(bytes, preamble.Length, bytes.Length - preamble.Length);
            else
                return encoding.GetString(bytes);
        }
    }
}
