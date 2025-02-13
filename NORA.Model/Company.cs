using Damco.Model;
using Damco.Model.Interfacing;
using Damco.Model.MultiTenancy;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NORA.Model
{

    [Serializable()]
    public class Company : IEntity, IInterfacedEntity, ILogged<MasterDataLog>
    {
        public int Id { get; set; }
        public CompanyTypes Types { get; set; }
        public List<Contact> Contacts { get; set; } = new List<Contact>();

        [MaxLength(50), Required()]
        public string Code { get; set; }
        [MaxLength(50), Required()]
        public string ShortName { get; set; }
        [MaxLength(100), Required()]
        public string Name { get; set; }
        [MaxLength(100)]
        public string Name2 { get; set; }
        [MaxLength(100)]
        public string Address1 { get; set; }
        [MaxLength(100)]
        public string Address2 { get; set; }
        [MaxLength(100)]
        public string Address3 { get; set; }
        [MaxLength(100)]
        public string Address4 { get; set; }
        [MaxLength(100)]
        public string City { get; set; }
        [MaxLength(100)]
        public string State { get; set; }
        [MaxLength(50)]
        public string ZipCode { get; set; }
        public int? CountryId { get; set; }
        public Country Country { get; set; }
        public string ContactEmail { get; set; }
        [MaxLength(50)]
        public string TimeZoneId { get; set; }
        [MaxLength(50)]
        public string TaxCode { get; set; }
        [MaxLength(50)]
        public string FinancialCode { get; set; }
        [MaxLength(50)]
        public string ScvCode { get; set; }
        public decimal? Longtitude { get; set; }
        public decimal? Latitude { get; set; }
        [ForeignKey("ItemId")]
        public List<CompanyAlias> Aliases { get; set; } = new List<CompanyAlias>();
        public List<CompanyUser> Users { get; set; } = new List<CompanyUser>();
        public List<CompanyZone> Zones { get; set; } = new List<CompanyZone>();
        public int? SolasMethodOfCalculation { get; set; }
        [MaxLength(50)]
        public string DBHCode { get; set; }
        public bool? UserDefinedYesNo01 { get; set; }
        public bool? UserDefinedYesNo02 { get; set; }
        public string EORI { get; set; }
        public bool IsOurCompany { get; set; }
        [MaxLength(100)]
        public string District { get; set; }

        [NotMapped()]
        public bool HasPendingErrorsFromInterface { get; set; }

        [NotMapped]
        public static Expression<Func<Company, string>> DisplayName = c => c.Name + " " + c.Code;
    }


    public class CompanyZone : IEntity, ILogged<MasterDataLog>
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        [CascadeDelete()]
        public Company Company { get; set; }
        public int ZoneId { get; set; }
        [CascadeDelete()]
        public Zone Zone { get; set; }
    }

    public class CompanyUser : IEntity
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        [CascadeDelete()]
        public Company Company { get; set; }
        public int UserId { get; set; }
        [CascadeDelete()]
        public Damco.Model.UserManagement.User User { get; set; }
    }

    public class CompanyAlias : AliasBase<Company>, ILogged<MasterDataLog>, IEntity
    {
        public static Expression<Func<CompanyAlias, string>> DisplayNameExpressionBasf =>
            ca => ca.Alias + " | " + ca.Item.Name + " (" + ca.Item.Code + ")";

        public static Expression<Func<CompanyAlias, string>> DisplayNameExpressionPrimark =>
            (ca) => ca.Alias == ca.Item.Code 
                    ? (ca.Alias + " | " + ca.Item.Name + " (" + ca.Item.Code + ")") 
                    : null;

        public static Expression<Func<CompanyAlias, string>> DisplayNameExpressionSelector(TenantCodes tenantCodes)
        {
            switch (tenantCodes)
            {
                case TenantCodes.DEBASFS001:
                    return DisplayNameExpressionBasf;

                case TenantCodes.GBPRIMARKHQ:
                    return DisplayNameExpressionPrimark;

                default:
                    return DisplayNameExpressionBasf;
            }
        }
    }

    [Flags()]
    public enum CompanyTypes
    {
        Unspecified = 0,
        Customer = 1,
        ShipTo = 2,
        ShipFrom = 4,
        ShipToAndFrom = 6,
        Transporter = 8,
        InternalOffice = 16,
        DangerousCargoSupplier = 32,
        CustomsClearanceSupplier = 64,
        PortFilingSupplier = 128,
        Terminal = 256,
        FinanceVendor = 512,
        Depot = 1024,
        EmptyPickUpLocation = 2048
    }

    [Serializable()]
    public abstract class ContactInfoHolder
    {
        [MaxLength(100)]
        public string Name { get; set; }
        public string Description { get; set; }
        [MaxLength(100)]
        public string Phone { get; set; }
        [MaxLength(100)]
        public string Fax { get; set; }
        [MaxLength(100)]
        public string MobilePhone { get; set; }
        [MaxLength(200)]
        public string Email { get; set; }
    }

    [Serializable()]
    public class Contact : ContactInfoHolder, IEntity, ILogged<MasterDataLog>

    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        [CascadeDelete()]
        public Company Company { get; set; }
        public byte[] Signature { get; set; }
        public bool UseForPDFCopy { get; set; }
    }

}
