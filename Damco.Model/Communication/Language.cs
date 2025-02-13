using Damco.Model.Interfacing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model.Communication
{
    public class Language : IEntity, ILogged<CommunicationMasterDataLog>
    {
        public int Id { get; set; }
        [MaxLength(50), Required()]
        public string Code { get; set; }
        [MaxLength(100), Required()]
        public string Name { get; set; }
    }
    public class LanguageAlias : AliasBase<Language>, IEntity
    {
    }
}
