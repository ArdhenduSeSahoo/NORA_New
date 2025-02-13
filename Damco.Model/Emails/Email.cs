using Damco.Model.UserManagement;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model.Emails
{
    [JsonObject()]
    [Serializable()]
    public class EmailMessage : IEntity, IAddTracking
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool BodyIsHtml { get; set; }
        public string SenderAddress { get; set; }
        public string SenderName { get; set; }
        public EmailPriority Priority { get; set; }
        public List<EmailRecipient> Recipients { get; set; } = new List<EmailRecipient>();
        public List<EmailAttachment> Attachments { get; set; } = new List<EmailAttachment>();
        public List<EmailMessageItem> Items { get; set; } = new List<EmailMessageItem>();
        public EmailMessageStatus Status { get; set; }
        [NotMapped()]
        public TextPlaceHolder Error
        {
            get { return this.ErrorAsString?.FromJson<TextPlaceHolder>(); }
            set { this.ErrorAsString = value?.ToJson(); }
        }
        public string ErrorAsString { get; set; }
        public DateTime AddDateTime { get; set; }
        public User AddUser { get; set; }
        public int? AddUserId { get; set; }
        public DateTime? SentDateTime { get; set; }
    }

    public enum EmailMessageStatus
    {
        PendingSend = 0,
        Sent = 1,
        UnsentTest = 2,
        Cancelled = 3,
        Error = 4
    }

    [JsonObject()]
    [Serializable()]
    public class EmailRecipient : IEntity
    {
        public int Id { get; set; }
        public int EmailMessageId { get; set; }
        [JsonIgnore()]
        [CascadeDelete()]
        public EmailMessage EmailMessage { get; set; }
        [Required()]
        public string Address { get; set; }
        public string Name { get; set; }
        public RecipientType Type { get; set; }
    }

    [JsonObject()]
    [Serializable()]
    public class EmailAttachment : IEntity
    {
        public int Id { get; set; }
        public int EmailMessageId { get; set; }
        [JsonIgnore()]
        [CascadeDelete()]
        public EmailMessage EmailMessage { get; set; }
        [Required()]
        public string FileName { get; set; }
        [Required()]
        public byte[] Data { get; set; }
        public string DataLink { get; set; }
        public string DataLinkHost { get; set; }
    }

    public class EmailMessageItem : IEntity
    {
        public int Id { get; set; }
        public int EmailMessageId { get; set; }
        [CascadeDelete()]
        public EmailMessage EmailMessage { get; set; }
        [MaxLength(200), Required()]
        public string ItemTypeAsString { get; set; }
        [NotMapped(), Required()]
        public Type ItemType
        {
            get { return ExpressionSerialization.GetTypeForName(this.ItemTypeAsString); }
            set { this.ItemTypeAsString = ExpressionSerialization.GetNameForType(value); }
        }
        public int ItemId { get; set; }

    }

    [Serializable()]
    public enum RecipientType
    {
        To,
        Cc,
        Bcc
    }
    [Serializable()]
    public enum EmailPriority
    {
        High = 1,
        Medium = 2,
        Low = 3
    }
}
