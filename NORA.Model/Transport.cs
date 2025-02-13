using Damco.Model;
using Damco.Model.Interfacing;
using Damco.Model.MultiTenancy;
using Damco.Model.UserManagement;
using NORA.Model.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORA.Model
{
    [Serializable()]
    public class TransportLog : ChangeLogBase, IEntity { }

    [Serializable()]
    public class Transport : IEntity, ILogged<TransportLog>, Damco.Model.Workflow.IWorkflowControlled, IInterfacedEntity, IStandardMeasurementContainer, IFinanceItem
    {
        public int Id { get; set; }
        public int? TransporterId { get; set; }
        public Company Transporter { get; set; }
        public int? FinanceVendorId { get; set; }
        public Company FinanceVendor { get; set; }
        public int? SubContracterId { get; set; }
        public Company SubContracter { get; set; }
        public string TransporterContactEmail { get; set; }
        [MaxLength(100)]
        public string TransporterContactName { get; set; }
        [MaxLength(100)]
        public string TransporterContactPhone { get; set; }
        public virtual List<TransportStop> Stops { get; set; } = new List<TransportStop>();
        public List<TransportPack> Packs { get; set; } = new List<TransportPack>();
        public int? StatusId { get; set; }
        public Damco.Model.Workflow.Status Status { get; set; }
        public DateTime? StatusChangeDateTime { get; set; }
        public DateTime? BookDateTime { get; set; }
        public DateTime? BookingConfirmedDateTime { get; set; }
        public bool AutomaticTransporterSelectionOverruled { get; set; }
        public DateTime CreationDateTime { get; set; }
        //public int? ShipmentGroupId { get; set; }
        //public ShipmentGroup ShipmentGroup { get; set; }
        public int? ModalityId { get; set; }
        public Modality Modality { get; set; }
        public int? OwnerId { get; set; }
        public string TransporterRemarks { get; set; }
        public string ShipperRemarks { get; set; }
        public bool ShipperRemarksHandled { get; set; }

        public string TransportReference { get; set; }
        public virtual List<TransportFinanceLine> FinanceLines { get; set; } = new List<TransportFinanceLine>();
        public ShipmentDirection? Direction { get; set; }
        //public bool HasPendingChangesFromInterface { get; set; }
        public bool HasPendingErrorsFromInterface { get; set; }
        public string BookingReference { get; set; }

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
        public bool VehicleNameChanged { get; set; }
        [MaxLength(50)]
        public string VoyageReference { get; set; }
        [MaxLength(50)]
        public string VoyageReferenceAdditional { get; set; }
        [MaxLength(50)]
        public string ExternalModalityCode { get; set; }

        [MaxLength(100)]
        public string DriverName { get; set; }
        [MaxLength(100)]
        public string DriverMobileNumber { get; set; }
        [MaxLength(100)]
        public string ChassisStyle { get; set; }
        public decimal? ChassisWeight { get; set; }
        [MaxLength(100)]
        public string CustomsNumber { get; set; }
        [MaxLength(100)]
        public string InsurancePolicyNumber { get; set; }

        public string UserDefinedText01 { get; set; }
        public string UserDefinedText02 { get; set; }
        public string UserDefinedText03 { get; set; }
        public string UserDefinedText04 { get; set; }
        public string UserDefinedText05 { get; set; }

        public DateTime? CYClosingDate { get; set; }
        public CarriageType? CarriageType { get; set; }

        public List<TransportDocument> Documents { get; set; } = new List<TransportDocument>();

        public List<TransportFile> Files { get; set; } = new List<TransportFile>();

        public decimal? Weight { get; set; }
        public decimal? Volume { get; set; }
        public decimal? Units { get; set; }
        public decimal? Cartons { get; set; }
        public decimal? Pallets { get; set; }
        public decimal? NetWeight { get; set; }
        public decimal? TotalGrossWeight { get; set; }

        public List<TransportDenialReason> DenialReasons { get; set; } = new List<TransportDenialReason>();

        public int? CarrierPlaceOfReceiptId { get; set; }
        public Company CarrierPlaceOfReceipt { get; set; }
        public int? CarrierPlaceOfDeliveryId { get; set; }
        public Company CarrierPlaceOfDelivery { get; set; }

        public int? CustomerPlaceOfReceiptId { get; set; }
        public Company CustomerPlaceOfReceipt { get; set; }
        public int? CustomerPlaceOfDeliveryId { get; set; }
        public Company CustomerPlaceOfDelivery { get; set; }

        [MaxLength(50)]
        public string ServiceContractNumber { get; set; }
        public int? VersionNumber { get; set; }

        [MaxLength(50)]
        public string ExternalTransporterCode { get; set; }

        public bool IsHot { get; set; }

        public int? CategoryId { get; set; }
        public ShipmentStepCategory Category { get; set; }
        public bool? ISFFilingDone { get; set; }
        public DateTime? ISFilingDoneDateTime { get; set; }

        //TODO : Remove RejectionReasonDescription and RejectionReasonCode as New Entity "TransportDenialReason" already exits
        [MaxLength(50)]
        public string RejectionReasonCode { get; set; }
        public string RejectionReasonDescription { get; set; }

        public int? DelegatedToId { get; set; }
        public InterfacingParty DelegatedTo { get; set; }

        public int? TransporterCommunicationProfileId { get; set; }
        public CommunicationProfile TransporterCommunicationProfile { get; set; }

        public bool WaiverRequired { get; set; }
        public bool WaiverDone { get; set; }

        public DateTime? SentToTransporterDateTime { get; set; }

        public string InternalContactEmail { get; set; }

        public bool ArrangeBooking { get; set; }
        public CarrierServiceType? PickUpCarrierServiceType { get; set; }
        public CarrierServiceType? DeliveryCarrierServiceType { get; set; }
        public int? PaymentTermId { get; set; }
        public PaymentTerm PaymentTerm { get; set; }
        public List<TransportEquipmentTransportSet> EquipmentTransportSets { get; set; } = new List<TransportEquipmentTransportSet>();
        public int? BookedUserId { get; set; }
        public User BookedUser { get; set; }
        public int? OfficeId { get; set; }
        public Company Office { get; set; }

        public int? VehicleId { get; set; }
        public Vehicle Vehicle { get; set; }
        public DateTime? Deadline { get; set; }

        public int? ProposalForTransportId { get; set; }
        [ForeignKey(nameof(ProposalForTransportId))]
        public Transport ProposalForTransport { get; set; }
        [ForeignKey(nameof(ProposalForTransportId))]
        public List<Transport> Proposals { get; set; } = new List<Transport>();
        public string RemarksFromTransporter { get; set; }
        public bool IsPreBooking { get; set; }
        public DateTime? CutOffDate { get; set; }
        public List<TransportParty> Parties { get; set; } = new List<TransportParty>();
        [MaxLength(50)]
        public string ExternalSystemUniqueId { get; set; }
        [MaxLength(50)]
        public string MessageId { get; set; }
        public string Commodity { get; set; }
        public int? TenantId { get; set; }
        public Tenant Tenant { get; set; }
        public List<TransportDangerousCargo> DangerousCargoes { get; set; } = new List<TransportDangerousCargo>();
        public string FinanceCalculationInfo { get; set; }
        public decimal? ChargeableWeight { get; set; }
        public int NumberOfRemindersSent { get; set; }
        public List<TransportHTS> HTSs { get; set; } = new List<TransportHTS>();
        public int? ActualTransporterFourPLId { get; set; }
        public Company ActualTransporterFourPL { get; set; }
        [Required]
        public bool AllowHBLNumeration { get; set; } = true;
        public bool HaulageCoordination { get; set; }
        [StringLength(50)]
        public string LoadingReference { get; set; }
    }

    public class TransportHTS : IEntity, ILogged<TransportLog>
    {
        public int Id { get; set; }
        [MaxLength(50), Required()]
        public string Code { get; set; }
        public int TransportId { get; set; }
        [CascadeDelete()]
        public Transport Transport { get; set; }
    }

    public interface IFinanceItem
    {
        string FinanceCalculationInfo { get; set; }
        int? FinanceVendorId { get; set; }
        string ServiceContractNumber { get; set; }
    }

    public class TransportDangerousCargo : IEntity, ILogged<TransportLog>, IDangerousCargo<TransportDangerousCargoAttribute>, IStandardMeasurementContainer
    {
        public int Id { get; set; }
        public int TransportId { get; set; }
        [CascadeDelete()]
        public Transport Transport { get; set; }
        public int? TypeId { get; set; }
        public DangerousCargoType Type { get; set; }
        [MaxLength(50)]
        public string ExternalIMDGHazardCode { get; set; }
        public int? IMDGHazardId { get; set; }
        public IMDGHazard IMDGHazard { get; set; }
        public int? UNDGCategoryId { get; set; }
        public UNDGCategory UNDGCategory { get; set; }
        public int? DangerousGoodsCategoryId { get; set; }
        public DangerousGoodsCategory DangerousGoodsCategory { get; set; }
        [MaxLength(50)]
        public string ClassificationCode { get; set; }
        [MaxLength(50)]
        public string ERGCode { get; set; }
        public decimal? Flashpoint { get; set; }
        public int? FlashpointUnitId { get; set; }
        public MeasurementUnit FlashpointUnit { get; set; }
        [MaxLength(50)]
        public string ExternalFlashpointUnitCode { get; set; }
        [MaxLength(50)]
        public string ExternalIMDGPackingGroupCode { get; set; }
        public int? IMDGPackingGroupId { get; set; }
        public IMDGPackingGroup IMDGPackingGroup { get; set; }
        public string EMSNumber { get; set; }
        public string PlacardUpperPart { get; set; }
        public string PlacardLowerPart { get; set; }
        public string LabelMarking1 { get; set; }
        public string LabelMarking2 { get; set; }
        public string LabelMarking3 { get; set; }
        [MaxLength(50)]
        public string EmergencyContactPhone { get; set; }
        [MaxLength(50)]
        public string EmergencyContactFax { get; set; }
        [MaxLength(50)]
        public string EmergencyContactMobile { get; set; }
        [MaxLength(200)]
        public string EmergencyContactEmail { get; set; }
        public string EmergencyContactInformation { get; set; }
        public List<TransportDangerousCargoAttribute> Attributes { get; set; } = new List<TransportDangerousCargoAttribute>();

        public string SegregationGroup { get; set; }
        public decimal? ControlTemperature { get; set; }
        public int? ControlTemperatureUnitId { get; set; }
        public MeasurementUnit ControlTemperatureUnit { get; set; }
        public decimal? EmergencyTemperature { get; set; }
        public int? EmergencyTemperatureUnitId { get; set; }
        public MeasurementUnit EmergencyTemperatureUnit { get; set; }
        public bool LimitedQuantity { get; set; }

        public bool? MarinePollutant { get; set; }
        public bool? EnvironmentallyHazardous { get; set; }
        public bool? Viscous { get; set; }
        public bool? ExtraViscous { get; set; }
        public int? DangerousCargoMeasurementTypeId { get; set; }

        public DangerousCargoMeasurementType DangerousCargoMeasurementType { get; set; }
        public decimal? Weight { get; set; }
        public decimal? Volume { get; set; }
        public decimal? Units { get; set; }
        public decimal? Cartons { get; set; }
        public decimal? Pallets { get; set; }
        public decimal? NetWeight { get; set; }
        public decimal? TotalGrossWeight { get; set; }
        public string ProperShippingName { get; set; }
        public decimal? ChargeableWeight { get; set; }
        public DangerousGoodsAggregationState? DangerousGoodsAggregationState { get; set; }
    }

    public class TransportDangerousCargoAttribute : AttributeBase<TransportDangerousCargo>, IEntity, ILogged<ShipmentLog>, IDangerousCargoAttribute
    {
    }

    public class TransportDenialReason : IEntity, ILogged<TransportLog>
    {
        public int Id { get; set; }
        public int TransportId { get; set; }
        public Transport Transport { get; set; }
        public int TransporterId { get; set; }
        public Company Transporter { get; set; }
        public DateTime DateTime { get; set; }
        public string Reason { get; set; }
        public string ReasonCode { get; set; }
        public int? HandlerPartyId { get; set; }
        public HandlingParty HandlerParty { get; set; }
    }

    public class TransportFile : IEntity, ILogged<TransportLog>
    {
        public int Id { get; set; }
        public int TransportId { get; set; }
        public Transport Transport { get; set; }
        [Required()]
        public byte[] Content { get; set; }
        [Required()]
        public string FileName { get; set; }
    }

    public class TransportHistory : EntityHistory<Transport>
    {
        public DateTime? PickUpDateTime { get; set; }
        public DateTime? DeliveryDateTime { get; set; }
    }
    [Serializable()]
    public class TransportStop : WorkOrder, IEntity, ILogged<TransportLog>
    {
        public int TransportId { get; set; }
        public virtual Transport Transport { get; set; }
        public bool IsFirstInTransport { get; set; }
        public bool IsLastInTransport { get; set; }
        public int? LegId { get; set; }
        public TransportLeg Leg { get; set; }
    }

    public class TransportLeg : IEntity, ILogged<TransportLog>
    {
        public int Id { get; set; }
        public int? VehicleId { get; set; }
        public Vehicle Vehicle { get; set; }
        [MaxLength(100)]
        public string VehicleName { get; set; }
        [MaxLength(50)]
        public string VehicleReference { get; set; }
        [MaxLength(50)]
        public string VoyageReference { get; set; }
        public bool? VehicleChanged { get; set; }
        public List<TransportStop> Stops { get; set; } = new List<TransportStop>();
        public int? SubJourneyId { get; set; }
        public SubJourney SubJourney { get; set; }
    }

    public class TransportPartyContact : PartyContact<TransportParty>, ILogged<TransportLog>, IEntity { }
    public class TransportParty : Party<TransportPartyContact>, ILogged<TransportLog>, IEntity, IPartyWithSpecialPartyType, IPartyWithDocumentAddressBlock
    {
        public int TransportId { get; set; }
        [CascadeDelete()]
        public Transport Transport { get; set; }
        public string DocumentAddressBlockHeader { get; set; }
        public string DocumentAddressBlockBody { get; set; }
        public SpecialPartyType? SpecialPartyType { get; set; }
    }
    public class TransportAttribute : AttributeBase<Transport>, ILogged<TransportLog>, IEntity { }
    public class TransportDate : DateBase<Transport>, ILogged<TransportLog>, IEntity { }
    public class TransportMeasurement : MeasurementBase<Transport>, ILogged<TransportLog>, IEntity { }

    //TODO: Call it event?
    [Serializable()]
    public class TransportEvent : IEntity, ILogged<TransportLog>
    {
        public int Id { get; set; }
        public int TransportId { get; set; }
        public Transport Transport { get; set; }
        public int? TransportStopId { get; set; }
        public TransportStop TransportStop { get; set; }
        public int? TransportPackId { get; set; }
        public TransportPack TransportPack { get; set; }
        public string TransporterMilestoneCode { get; set; }
        public string Description { get; set; }
        public string Remarks { get; set; }
        public DateTime DateTime { get; set; }
        public TransportEventType Type { get; set; }
    }

    [Serializable()]
    public enum TransportEventType
    {
        DeliveryCompleted = 1,
        DeliveryFailed = 2,
        InProgress = 3
    }

    [Serializable()]
    public class TransportPack : IEntity, ILogged<TransportLog>
    {
        public int Id { get; set; }
        public int TransportId { get; set; }
        public Transport Transport { get; set; }
        public int ShipmentPackId { get; set; }
        public ShipmentPack ShipmentPack { get; set; }
        [MaxLength(100)]
        public string TrackingNumber { get; set; }
        public List<TransportEvent> Events { get; set; }

    }

    public abstract class EquipmentTransportSetBase<Tmeasurements> : IStandardMeasurementContainer, IEquipmentTransportMeasurementContainer
    {
        public int Id { get; set; }
        public int TypeId { get; set; }
        public EquipmentTransportType Type { get; set; }
        public int NumberOfEquipmentTransports { get; set; }
        public decimal? RequiredTemperature { get; set; } //Canonical
        public List<Tmeasurements> Measurements { get; set; } = new List<Tmeasurements>();
        public decimal? Weight { get; set; }
        public decimal? Volume { get; set; }
        public decimal? Units { get; set; }
        public decimal? Cartons { get; set; }
        public decimal? Pallets { get; set; }
        public decimal? NetWeight { get; set; }
        public decimal? TotalGrossWeight { get; set; }
        public decimal? TareWeight { get; set; }
        public EquipmentTransportTypeFamily? Family { get; set; }

        public int? SubstitutedTypeId { get; set; }
        public EquipmentTransportType SubstitutedType { get; set; }
        public int? NumberOfSubstitutedEquipmentTransports { get; set; }
        public decimal? ChargeableWeight { get; set; }
    }

    public class TransportEquipmentTransportSet : EquipmentTransportSetBase<TransportEquipmentTransportSetMeasurement>, IEntity, ILogged<TransportLog>
    {
        public int TransportId { get; set; }
        public Transport Transport { get; set; }
        public List<ShipmentGroupTransportEquipmentTransportSet> ShipmentGroupEquipmentTransportSets { get; set; } = new List<ShipmentGroupTransportEquipmentTransportSet>();
    }

    public class TransportEquipmentTransportSetMeasurement : MeasurementBase<TransportEquipmentTransportSet>, IEntity, ILogged<TransportLog>
    {
    }

}
