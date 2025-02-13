using Damco.Model;
using Damco.Model.Interfacing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NORA.Model
{
    public class ShipmentLoad : IEntity, ILogged<ShipmentLog>, IInterfacedEntity, IStandardMeasurementContainer
    {
        public int Id { get; set; }
        [MaxLength(50), Required()]
        public string Code { get; set; }
        [MaxLength(50)]
        public string Status { get; set; }
        public bool HasPendingErrorsFromInterface { get; set; }
        public DateTime? LoadDateTime { get; set; }
        public List<ShipmentSplit> Splits { get; set; } = new List<ShipmentSplit>();
        public List<ShipmentLoadDetail> Details { get; set; } = new List<ShipmentLoadDetail>();
        public decimal? Weight { get; set; }
        public decimal? Volume { get; set; }
        public decimal? Units { get; set; }
        public decimal? Cartons { get; set; }
        public decimal? Pallets { get; set; }
        public decimal? NetWeight { get; set; }
        public decimal? TotalGrossWeight { get; set; }
        public decimal? ChargeableWeight { get; set; }
    }

    public class ShipmentLoadDetail : IEntity, ILogged<ShipmentLog>, IStandardMeasurementContainer
    {
        public int Id { get; set; }
        public int ShipmentLoadId { get; set; }
        [CascadeDelete()]
        public ShipmentLoad ShipmentLoad { get; set; }
        public int ShipmentDetailId { get; set; }
        [CascadeDelete()]
        public ShipmentDetail ShipmentDetail { get; set; }
        public decimal? Weight { get; set; }
        public decimal? Volume { get; set; }
        public decimal? Units { get; set; }
        public decimal? Cartons { get; set; }
        public decimal? Pallets { get; set; }
        public decimal? NetWeight { get; set; }
        public decimal? TotalGrossWeight { get; set; }
        public decimal? ChargeableWeight { get; set; }
    }
}
