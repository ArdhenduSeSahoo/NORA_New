using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Damco.Model;
using Damco.Model.Workflow;
using System.ComponentModel.DataAnnotations;
using Damco.Model.Interfacing;

namespace NORA.Model
{
    public class DangerousCargoLog : ChangeLogBase, IEntity { }
    public class DangerousCargoOrder : WorkOrder, IEntity, ILogged<DangerousCargoLog>, IInterfacedEntity
    {
        [MaxLength(50)]
        public string Reference { get; set; }
        public string SupplierComments { get; set; }
        public List<DangerousCargoOrderError> Errors { get; set; }
        //public bool HasPendingChangesFromInterface { get; set; }
        public bool HasPendingErrorsFromInterface { get; set; }
    }

    public class DangerousCargoOrderError : IEntity, ILogged<DangerousCargoLog>
    {
        public int Id { get; set; }
        public int DangerousCargoOrderId { get; set; }
        public DangerousCargoOrder DangerousCargoOrder { get; set; }
        [MaxLength(50)]
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public int? HandlerPartyId { get; set; }
        public HandlingParty HandlerParty { get; set; }
    }
}
