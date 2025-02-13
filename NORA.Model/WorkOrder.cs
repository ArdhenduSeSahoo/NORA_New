using Damco.Model;
using Damco.Model.Interfacing;
using Damco.Model.UserManagement;
using Damco.Model.Workflow;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Damco.Model.MultiTenancy;

namespace NORA.Model
{
    public abstract class WorkOrder : IEntity, IWorkflowControlled
    {
        public int Id { get; set; }
        public int Sequence { get; set; }
        public WorkOrderParty Party { get; set; }
        public ShipmentStepType Type { get; set; }
        public DateTime? OriginalPlannedStartDateTime { get; set; }
        public DateTime? OriginalPlannedEndDateTime { get; set; }
        public DateTime? LastPlannedStartDateTime { get; set; }
        public DateTime? LastPlannedEndDateTime { get; set; }
        public DateTime? ActualStartDateTime { get; set; }
        public DateTime? ActualEndDateTime { get; set; }
        public string HandlerInstructions { get; set; }
        public List<ShipmentSplitStep> ShipmentSplitSteps { get; set; } = new List<ShipmentSplitStep>();
        public int? HandlerId { get; set; }
        public Company Handler { get; set; }
        public int? OwnerId { get; set; }
        public User Owner { get; set; }
        public string GroupKey { get; set; }
        public int? CategoryId { get; set; }
        public ShipmentStepCategory Category { get; set; }
        public int? StatusId { get; set; }
        [ForeignKey(nameof(StatusId))]
        public Status Status { get; set; }
        public DateTime? StatusChangeDateTime { get; set; }
        public int? VersionNumber { get; set; }
        public int? PairNumber { get; set; }
        public bool IsHot { get; set; }
        public List<WorkOrderDetail> Details { get; set; } = new List<WorkOrderDetail>();
        public List<WorkOrderDocument> Documents { get; set; } = new List<WorkOrderDocument>();
        public int? HandlerCommunicationProfileId { get; set; }
        public CommunicationProfile HandlerCommunicationProfile { get; set; }
        public string HandlerContactEmail { get; set; }
        public string InternalContactEmail { get; set; }
        public DateTime? SentToHandlerDateTime { get; set; }
        public int? TenantId { get; set; }
        public Tenant Tenant { get; set; }
    }

    public class WorkOrderLog : ChangeLogBase, IEntity { }

    public class WorkOrderPartyContact : PartyContact<WorkOrderParty>, ILogged<WorkOrderLog>, IEntity { }
    public class WorkOrderParty : Party<WorkOrderPartyContact>, ILogged<WorkOrderLog>, IEntity
    {
        [ForeignKey("Id")]
        public WorkOrder WorkOrder { get; set; }
    }

    public class WorkOrderDetail : IEntity, ILogged<WorkOrderLog>
    {
        public int Id { get; set; }
        public int WorkOrderId { get; set; }
        [CascadeDelete()]
        public WorkOrder WorkOrder { get; set; }
        public int ShipmentSplitDetailId { get; set; }
        [CascadeDelete()]
        public ShipmentSplitDetail ShipmentSplitDetail { get; set; }
    }

    public class WorkOrderDocument : IEntity, ILogged<WorkOrderLog>
    {
        public int Id { get; set; }
        public int WorkOrderId { get; set; }
        [CascadeDelete()]
        public WorkOrder WorkOrder { get; set; }
        public int TransportationDocumentId { get; set; }
        [CascadeDelete()]
        public TransportationDocument TransportationDocument { get; set; }
    }

}
