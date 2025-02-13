using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model.BackgroundProcessing
{
    /// <summary>
    /// Maintains details about the files.
    /// </summary>
    /// <remarks>
    /// A binary/text file is being processed.
    /// </remarks>
    public class ProcessFile : IEntity
    {
        /// <summary>
        /// Unique Id of the entry.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Id of the Background Process.
        /// </summary>
        public int? BackgroundProcessId { get; set; }

        /// <summary>
        /// Records BackgroundProcess details.
        /// </summary>
        public BackgroundProcess BackgroundProcess { get; set; }

        /// <summary>
        /// Name of the file processed.
        /// </summary>
        [Required(), MaxLength(400)]
        public string FileName { get; set; }
    }

    /// <summary>
    /// Processes file of the type Binary.
    /// </summary>
    public class ProcessBinaryFile : ProcessFile, IEntity
    {
        /// <summary>
        /// Data of the file as bytes.
        /// </summary>
        public byte[] Data { get; set; }
    }

    /// <summary>
    /// Processes file of the type Text.
    /// </summary>
    public class ProcessTextFile : ProcessFile, IEntity
    {
        /// <summary>
        /// Data of the file as string.
        /// </summary>
        public string Data { get; set; }
    }
}
