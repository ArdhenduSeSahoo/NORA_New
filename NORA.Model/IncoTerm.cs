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

    public class IncoTerm : IEntity, ILogged<MasterDataLog>
    {
        public int Id { get; set; }
        [MaxLength(50), Required()]
        public string Code { get; set; }
        [MaxLength(100), Required()]
        public string Name { get; set; }
    }
    public class IncoTermAlias : AliasBase<IncoTerm>, IEntity, ILogged<MasterDataLog> { }

}
