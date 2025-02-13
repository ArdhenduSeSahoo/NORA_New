using Damco.Model.MultiTenancy;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model.Interfacing
{
    public class InboundMessage : IEntity
    {
        public int Id { get; set; }
        public bool IsProcessed { get; set; }
        public int InterfaceSetupId { get; set; }
        public InterfaceSetup InterfaceSetup { get; set; }
        [MaxLength(200)]
        public string EntityTypeAsString { get; set; }
        [NotMapped()]
        public Type EntityType
        {
            get { return this.EntityTypeAsString == null ? null : ExpressionSerialization.GetTypeForName(this.EntityTypeAsString); }
            set { this.EntityTypeAsString = value == null ? null : ExpressionSerialization.GetNameForType(value); }
        }
        public int? EntityId { get; set; }
        [NotMapped()]
        public IInterfacedEntity Entity
        {
            set
            {
                this.EntityType = value?.GetType();
                this.EntityId = value?.Id;
            }
        }
        public bool ReplacedByNewerVersion { get; set; }
        public bool MessageIgnored { get; set; }
        public List<InboundMessageError> Errors { get; set; } = new List<InboundMessageError>();
        [MaxLength(100)]
        public string ItemReference { get; set; }
        [MaxLength(200)]
        public string ItemUniqueId { get; set; }
        [MaxLength(200)]
        public string CorrelationId { get; set; }
        public bool UpdateOnly { get; set; }
        public List<InboundMessageContent> Contents { get; set; } = new List<InboundMessageContent>();
        [MaxLength(100)]
        public string InterchangeControlNumber { get; set; }
        public int? ExpectedNumberOfParts { get; set; }
        public int? ReceivedNumberOfParts { get; set; }
        [Required(), MaxLength(24)]
        public string MessageContentsId { get; set; }
        public SaveResult Result { get; set; }
        public DateTime ReceivedDateTime { get; set; }
        public int? OwnerId { get; set; }
        public UserManagement.User Owner { get; set; }
        [MaxLength(100)]
        public string ExternalMessageType { get; set; } //Code the customer uses for this message type
        public string SenderContactEmail { get; set; }
        public int? TenantId { get; set; }
        public Tenant Tenant { get; set; }
        public DateTime? RetryDateTime { get; set; }
        public int? RetryUserId { get; set; }
    }

    public class InboundMessageContent : IEntity
    {
        public int Id { get; set; }
        public int InboundMessageId { get; set; }
        [CascadeDelete()]
        public InboundMessage InboundMessage { get; set; }
        [MaxLength(100)]
        public string ChildUniqueId { get; set; }
        [MaxLength(100)]
        public string ChildReference { get; set; }
        [Required()]
        public string InboundMessageAsString { get; set; }
        public string InboundMessageAsStringLink { get; set; }
        public string InboundMessageAsStringLinkHost { get; set; }
        [Required(), MaxLength(24)]
        public string MessageContentId { get; set; }
        public DateTime ReceivedDateTime { get; set; }
        public bool ReplacedByOtherContent { get; set; }
    }

    public class ValueComparison
    {
        public int InboundMessageId { get; set; }
        [Key()]
        public string ValueName { get; set; }
        public object OldValue { get; set; }
        public object NewValue { get; set; }
        public bool HasDifference { get { return !object.Equals(this.OldValue, this.NewValue); } }
    }

}
