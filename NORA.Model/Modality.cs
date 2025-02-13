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
    public class Modality : IEntity, ILogged<MasterDataLog>
    {
        public int Id { get; set; }
        [Required()]
        public string Name { get; set; }
        [MaxLength(50)]
        public string Code { get; set; }
    }
    public class ModalityAlias: AliasBase<Modality>, ILogged<MasterDataLog>, IEntity { }
}
