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
    public class Vehicle: IEntity, ILogged<MasterDataLog>
    {
        public int Id { get; set; }
        [Required(), MaxLength(100)]
        public string Name { get; set; }
        [Required(), MaxLength(50)]
        public string Reference { get; set; }
        public int? CountryId { get; set; } 
        public Country Country { get; set; }
        [ForeignKey("ItemId")]
        public List<VehicleAlias> Aliases { get; set; } = new List<VehicleAlias>();
        public int ModalityId { get; set; }
        public Modality Modality { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class VehicleAlias: AliasBase<Vehicle>, ILogged<MasterDataLog>, IEntity { }
    
    public class VoyageStopInfo: IEntity
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public Vehicle Vehicle { get; set; }
        public int CompanyId { get; set; }
        public Company Company { get; set; }
        public DateTime ApproximateDeliveryDateTime { get; set; }
        [MaxLength(50)]
        public string VehicleReferenceAdditional { get; set; }
    }

    public class VoyageLegInfo : IEntity
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public Vehicle Vehicle { get; set; }
        public int PickUpCompanyId { get; set; }
        public Company PickUpCompany { get; set; }
        public int DeliveryCompanyId { get; set; }
        public Company DeliveryCompany { get; set; }
        public DateTime ApproximateDeliveryDateTime { get; set; }
        [MaxLength(50)]
        public string VoyageReferenceAdditional { get; set; }
        public int? CarrierId { get; set; }
        public Company Carrier { get; set; }
    }



    /*
     * 
     *         public string VehicleReference { get; set; }
        [MaxLength(50)]
        public string VehicleReferenceAdditional { get; set; }
        [MaxLength(50)]
        public string VehicleReferenceAdditional2 { get; set; }
        public string VehicleName { get; set; }
        public string VoyageReference { get; set; }
        [MaxLength(50)]
        public string VoyageReferenceAdditional { get; set; }
        */

}
