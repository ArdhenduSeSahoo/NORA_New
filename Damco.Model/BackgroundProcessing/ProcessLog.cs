using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model.BackgroundProcessing
{

    [Serializable()]
    public enum LogType
    {
        Information,
        Action,
        InputError,
        SystemError,
        Warning,
        Verbose
    }

    /// <summary>
    /// Acts as a source for Log entry.
    /// </summary>
    /// <remarks>
    /// Gets Id by sourceName passed through ProcessLogSourceService.
    /// </remarks>
    public class ProcessLogSource : IEntity
    {
        public int Id { get; set; }
        [Required(), MaxLength(100)]
        public string Name { get; set; }
    }

    /// <summary>
    /// Creates log for changes/exceptions occurred.
    /// </summary>
    public class ProcessLog : IEntity
    {
        /// <summary>
        /// Unique Id of the log entry.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Type of the exception to be logged.
        /// </summary>
        public LogType Type { get; set; }

        /// <summary>
        /// Date/Time details of the log entry.
        /// </summary>
        public DateTime DateTime { get; set; }

        /// <summary>
        /// Id of the logged in user.
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// Records details of the logged in user.
        /// </summary>
        public UserManagement.User User { get; set; }

        /// <summary>
        /// Id of the context source.
        /// </summary>
        /// <remarks>
        /// If null, new ProcessLogSourceService instance is created with source name from the log context.
        /// </remarks>
        public int SourceId { get; set; }

        /// <summary>
        /// Source/Process where the exception occurred.
        /// </summary>
        public ProcessLogSource Source { get; set; }

        /// <summary>
        /// UTC Date/Time value as string with the "en-US" as culture.
        /// </summary>
        [Required(), MaxLength(100)]
        public string RunId { get; set; }

        /// <summary>
        /// Id of the background process running.
        /// </summary>
        public int? BackgroundProcessId { get; set; }

        /// <summary>
        /// Contains details about the background process.
        /// </summary>
        public BackgroundProcess BackgroundProcess { get; set; }

        /// <summary>
        /// Id of the File - text/binary.
        /// </summary>
        public int? FileId { get; set; }

        /// <summary>
        /// Contains details about the file like data or BackGroundProcessId or FileName.
        /// </summary>
        public ProcessFile File { get; set; }

        /// <summary>
        /// Exception message is passed through logContext to be saved as JSON string.
        /// </summary>
        /// <remarks>
        /// This field is excluded from database mapping.
        /// </remarks>
        [NotMapped()]
        public TextPlaceHolder Info
        {
            get { return this.InfoAsString.FromJson<TextPlaceHolder>(); }
            set { this.InfoAsString = value.ToJson(); }
        }

        /// <summary>
        /// Exception message is passed through logContext to be saved as JSON string.
        /// </summary>
        public string InfoAsString { get; set; }

        /// <summary>
        /// Attributes to be set by the ProcessLogContext.
        /// </summary>
        /// <remarks>
        /// Values from Attribute01 to Attribute05 are trimmed to length 200 while creating the log.
        /// </remarks>
        [MaxLength(200)]
        public string Attribute01 { get; set; }
        [MaxLength(200)]
        public string Attribute02 { get; set; }
        [MaxLength(200)]
        public string Attribute03 { get; set; }
        [MaxLength(200)]
        public string Attribute04 { get; set; }
        [MaxLength(200)]
        public string Attribute05 { get; set; }
        public string Attribute06 { get; set; }
        public string Attribute07 { get; set; }
        public string Attribute08 { get; set; }
        public string Attribute09 { get; set; }
        public string Attribute10 { get; set; }

    }

}
