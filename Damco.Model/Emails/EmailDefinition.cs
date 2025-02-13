using Damco.Model.DataSourcing;
using Damco.Model.FileBuilding;
using Damco.Model.Workflow;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model.Emails
{

    public class EmailDefinition : Operation, IEntity, IRequiresDataExtract
    {
        //Primary key and some other fields come from "Operation"
        [NotMapped()]
        public int DataExtractId { get; set; }
        [NotMapped()]
        public DataExtract DataExtract { get; set; }
        public string Subject { get; set; }
        [Required()]
        public string Body { get; set; }
        public bool BodyIsHtml { get; set; }
        public string SenderAddress { get; set; }
        public string SenderName { get; set; }
        [NotMapped()]
        public int SenderNameDataExtractFieldId { get; set; }
        [NotMapped()]
        public DataExtractField SenderNameDataExtractField { get; set; }
        [NotMapped()]
        public int SenderAddressDataExtractFieldId { get; set; }
        [NotMapped()]
        public DataExtractField SenderAddressDataExtractField { get; set; }
        [NotMapped()]
        public int SenderAddressGetterDataFieldId { get; set; }
        [NotMapped()]
        public DataField SenderAddressGetterDataField { get; set; }
        public bool BccToSender { get; set; }
        public EmailPriority Priority { get; set; }
        [NotMapped()]
        public List<EmailDefinitionRecipient> Recipients { get; set; } = new List<EmailDefinitionRecipient>();
        [NotMapped()]
        public List<EmailDefinitionAttachment> Attachments { get; set; } = new List<EmailDefinitionAttachment>();
        [NotMapped()]
        public List<EmailDefinitionGroupingField> GroupingFields { get; set; } = new List<EmailDefinitionGroupingField>();
        [NotMapped()]
        public bool GroupPerItem { get; set; }
    }

    public class EmailDefinitionGroupingField
    {
        public int Id { get; set; }
        public int EmailDefinitionId { get; set; }
        public EmailDefinition EmailDefinition { get; set; }
        public int DataExtractFieldId { get; set; }
        public DataExtractField DataExtractField { get; set; }
    }

    public class EmailDefinitionRecipient
    {
        public int Id { get; set; }
        public int EmailDefinitionId { get; set; }
        public EmailMessage EmailDefinition { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public int? NameDataExtractFieldId { get; set; }
        public DataExtractField NameDataExtractField { get; set; }
        public int? AddressDataExtractFieldId { get; set; }
        public DataExtractField AddressDataExtractField { get; set; }
        public int AddressGetterDataFieldId { get; set; }
        public DataField AddressGetterDataField { get; set; }
        public RecipientType Type { get; set; }
    }

    public class EmailDefinitionAttachment
    {
        public int Id { get; set; }
        public int EmailDefinitionMessageId { get; set; }
        public EmailDefinition EmailDefinition { get; set; }
        public int BuilderOperationId { get; set; }
        public Operation BuilderOperation { get; set; }
        public bool FilterData { get; set; }
        public string FileName { get; set; }
    }
}
