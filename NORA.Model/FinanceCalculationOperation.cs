using Damco.Model;
using Damco.Model.DataSourcing;
using Damco.Model.Interfacing;
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
    public class FinanceCalculationOperation : Operation, IEntity
    {
        public List<FinanceCalculationOperationPart> Parts { get; set; }
    }

    public abstract class FinanceCalculationOperationPart : IEntity
    {
        public int Id { get; set; }
        [MaxLength(100), Required()]
        public string Name { get; set; }
        public int FinanceCalculationOperationId { get; set; }
        public FinanceCalculationOperation FinanceCalculationOperation { get; set; }
        public int? FinanceCalculationTypeId { get; set; }
        public FinanceCalculationType FinanceCalculationType { get; set; }
        public int? PredicateDataFieldId { get; set; }
        public DataField PredicateDataField { get; set; }
    }

    public class FinanceCalculationType : IEntity
    {
        public int Id { get; set; }
        [MaxLength(100), Required()]
        public string Name { get; set; }
    }

    public interface IFinanceLineTemplate
    {
        int ChargeId { get; set; }
        Charge Charge { get; set; }
        FinanceLineType DefaultType { get; set; }
        int? DefaultCurrencyId { get; set; }
        Currency DefaultCurrency { get; set; }
        string DefaultCustomerChargeFinancialCode { get; set; }
        int? DefaultQuantityUnitId { get; set; }
        MeasurementUnit DefaultQuantityUnit { get; set; }
        decimal? DefaultRatePerUnit { get; set; }
        string DefaultComments { get; set; }
    }

    public class FinanceCalculationOperationRateContractSet : FinanceCalculationOperationPart, IEntity, IFinanceLineTemplate
    {
        public int ChargeId { get; set; }
        public Charge Charge { get; set; }
        public int RateContractSetId { get; set; }
        public RateContractSet RateContractSet { get; set; }
        public bool CreateLineIfContractMissing { get; set; }
        public bool CreateLineIfRateMissing { get; set; }
        public FinanceLineType DefaultType { get; set; }
        public int? DefaultCurrencyId { get; set; }
        public Currency DefaultCurrency { get; set; }
        public string DefaultCustomerChargeFinancialCode { get; set; }
        public int? DefaultQuantityUnitId { get; set; }
        public MeasurementUnit DefaultQuantityUnit { get; set; }
        public decimal? DefaultRatePerUnit { get; set; }
        public string DefaultComments { get; set; }
        public int DefaultQuantitySourceId { get; set; }
        public FinanceCalculationQuantitySource DefaultQuantitySource { get; set; }
        public bool ProvidesServiceContractNumberForTransport { get; set; }
        public int? ShipmentStepCategoryId { get; set; }
        public ShipmentStepCategory ShipmentStepCategory { get; set; }
    }

    public class FinanceCalculationOperationExternalSource : FinanceCalculationOperationPart, IEntity
    {
        public int ServiceCallId { get; set; }
        //public OutboundServiceCall ServiceCall { get; set; } //TODO: Add back once outbound service call becomes a table
        public int InterfaceSetupId { get; set; }
        public InterfaceSetup InterfaceSetup { get; set; }
        public List<FinanceCalculationOperationExternalSourceCharge> ExpectedCharges { get; set; }
    }

    public class FinanceCalculationOperationExternalSourceCharge : IEntity, IFinanceLineTemplate
    {
        public int Id { get; set; }
        public int FinanceCalculationOperationExternalSourceId { get; set; }
        [CascadeDelete()]
        public FinanceCalculationOperationExternalSource FinanceCalculationOperationExternalSource { get; set; }
        public int ChargeId { get; set; }
        public Charge Charge { get; set; }
        public bool CreateLineIfMissing { get; set; }
        public FinanceLineType DefaultType { get; set; }
        public int? DefaultCurrencyId { get; set; }
        public Currency DefaultCurrency { get; set; }
        public string DefaultCustomerChargeFinancialCode { get; set; }
        public int? DefaultQuantityUnitId { get; set; }
        public MeasurementUnit DefaultQuantityUnit { get; set; }
        public decimal? DefaultRatePerUnit { get; set; }
        public string DefaultComments { get; set; }
    }


}
