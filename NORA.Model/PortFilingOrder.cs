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
    public class PortFilingLog : ChangeLogBase, IEntity { }
    public class PortFilingOrder : WorkOrder, IEntity, ILogged<PortFilingLog>, IInterfacedEntity
    {
        [MaxLength(50)]
        public string Reference { get; set; }
        [MaxLength(50)]
        public string BookingReference { get; set; }
        [MaxLength(50)]
        public string PortFilingReference { get; set; }
        public string PortFilingStatusDescription { get; set; }
        [MaxLength(50)]
        public string PortFilingStatusCode { get; set; }
        public string SupplierComments { get; set; }
        //TODO: Delete next two
        //public int? DepotId { get; set; }
        //public Company Depot { get; set; }
        //public bool HasPendingChangesFromInterface { get; set; }
        public bool HasPendingErrorsFromInterface { get; set; }
        public List<PortFilingOrderError> Errors { get; set; } = new List<PortFilingOrderError>();
    }

    public class PortFilingOrderError : IEntity, ILogged<PortFilingLog>
    {
        public int Id { get; set; }
        public int PortFilingOrderId { get; set; }
        public PortFilingOrder PortFilingOrder { get; set; }
        [MaxLength(50)]
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public int? HandlerPartyId { get; set; }
        public HandlingParty HandlerParty { get; set; }
    }
}
