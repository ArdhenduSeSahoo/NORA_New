using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Damco.Model;
using System.ComponentModel.DataAnnotations;
using Damco.Model.UserManagement;

namespace Damco.Model.BackgroundProcessing
{
    [Serializable()]
    public enum BackgroundProcessStatus
    {
        Pending = 1,
        Running = 2,
        Error = 3,
        Finished = 4,
        ErrorSolved = 5
    }

    [Serializable()]
    public enum BackgroundProcessCallerType
    {
        Trigger = 1,
        TriggerFromOtherInstance = 2,
        FallbackScheduler = 3
    }

    /// <summary>
    /// Maintains details about the background processes/services.
    /// </summary>
    /// <remarks>
    /// Records all the background processes / services activites in the database. 
    /// Records the description/stacktrace of exceptions(if occurred any).
    /// </remarks>
    [Serializable()]
    public class BackgroundProcess : IEntity
    {
        /// <summary>
        /// Unique Id of the activity of background process/service.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The status of the background process/service. 
        /// </summary>
        public BackgroundProcessStatus Status { get; set; }

        /// <summary>
        /// The activity of the background process/service.
        /// </summary>
        public string ActivityAsString { get; set; }

        /// <summary>
        /// The Enqued DateTime of the background process/service.
        /// </summary>
        public DateTime EnqueDateTime { get; set; }

        /// <summary>
        /// StartedDateTime indicating when background process/service is started.
        /// </summary>
        public DateTime? StartedDateTime { get; set; }

        /// <summary>
        /// Indicates when background process/sevrice is ended.
        /// </summary>
        public DateTime? EndedDateTime { get; set; }

        /// <summary>
        /// The error text to be displayed on the UI.
        /// </summary>
        /// <remarks>
        /// This field is excluded from database mapping.
        /// </remarks>
        [NotMapped()]
        public TextPlaceHolder Error
        {
            get { return this.ErrorAsString?.FromJson<TextPlaceHolder>(); }
            set { this.ErrorAsString = value?.ToJson(); }
        }

        /// <summary>
        /// Exception occurred is stores as JSON string.
        /// </summary>
        /// <remarks>
        /// When an exception occurs in the process/service, it is logged in the database as stacktrace.
        /// </remarks>
        public string ErrorAsString { get; set; }

        /// <summary>
        /// The Caller type of the background process/service.
        /// </summary>
        public BackgroundProcessCallerType? CallerType { get; set; }

        public int NumberOfTriesDone { get; set; }

        public int? CategoryId { get; set; }
        public BackgroundProcessCategory Category { get; set; }

        public int? UserId { get; set; }
        public User User { get; set; }

        public int? CreatorBackgroundProcessId { get; set; }
    }

    public class BackgroundProcessCategory : IEntity
    {
        public int Id { get; set; }
        [MaxLength(100), Required(), ForceUnique()]
        public string Code { get; set; }
    }
}
