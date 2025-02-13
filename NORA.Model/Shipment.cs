using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Damco.Model;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Damco.Model.Interfacing;
using Damco.Model.Workflow;
using Damco.Model.MultiTenancy;

namespace NORA.Model
{
    public class ShipmentLog : ChangeLogBase, IEntity { }

    public class Shipment : IEntity, ILogged<ShipmentLog>, IWorkflowControlled, IStandardMeasurementContainer, IInterfacedEntity
    {
        public int? VersionNumber { get; set; }
        public int Id { get; set; }

        #region References
        [MaxLength(50)]
        public string CSNumber { get; set; }

        public int CustomerId { get; set; }
        public Company Customer { get; set; }
        //public int? EndCustomerPartyId { get; set; }
        //public virtual ShipmentParty EndCustomerParty { get; set; }
        //public int? ConsigneePartyId { get; set; }
        //public virtual ShipmentParty ConsigneeParty { get; set; }
        //public int? ShipperPartyId { get; set; }
        //public ShipmentParty ShipperParty { get; set; }

        [MaxLength(50)]
        public string ShipmentReference { get; set; }
        #endregion

        public decimal? Weight { get; set; }
        public decimal? Volume { get; set; }
        public decimal? Units { get; set; }
        public decimal? Cartons { get; set; }
        public decimal? Pallets { get; set; }
        public decimal? TotalGrossWeight { get; set; }

        public DateTime? OrderReceivedDateTime { get; set; }

        #region User defined
        [MaxLength(100)]
        public string UserDefinedText01 { get; set; }
        [MaxLength(100)]
        public string UserDefinedText02 { get; set; }
        [MaxLength(100)]
        public string UserDefinedText03 { get; set; }
        [MaxLength(100)]
        public string UserDefinedText04 { get; set; }
        [MaxLength(100)]
        public string UserDefinedText05 { get; set; }
        [MaxLength(100)]
        public string UserDefinedText06 { get; set; }
        [MaxLength(100)]
        public string UserDefinedText07 { get; set; }
        [MaxLength(100)]
        public string UserDefinedText08 { get; set; }
        [MaxLength(100)]
        public string UserDefinedText09 { get; set; }
        [MaxLength(100)]
        public string UserDefinedText10 { get; set; }

        public string UserDefinedText11 { get; set; }
        public string UserDefinedText12 { get; set; }
        public string UserDefinedText13 { get; set; }
        public string UserDefinedText14 { get; set; }
        public string UserDefinedText15 { get; set; }
        public string UserDefinedText16 { get; set; }
        public string UserDefinedText17 { get; set; }
        public string UserDefinedText18 { get; set; }
        public string UserDefinedText19 { get; set; }
        public string UserDefinedText20 { get; set; }

        public DateTime? UserDefinedDateTime01 { get; set; }
        public DateTime? UserDefinedDateTime02 { get; set; }
        public DateTime? UserDefinedDateTime03 { get; set; }
        public DateTime? UserDefinedDateTime04 { get; set; }
        public DateTime? UserDefinedDateTime05 { get; set; }
        public DateTime? UserDefinedDateTime06 { get; set; }
        public DateTime? UserDefinedDateTime07 { get; set; }
        public DateTime? UserDefinedDateTime08 { get; set; }
        public DateTime? UserDefinedDateTime09 { get; set; }
        public DateTime? UserDefinedDateTime10 { get; set; }
        public DateTime? UserDefinedDateTime11 { get; set; }
        public DateTime? UserDefinedDateTime12 { get; set; }
        public DateTime? UserDefinedDateTime13 { get; set; }
        public DateTime? UserDefinedDateTime14 { get; set; }
        public DateTime? UserDefinedDateTime15 { get; set; }
        #endregion

        public decimal? UserDefinedNumber01 { get; set; }
        public decimal? UserDefinedNumber02 { get; set; }
        public decimal? UserDefinedNumber03 { get; set; }
        public decimal? UserDefinedNumber04 { get; set; }
        public decimal? UserDefinedNumber05 { get; set; }

        #region Children
        [ForeignKey("ShipmentId")]
        public virtual List<ShipmentStep> Steps { get; set; } = new List<ShipmentStep>();
        [ForeignKey("ShipmentId")]
        public virtual List<ShipmentDetail> Details { get; set; } = new List<ShipmentDetail>();
        public virtual List<ShipmentPack> Packs { get; set; } = new List<ShipmentPack>();
        public List<ShipmentSplit> Splits { get; set; } = new List<ShipmentSplit>();
        public List<ShipmentAttribute> Attributes { get; set; } = new List<ShipmentAttribute>();
        public List<ShipmentDate> Dates { get; set; } = new List<ShipmentDate>();
        public List<ShipmentMeasurement> Measurements { get; set; } = new List<ShipmentMeasurement>();
        #endregion
        [ForeignKey("ShipmentId")]
        public List<ShipmentParty> Parties { get; set; } = new List<ShipmentParty>();
        public int? OfficeId { get; set; }
        public Company Office { get; set; }

        public int? StatusId { get; set; }
        public Status Status { get; set; }
        public DateTime? StatusChangeDateTime { get; set; }

        public decimal? NetWeight { get; set; }

        public int? PaymentTermId { get; set; }
        public PaymentTerm PaymentTerm { get; set; }
        public MovementType? PickUpMovementType { get; set; }
        public MovementType? DeliveryMovementType { get; set; }
        public DateTime? ExpectedReceiptDateTime { get; set; }
        [MaxLength(50)]
        public string SourceId { get; set; }
        [MaxLength(50)]
        public string SourceSystem { get; set; }
        public int? TenantId { get; set; }
        public Tenant Tenant { get; set; }
        public string BookingRemarks { get; set; }
        public bool HasPendingErrorsFromInterface { get; set; }
        [MaxLength(50)]
        public string InternalBookingNumber { get; set; }
        public decimal? ChargeableWeight { get; set; }

        public EquipmentTransportTypeFamily? EquipmentTransportTypeFamily { get; set; }

        public bool FourPL { get; set; }
        public DateTime? ActualReceiptDateTime { get; set; }
    }

    public class ShipmentPartyContact : PartyContact<ShipmentParty>, ILogged<ShipmentLog>, IEntity { }
    public class ShipmentParty : Party<ShipmentPartyContact>, ILogged<ShipmentLog>, IEntity
    {
        public int? ShipmentId { get; set; }
        [CascadeDelete()]
        public Shipment Shipment { get; set; }
    }

    public class ShipmentAttribute : AttributeBase<Shipment>, ILogged<ShipmentLog>, IEntity { }
    public class ShipmentDate : DateBase<Shipment>, ILogged<ShipmentLog>, IEntity { }
    public class ShipmentMeasurement : MeasurementBase<Shipment>, ILogged<ShipmentLog>, IEntity { }

}
