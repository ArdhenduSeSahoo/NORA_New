using Damco.Model;
using NORA.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORA.Model
{
    public class ExpectedShipmentGroup : IEntity, ILogged<ShipmentGroupLog>
    {
        public int Id { get; set; }
        public int ShipmentGroupId { get; set; }
        public ShipmentGroup ShipmentGroup { get; set; }
        public int? CategoryId { get; set; }
        public ShipmentStepCategory Category { get; set; }
        public int? ModalityId { get; set; }
        public Modality Modality { get; set; }
    }
}
