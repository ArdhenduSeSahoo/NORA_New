using Damco.Model;
using Damco.Model.Workflow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Damco.Model.Interfacing;
using System.ComponentModel.DataAnnotations;
using Damco.Model.UserManagement;
using Damco.Model.MultiTenancy;

namespace NORA.Model
{
    public class DocumentCreationOrder : IEntity, IWorkflowControlled, ILogged<DocumentCreationLog>, IInterfacedEntity
    {
        public int? VersionNumber { get; set; }
        public int Id { get; set; }
        public int? StatusId { get; set; }
        public Status Status { get; set; }
        public DateTime? StatusChangeDateTime { get; set; }
        public int TransportationDocumentId { get; set; }
        public TransportationDocument TransportationDocument { get; set; }
        public bool IsHot { get; set; }
        public string InternalContactEmail { get; set; }
        //public List<DocumentCreationOrderError> Errors { get; set; }
        public bool HasPendingErrorsFromInterface { get; set; }
        public DateTime? ReleasedDateTime { get; set; }
        public int? AmendmentReasonId { get; set; }
        public AmendmentReason AmendmentReason { get; set; }
        public int? ApproverUserId { get; set; }
        public User ApproverUser { get; set; }
        public int? TenantId { get; set; }
        public Tenant Tenant { get; set; }
        [MaxLength(50)]
        public string CorrelationId { get; set; }

        public bool OverwriteDescription { get; set; }
    }

    public class DocumentCreationOrderFreeTextDescription : ILogged<DocumentCreationLog>
    {
        public int Id { get; set; }
        [Required]
        public int DocumentCreationOrderId { get; set; }

        public string MarksAndNumbers { get; set; }
        public int? NumberOfPackages { get; set; }

        public string DescriptionOfGoods { get; set; }

        public decimal? GrossWeightKgs { get; set; }

        public decimal? MeasurementCBM { get; set; }

    }

    public class AmendmentReason: IEntity, ILogged<MasterDataLog>
    {
        public int Id { get; set; }
        [Required()]
        public string Description { get; set; }
    }

    public class DocumentCreationLog : ChangeLogBase, IEntity
    {
    }
}
