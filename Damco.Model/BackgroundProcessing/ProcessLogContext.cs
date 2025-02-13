using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model.BackgroundProcessing
{
    public enum ExceptionType
    {
        Unknown = 0,
        System = 1,
        Input = 2
    }

    /// <summary>
    /// Complex type ProcessLogContext.
    /// </summary>
    /// <remarks>
    /// Used while creating logs for exceptions.
    /// </remarks>
    public class ProcessLogContext
    {
        public ProcessLogContext() { }

        /// <summary>
        /// Initializes an instance of ProcessLogContext with sourceName.
        /// </summary>
        /// <param name="sourceName">Usually the activity of the process.</param>
        public ProcessLogContext(string sourceName) : this(sourceName, null, null) { }

        /// <summary>
        /// Initializes an instance of ProcessLogContext with sourceName and runId.
        /// </summary>
        /// <param name="sourceName">Usually the activity of the process.</param>
        /// <param name="runId">Date/Time value as string with "en-US"as the culture.</param>
        public ProcessLogContext(string sourceName, string runId) : this(sourceName, runId, null) { }

        /// <summary>
        /// Initializes an instance of ProcessLogContext with sourceName and backgroundProcessId.
        /// </summary>
        /// <param name="sourceName">Usually the activity of the process.</param>
        /// <param name="backgroundProcessId">ID of the background Process/service that is running.</param>
        public ProcessLogContext(string sourceName, int? backgroundProcessId) : this(sourceName, null, backgroundProcessId) { }

        /// <summary>
        /// Initializes an instance of ProcessLogContext with sourceName, runId and backgroundProcessId.
        /// </summary>
        /// <param name="sourceName">Usually the activity of the process.</param>
        /// <param name="runId">Date/Time value as string with "en-US"as the culture.</param>
        /// <param name="backgroundProcessId">ID of the background Process/service that is running.</param>
        public ProcessLogContext(string sourceName, string runId, int? backgroundProcessId)
        {
            this.SourceName = sourceName;
            this.RunId = runId ?? DateTime.UtcNow.ToString("yyyyMMdd_HHmmss_ffffff", System.Globalization.CultureInfo.GetCultureInfo("en-US"));
            this.BackgroundProcessId = backgroundProcessId;
        }

        /// <summary>
        /// Name of the source/activity that describes the activity.
        /// </summary>
        /// <remarks>
        /// For example - Send data to HighJump or Create order in B2C etc.
        /// </remarks>
        public string SourceName { get; set; }

        /// <summary>
        /// ID of the source.
        /// </summary>
        public int? SourceId { get; set; }

        /// <summary>
        /// Date/Time value as string with "en-US"as the culture.
        /// </summary>
        public string RunId { get; set; }

        /// <summary>
        /// ID of the background Process/service that is running.
        /// </summary>
        public int? BackgroundProcessId { get; set; }

        /// <summary>
        /// Id of the user logged in.
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// Id of the file.
        /// </summary>
        /// <remarks>
        /// If the file is new, ID is generated in ProcessLogService.
        /// </remarks>
        public int? FileId { get; set; }

        /// <summary>
        /// Name of the file where the log data is written.
        /// </summary>
        string _fileName;
        public string FileName { get { return _fileName; } set { _fileName = value; this.FileId = null; } }

        /// <summary>
        /// Data of the log in Text format.
        /// </summary>
        string _fileTextData;
        public string FileTextData { get { return _fileTextData; } set { _fileTextData = value; this.FileId = null; } }

        /// <summary>
        /// Data of the log in binary format.
        /// </summary>
        byte[] _fileBinaryData;
        public byte[] FileBinaryData { get { return _fileBinaryData; } set { _fileBinaryData = value; this.FileId = null; } }

        /// <summary>
        /// Writes data to the file.
        /// </summary>
        /// <param name="name">Name of the file where the log data is to be created.</param>
        /// <param name="data">Log data of type text to be written to a file.</param>
        /// <remarks>
        /// Has an overload for byte format.
        /// </remarks>
        public void SetFile(string name, string data)
        {
            this.FileName = name;
            this.FileTextData = data;
        }

        /// <summary>
        /// Writes data to the file.
        /// </summary>
        /// <param name="name">Name of the file where the log data is to be created.</param>
        /// <param name="data">Log data of type binary to be written to a file.</param>
        public void SetFile(string name, byte[] data)
        {
            this.FileName = name;
            this.FileBinaryData = data;
        }

        /// <summary>
        /// Clears all the file details.
        /// </summary>
        public void ClearFile()
        {
            this.FileName = null;
            this.FileBinaryData = null;
            this.FileTextData = null;
        }

        public string Attribute01 { get; set; }
        public string Attribute02 { get; set; }
        public string Attribute03 { get; set; }
        public string Attribute04 { get; set; }
        public string Attribute05 { get; set; }
        public string Attribute06 { get; set; }
        public string Attribute07 { get; set; }
        public string Attribute08 { get; set; }
        public string Attribute09 { get; set; }
        public string Attribute10 { get; set; }

        /// <summary>
        /// Sets attribute values.
        /// </summary>
        /// <param name="attributes">Values passed from service.</param>
        public void SetAttributes(params string[] attributes)
        {
            this.Attribute01 = (attributes == null || attributes.Length < 1 ? null : attributes[0]);
            this.Attribute02 = (attributes == null || attributes.Length < 2 ? null : attributes[1]);
            this.Attribute03 = (attributes == null || attributes.Length < 3 ? null : attributes[2]);
            this.Attribute04 = (attributes == null || attributes.Length < 4 ? null : attributes[3]);
            this.Attribute05 = (attributes == null || attributes.Length < 5 ? null : attributes[4]);
            this.Attribute06 = (attributes == null || attributes.Length < 6 ? null : attributes[5]);
            this.Attribute07 = (attributes == null || attributes.Length < 7 ? null : attributes[6]);
            this.Attribute08 = (attributes == null || attributes.Length < 8 ? null : attributes[7]);
            this.Attribute09 = (attributes == null || attributes.Length < 9 ? null : attributes[8]);
            this.Attribute10 = (attributes == null || attributes.Length < 10 ? null : attributes[9]);
        }

        /// <summary>
        /// Sets null as the attribute values.
        /// </summary>
        public void ClearAttributes()
        {
            SetAttributes(null);
        }

        /// <summary>
        /// Gets or sets type of the Exception .
        /// </summary>
        public ExceptionType ExceptionType { get; set; } = ExceptionType.Unknown;
    }
}
