using Damco.Model.Interfacing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model.MultiTenancy
{
    public class MultiTenancyMasterDataLog : ChangeLogBase, IEntity { }

    public class Tenant : IEntity, ILogged<MultiTenancyMasterDataLog>
    {
        public int Id { get; set; }
        [MaxLength(50), Required()]
        public string Code { get; set; }
        public List<TenantAlias> Aliases { get; set; } = new List<TenantAlias>();
        public List<TenantTenantFamily> Families { get; set; } = new List<TenantTenantFamily>();
        [MaxLength(200)]
        public string TeamName { get; set; }
        public string MessageCloser { get; set; }
    }

    public enum TenantCodes
    {
        DEBASFS001, GBPRIMARKHQ
    }

    public class TenantAlias : AliasBase<Tenant>, IEntity { }

    public class TenantTenantFamily: IEntity, ILogged<MultiTenancyMasterDataLog>
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        [CascadeDelete()]
        public Tenant Tenant { get; set; }
        public int FamilyId { get; set; }
        [CascadeDelete()]
        public TenantFamily Family { get; set; }
    }

}
