using Damco.Model;
using Damco.Model.Interfacing;
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
    public class DangerousCargoType : IEntity, ILogged<MasterDataLog>
    {
        public int Id { get; set; }
        [MaxLength(50), Required()]
        public string Code { get; set; }
        [MaxLength(100), Required()]
        public string Name { get; set; }
        [ForeignKey("ItemId")]
        public List<DangerousCargoTypeAlias> Aliases { get; set; } = new List<DangerousCargoTypeAlias>();
    }
    public class DangerousCargoTypeAlias : AliasBase<DangerousCargoType>, IEntity, ILogged<MasterDataLog> { }

    public interface IDangerousCargo
    {
        int? TypeId { get; set; }
        DangerousCargoType Type { get; set; }
        string ExternalIMDGHazardCode { get; set; }
        int? IMDGHazardId { get; set; }
        IMDGHazard IMDGHazard { get; set; }
        int? UNDGCategoryId { get; set; }
        UNDGCategory UNDGCategory { get; set; }
        int? DangerousGoodsCategoryId { get; set; }
        DangerousGoodsCategory DangerousGoodsCategory { get; set; }
        string ClassificationCode { get; set; }
        string ERGCode { get; set; }
        decimal? Flashpoint { get; set; }
        int? FlashpointUnitId { get; set; }
        MeasurementUnit FlashpointUnit { get; set; }
        string ExternalFlashpointUnitCode { get; set; }
        string ExternalIMDGPackingGroupCode { get; set; }
        int? IMDGPackingGroupId { get; set; }
        IMDGPackingGroup IMDGPackingGroup { get; set; }
        string EMSNumber { get; set; }
        string PlacardUpperPart { get; set; }
        string PlacardLowerPart { get; set; }
        string LabelMarking1 { get; set; }
        string LabelMarking2 { get; set; }
        string LabelMarking3 { get; set; }
        string EmergencyContactPhone { get; set; }
        string EmergencyContactFax { get; set; }
        string EmergencyContactMobile { get; set; }
        string EmergencyContactEmail { get; set; }
        string EmergencyContactInformation { get; set; }
        string SegregationGroup { get; set; }
        decimal? ControlTemperature { get; set; }
        int? ControlTemperatureUnitId { get; set; }
        MeasurementUnit ControlTemperatureUnit { get; set; }
        decimal? EmergencyTemperature { get; set; }
        int? EmergencyTemperatureUnitId { get; set; }
        MeasurementUnit EmergencyTemperatureUnit { get; set; }
        bool LimitedQuantity { get; set; }
        bool? MarinePollutant { get; set; }
        bool? EnvironmentallyHazardous { get; set; }
        bool? Viscous { get; set; }
        bool? ExtraViscous { get; set; }
        int? DangerousCargoMeasurementTypeId { get; set; }
        DangerousCargoMeasurementType DangerousCargoMeasurementType { get; set; }
    }

    public interface IDangerousCargo<Tattribute>: IDangerousCargo
        where Tattribute : IDangerousCargoAttribute
    {
        List<Tattribute> Attributes { get; set; }
    }
    public interface IDangerousCargoAttribute
    {
        int? TypeId { get; set; }
        AttributeType Type { get; set; }
        string ExternalTypeCode { get; set; }
        string Value { get; set; }
        int ItemId { get; set; }
        int Sequence { get; set; }
    }

    public class ShipmentDetailDangerousCargo : IEntity, ILogged<ShipmentLog>, IDangerousCargo<ShipmentDetailDangerousCargoAttribute>
    {
        public int Id { get; set; }
        public int ShipmentDetailId { get; set; }
        [CascadeDelete()]
        public ShipmentDetail ShipmentDetail { get; set; }
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
        public List<ShipmentDetailDangerousCargoAttribute> Attributes { get; set; } = new List<ShipmentDetailDangerousCargoAttribute>();

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
        public DangerousGoodsAggregationState? DangerousGoodsAggregationState { get; set; }

        public DangerousCargoMeasurementType DangerousCargoMeasurementType { get; set; }
    }

    public enum DangerousGoodsAggregationState
    {
        Liquid = 1,
        Solid = 2,
        Gas = 3
    }
    public class DangerousCargoMeasurementType : IEntity, ILogged<MasterDataLog>
    {
        public int Id { get; set; }
        [MaxLength(50), Required()]
        public string Code { get; set; }
        [MaxLength(100), Required()]
        public string Name { get; set; }
    }

    public class UNDGCategory : IEntity, ILogged<MasterDataLog>
    {
        public int Id { get; set; }
        [MaxLength(50), Required()]
        public string Code { get; set; }
        [ForeignKey("ItemId")]
        public List<UNDGCategoryAlias> Aliases { get; set; } = new List<UNDGCategoryAlias>();
    }
    public class UNDGCategoryAlias : AliasBase<UNDGCategory>, IEntity, ILogged<MasterDataLog> { }

    public class DangerousGoodsCategory : IEntity, ILogged<MasterDataLog>
    {
        public int Id { get; set; }
        [MaxLength(50), Required()]
        public string Code { get; set; }
        [MaxLength(100), Required()]
        public string Name { get; set; }
        public string Description { get; set; }
        public int? UNDGCategoryId { get; set; }
        public UNDGCategory UNDGCategory { get; set; }
        [ForeignKey("ItemId")]
        public List<DangerousGoodsCategoryAlias> Aliases { get; set; } = new List<DangerousGoodsCategoryAlias>();
    }
    public class DangerousGoodsCategoryAlias : AliasBase<DangerousGoodsCategory>, IEntity, ILogged<MasterDataLog> { }

    public class ShipmentDetailDangerousCargoAttribute : AttributeBase<ShipmentDetailDangerousCargo>, IEntity, ILogged<ShipmentLog>, IDangerousCargoAttribute
    {
    }

    public class IMDGPackingGroup : IEntity, ILogged<MasterDataLog>
    {
        public int Id { get; set; }
        [MaxLength(50), Required()]
        public string Code { get; set; }
        [MaxLength(50), Required()]
        public string Name { get; set; }
    }
    public class IMDGPackingGroupAlias : AliasBase<IMDGPackingGroup>, IEntity, ILogged<MasterDataLog> { }

    public class IMDGHazard : IEntity, ILogged<MasterDataLog>
    {
        public int Id { get; set; }
        [MaxLength(50), Required()]
        public string Code { get; set; }
        [MaxLength(50), Required()]
        public string Name { get; set; }
    }
    public class IMDGHazardAlias : AliasBase<IMDGHazard>, IEntity, ILogged<MasterDataLog> { }

    public class IMDGSetup : IEntity, ILogged<MasterDataLog>
    {
        public int Id { get; set; }
        public int IMDGHazardId { get; set; }
        public IMDGHazard IMDGHazard { get; set; }
        public int IMDGPackingGroupId { get; set; }
        public IMDGPackingGroup IMDGPackingGroup { get; set; }
        public bool RequiresSurcharge { get; set; }
    }

}
