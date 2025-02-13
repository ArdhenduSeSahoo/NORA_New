using Damco.Model.DataSourcing;
using Damco.Model.Workflow;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Damco.Model;

namespace NORA.Model
{
    [NotMapped()]
    public class ShipmentStepOperation : Operation
    {
        public ShipmentStepType StepType { get; set; }
        public int? StepCategoryId { get; set; }
        public int? StepPairNumber { get; set; }
        public bool CreateWorkOrder { get; set; }
        public List<ShipmentStepOperationGroupingField> WorkOrderGroupingFields { get; set; } = new List<ShipmentStepOperationGroupingField>();
        public Operation PrepareNewWorkOrdersOperation { get; set; }
        public Operation PrepareCancelledWorkOrdersOperation { get; set; }
        public Operation PrepareChangedWorkOrdersOperation { get; set; }
        public Operation PrepareUnchangedWorkOrdersOperation { get; set; }
        public Operation FinishNewWorkOrdersOperation { get; set; }
        public Operation FinishCancelledWorkOrdersOperation { get; set; }
        public Operation FinishChangedWorkOrdersOperation { get; set; }
        public Operation FinishUnchangedWorkOrdersOperation { get; set; }
        public bool AddLinkOnDetailsLevel { get; set; }
        public string PredicateAsString { get; set; }
        [NotMapped()]
        public Expression<Func<ShipmentSplit, bool>> Predicate
        {
            get { return this.PredicateAsString?.DeserializeExpression<Func<ShipmentSplit, bool>>(); }
            set { this.PredicateAsString = value?.SerializeToString(); }
        }
        public bool CheckPredicateInDb { get; set; }
        public string DetailPredicateAsString { get; set; }
        [NotMapped()]
        public Expression<Func<ShipmentSplitDetail, bool>> DetailPredicate
        {
            get { return this.DetailPredicateAsString?.DeserializeExpression<Func<ShipmentSplitDetail, bool>>(); }
            set { this.DetailPredicateAsString = value?.SerializeToString(); }
        }
        public List<ShipmentStepOperationDocument> Documents { get; set; } = new List<ShipmentStepOperationDocument>();
        public bool CurrentWorkOrdersAreLeading { get; set; }
        public bool UseLimitedSimilarityForLinking { get; set; }
        public bool IncludeEquipmentTransportSets { get; set; }
        public bool IncludeEquipmentTransportSetsForSI { get; set; }
        public int? WorkOrderKind { get; set; }
        public bool CopyShipmentParties { get; set; }
        public bool GetStopsFromOtherTransports { get; set; }
        public bool GetInfoFromOtherTransports { get; set; }
        public bool AlignShipmentSteps { get; set; }
        public bool UpdateCarrierCargoDelivery { get; set; }
        public bool CopyFirstShipment { get; set; }
    }

    public class ShipmentStepOperationEndUserInput
    {
        public int? LinkToExistingTransportId { get; set; }
    }

    public class ShipmentGroupPotentialTransport
    {
        public int ShipmentGroupId { get; set; }
        public Transport Transport { get; set; }
    }

    public class ShipmentStepOperationGroupingField
    {
        public int DataFieldId { get; set; }
        public DataField DataField { get; set; }
        public bool SplitFurtherIfNeeded { get; set; }
    }

    public class ShipmentStepOperationDocument
    {
        public int DocumentTypeId { get; set; }
        public DocumentType DocumentType { get; set; }
        public string PredicateAsString { get; set; }
        public Expression<Func<Tentity, bool>> GetPredicate<Tentity>()
        {
            return this.PredicateAsString?.DeserializeExpression<Func<Tentity, bool>>();
        }
        public void SetPredicate<Tentity>(Expression<Func<Tentity, bool>> predicate)
        {
            this.PredicateAsString = predicate?.SerializeToString();
        }
        public string ReferenceGetterAsString { get; set; }

        public Expression<Func<Tentity, string>> GetReferenceGetter<Tentity>()
        {
            return this.ReferenceGetterAsString?.DeserializeExpression<Func<Tentity, string>>();
        }
        public void SetReferenceGetter<Tentity>(Expression<Func<Tentity, string>> referenceGetter)
        {
            this.ReferenceGetterAsString = referenceGetter?.SerializeToString();
        }

        public string UniqueIdGetterAsString { get; set; }
        public Expression<Func<Tentity, string>> GetUniqueIdGetter<Tentity>()
        {
            return this.UniqueIdGetterAsString?.DeserializeExpression<Func<Tentity, string>>();
        }
        public void SetUniqueIdGetter<Tentity>(Expression<Func<Tentity, string>> uniqueIdGetter)
        {
            this.UniqueIdGetterAsString = uniqueIdGetter?.SerializeToString();
        }
    }
}
