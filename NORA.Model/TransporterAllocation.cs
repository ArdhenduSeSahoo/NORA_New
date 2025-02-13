using Damco.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORA.Model
{
    public class TransporterAllocation: IEntity
    {
        public int Id { get; set; }
        public int? OriginZoneId { get; set; }
        public Zone DestinationZone { get; set; }
        public int? DestinationZoneId { get; set; }
        public Zone OriginZone { get; set; }
        public DateTime? FromDateTime { get; set; }
        public DateTime? ToDateTime { get; set; }
        public int? TransporterId { get; set; }
        public Company Transporter { get; set; }
        public int? CustomerId { get; set; }
        public Company Customer { get; set; }
        [Range(0, 1)]
        public decimal AllocationFactor { get; set; }
    }
}
