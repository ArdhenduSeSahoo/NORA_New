using Damco.Model;
using Damco.Model.DataSourcing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NORA.Model
{
    public class FinanceCalculationQuantitySource : IEntity, ILogged<MasterDataLog>
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }
        public string Description { get; set; }
        public int? MeasurementUnitId { get; set; }
        public MeasurementUnit MeasurementUnit { get; set; }
        public List<FinanceCalculationQuantitySourceDataMap> DataMaps { get; set; } = new List<FinanceCalculationQuantitySourceDataMap>();
    }

    public class FinanceCalculationQuantitySourceDataMap : IEntity, ILogged<MasterDataLog>
    {
        public int Id { get; set; }
        public int FinanceCalculationQuantitySourceId { get; set; }
        [CascadeDelete()]
        public FinanceCalculationQuantitySource FinanceCalculationQuantitySource { get; set; }

        [Required()]
        public string EntityTypeAsString { get; set; }
        [NotMapped()]
        public Type EntityType
        {
            get { return this.EntityTypeAsString?.GetTypeForName(); }
            set { this.EntityTypeAsString = value?.GetNameForType(); }
        }

        //Default: Count() of the items
        public int? QuantityDataFieldId { get; set; }
        public DataField QuantityDataField { get; set; }

        public int? ItemSelectorDataFieldId { get; set; }
        public DataField ItemSelectorDataField { get; set; }

        public int? EquipmentTransportTypeDataFieldId { get; set; }
        public DataField EquipmentTransportTypeDataField { get; set; }

        public int? ItemPredicateDataFieldId { get; set; }
        public DataField ItemPredicateDataField { get; set; }

        public int? ComboDataFieldId { get; set; }
        public DataField ComboDataField { get; set; }

        public int? ComboAllocationDataFieldId { get; set; }
        public DataField ComboAllocationDataField { get; set; }

        public int? CategoryCodeDataFieldId { get; set; }
        public DataField CategoryCodeDataField { get; set; }

        public int? RelevantDocumentDataFieldId { get; set; }
        public DataField RelevantDocumentDataField { get; set; }

    }
}
