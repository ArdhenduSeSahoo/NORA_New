using Damco.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORA.Model
{

    public class ShipmentPack : IEntity, ILogged<ShipmentLog>
    {
        public int Id { get; set; }

        public int ShipmentId { get; set; }
        public virtual Shipment Shipment { get; set; }

        [MaxLength(100)]
        public string PackNumber { get; set; }
        [MaxLength(100)]
        public string SealNumber { get; set; }
        public decimal? Weight { get; set; }
        public decimal? Volume { get; set; }
        public decimal? Units { get; set; }
        public decimal? Cartons { get; set; }
        public decimal? Pallets { get; set; }

        public virtual List<ShipmentPackDetail> Details { get; set; }
    }

    public class ShipmentPackDetail : IEntity, ILogged<ShipmentLog>
    {
        public int Id { get; set; }

        public int ShipmentPackId { get; set; }
        public virtual ShipmentPack ShipmentPack { get; set; }

        public int ShipmentDetailId { get; set; }
        public virtual ShipmentDetail ShipmentDetail { get; set; }
        public decimal? Weight { get; set; }
        public decimal? Volume { get; set; }
        public decimal? Units { get; set; }
        public decimal? Cartons { get; set; }
        public decimal? Pallets { get; set; }

        public string UnitOfMeasure { get; set; }
    }
}
