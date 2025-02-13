using Damco.Model;
using Damco.Model.Interfacing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORA.Model
{
    public class EquipmentTransportType : IEntity, ILogged<MasterDataLog>
    {
        public int Id { get; set; }
        [MaxLength(50), Required()]
        public string Code { get; set; }
        public decimal? TEU { get; set; }
        public EquipmentTransportTypeFamily Family { get; set; }
        public int? CategoryId { get; set; }
        public decimal? MaximumWeight { get; set; }
        public decimal? MaximumNetWeight { get; set; }
        public decimal? MaximumTareWeight { get; set; }
        public EquipmentTransportCategory Category { get; set; }
        public bool IsInlet { get; set; }
        public bool SolasApplicable { get; set; }
        public bool StandardContainerNumbers { get; set; }
    }

    [Flags()]
    public enum EquipmentTransportTypeFamily
    {
        Normal = 0,
        GarmentOnHanger = 1,
        Reefer = 2,
        ReeferWithGarmentsOnHanger = 3
    }

    public class EquipmentTransportTypeAlias : AliasBase<EquipmentTransportType>, IEntity, ILogged<MasterDataLog> { }

    public class EquipmentTransportCategory : IEntity, ILogged<MasterDataLog>
    {
        public int Id { get; set; }
        [MaxLength(50), Required()]
        public string Code { get; set; }
    }
}
