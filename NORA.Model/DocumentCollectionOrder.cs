using Damco.Model;
using Damco.Model.Workflow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORA.Model
{
    public class DocumentCollectionLog : ChangeLogBase, IEntity { }

    public class DocumentCollectionOrder : IEntity , IWorkflowControlled, ILogged<DocumentCollectionLog>
    {
        public int? VersionNumber { get; set; }
        public int Id { get; set; }
        public int? StatusId { get; set; }
        public Status Status { get; set; }
        public DateTime? StatusChangeDateTime { get; set; }
        public string Reference { get; set; }
        public List<DocumentCollectionOrderDetail> Details { get; set; } = new List<DocumentCollectionOrderDetail>();
        public bool IsHot { get; set; }
        public DateTime? DoneDateTime { get; set; }
        public string InternalContactEmail { get; set; }
    }

    public class DocumentCollectionOrderDetail : IEntity, ILogged<DocumentCollectionLog>
    {
        public int Id { get; set; }
        public int DocumentCollectionOrderId { get; set; }
        [CascadeDelete()]
        public DocumentCollectionOrder DocumentCollectionOrder { get; set; }
        public int TransportationDocumentId { get; set; }
        [CascadeDelete()]
        public TransportationDocument TransportationDocument { get; set; }
    }

}
