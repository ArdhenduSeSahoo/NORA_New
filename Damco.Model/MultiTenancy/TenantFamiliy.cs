using Damco.Model.ScreenTemplating;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model.MultiTenancy
{
    public class TenantFamily : IEntity, ILogged<MultiTenancyMasterDataLog>
    {
        public int Id { get; set; }
        [MaxLength(100), Required()]
        public string Name { get; set; }
        public string Description { get; set; }
        public string Signature { get; set; }
        public List<TenantFamilyTag> Tags { get; set; } = new List<TenantFamilyTag>();
        public List<TenantFamilyPage> Pages { get; set; } = new List<TenantFamilyPage>();
        public List<TenantFamilyOperation> Operations { get; set; } = new List<TenantFamilyOperation>();
        public List<TenantTenantFamily> Tenants { get; set; } = new List<TenantTenantFamily>();
    }

    public class TenantFamilyTag : IEntity, ILogged<MultiTenancyMasterDataLog>
    {
        public int Id { get; set; }
        [MaxLength(50), Required()]
        public string Tag { get; set; }
        public int TenantFamilyId { get; set; }
        [CascadeDelete()]
        public TenantFamily TenantFamily { get; set; }
    }

    public class TenantFamilyPage : IEntity, ILogged<MultiTenancyMasterDataLog>
    {
        public int Id { get; set; }
        public int TenantFamilyId { get; set; }
        [CascadeDelete()]
        public TenantFamily TenantFamily { get; set; }
        [Required(), MaxLength(100)]
        public string PageDesignTag { get; set; } //TODO replace with a "real" link to the page design later
        //public int PageDesignId { get; set; }
        //public PageDesign PageDesign { get; set; }
    }


    public class TenantFamilyOperation : IEntity, ILogged<MultiTenancyMasterDataLog>
    {
        public int Id { get; set; }
        public int TenantFamilyId { get; set; }
        [CascadeDelete()]
        public TenantFamily TenantFamily { get; set; }
        [Required(), MaxLength(100)]
        public string PageDesignOperationTag { get; set; } //TODO replace with a "real" link to the page design operation later
        //public int? PageDesignOperationId { get; set; }
        //public PageDesignOperation PageDesignOperation { get; set; }
    }

}
