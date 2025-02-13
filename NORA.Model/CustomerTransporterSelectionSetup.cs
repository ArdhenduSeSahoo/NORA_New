using Damco.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORA.Model
{
    public class CustomerTransporterSelectionSetup : IEntity, ILogged<MasterDataLog>
    {
        int ILogged.Id { get { return this.CustomerId; } }

        [Key()]
        [ForeignKey("Customer")]
        public int CustomerId { get; set; }
        public Company Customer { get; set; }
        [Range(0, 1)]
        public decimal RateImportance { get; set; }
        [Range(0, 1)]
        public decimal LeadTimeImportance { get; set; }
        [Range(0, 1)]
        public decimal PerformanceImportance { get; set; }
        [Range(0, 1)]
        public decimal AllocationImportance { get; set; }

    }
}
