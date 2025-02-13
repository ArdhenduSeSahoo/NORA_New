//Utilities for use during debugging
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Common
{
    /// <summary>
    /// Utilities for use during debugging
    /// </summary>
    public static class DebugUtil
    {
#if DEBUG
        static Stopwatch _sinceLast = new Stopwatch();
        /// <summary>
        /// Initialize the instance by starting the Stopwatch instance.
        /// </summary>
        static DebugUtil()
        {
            _sinceLast.Start();
        }

        /// <summary>
        /// Shows the debug line number and time elapsed since the last debugging start.
        /// </summary>
        /// <remarks>
        /// Writes a message followed by a line terminator to the trace listeners n the Listeners collection.
        /// </remarks>
        /// <param name="lineNumber">Line number of the debug point.</param>
        public static void LogTime(Func<string> infoGetter = null, [CallerFilePath]string file = null, [CallerLineNumber]int lineNumber = 0)
        {
            //if (!file.Contains("Operation") && !file.Contains("Import") && !file.Contains("Definition") && !file.Contains("Transition"))
              //  return;
            _sinceLast.Stop();
            var info = $"DEBUGINFO\t{Path.GetFileNameWithoutExtension(file)}\t{lineNumber}\t{_sinceLast.ElapsedMilliseconds}\t{infoGetter?.Invoke()}";
            Debug.WriteLine(info);
            //System.IO.File.AppendAllLines(@"C:\t\DEBUGINFO.txt", info.ToSingletonCollection());
            _sinceLast.Reset();
            _sinceLast.Start();
        }

        public static void Reset()
        {
            _sinceLast.Reset();
            _sinceLast.Start();
            _actions.Clear();
        }

        static List<string> _actions = new List<string>();
        public static void RecordAction(string info)
        {
            _actions.Add(info);
        }

        public static IEnumerable<string> Actions
        {
            get { return _actions.AsReadOnly(); }
        }
#else
        public static void LogTime(Func<string> infoGetter = null, [CallerFilePath]string file = null, [CallerLineNumber]int lineNumber = 0)
        {
        }

        public static void Reset()
        {
        }

        static List<string> _actions = new List<string>();
        public static void RecordAction(string info)
        {
        }

        public static IEnumerable<string> Actions
        {
            get { return Enumerable.Empty<string>(); }
        }
#endif

    }
}
