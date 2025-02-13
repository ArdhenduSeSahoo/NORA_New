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
    public abstract class DateBase
    {
        public int Id { get; set; }
        public int? TypeId { get; set; }
        public DateType Type { get; set; }
        [MaxLength(50)]
        public string ExternalTypeCode { get; set; }
        [Required()]
        public DateTime Value { get; set; }
        public int ItemId { get; set; }
    }

    public abstract class DateBase<T> : DateBase
    {
        [CascadeDelete()]
        public T Item { get; set; }
    }

    public class DateType : IEntity, ILogged<MasterDataLog>
    {
        public int Id { get; set; }
        [Required(), MaxLength(50)]
        public string Code { get; set; }
        [Required(), MaxLength(100)]
        public string Name { get; set; }
        [ForeignKey("ItemId")]
        public List<DateTypeAlias> Aliases { get; set; } = new List<DateTypeAlias>();
    }
    public class DateTypeAlias : AliasBase<DateType>, ILogged<MasterDataLog>, IEntity { }
}
