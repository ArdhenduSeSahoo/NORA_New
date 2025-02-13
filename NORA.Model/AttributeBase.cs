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
    public abstract class AttributeBase
    {
        public int Id { get; set; }
        public int? TypeId { get; set; }
        public AttributeType Type { get; set; }
        [MaxLength(50)]
        public string ExternalTypeCode { get; set; }
        [Required()]
        public string Value { get; set; }
        public int ItemId { get; set; }
        public int Sequence { get; set; }
    }

    public abstract class AttributeBase<T> : AttributeBase
    {
        [CascadeDelete()]
        public T Item { get; set; }
    }

    public class AttributeType : IEntity, ILogged<MasterDataLog>
    {
        public int Id { get; set; }
        [Required(), MaxLength(50)]
        public string Code { get; set; }
        [Required(), MaxLength(100)]
        public string Name { get; set; }

        [ForeignKey("ItemId")]
        public List<AttributeTypeAlias> Aliases { get; set; } = new List<AttributeTypeAlias>();
    }
    public class AttributeTypeAlias : AliasBase<AttributeType>, ILogged<MasterDataLog>, IEntity { }
}
