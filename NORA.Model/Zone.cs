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
    public class Zone : IEntity, ILogged<MasterDataLog>
    {
        public int Id { get; set; }
        [Required(), StringLength(100)]
        public string Name { get; set; }
        [MaxLength(50)]
        public string Code { get; set; }

        [ForeignKey("ZoneId")]
        public List<ZoneZipCodeRange> ZipCodeRanges { get; set; } = new List<ZoneZipCodeRange>();

        [ForeignKey("ZoneId")]
        public List<ZoneCity> Cities { get; set; } = new List<ZoneCity>();
        public int? ParentId { get; set; }
        public Zone Parent { get; set; }
        public int SetId { get; set; }
        public ZoneSet Set { get; set; }
        [ForeignKey("ItemId")]
        public List<ZoneAlias> Aliases { get; set; } = new List<ZoneAlias>();
    }
    public class ZoneAlias : AliasBase<Zone>, ILogged<MasterDataLog>, IEntity { }

    public class ZoneZipCodeRange : IEntity, ILogged<MasterDataLog>
    {
        public int Id { get; set; }
        public int ZoneId { get; set; }
        [CascadeDelete()]
        public Zone Zone { get; set; }
        public int CountryId { get; set; }
        public Country Country { get; set; }
        [MaxLength(50), Required()]
        public string FromZipCode { get; set; }
        [MaxLength(50), Required()]
        public string ToZipCode { get; set; }
    }

    public class ZoneCity : IEntity, ILogged<MasterDataLog>
    {
        public int Id { get; set; }
        public int ZoneId { get; set; }
        [CascadeDelete()]
        public Zone Zone { get; set; }
        public int CountryId { get; set; }
        public Country Country { get; set; }
        [MaxLength(100), Required()]
        public string City { get; set; }
    }

    public class ZoneSet : ILogged<MasterDataLog>, IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<RateContract> RateContracts { get; set; } = new List<RateContract>();
    }
}
