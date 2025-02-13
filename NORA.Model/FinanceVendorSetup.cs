using Damco.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORA.Model
{
    public class FinanceVendorSetup: IEntity, ILogged<MasterDataLog>
    {
        public int Id { get; set; }
        public int WorkOrderVendorId { get; set; } //Transport.TransportId 
        public Company WorkOrderVendor { get; set; }
        public int? PickUpLocationId { get; set; } //Party.CompanyId of the first Pickup stop
        public Company PickUpLocation { get; set; }
        public int? DeliveryLocationId { get; set; } //Party.CompanyId of the last delivery stop
        public Company DeliveryLocation { get; set; }
        public int? CustomerId { get; set; } //Customer Id of teh first shipment in the tranport, see financeorderservice line 40
        public Company Customer { get; set; }
        public int FinanceVendorId { get; set; }
        public Company FinanceVendor { get; set; }
        public int? ShipmentStepCategoryId { get; set; }
        public ShipmentStepCategory ShipmentStepCategory { get; set; }
    }
}
