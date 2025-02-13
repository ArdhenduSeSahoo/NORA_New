using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Damco.Model;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using NORA.Model.Base;

namespace NORA.Model
{
    [Serializable()]
    public class ShipmentSplitLog : ChangeLogBase, IEntity { }

    [Serializable()]
    public class ShipmentSplit : IEntity, IDeletedProperties, ILogged<ShipmentSplitLog>
    {
        public int Id { get; set; }

        #region Children
        public virtual List<ShipmentSplitStep> Steps { get; set; } = new List<ShipmentSplitStep>();
        public virtual List<ShipmentSplitDetail> Details { get; set; } = new List<ShipmentSplitDetail>();
        #endregion

        public int? RequestedEquipmentTransportId { get; set; }
        public EquipmentTransport RequestedEquipmentTransport { get; set; }
        public int? PlannedEquipmentTransportId { get; set; }
        public EquipmentTransport PlannedEquipmentTransport { get; set; }
        public int? ActualEquipmentTransportId { get; set; }
        public EquipmentTransport ActualEquipmentTransport { get; set; }
        public int ShipmentId { get; set; }
        public Shipment Shipment { get; set; }
        public List<ShipmentSplitDocument> Documents { get; set; } = new List<ShipmentSplitDocument>();
        public int? ShipmentGroupId { get; set; }
        public ShipmentGroup ShipmentGroup { get; set; }
        public int? ShipmentStepCategoryId { get; set; }
        public ShipmentStepCategory ShipmentStepCategory { get; set; }
        public int? LoadId { get; set; }
        public ShipmentLoad Load { get; set; }
        public bool? IsDeleted { get; set; }
    }

    [Serializable()]
    public class ShipmentSplitStep : IEntity, IDeletedProperties, ILogged<ShipmentSplitLog>
    {
        public int Id { get; set; }
        public int ShipmentSplitId { get; set; }
        public ShipmentSplit ShipmentSplit { get; set; }
        public int? WorkOrderId { get; set; }
        [CascadeDelete()]
        public WorkOrder WorkOrder { get; set; }
        public int ShipmentStepId { get; set; }
        [CascadeDelete()]
        public ShipmentStep ShipmentStep { get; set; }

        public DateTime? OriginalPlannedStartDateTime { get; set; }
        public DateTime? OriginalPlannedEndDateTime { get; set; }
        public DateTime? LastPlannedStartDateTime { get; set; }
        public DateTime? LastPlannedEndDateTime { get; set; }
        public DateTime? ActualStartDateTime { get; set; }
        public DateTime? ActualEndDateTime { get; set; }
        public int? WorkOrderKind { get; set; }

        public bool? IsDeleted { get; set; }
    }

    [Serializable()]
    public class ShipmentSplitDetail : IEntity, ILogged<ShipmentSplitLog>, IDeletedProperties, IStandardMeasurementContainer
    {
        public int Id { get; set; }
        public int ShipmentSplitId { get; set; }
        public ShipmentSplit ShipmentSplit { get; set; }
        public int ShipmentDetailId { get; set; }
        [CascadeDelete()]
        public ShipmentDetail ShipmentDetail { get; set; }
        public decimal? Weight { get; set; }
        public decimal? Volume { get; set; }
        public decimal? Units { get; set; }
        public decimal? Cartons { get; set; }
        public decimal? Pallets { get; set; }
        public decimal? NetWeight { get; set; }
        public decimal? TotalGrossWeight { get; set; }
        public List<WorkOrderDetail> WorkOrderDetails { get; set; } = new List<WorkOrderDetail>();
        public decimal? ChargeableWeight { get; set; }
        public List<ShipmentSplitDetailCartonRange> CartonRanges { get; set; } = new List<ShipmentSplitDetailCartonRange>();
        public List<ShipmentSplitDetailIssueDetail> IssueDetails { get; set; } = new List<ShipmentSplitDetailIssueDetail>();

        public bool? IsDeleted { get; set; }
    }

    public class ShipmentSplitDetailCartonRange : IEntity, IDeletedProperties, ILogged<ShipmentSplitLog>
    {
        public int Id { get; set; }
        public int ShipmentSplitDetailId { get; set; }
        [CascadeDelete()]
        public ShipmentSplitDetail ShipmentSplitDetail { get; set; }
        public int CartonRangeId { get; set; }
        [CascadeDelete()]
        public CartonRange CartonRange { get; set; }
        public int? Cartons { get; set; }
        public int FromCartonNumber { get; set; }
        public int ToCartonNumber { get; set; }
        public int? Units { get; set; }

        public bool? IsDeleted { get; set; }
    }
}
