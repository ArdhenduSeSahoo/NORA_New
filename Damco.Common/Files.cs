using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Common
{
    /// <summary>
    /// Utilities to handle file related operations.
    /// </summary>
    public static class Files
    {
        public enum FileLockedAction
        {
            OpenAnyWay = 1,
            Error = 2,
        }

        public static string GetCleanFileName(this string text, string extension)
        {
            return new string(
                text
                .Select(c => char.IsWhiteSpace(c)
                    || c == '\r' || c == '\n' || c == '.' || c == '?' || c == '!'
                    || c == '\\' || c == '/' || c == ':' || c == '~' || c == '#'
                    || c == '%' || c == '&' || c == '+' || c == '|' || c == '"'
                    || c == '{' || c == '}' || c == '<' || c == '>'
                    ? ' ' : c   //Change some stuff to space
                )
                .Where(c => char.IsLetterOrDigit(c) || c == ' ' || c == '-' || c == '_') //Allowed characters
                .ToArray()
            ).Trim() + "." + extension.TrimStart('.');
        }

        public static string AddWithUniqueFileName<T>(this Dictionary<string, T> dictionary, string name, T value)
        {
            string nameToUse = name;
            if (dictionary.ContainsKey(nameToUse))
            {
                int number = 0;
                do
                {
                    number++;
                    nameToUse = System.IO.Path.GetFileNameWithoutExtension(name) + number.ToString() + System.IO.Path.GetExtension(name);
                } while (dictionary.ContainsKey(nameToUse));
            }
            dictionary.Add(nameToUse, value);
            return nameToUse;
        }


        /// <summary>
        /// Read the file from the specified location depending on the lock state of the file.
        /// </summary>
        /// <param name="sourceLocation">File location.</param>
        /// <param name="lockedAction">State of the file.</param>
        /// <returns>A byte array containing the contents of the file.</returns>
        public static byte[] ReadAllBytes(string sourceLocation, FileLockedAction lockedAction)
        {
            //if (sourceLocation.ToUpper().Contains("FTP://"))
            //    return FTPUtils.DownloadFile(sourceLocation, false);
            //else
            //{
            if (lockedAction == FileLockedAction.Error)
                return File.ReadAllBytes(sourceLocation);
            else if (lockedAction == FileLockedAction.OpenAnyWay)
            {
                using (FileStream fs = new FileStream(sourceLocation, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    byte[] abytResult = new byte[fs.Length];
                    int intCount;
                    int intOffSet = 0;
                    while ((intCount = fs.Read(abytResult, intOffSet, Math.Min((int)fs.Length - intOffSet, (int)2048))) > 0)
                        intOffSet += intCount;
                    return abytResult;
                }
            }
            else
                throw new ArgumentException($"{nameof(lockedAction)} is invalid", nameof(lockedAction));
            //}
        }


        /// <summary>
        /// Combines the strings in Array into single string path.
        /// </summary>
        /// <param name="paths">No of Paths to be combined.</param>
        /// <returns>Path to fit within a certain number of characters by replacing path components with ellipses.
        /// <para> Eg. c:\\a\\b\\c\\d\\e\\f\\path\\to\\be\\shortened\\" </para>
        /// <para> c:\a\b\c\d...\shortened </para>
        /// </returns>
        public static string CombinePathsAndSimplify(params string[] paths)
        {
            string combined = Path.Combine(paths);
            StringBuilder sb = new StringBuilder(Math.Max(260, 2 * combined.Length));
            PathCanonicalize(sb, combined);
            return sb.ToString();
        }

        [DllImport("shlwapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool PathCanonicalize([Out] StringBuilder dst, string src);
    }
}
