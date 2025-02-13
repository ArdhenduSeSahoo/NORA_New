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
    public class SolasLog : ChangeLogBase, IEntity { }
    public class SolasOrder : WorkOrder, IEntity, ILogged<SolasLog>, IInterfacedEntity
    {
        public string SupplierComments { get; set; }
        //public bool HasPendingChangesFromInterface { get; set; }
        public bool HasPendingErrorsFromInterface { get; set; }
        public List<SolasOrderError> Errors { get; set; }
    }

    public class SolasOrderError : IEntity, ILogged<SolasLog>
    {
        public int Id { get; set; }
        public int SolasOrderId { get; set; }
        public SolasOrder SolasOrder { get; set; }
        [MaxLength(50)]
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public int? HandlerPartyId { get; set; }
        public HandlingParty HandlerParty { get; set; }
    }

}
