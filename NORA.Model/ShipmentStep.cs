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
    public class ShipmentStep : IEntity, ILogged<ShipmentLog>
    {
        public int Id { get; set; }
        public int ShipmentId { get; set; }
        public virtual Shipment Shipment { get; set; }
        public int Sequence { get; set; }
        public DateTime? CanStartDateTime { get; set; }
        public DateTime? ShouldEndDateTime { get; set; }
        public int? PartyId { get; set; }
        public ShipmentParty Party { get; set; }
        public int? CustomerRequestedHandlerId { get; set; }
        public Company CustomerRequestedHandler { get; set; }
        public ShipmentStepType Type { get; set; }
        public bool CustomerRequested { get; set; }
        public string HandlerInstructions { get; set; }
        public int? CategoryId { get; set; }
        public ShipmentStepCategory Category { get; set; }
        public List<ShipmentSplitStep> SplitSteps { get; set; } = new List<ShipmentSplitStep>();
        public int? PairNumber { get; set; }
        public int? PlannedModalityId { get; set; }
        public Modality PlannedModality { get; set; }
        [MaxLength(50)]
        public string PlannedModalityExternalCode { get; set; }
    }

    public enum ShipmentStepType
    {
        CargoPickUp = 1,
        CargoDelivery = 2,
        CustomsClearance = 3,
        DangerousCargo = 4,
        PortFiling = 5,
        Solas = 6,
        EmptyContainerPickUp = 7,

        //TODO: Remove the old ones + backwards compatibilty
        CustomerCargoReceipt = 8,
        CarrierCargoReceipt = 9,
        CustomerCargoDelivery = 10,
        CarrierCargoDelivery = 11,
    }

    public class ShipmentStepCategory : IEntity, ILogged<MasterDataLog>
    {
        public int Id { get; set; }
        [Required(), MaxLength(100)]
        public string Code { get; set; }
        [Required(), MaxLength(100)]
        public string Name { get; set; }
        [NotMapped()]
        public string[] Tags
        {
            get { return this.TagsAsString?.FromJson<string[]>() ?? new string[] { }; }
            set { this.TagsAsString = (value == null || value.Length == 0 ? null : value.ToJson()); }
        }
        public string TagsAsString { get; set; }
    }
    public class ShipmentStepCategoryAlias : AliasBase<ShipmentStepCategory>, IEntity, ILogged<MasterDataLog> { }
}
