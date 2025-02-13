using Damco.Model;
using Damco.Model.Interfacing;
using Damco.Model.UserManagement;
using Damco.Model.Workflow;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORA.Model
{
    public class FinanceOrder : IWorkflowControlled, IEntity, ILogged<FinanceLog>, IInterfacedEntity
    {
        public int? VersionNumber { get; set; }
        public int Id { get; set; }
        public int? StatusId { get; set; }
        public Status Status { get; set; }
        public DateTime? StatusChangeDateTime { get; set; }
        public List<FinanceOrderShipmentGroup> ShipmentGroups { get; set; } = new List<FinanceOrderShipmentGroup>();
        public string ProofOfCustomerApprovalLocation { get; set; }
        public string Comments { get; set; }
        public int? OwnerId { get; set; }
        public User Owner { get; set; }
        public int? ApproverId { get; set; }
        public User Approver { get; set; }
        public string Reference { get; set; }
        public int? CustomerId { get; set; }
        public Company Customer { get; set; }
        public int? PayerPartyId { get; set; }
        public FinanceParty PayerParty { get; set; }
        public int? OfficeId { get; set; }
        public Company Office { get; set; }
        //public bool HasPendingChangesFromInterface { get; set; }
        public bool HasPendingErrorsFromInterface { get; set; }
        public bool IsHot { get; set; }
        public DateTime? TriggerDateTime { get; set; }
        public string InternalContactEmail { get; set; }
        public bool PreventAutoCalculation { get; set; }
        public DateTime? CalculationTriggerDateTime { get; set; }
        public bool? EmailNotification { get; set; }
    }

    public class FinancePartyContact : PartyContact<FinanceParty>, ILogged<FinanceLog>, IEntity { }
    public class FinanceParty : Party<FinancePartyContact>, ILogged<FinanceLog>, IEntity
    {
    }

    public class FinanceOrderShipmentGroup : IEntity, ILogged<FinanceLog>
    {
        public int Id { get; set; }
        public int ShipmentGroupId { get; set; }
        [CascadeDelete()]
        public ShipmentGroup ShipmentGroup { get; set; }
        public int FinanceOrderId { get; set; }
        [CascadeDelete()]
        public FinanceOrder FinanceOrder { get; set; }
    }
}
