using Damco.Model;
using Damco.Model.Interfacing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Damco.Model.Interfacing
{
    public abstract class AliasBase
    {
        public int Id { get; set; }
        public int InterfaceSetupId { get; set; }
        public InterfaceSetup InterfaceSetup { get; set; }
        public int ItemId { get; set; }
        [MaxLength(200), Required()]
        public string Alias { get; set; }
        public int? CategoryId { get; set; }
        public AliasCategory Category { get; set; }
    }

    public abstract class AliasBase<T> : AliasBase
        where T : IEntity
    {
        public T Item { get; set; }
    }

    public class AliasCategory : IEntity, ILogged<InterfacingMasterDataLog>
    {
        public int Id { get; set; }
        [MaxLength(50), Required()]
        public string Code { get; set; }
        public bool AllowMissingAlias { get; set; }
    }
}