using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORA.Model
{
    public class CalculatedRateOfTransporter
    {
        public int Id { get; set; }
        public int TransporterId { get; set; }
        public Company Transporter { get; set; }
        public decimal TotalCost { get; set; }
        public int TransportId { get; set; }
    }
}
