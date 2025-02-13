using Damco.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORA.Model
{
    public class FinanceReason : IEntity, ILogged<MasterDataLog>
    {
        public int Id { get; set; }
        [MaxLength(50)]
        public string Code { get; set; }
        public string Description { get; set; }
        public FinanceLineType? LineType { get; set; }
        public bool RequiresReference { get; set; }
        public ReasonParty Party { get; set; }
    }
    public enum ReasonParty
    {
        Us = 1,
        Customer = 2,
        Vendor = 3
    }
}
