using Damco.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORA.Model
{
    public class CompanyCommunicationSetup : IEntity, ILogged<MasterDataLog>
    {
        public int Id { get; set; }
        public int? HandlerId { get; set; } //Transport.TransportId 
        [CascadeDelete()]
        public Company Handler { get; set; }
        public int CategoryId { get; set; }
        public CommunicationSetupCategory Category { get; set; }
        public int? HandlingPartyId { get; set; }
        public HandlingParty HandlingParty { get; set; }
        public int? PickUpLocationId { get; set; } //Party.CompanyId of the first Pickup stop
        public Company PickUpLocation { get; set; }
        public int? DeliveryLocationId { get; set; } //Party.CompanyId of the last delivery stop
        public Company DeliveryLocation { get; set; }
        public int? CustomerId { get; set; } //Customer Id of teh first shipment in the tranport, see financeorderservice line 40
        public Company Customer { get; set; }
        public int? OfficeId { get; set; }
        public Company Office { get; set; }
        public int? CommunicationProfileId { get; set; }
        public CommunicationProfile CommunicationProfile { get; set; }
        public string ContactEmail { get; set; }
        public int? ModalityId { get; set; }
        public Modality Modality { get; set; }
        public int? EmailDefinitionId { get; set; }
        public Damco.Model.Emails.EmailDefinition EmailDefinition { get; set; }
    }

    public class CommunicationSetupCategory : IEntity, ILogged<MasterDataLog>
    {
        public int Id { get; set; }
        [MaxLength(50), Required()]
        public string Code { get; set; }
        [MaxLength(100), Required()] 
        public string Name { get; set; }
        public List<CommunicationSetupCategoryHandlingParty> HandlerParties { get; set; } = new List<CommunicationSetupCategoryHandlingParty>();
        public string TagsAsString { get; set; }
        [NotMapped()]
        public string[] Tags
        {
            get { return this.TagsAsString?.FromJson<string[]>(); }
            set { this.TagsAsString = value?.ToJson(); }
        } 
    }

    public class HandlingParty : IEntity, ILogged<MasterDataLog>
    {
        public int Id { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }
        public List<CommunicationSetupCategoryHandlingParty> CommunicationSetupCategories { get; set; } = new List<CommunicationSetupCategoryHandlingParty>(); 
    }

    public class CommunicationSetupCategoryHandlingParty : IEntity, ILogged<MasterDataLog>
    {
        public int Id { get; set; }
        public int CommunicationSetupCategoryId { get; set; }
        public CommunicationSetupCategory CommunicationSetupCategory { get; set; }
        public int HandlingPartyId { get; set; }
        public HandlingParty HandlingParty { get; set; }
    }

}
