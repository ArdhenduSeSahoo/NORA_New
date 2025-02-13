using Damco.Model.Interfacing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model.Workflow
{
    /// <summary>
    /// Maintains status details of an activity.
    /// </summary>
    /// <remarks>
    /// For example, status of a shipment or an order etc.
    /// </remarks>
    public class Status : IEntity, ILogged<WorkflowSetupLog>
    {
        /// <summary>
        /// Unique ID of the status.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of the status. 
        /// </summary>
        [Required(), StringLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// Property - gets string value. Converts JSON string to System.string.
        /// sets JSON value. Returns null if the value is null or the length is zero.
        /// This property is excluded from database mapping.
        /// </summary>
        [NotMapped()]
        public string[] Tags
        {
            get { return this.TagsAsString?.FromJson<string[]>() ?? new string[] { }; }
            set { this.TagsAsString = (value == null || value.Length == 0 ? null : value.ToJson()); }
        }
        /// <summary>
        /// Gets or sets TagsAsString.
        /// </summary>
        public string TagsAsString { get; set; }
        public string EntityTypeAsString { get; set; }
        [NotMapped()]
        public Type EntityType
        {
            get { return ExpressionSerialization.GetTypeForName(this.EntityTypeAsString); }
            set { this.EntityTypeAsString = ExpressionSerialization.GetNameForType(value); }
        }

        public UpdateHandlingType InboundMessageUpdateHandlingType { get; set; }

        [MaxLength(50)]
        public string Code { get; set; }

        public string Description { get; set; }

        public StandardStatus StandardStatus { get; set; }
    }

    public enum StandardStatus
    {
        Undefined = 0,
        Pending = 1,
        InProgress = 2,
        Done = 3,
        Cancelled = 4
    }

    public enum UpdateHandlingType
    {
        Unspecified = 0,
        Always = 1,
        UserChoice = 2,
        Never = 3
    }

    public interface IWorkflowControlled : IEntity
    {
        int? StatusId { get; set; }
        Status Status { get; set; }
        DateTime? StatusChangeDateTime { get; set; }
        int? VersionNumber { get; set; }
    }

    public class StatusAlias : AliasBase<Status>, IEntity
    {
    }
}
