using Damco.Model;
using Damco.Model.MultiTenancy;
using Damco.Model.UserManagement;
using Damco.Model.Workflow;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORA.Model
{
    public class IssueLog : ChangeLogBase, IEntity { }
    public class Issue : IEntity, ILogged<IssueLog>, IModifyTracking, IWorkflowControlled, IStandardMeasurementContainer
    {
        public int Id { get; set; }
        public int IssueTypeId { get; set; }
        [MaxLength(100)]
        public string Title { get; set; }
        public string Description { get; set; }
        public string Comments { get; set; }
        public IssueType Type { get; set; }
        public int? HandlingPartyId { get; set; }
        public int? ReasonId { get; set; }
        public IssueReason Reason { get; set; }
        public HandlingParty HandlingParty { get; set; }
        public List<IssueDetail> Details { get; set; } = new List<IssueDetail>();
        public DateTime AddDateTime { get; set; }
        public User AddUser { get; set; }
        public int? AddUserId { get; set; }
        public DateTime EditDateTime { get; set; }
        public User EditUser { get; set; }
        public int? EditUserId { get; set; }
        public int? LocationCompanyId { get; set; }
        public Company LocationCompany { get; set; }
        public int? TenantId { get; set; }
        public Tenant Tenant { get; set; }
        [MaxLength(50)]
        public string CorrelationId { get; set; }
        public int? StatusId { get; set; }
        public Status Status { get; set; }
        public DateTime? StatusChangeDateTime { get; set; }
        public int? VersionNumber { get; set; }
        public decimal? Weight { get; set; }
        public decimal? Volume { get; set; }
        public decimal? Units { get; set; }
        public decimal? Cartons { get; set; }
        public decimal? Pallets { get; set; }
        public decimal? NetWeight { get; set; }
        public decimal? TotalGrossWeight { get; set; }
        public decimal? ChargeableWeight { get; set; }
    }

    public class IssueType : IEntity, ILogged<MasterDataLog>
    {
        public int Id { get; set; }
        [Required(), MaxLength(100)]
        public string Name { get; set; }
        [Required(), MaxLength(50)]
        public string Code { get; set; }
        public string TagsAsString { get; set; }
        [NotMapped()]
        public string[] Tags
        {
            get { return this.TagsAsString?.FromJson<string[]>(); }
            set { this.TagsAsString = value?.ToJson(); }
        }
        public List<IssueTypeTenantFamily> TenantFamilies { get; set; } = new List<IssueTypeTenantFamily>();
        public List<IssueTypeReason> IssueReasons { get; set; } = new List<IssueTypeReason>();
    }

    public class IssueReason : IEntity, ILogged<MasterDataLog>
    {
        public int Id { get; set; }
        [Required(), MaxLength(50)]
        public string Code { get; set; }
        [Required(), MaxLength(100)]
        public string Name { get; set; }
        public List<IssueTypeReason> IssueTypes { get; set; } = new List<IssueTypeReason>();
    }

    public class IssueTypeReason: IEntity, ILogged<MasterDataLog>
    {
        public int Id { get; set; }
        public int IssueTypeId { get; set; }
        [CascadeDelete()]
        public IssueType IssueType { get; set; }
        public int IssueReasonId { get; set; }
        [CascadeDelete()]
        public IssueReason IssueReason { get; set; }
    }

    public class IssueReasonModality : IEntity, ILogged<MasterDataLog>
    {
        public int Id { get; set; }
        public int IssueReasonId { get; set; }
        [CascadeDelete()]
        public IssueReason IssueReason { get; set; }
        public int ModalityId { get; set; }
        [CascadeDelete()]
        public Modality Modality { get; set; }
    }

    public class IssueTypeTenantFamily : IEntity, ILogged<MasterDataLog>
    {
        public int Id { get; set; }
        public int IssueTypeId { get; set; }
        [CascadeDelete()]
        public IssueType IssueType { get; set; }
        public int TenantFamilyId { get; set; }
        [CascadeDelete()]
        public TenantFamily TenantFamily { get; set; }
    }

    public abstract class IssueDetail : IEntity, ILogged<IssueLog>
    {
        public int Id { get; set; }
        public int IssueId { get; set; }
        public Issue Issue { get; set; }
    }

    public interface IIssueDetail<T> where T : IEntity
    {
        void SetLinkEntity(T linkEntity);
    }

    public class TransportIssueDetail : IssueDetail, IEntity, IIssueDetail<Transport>
    {
        public int TransportId { get; set; }
        public Transport Transport { get; set; }
        public void SetLinkEntity(Transport linkEntity) { this.Transport = linkEntity; }
    }

    public class ShipmentIssueDetail : IssueDetail, IEntity, IIssueDetail<Shipment>
    {
        public int ShipmentId { get; set; }
        public Shipment Shipment { get; set; }
        public void SetLinkEntity(Shipment linkEntity) { this.Shipment = linkEntity; }
    }

    public class ShipmentGroupIssueDetail : IssueDetail, IEntity, IIssueDetail<ShipmentGroup>
    {
        public int ShipmentGroupId { get; set; }
        public ShipmentGroup ShipmentGroup { get; set; }
        public void SetLinkEntity(ShipmentGroup linkEntity) { this.ShipmentGroup = linkEntity; }
    }

    public class ShipmentSplitDetailIssueDetail: IssueDetail, IEntity, IIssueDetail<ShipmentSplitDetail>
    {
        public int ShipmentSplitDetailId { get; set; }
        [CascadeDelete()]
        public ShipmentSplitDetail ShipmentSplitDetail { get; set; }
        public void SetLinkEntity(ShipmentSplitDetail linkEntity) { this.ShipmentSplitDetail = linkEntity; }
    }

}
