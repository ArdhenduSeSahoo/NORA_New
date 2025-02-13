using Damco.Model;
using Damco.Model.Interfacing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORA.Model
{
    public class PackageType : IEntity, ILogged<MasterDataLog>
    {
        public int Id { get; set; }
        [Required(), MaxLength(50)]
        public string UNECECode { get; set; }
        [MaxLength(50)]
        public string IMDGCode { get; set; }
        [MaxLength(50)]
        public string CodeSuffix { get; set; }
        [Required(), MaxLength(100)]
        public string Name { get; set; }
        [ForeignKey("ItemId")]
        public List<PackageTypeAlias> Aliases { get; set; } = new List<PackageTypeAlias>();
    }
    public class PackageTypeAlias : AliasBase<PackageType>, ILogged<MasterDataLog>, IEntity { }
}
