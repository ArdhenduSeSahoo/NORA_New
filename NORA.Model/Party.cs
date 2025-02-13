using Damco.Model;
using Damco.Model.Interfacing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORA.Model
{
    public abstract class Party
    {
        [Key()]
        public int Id { get; set; }
        [MaxLength(50)]
        public string ExternalCode { get; set; }
        [MaxLength(50)]
        public string ShortName { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }
        [MaxLength(100)]
        public string Name2 { get; set; }
        public string AddressBlock { get; set; }
        public bool AddressBlockIsProcessed { get; set; }
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
        public int? ZoneId { get; set; }
        public Zone Zone { get; set; }
        public int? CompanyId { get; set; }
        public virtual Company Company { get; set; }
        public int? ExternalCompanyAliasId { get; set; }
        public virtual CompanyAlias ExternalCompanyAlias { get; set; }
        public int? TypeId { get; set; }
        public PartyType Type { get; set; }
        [MaxLength(50)]
        public string ExternalTypeCode { get; set; }
        [MaxLength(50)]
        public string TaxCode { get; set; }
        public string RegionDescription { get; set; }
        [MaxLength(100)]
        public string District { get; set; }
        [MaxLength(50)]
        public string ExternalLocationCode { get; set; }
        public string AdditionalAddressDetails { get; set; }
        [MaxLength(50)]
        public string Reference1 { get; set; }
        [MaxLength(50)]
        public string Reference2 { get; set; }
        [MaxLength(50)]
        public string ExternalCountryCode { get; set; }
        [MaxLength(100)]
        public string ExternalCountryName { get; set; }
    }
    public abstract class Party<Tcontact> : Party, IPartyWithContacts
        where Tcontact: PartyContact 
    {
        public List<Tcontact> Contacts { get; set; } = new List<Tcontact>();

        IEnumerable<PartyContact> IPartyWithContacts.GetContacts() => this.Contacts.Cast<PartyContact>();
    }

    public interface IPartyWithContacts
    {
        IEnumerable<PartyContact> GetContacts();
    }

    public abstract class PartyContact : ContactInfoHolder
    {
        public int Id { get; set; }
        public int? ContactId { get; set; }
        public virtual Contact Contact { get; set; }
    }
    public abstract class PartyContact<Tparty> : PartyContact
    {
        public int PartyId { get; set; }
        [CascadeDelete()]
        public Tparty Party { get; set; }
    }

    public class PartyType : IEntity, ILogged<MasterDataLog>
    {
        public int Id { get; set; }
        [Required(), MaxLength(50)]
        public string Code { get; set; }
        [Required(), MaxLength(100)]
        public string Name { get; set; }
        public SpecialPartyTypes SpecialTypes { get; set; }
        public int? OrderNumber { get; set; }
        public int? DocumentOrderNumber { get; set; }
    }

    [Flags()]
    public enum SpecialPartyTypes
    {
        None = 0,
        UltimateConsignee = 1,
        Consignee = 2,
        Shipper = 4,
        Payer = 8,
        Notify1 = 16,
        Notify2 = 32,
        OriginalShipper = 64,
        Notify3 = 128,
    }


    public class PartyTypeAlias : AliasBase<PartyType>, ILogged<MasterDataLog>, IEntity { }

}
