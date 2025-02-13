using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Common
{
    public static class IOUtils
    {
        public static byte[] ReadAllBytesNoLock(string fileName)
        {
            byte[] result;
            using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var target = new MemoryStream())
            {
                byte[] buffer = new byte[1024];
                int numBytes = 0;
                while ((numBytes = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                    target.Write(buffer, 0, numBytes);
                result = target.ToArray();
            }
            return result;
        }

    }
}
