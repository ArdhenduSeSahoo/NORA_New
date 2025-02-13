using Damco.Model;
using Damco.Model.Interfacing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORA.Model
{
    public class PaymentTerm: IEntity, ILogged<MasterDataLog>
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }
    public class PaymentTermAlias: AliasBase<PaymentTerm>, IEntity
    {
    }
}
