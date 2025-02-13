using Damco.Model;
using Damco.Model.Interfacing;
using Damco.Model.MultiTenancy;
using Damco.Model.UserManagement;
using Damco.Model.Workflow;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORA.Model
{
    public class ShipmentGroupLog : ChangeLogBase, IEntity { }

    public class ShipmentGroup : IEntity, IInterfacedEntity, ILogged<ShipmentGroupLog>, IStandardMeasurementContainer, IWorkflowControlled, IPossiblyLimitedWorfklow, IFinanceItem
    {
        public virtual int Id { get; set; }
        [MaxLength(50)]
        public virtual string Reference { get; set; }
        [MaxLength(50)]
        public string CorrelationId { get; set; }
        public int? ModalityId { get; set; }
        public Modality Modality { get; set; }
        public virtual List<ShipmentGroupFinanceLine> FinanceLines { get; set; } = new List<ShipmentGroupFinanceLine>();
        //public bool HasPendingChangesFromInterface { get; set; }
        public bool HasPendingErrorsFromInterface { get; set; }
        //public List<Transport> Transports { get; set; } = new List<Transport>();
        public List<ShipmentGroupAttribute> Attributes { get; set; } = new List<ShipmentGroupAttribute>();
        public List<ShipmentGroupDate> Dates { get; set; } = new List<ShipmentGroupDate>();
        public List<ShipmentGroupMeasurement> Measurements { get; set; } = new List<ShipmentGroupMeasurement>();
        public List<ShipmentGroupParty> Parties { get; set; } = new List<ShipmentGroupParty>();
        public List<ShipmentGroupDocument> Documents { get; set; } = new List<ShipmentGroupDocument>();
        public List<ShipmentGroupEquipmentTransport> EquipmentTransports { get; set; } = new List<ShipmentGroupEquipmentTransport>();
        [MaxLength(50)]
        public string Version { get; set; }
        [MaxLength(50)]
        public string VehicleReference { get; set; }
        [MaxLength(50)]
        public string VehicleReferenceAdditional { get; set; }
        [MaxLength(50)]
        public string VehicleReferenceAdditional2 { get; set; }
        [MaxLength(100)]
        public string VehicleName { get; set; }
        [MaxLength(50)]
        public string VoyageReference { get; set; }
        public int? TransporterId { get; set; }
        public Company Transporter { get; set; }
        public int? SubContracterId { get; set; }
        public Company SubContracter { get; set; }
        [MaxLength(50)]
        public string ExternalModalityCode { get; set; }
        public ShipmentDirection? Direction { get; set; }
        public string BookingReference { get; set; }
        public DateTime? CYClosingDate { get; set; }
        public CarriageType? CarriageType { get; set; }

        public decimal? Weight { get; set; }
        public decimal? Volume { get; set; }
        public decimal? Units { get; set; }
        public decimal? Cartons { get; set; }
        public decimal? Pallets { get; set; }
        public decimal? NetWeight { get; set; }
        public decimal? TotalGrossWeight { get; set; }

        [MaxLength(50)]
        public string ExternalTransporterCode { get; set; }

        public List<ShipmentSplit> ShipmentSplits { get; set; } = new List<ShipmentSplit>();

        public int? CarrierPlaceOfReceiptId { get; set; }
        public Company CarrierPlaceOfReceipt { get; set; }
        public int? CarrierPlaceOfDeliveryId { get; set; }
        public Company CarrierPlaceOfDelivery { get; set; }

        public int? CustomerPlaceOfReceiptId { get; set; }
        public Company CustomerPlaceOfReceipt { get; set; }
        public int? CustomerPlaceOfDeliveryId { get; set; }
        public Company CustomerPlaceOfDelivery { get; set; }

        public bool IsHot { get; set; }
        public int? StatusId { get; set; }
        public Status Status { get; set; }
        public DateTime? StatusChangeDateTime { get; set; }
        public int? VersionNumber { get; set; }

        public int? OwnerId { get; set; }
        public User Owner { get; set; }

        public List<ExpectedShipmentGroup> ExpectedGroups { get; set; } = new List<ExpectedShipmentGroup>();

        public int? ComboId { get; set; }
        public ShipmentGroupCombo Combo { get; set; }

        public List<FinanceOrderShipmentGroup> FinanceOrders { get; set; } = new List<FinanceOrderShipmentGroup>();

        //TODO Remove and find better way to update all the steps
        public int? EmptyPickUpLocationId { get; set; }
        public Company EmptyPickUpLocation { get; set; }

        //public List<WorkOrder> WorkOrders { get; set; } = new List<WorkOrder>();

        [MaxLength(50)]
        public string ServiceContractNumber { get; set; }

        public bool DocumentTypesSwitched { get; set; }
        public List<ShipmentGroupComment> Comments { get; set; }

        public bool ArrangeBooking { get; set; }
        public bool LimitedWorkflow { get; set; }
        public bool AllowDetailComboLogic { get; set; } = true;
        public int? VehicleId { get; set; }
        public Vehicle Vehicle { get; set; }
        public DateTime? MissingInfoMessageSendDateTime { get; set; }
        public string FinanceCalculationInfo { get; set; }

        public List<ShipmentGroupEquipmentTransportSet> EquipmentTransportSets { get; set; } = new List<ShipmentGroupEquipmentTransportSet>();

        [MaxLength(50)]
        public string SourceId { get; set; }
        [MaxLength(50)]
        public string SourceSystem { get; set; }
        public int? TenantId { get; set; }
        public Tenant Tenant { get; set; }
        public int? FinanceVendorId { get; set; }
        public Company FinanceVendor { get; set; }
        public decimal? ChargeableWeight { get; set; }
        public bool NeedsSideLoader { get; set; }
   
        [MaxLength(50)]
        public string SourceFileType { get; set; }
    }

    public class ShipmentGroupComment : IEntity, ILogged<ShipmentGroupLog>, IUpdateTracking
    {
        public int Id { get; set; }
        public int ShipmentGroupId { get; set; }
        public ShipmentGroup ShipmentGroup { get; set; }
        public DateTime EditDateTime { get; set; }
        public User EditUser { get; set; }
        public int? EditUserId { get; set; }
        [Required()]
        public string Comment { get; set; }
    }

    public class ShipmentGroupCombo : IEntity
    {
        public int Id { get; set; }
        public List<ShipmentGroup> ShipmentGroups { get; set; }
    }

    public enum ShipmentDirection
    {
        Import,
        Export
    }

    public enum CarriageType
    {
        House,
        Direct
    }

    public class ShipmentGroupPartyContact : PartyContact<ShipmentGroupParty>, ILogged<ShipmentGroupLog>, IEntity { }
    public class ShipmentGroupParty : Party<ShipmentGroupPartyContact>, ILogged<ShipmentGroupLog>, IEntity
    {
        public int ShipmentGroupId { get; set; }
        [CascadeDelete()]
        public ShipmentGroup ShipmentGroup { get; set; }
    }
    public class ShipmentGroupAttribute : AttributeBase<ShipmentGroup>, ILogged<ShipmentGroupLog>, IEntity { }
    public class ShipmentGroupDate : DateBase<ShipmentGroup>, ILogged<ShipmentGroupLog>, IEntity { }
    public class ShipmentGroupMeasurement : MeasurementBase<ShipmentGroup>, ILogged<ShipmentGroupLog>, IEntity { }


    public class ShipmentGroupEquipmentTransportSet : EquipmentTransportSetBase<ShipmentGroupEquipmentTransportSetMeasurement>, IEntity, ILogged<ShipmentGroupLog>
    {
        public int ShipmentGroupId { get; set; }
        public ShipmentGroup ShipmentGroup { get; set; }
        public List<ShipmentGroupTransportEquipmentTransportSet> TransportEquipmentTransportSets { get; set; } = new List<ShipmentGroupTransportEquipmentTransportSet>();
    }

    public class ShipmentGroupTransportEquipmentTransportSet : IEntity, ILogged<ShipmentGroupLog>
    {
        public int Id { get; set; }
        public int ShipmentGroupEquipmentTransportSetId { get; set; }
        [CascadeDelete()]
        public ShipmentGroupEquipmentTransportSet ShipmentGroupEquipmentTransportSet { get; set; }
        public int TransportEquipmentTransportSetId { get; set; }
        [CascadeDelete()]
        public TransportEquipmentTransportSet TransportEquipmentTransportSet { get; set; }
        public int NumberOfEquipmentTransports { get; set; }
    }

    public class ShipmentGroupEquipmentTransportSetMeasurement : MeasurementBase<ShipmentGroupEquipmentTransportSet>, IEntity, ILogged<ShipmentGroupLog>
    {
    }

}
