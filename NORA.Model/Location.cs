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
    public class Location : IEntity, ILogged<MasterDataLog>
    {
        public int Id { get; set; }
        [MaxLength(50)]
        public string Code { get; set; }
        [MaxLength(100), Required()]
        public string Name { get; set; }
        public decimal? Longtitude { get; set; }
        public decimal? Latitude { get; set; }
        [ForeignKey("ItemId")]
        public List<LocationAlias> Aliases { get; set; } = new List<LocationAlias>();

    }
    public class LocationAlias : AliasBase<Location>, IEntity, ILogged<MasterDataLog> { }
}
