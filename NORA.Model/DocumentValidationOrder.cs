using Damco.Model;
using Damco.Model.UserManagement;
using Damco.Model.Workflow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Damco.Model.Interfacing;
using System.ComponentModel.DataAnnotations;

namespace NORA.Model
{

    public class DocumentValidationOrder : IEntity, IWorkflowControlled, ILogged<DocumentValidationLog>, IInterfacedEntity
    {
        public int? VersionNumber { get; set; }
        public int Id { get; set; }
        public int? StatusId { get; set; }
        public Status Status { get; set; }
        public DateTime? StatusChangeDateTime { get; set; }
        [CascadeDelete()]
        public int TransportationDocumentId { get; set; }
        public TransportationDocument TransportationDocument { get; set; }
        public List<DocumentValidationOrderField> Fields { get; set; }
        public string Comments { get; set; }
        public bool IsHot { get; set; }
        public int NumberOfRemindersSent { get; set; }
        public bool RequiresFourEyes { get; set; }
        public int? FirstApproverUserId { get; set; }
        public User FirstApproverUser { get; set; }
        public int? LastApproverUserId { get; set; }
        public User LastApproverUser { get; set; }
        public bool FirstApprovedAutomatically { get; set; }
        public bool LastApprovedAutomatically { get; set; }
        public bool RejectedAutomatically { get; set; }
        //public bool HasPendingChangesFromInterface { get; set; }
        public bool HasPendingErrorsFromInterface { get; set; }
        [MaxLength(100)]
        public string FirstApproverName { get; set; }
        [MaxLength(100)]
        public string SecondApproverName { get; set; }
        public DateTime? DraftDocumentCheckedDateTime { get; set; }
        public DateTime? FinalDocumentCheckedDateTime { get; set; }
        public string InternalContactEmail { get; set; }
        public int? RejectionHandlingPartyId { get; set; }
        public HandlingParty RejectionHandlingParty { get; set; }
    }

    public class DocumentValidationOrderField : Damco.Model.DataValidation.DataValidationField, IEntity, ILogged<DocumentValidationLog>
    {
        public int DocumentValidationOrderId { get; set; }
        public DocumentValidationOrder DocumentValidationOrder { get; set; }
    }

    public class DocumentValidationLog : Damco.Model.ChangeLogBase, IEntity
    {
    }

}
