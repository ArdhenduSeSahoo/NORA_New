using Damco.Model;
using Damco.Model.DataSourcing;
using Damco.Model.ScreenTemplating;
using Damco.Model.UserManagement;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORA.Model
{
    public class RateContractSet : ILogged<MasterDataLog>, IEntity
    {
        public int Id { get; set; }
        [Required(), MaxLength(100)]
        public string Name { get; set; }
    }

    public class RateContract : ILogged<MasterDataLog>, IEntity
    {
        public int Id { get; set; }

        public int RateContractSetId { get; set; }
        public RateContractSet RateContractSet { get; set; }

        public int? TransporterId { get; set; }
        public Company Transporter { get; set; }
        public int? CustomerId { get; set; }
        public Company Customer { get; set; }
        public int? SubContracterId { get; set; }
        public Company SubContracter { get; set; }

        public DateTime? FromDateTime { get; set; }
        public DateTime? ToDateTime { get; set; }

        public int? ValidityDateDataFieldId { get; set; }
        public DataField ValidityDateDataField { get; set; }

        //public RateMeasurementDataSource ScaleDataSource { get; set; }
        //public MeasurementSourceType ScaleStandardMeasurementType { get; set; }
        ////Next ones only needed of ScaleStandardMeasurementType = Custom
        //public int? ScaleMeasurementTypeId { get; set; }
        //public MeasurementType ScaleMeasurementType { get; set; }
        //public int? ScaleMeasurementCategoryId { get; set; }
        //public MeasurementCategory ScaleMeasurementCategory { get; set; }
        //public int? ScaleMeasurementUnitId { get; set; }
        //public MeasurementUnit ScaleMeasurementUnit { get; set; }
        //public SlidingScaleType? SlidingScaleType { get; set; }

        public int FinanceCalculationQuantitySourceId { get; set; }
        public FinanceCalculationQuantitySource FinanceCalculationQuantitySource { get; set; }

        //public RateDataSource QuantityDataSource { get; set; }
        //public RateSourceType MultiplyStandardMeasurementType { get; set; }
        //Next ones only needed of MultiplyStandardMeasurementType = Custom
        //public int? MultiplyMeasurementTypeId { get; set; }
        //public MeasurementType MultiplyMeasurementType { get; set; }
        //public int? MultiplyMeasurementCategoryId { get; set; }
        //public MeasurementCategory MultiplyMeasurementCategory { get; set; }
        //public int? MultiplyMeasurementUnitId { get; set; }
        //public MeasurementUnit MultiplyMeasurementUnit { get; set; }

        public List<Rate> Rates { get; set; } = new List<Rate>();

        [MaxLength(50)]
        public string ServiceContractNumber { get; set; }
        [MaxLength(50)]
        public string ServiceContractSuffix { get; set; }

        public int? ZoneSetId { get; set; }
        public ZoneSet ZoneSet { get; set; }
    }

    //public enum RateDataSource
    //{
    //    //Transport = 1,
    //    CustomsClearanceOrder = 2,
    //    EquipmentTransport = 3,
    //    //ShipmentDetail,
    //    //Shipment,
    //    //ShipmentGroup,
    //}

    //public enum RateSourceType
    //{
    //    None = 0,
    //    //Custom = 1,
    //    NumberOfItems = 2,
    //    //StandardWeight = 3,
    //    //StandardVolume = 4,
    //    //StandardUnits = 5,
    //    //StandardCartons = 6,
    //    //StandardPallets = 7
    //}

    //public enum SlidingScaleType
    //{
    //    /// <summary>
    //    /// E.g. for these rates:
    //    /// Up to 5: 10 EUR
    //    /// Up to 10: 8 EUR
    //    /// Quantity 4 causes total cost of 4*10 = 40
    //    /// Quantity 7 causes total cost of 7*8 = 54
    //    /// </summary>
    //    UseHighestScaleForWholeQuantity = 1,
    //    /// <summary>
    //    /// E.g. for these rates:
    //    /// Up to 5: 10 EUR
    //    /// Up to 10: 8 EUR
    //    /// Quantity 4 causes total cost of 4*10 = 40
    //    /// Quantity 7 causes total cost of (5*10)+(2*8) = 66
    //    /// </summary>
    //    UseSeparateScalesForPartsOfQuantity = 2
    //}

    public class Rate : IEntity, IUpdateTracking 
    {
        public int Id { get; set; }
        public int RateContractId { get; set; }
        public RateContract RateContract { get; set; }
        public int? OriginZoneId { get; set; }
        public Zone DestinationZone { get; set; }
        public int? DestinationZoneId { get; set; }
        public Zone OriginZone { get; set; }
        public int? EquipmentTransportCategoryId { get; set; }
        public EquipmentTransportCategory EquipmentTransportCategory { get; set; }
        //public int? CalendarTypeId { get; set; }
        //public CalendarType CalendarType { get; set; }
        public int CurrencyId { get; set; }
        public Currency Currency { get; set; }
        public decimal Tariff { get; set; }
        public DateTime EditDateTime { get; set; }
        public User EditUser { get; set; }
        public int? EditUserId { get; set; }
        //public decimal? MaximumTotalTariff { get; set; }
        //public decimal? MinimumTotalTariff { get; set; }
        //public List<RateScale> Scales { get; set; }
    }

    //public class RateScale : IxEntity
    //{
    //    public int Id { get; set; }
    //    public int RateId { get; set; }
    //    public Rate Rate { get; set; }
    //    public decimal UpToQuantity { get; set; }
    //    public decimal Tariff { get; set; }
    //}

}
