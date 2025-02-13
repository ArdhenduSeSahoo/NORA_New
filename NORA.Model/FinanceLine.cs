using Damco.Model;
using Damco.Model.UserManagement;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORA.Model
{
    public class FinanceLine : IEntity, ILogged<FinanceLog>, IUpdateTracking
    {
        public int Id { get; set; }
        public decimal? Amount { get; set; }
        public FinanceLineType Type { get; set; }
        public int ChargeId { get; set; }
        public Charge Charge { get; set; }
        [MaxLength(50)]
        public string ChargeCode { get; set; }
        public int? CurrencyId { get; set; }
        public Currency Currency { get; set; }
        [MaxLength(50)]
        public string CustomerChargeFinancialCode { get; set; }
        public decimal? Quantity { get; set; }
        public int? QuantityUnitId { get; set; }
        public MeasurementUnit QuantityUnit { get; set; }
        public decimal? RatePerUnit { get; set; }
        public string Comments { get; set; }
        public bool ChangedSinceApproval { get; set; }
        public int? FinanceCalculationOperationPartId { get; set; }
        public FinanceCalculationOperationPart FinanceCalculationOperationPart { get; set; } //TODO: Add once this table is added to the DB
        public bool ManualDetailsEntry { get; set; }
        public int? OwnerId { get; set; }
        public User Owner { get; set; }
        public int? FinanceReasonId { get; set; }
        public FinanceReason Reason { get; set; }
        public int? EquipmentTransportTypeId { get; set; }
        public EquipmentTransportType EquipmentTransportType { get; set; }
        public int? VendorId { get; set; }
        public Company Vendor { get; set; }
        public int? CategoryId { get; set; }
        public FinanceLineCategory Category { get; set; }
        public int? RelevantDocumentId { get; set; }
        public TransportationDocument RelevantDocument { get; set; }
        public DateTime EditDateTime { get; set; }
        public User EditUser { get; set; }
        public int? EditUserId { get; set; }
        public int? RateContractId { get; set; }
        public RateContract RateContract { get; set; }
        public bool CalculatedWithMissingInformation { get; set; }
        public bool LockedForChanges { get; set; }
        public bool IsApproved { get; set; }
        public bool IsDisabled { get; set; }
    }

    public class FinanceLineHistory : EntityHistory<FinanceLine>
    {
        public int FinanceOrderStatusId { get; set; } //TODO: Remove
        public bool Approved { get; set; }
    }

    public class ShipmentGroupFinanceLine : FinanceLine, IEntity, ILogged<FinanceLog>
    {
        public int ShipmentGroupId { get; set; }
        [CascadeDelete()]
        public ShipmentGroup ShipmentGroup { get; set; }
    }

    public class TransportFinanceLine : FinanceLine, IEntity, ILogged<FinanceLog>
    {
        public int TransportId { get; set; }
        [CascadeDelete()]
        public Transport Transport { get; set; }
    }

    public class FinanceLog : ChangeLogBase, IEntity
    {
    }

    public enum FinanceLineType
    {
        Cost = 1,
        Income = 2,
        Passthrough = 3
    }
}
