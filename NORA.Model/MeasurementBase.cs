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
    public abstract class MeasurementBase
    {
        public int Id { get; set; }
        public int? TypeId { get; set; }
        public MeasurementType Type { get; set; }
        [MaxLength(50)]
        public string ExternalTypeCode { get; set; }
        public int? MeasurementCategoryId { get; set; }
        public MeasurementCategory MeasurementCategory { get; set; }
        [MaxLength(50)]
        public string ExternalCategoryCode { get; set; }
        public int? UnitId { get; set; }
        public MeasurementUnit Unit { get; set; }
        [MaxLength(50)]
        public string ExternalUnitCode { get; set; }
        public decimal? Value { get; set; }
        public decimal? MinimumValue { get; set; }
        public decimal? MaximumValue { get; set; }
    }

    public abstract class MeasurementBase<T> : MeasurementBase
    {
        [CascadeDelete()]
        public T Item { get; set; }
        public int ItemId { get; set; }
    }

    public interface IStandardMeasurementContainer
    {
        decimal? Weight { get; set; }
        decimal? Volume { get; set; }
        decimal? Units { get; set; }
        decimal? Cartons { get; set; }
        decimal? Pallets { get; set; }
        decimal? NetWeight { get; set; }
        decimal? TotalGrossWeight { get; set; }
        decimal? ChargeableWeight { get; set; }
    }

    public enum StandardMeasurementType
    {
        None = 0,
        Weight = 1,
        Volume = 2,
        Units = 3,
        Cartons = 4,
        Pallets = 5,
        Temperature = 6
    }
    public enum StandardMeasurementCategory
    {
        None = 0,
        Gross = 1,
        Net = 2,
        Tare = 3,
        Required = 4,
        TotalGross = 5,
        Chargeable = 6
    }

    /// <summary>
    /// Weight, volume etc.
    /// </summary>
    public class MeasurementType : IEntity, ILogged<MasterDataLog>
    {
        public int Id { get; set; }
        [Required(), MaxLength(50)]
        public string Code { get; set; }
        [Required(), MaxLength(100)]
        public string Name { get; set; }
        public StandardMeasurementType StandardType { get; set; }
        public int? DefaultUnitId { get; set; }
        public MeasurementUnit DefaultUnit { get; set; }
        public List<MeasurementTypeAlias> Aliases { get; set; } = new List<MeasurementTypeAlias>();
    }

    public class MeasurementTypeAlias : AliasBase<MeasurementType>, ILogged<MasterDataLog>, IEntity { }

    /// <summary>
    /// Kgs, CBM etc.
    /// </summary>
    public class MeasurementUnit : ILogged<MasterDataLog>, IEntity
    {
        [Key()]
        public int Id { get; set; }
        [MaxLength(50), Required()]
        public string Code { get; set; }
        [Required(), MaxLength(100)]
        public string Name { get; set; }
        [ForeignKey(nameof(MeasurementType))]
        public int? MeasurementTypeId { get; set; }
        [ForeignKey(nameof(MeasurementTypeId))]
        public MeasurementType MeasurementType { get; set; }
        public decimal? ConversionMultiplier { get; set; }
        public decimal? ConversionAdder { get; set; }

        [ForeignKey("ItemId")]
        public List<MeasurementUnitAlias> Aliases { get; set; } = new List<MeasurementUnitAlias>();
    }
    public class MeasurementUnitAlias : AliasBase<MeasurementUnit>, ILogged<MasterDataLog>, IEntity { }

    /// <summary>
    /// Gross, net, etc.
    /// </summary>
    public class MeasurementCategory : IEntity, ILogged<MasterDataLog>
    {
        public int Id { get; set; }
        [MaxLength(50), Required()]
        public string Code { get; set; }
        [Required(), MaxLength(100)]
        public string Name { get; set; }
        public StandardMeasurementCategory StandardCategory { get; set; }
        [ForeignKey("ItemId")]
        public List<MeasurementCategoryAlias> Aliases { get; set; } = new List<MeasurementCategoryAlias>();
    }

    public class MeasurementCategoryAlias : AliasBase<MeasurementCategory>, ILogged<MasterDataLog>, IEntity { }
}
