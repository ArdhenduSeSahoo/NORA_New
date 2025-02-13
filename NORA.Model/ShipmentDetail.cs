using Damco.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NORA.Model.Base;

namespace NORA.Model
{
    public class ShipmentDetail : IEntity, ILogged<ShipmentLog>, IDeletedProperties, IStandardMeasurementContainer
    {
        public int Id { get; set; }

        public int ShipmentId { get; set; }
        public Shipment Shipment { get; set; }

        [MaxLength(200)]
        public string UniqueId { get; set; }

        #region References
        [MaxLength(50)]
        public string SONumber { get; set; }
        [MaxLength(50)]
        public string PONumber { get; set; }
        [MaxLength(50)]
        public string SKU { get; set; }
        [MaxLength(50)]
        public string LotNumber { get; set; }
        [MaxLength(100)]
        public string LotAttribute01 { get; set; }
        [MaxLength(100)]
        public string LotAttribute02 { get; set; }
        [MaxLength(100)]
        public string LotAttribute03 { get; set; }
        [MaxLength(100)]
        public string LotAttribute04 { get; set; }
        [MaxLength(100)]
        public string LotAttribute05 { get; set; }
        [MaxLength(50)]
        public string LineNumber { get; set; }
        #endregion

        public decimal? Weight { get; set; }
        //public int? WeightUnitId { get; set; }
        //public MeasurementUnit WeightUnit { get; set; }
        //[MaxLength(50)]
        //public string WeightUnitCode { get; set; }
        public decimal? Volume { get; set; }
        //[MaxLength(50)]
        //public string VolumeUnitCode { get; set; }
        //public int? VolumeUnitId { get; set; }
        //public MeasurementUnit VolumeUnit { get; set; }
        public decimal? Units { get; set; }
        public decimal? Cartons { get; set; }
        public decimal? Pallets { get; set; }
        public decimal? NetWeight { get; set; }
        public decimal? TotalGrossWeight { get; set; } //TODO: Add to screen

        public decimal? UnitsPerCarton { get; set; }

        //[MaxLength(50)]
        //public string NetWeightUnitCode { get; set; }
        //public int? NetWeightUnitId { get; set; }
        //public MeasurementUnit NetWeightUnit { get; set; }

        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }
        public decimal? LoadingMeter { get; set; }

        public int? UnitPackageTypeId { get; set; }
        public PackageType UnitPackageType { get; set; }
        [MaxLength(50)]
        public string ExternalUnitPackageTypeCode { get; set; }
        public string UnitPackageTypeDescription { get; set; }

        public int? PalletPackageTypeId { get; set; }
        public PackageType PalletPackageType { get; set; }
        [MaxLength(50)]
        public string ExternalPalletPackageTypeCode { get; set; }
        public string PalletPackageTypeDescription { get; set; }

        public int? CartonPackageTypeId { get; set; }
        public PackageType CartonPackageType { get; set; }
        [MaxLength(50)]
        public string ExternalCartonPackageTypeCode { get; set; }
        public string CartonPackageTypeDescription { get; set; }

        public string MRN { get; set; }

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
        #endregion

        public string UnitOfMeasure { get; set; }
        public string Commodity { get; set; }
        public string GoodsDescription { get; set; }
        public string GoodsDescriptionForCustomer { get; set; }
        public string GoodsDescriptionRemarks { get; set; }
        public string PackagingInformation { get; set; }
        public string ProperShippingName { get; set; }

        public string PackagingSizeDescription { get; set; }

        public List<ShipmentDetailHTS> HTSs { get; set; } = new List<ShipmentDetailHTS>();

        public List<ShipmentDetailDangerousCargo> DangerousCargoes { get; set; } = new List<ShipmentDetailDangerousCargo>();
        public List<ShipmentDetailDate> Dates { get; set; } = new List<ShipmentDetailDate>();
        public List<ShipmentDetailAttribute> Attributes { get; set; } = new List<ShipmentDetailAttribute>();
        public List<ShipmentDetailMeasurement> Measurements { get; set; } = new List<ShipmentDetailMeasurement>();
        public List<ShipmentDetailParty> Parties { get; set; } = new List<ShipmentDetailParty>();
        public List<ShipmentSplitDetail> SplitDetails { get; set; } = new List<ShipmentSplitDetail>();
        public List<ShipmentDetailIncoTerm> IncoTerms { get; set; } = new List<ShipmentDetailIncoTerm>();
        public bool MixedPallets { get; set; }
        public string MixedPalletsReference { get; set; }
        public string LetterOfCreditNumber { get; set; }
        public DateTime? LetterOfCreditIssueDate { get; set; }

        public string ShipperRemarks { get; set; }

        public int? ComboId { get; set; }
        public ShipmentDetailCombo Combo { get; set; }
        public List<ShipmentDetailCartonRange> CartonRanges { get; set; } = new List<ShipmentDetailCartonRange>();

        public decimal? PackWidth { get; set; }
        public decimal? PackHeight { get; set; }
        public decimal? PackDepth { get; set; }
        public decimal? ChargeableWeight { get; set; }
        public bool? IsDeleted { get; set; }
        public List<ShipmentLoadDetail> LoadDetails { get; set; } = new List<ShipmentLoadDetail>();
        public string CertificateNumber { get; set; }
        public string PouchNumber { get; set; }
    }

    public class ShipmentDetailCartonRange : IEntity, IDeletedProperties, ILogged<ShipmentLog>
    {
        public int Id { get; set; }
        public int ShipmentDetailId { get; set; }
        [CascadeDelete()]
        public ShipmentDetail ShipmentDetail { get; set; }
        public int CartonRangeId { get; set; }
        [CascadeDelete()]
        public CartonRange CartonRange { get; set; }
        public int? Units { get; set; }
        public bool? IsDeleted { get; set; }
    }

    public class CartonRange : IEntity, IDeletedProperties, ILogged<ShipmentLog>
    {
        public int Id { get; set; }
        [MaxLength(50)]
        public string PONumber { get; set; }
        public int? Cartons { get; set; }
        public int FromCartonNumber { get; set; }
        public int ToCartonNumber { get; set; }
        [MaxLength(50)]
        public string ExternalTypeCode { get; set; }
        [MaxLength(50)]
        public string ExternalSkuPrefix { get; set; }
        //TODO: Additional information fields
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }
        public bool? IsDeleted { get; set; }
        public List<ShipmentDetailCartonRange> ShipmentDetails { get; set; } = new List<ShipmentDetailCartonRange>();
    }

    public class ShipmentDetailCombo : IEntity, ILogged<ShipmentLog>
    {
        public int Id { get; set; }
        public string UniqueId { get; set; }
        public List<ShipmentDetail> Details { get; set; } = new List<ShipmentDetail>();
    }

    public class ShipmentDetailPartyContact : PartyContact<ShipmentDetailParty>, ILogged<ShipmentLog>, IEntity { }
    public class ShipmentDetailParty : Party<ShipmentDetailPartyContact>, ILogged<ShipmentLog>, IEntity //, IEquatable<ShipmentDetailParty>
    {
        public int ShipmentDetailId { get; set; }
        [CascadeDelete()]
        public ShipmentDetail ShipmentDetail { get; set; }

        //public bool Equals(ShipmentDetailParty other)
        //{
        //    return other != null && other.Id == this.Id;
        //}
    }

    public class ShipmentDetailAttribute : AttributeBase<ShipmentDetail>, ILogged<ShipmentLog>, IEntity { }
    public class ShipmentDetailDate : DateBase<ShipmentDetail>, ILogged<ShipmentLog>, IEntity { }

    public class ShipmentDetailMeasurement : MeasurementBase<ShipmentDetail>, ILogged<ShipmentLog>, IEntity { }

    public class ShipmentDetailHTS : IEntity, ILogged<ShipmentLog>
    {
        public int Id { get; set; }
        [MaxLength(50), Required()]
        public string Code { get; set; }
        public int ShipmentDetailId { get; set; }
        [CascadeDelete()]
        public ShipmentDetail ShipmentDetail { get; set; }
    }

    public class ShipmentDetailIncoTerm : IEntity, ILogged<ShipmentLog>
    {
        public int Id { get; set; }
        public int IncoTermId { get; set; }
        public IncoTerm IncoTerm { get; set; }
        public int ShipmentDetailId { get; set; }
        [CascadeDelete()]
        public ShipmentDetail ShipmentDetail { get; set; }
        public int? LocationId { get; set; }
        public Location Location { get; set; }
        [MaxLength(50)]
        public string LocationCode { get; set; }
    }

}
