using Damco.Model;
using Damco.Model.Interfacing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORA.Model
{
    public class Charge : IEntity, ILogged<MasterDataLog>
    {
        public int Id { get; set; }
        [MaxLength(50), Required()]
        public string Code { get; set; }
        [MaxLength(100), Required()]
        public string Name { get; set; }
        //public string FinancialCode { get; set; } Moved to finops
        public bool PassThrough { get; set; }
        public bool AllowManualEntry { get; set; }
        public bool IsActive { get; set; }
    }

    public class ChargeAlias : AliasBase<Charge>, ILogged<MasterDataLog>, IEntity { }
}
