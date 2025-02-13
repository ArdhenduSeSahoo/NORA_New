using Damco.Model;
using Damco.Model.Workflow;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORA.Model
{
    public class CustomsClearanceLog : ChangeLogBase, IEntity { }
    public class CustomsClearanceOrder : WorkOrder, IEntity, ILogged<CustomsClearanceLog>
    {
        [MaxLength(50)]
        public string Reference { get; set; }
        [MaxLength(50)]
        public string CustomsReleaseNumber { get; set; }
        public string SupplierComments { get; set; }
        public DateTime? ClearanceDate { get; set; }
        [MaxLength(50)]
        public string T1DeclarationNumber { get; set; }
        public int? MRNControlMeasureId { get; set; }
        public MRNControlMeasure MRNControlMeasure { get; set; }

        public int? CustomsClearanceTypeId { get; set; }
        public CustomsClearanceType CustomsClearanceType { get; set; }
        public int? CustomsClearanceShippingTypeId { get; set; }
        public CustomsClearanceShippingType CustomsClearanceShippingType { get; set; }
        public int? CustomsClearanceInvestigationTypeId { get; set; }
        public CustomsClearanceInvestigationType CustomsClearanceInvestigationType { get; set; }

        public int? DepotId { get; set; }
        public Company Depot { get; set; }
        public int? ExitCompanyId { get; set; }
        public Company ExitCompany { get; set; }
    }

    //Also called "MRN type"
    public class CustomsClearanceType : ILogged<MasterDataLog>, IEntity
    {
        public int Id { get; set; }
        [Required(), MaxLength(50)]
        public string Code { get; set; }
        [Required(), MaxLength(100)]
        public string Name { get; set; }
        public List<CustomsClearanceTypeCountry> Countries { get; set; }
        public int? MinimumLengthMRN { get; set; }
        public int? MaximumLengthMRN { get; set; }
        public List<CustomsClearanceOrder> CustomsClearanceOrders { get; set; } = new List<CustomsClearanceOrder>();
    }

    //Also called "Versand status"
    public class CustomsClearanceShippingType : IEntity, ILogged<MasterDataLog>
    {
        public int Id { get; set; }
        [Required(), MaxLength(50)]
        public string Code { get; set; }
        [Required(), MaxLength(100)]
        public string Name { get; set; }
    }

    //Also called "MRN control status"
    public class CustomsClearanceInvestigationType : IEntity, ILogged<MasterDataLog>
    {
        public int Id { get; set; }
        [Required(), MaxLength(50)]
        public string Code { get; set; }
        [Required(), MaxLength(100)]
        public string Name { get; set; }
    }



    //Not used (yet)
    public class CustomsClearanceTypeCountry : ILogged<MasterDataLog>, IEntity
    {
        public int Id { get; set; }
        public int CustomsClearanceTypeId { get; set; }
        public CustomsClearanceType CustomsClearanceType { get; set; }
        public int CountryId { get; set; }
        public Country Country { get; set; }
    }

}
