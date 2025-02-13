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
    public class Country : IEntity, ILogged<MasterDataLog>
    {
        public int Id { get; set; }
        [Required(), MaxLength(50)]
        public string Name { get; set; }
        [Required(), MaxLength(50)]
        public string Code { get; set; }
        public int? SmallGroupId { get; set; }
        public CountryGroup SmallGroup { get; set; }
        public int? LargeGroupId { get; set; }
        public CountryGroup LargeGroup { get; set; }
        public int? ContinentId { get; set; }
        public CountryGroup Continent { get; set; }
        [ForeignKey("ItemId")]
        public List<CountryAlias> Aliases { get; set; } = new List<CountryAlias>();
        public bool Disabled { get; set; }
    }
    public class CountryAlias : AliasBase<Country>, IEntity, ILogged<MasterDataLog> { }

    public class CountryGroup : IEntity, ILogged<MasterDataLog>
    {
        public int Id { get; set; }
        [Required(), MaxLength(100)]
        public string Name { get; set; }
        public CountryGroupType Type { get; set; }
    }

    [Flags()]
    public enum CountryGroupType
    {
        /// <summary>
        /// E.g. Benelux
        /// </summary>
        Small = 1,
        /// <summary>
        /// E.g. EU
        /// </summary>
        Large = 2,
        /// <summary>
        /// E.g. Europe
        /// </summary>
        Continent = 4
    }

}
