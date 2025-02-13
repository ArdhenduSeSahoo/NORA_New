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
    public class EventType : IEntity, ILogged<MasterDataLog>
    {
        public int Id { get; set; }
        [MaxLength(50), Required()]
        public string Code { get; set; }
        [MaxLength(100), Required()]
        public string Name { get; set; }
        public ShipmentStepType? StepType { get; set; }
        public int? AllowedDaysBeforeStepDate { get; set; }
        public int? AllowedDaysAfterStepDate { get; set; }
        public bool IsStepStart { get; set; }
        public bool IsStepEnd { get; set; }
        public bool IsCutOff { get; set; }
    }

    public class EventTypeAlias : AliasBase<EventType>, IEntity, ILogged<MasterDataLog>
    {
        public UpdateEventDateType? UpdateEventDateType { get; set; }
    }

    public enum UpdateEventDateType
    {
        Actual = 1,
        Planned = 2
    }
}
