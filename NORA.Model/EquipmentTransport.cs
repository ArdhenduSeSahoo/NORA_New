using Damco.Model;
using Damco.Model.Interfacing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORA.Model
{
    public class EquipmentTransport : IEntity, ILogged<ShipmentLog>, IStandardMeasurementContainer, IEquipmentTransportMeasurementContainer, IInterfacedEntity
    {
        public int Id { get; set; }

        [MaxLength(50)]
        public string UniqueId { get; set; }

        [ForeignKey(nameof(ShipmentSplit.RequestedEquipmentTransportId))]
        public List<ShipmentSplit> RequestedShipmentSplits { get; set; } = new List<ShipmentSplit>();

        [ForeignKey(nameof(ShipmentSplit.PlannedEquipmentTransportId))]
        public List<ShipmentSplit> PlannedShipmentSplits { get; set; } = new List<ShipmentSplit>();

        [ForeignKey(nameof(ShipmentSplit.ActualEquipmentTransportId))]
        public List<ShipmentSplit> ActualShipmentSplits { get; set; } = new List<ShipmentSplit>();
        [MaxLength(50)]
        public string EquipmentIdentification { get; set; }
        [MaxLength(50)]
        public string TransportIdentification { get; set; } //e.g. seal number
        [MaxLength(50)]
        public string TransportIdentification2 { get; set; }
        [MaxLength(50)]
        public string TransportIdentification3 { get; set; }

        public int TypeId { get; set; }
        public EquipmentTransportType Type { get; set; }
        public List<EquipmentTransportAttribute> Attributes { get; set; } = new List<EquipmentTransportAttribute>();
        public List<EquipmentTransportDate> Dates { get; set; } = new List<EquipmentTransportDate>();
        public List<EquipmentTransportMeasurement> Measurements { get; set; } = new List<EquipmentTransportMeasurement>();
        public List<EquipmentTransportParty> Parties { get; set; } = new List<EquipmentTransportParty>();
        public decimal? RequiredTemperature { get; set; }
        public MovementType? PickUpMovementType { get; set; }
        public MovementType? DeliveryMovementType { get; set; }
        public HaulageArrangement? PickUpHaulageArrangement { get; set; }
        public bool? HasEmptyPickUp { get; set; }
        public HaulageArrangement? DeliveryHaulageArrangement { get; set; }
        public CarrierServiceType? PickUpCarrierServiceType { get; set; }
        public CarrierServiceType? DeliveryCarrierServiceType { get; set; }
        public EquipmentTransportSolasInfo Solas { get; set; }
        public string ShipperRemarks { get; set; }

        public decimal? Weight { get; set; }
        public decimal? Volume { get; set; }
        public decimal? Units { get; set; }
        public decimal? Cartons { get; set; }
        public decimal? Pallets { get; set; }
        public decimal? NetWeight { get; set; }
        public decimal? TareWeight { get; set; }
        public decimal? TotalGrossWeight { get; set; }
        public EquipmentSupplier? EquipmentSupplier { get; set; }
        public SealingParty? SealingParty { get; set; }
        public decimal? BookedVolume { get; set; }
        public decimal? BookedWeight { get; set; }
        public EquipmentTransportTypeFamily? Family { get; set; }

        public List<ShipmentGroupEquipmentTransport> ShipmentGroups { get; set; } = new List<ShipmentGroupEquipmentTransport>();

        public List<EquipmentTransportEvent> Events { get; set; } = new List<EquipmentTransportEvent>();
        public decimal? ChargeableWeight { get; set; }

        public int? SubTypeId { get; set; }
        public EquipmentTransportSubType SubType { get; set; }

        public int? AdditionalTypeId { get; set; }
        public EquipmentTransportAdditionalType AdditionalType { get; set; }

        public bool IsHot { get; set; }
        public bool HasPendingErrorsFromInterface { get; set; }
        public decimal? CalculatedVolume { get; set; }
    }

    public class EquipmentTransportCombo : IEntity
    {
        public int Id { get; set; }
    }

    public enum EquipmentSupplier
    {
        Carrier = 1,
        Shipper = 2
    }

    public interface IEquipmentTransportMeasurementContainer
    {
        decimal? TareWeight { get; set; }
        decimal? RequiredTemperature { get; set; }
    }

    public enum SealingParty
    {
        Carrier = 1,
        Shipper = 2,
        Customs = 3,
        TerminalOperator = 4,
        QuarantineAgency = 5
    }

    //AKO removed weighing party because it was wrongly designed
    //public enum WeighingParty
    //{
    //    Shipper = 2 //Align with sealing party
    //}

    public class EquipmentTransportSolasInfo : IEntity, ILogged<ShipmentLog>
    {
        [Key()]
        public int EquipmentTransportId { get; set; }
        public EquipmentTransport EquipmentTransport { get; set; }
        int ILogged.Id { get { return this.EquipmentTransportId; } }
        public decimal? EquipmentGrossWeight { get; set; }
        public int? EquipmentGrossWeightUnitId { get; set; }
        public MeasurementUnit EquipmentGrossWeightUnit { get; set; }
        [MaxLength(50)]
        public string ExternalEquipmentGrossWeightUnitCode { get; set; }
        public string SafetyPhrase { get; set; }
        public string CountrySpecificInformation { get; set; }
        public string AuthorizedOfficialName { get; set; }
        public DateTime? VGMDeterminationDateTime { get; set; }
        //public WeighingParty? WeighingParty { get; set; }
        public int? VGMDeterminationMethod { get; set; }
    }

    public enum MovementType
    {
        Bulk,
        FCL,
        LCL,
        FTL,
        LTL
    }

    public enum HaulageArrangement
    {
        Merchant = 0,
        Carrier = 1
    }

    public enum CarrierServiceType
    {
        Port,
        Door
    }

    public class EquipmentTransportPartyContact : PartyContact<EquipmentTransportParty>, ILogged<ShipmentLog>, IEntity { }
    public class EquipmentTransportParty : Party<EquipmentTransportPartyContact>, ILogged<ShipmentLog>, IEntity
    {
        public int EquipmentTransportId { get; set; }
        [CascadeDelete()]
        public EquipmentTransport EquipmentTransport { get; set; }
    }

    public class EquipmentTransportMeasurement : MeasurementBase<EquipmentTransport>, ILogged<ShipmentLog>, IEntity
    {
        public List<EquipmentTransportMeasurementPart> Parts { get; set; } = new List<EquipmentTransportMeasurementPart>();
    }

    public class EquipmentTransportMeasurementPart : MeasurementBase, ILogged<ShipmentLog>, IEntity
    {
        [CascadeDelete()]
        public EquipmentTransportMeasurement EquipmentTransportMeasurement { get; set; }
        [ForceUnique("ParentAndCorrelationId")]
        public int EquipmentTransportMeasurementId { get; set; }
        [Required(), MaxLength(50), ForceUnique("ParentAndCorrelationId")]
        public string CorrelationId { get; set; }
    }

    public class EquipmentTransportAttribute : AttributeBase<EquipmentTransport>, ILogged<ShipmentLog>, IEntity { }
    public class EquipmentTransportDate : DateBase<EquipmentTransport>, ILogged<ShipmentLog>, IEntity { }

    public class ShipmentGroupEquipmentTransport : IEntity, ILogged<ShipmentGroupLog>
    {
        public int Id { get; set; }
        public int ShipmentGroupId { get; set; }
        [CascadeDelete()]
        public ShipmentGroup ShipmentGroup { get; set; }
        public int EquipmentTransportId { get; set; }
        [CascadeDelete()]
        public EquipmentTransport EquipmentTransport { get; set; }
    }

    public class EquipmentTransportEvent : IEntity, ILogged<ShipmentLog>, IInterfacedEntity
    {
        public int Id { get; set; }
        public int EquipmentTransportId { get; set; }
        public EquipmentTransport EquipmentTransport { get; set; }
        public int EventTypeId { get; set; }
        public EventType EventType { get; set; }
        public DateTime? PlannedDateTime { get; set; }
        public DateTime? ActualDateTime { get; set; }
        public bool HasPendingErrorsFromInterface { get; set; }
        public int? CompanyId { get; set; }
        public Company Company { get; set; }
        public int? TriggeringEquipmentTransportId { get; set; }
        public int? UserId { get; set; }
    }

}
