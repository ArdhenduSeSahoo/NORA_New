using Damco.Model.DataSourcing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model.ScreenTemplating
{
    public class TableTemplate : Template
    {
        public int DataSourceId { get; set; }
        public DataSource DataSource { get; set; }
        public List<TableTemplateField> Fields { get; set; } = new List<TableTemplateField>();
        public List<TableTemplateColorCodingRule> ColorCodingRules { get; set; } = new List<TableTemplateColorCodingRule>();

        public int DefaultRowHeight { get; set; }
    }

    public class TableTemplateColorCodingRule
    {
        public int TableTemplateId { get; set; }
        public TableTemplate TableTemplate { get; set; }
        public int? TableTemplateFieldId { get; set; }
        public TableTemplateField TableTemplateField { get; set; }
        public ContextualStyle Style { get; set; }
        [NotMapped()]
        public Expression<Func<DynamicEntity, bool>> Condition
        {
            get { return this.ConditionAsString?.DeserializeExpression<Func<DynamicEntity, bool>>(); }
            set { this.ConditionAsString = value?.SerializeToString(); }
        }
        public string ConditionAsString { get; set; }
        public int Sequence { get; set; }
    }

    public enum ContextualStyle
    {
        Active = 1,
        Success = 2,
        Info = 3,
        Warning = 4,
        Danger = 5,
        grey = 6,
        Purple = 7

    }

    public class TableTemplateField
    {
        public int Id { get; set; }
        public int OrderNumber { get; set; }
        [Translatable(), Required()]
        public string DisplayName { get; set; }
        [NotMapped()]
        public Expression<Func<IEnumerable<DynamicEntity>, bool>> ShowPredicate
        {
            get { return this.ShowPredicateAsString.DeserializeExpression<Func<IEnumerable<DynamicEntity>, bool>>(); }
            set { this.ShowPredicateAsString = value.SerializeToString(); }
        }
        public string ShowPredicateAsString { get; set; }
        public int FieldDisplayId { get; set; }
        public FieldDisplay FieldDisplay { get; set; }
        public List<TableTemplateColorCodingRule> ColorCodingRules { get; set; }
        public bool HiddenByDefault { get; set; }
        public int? Width { get; set; }
        public bool UsePercentageToWidth { get; set; }
        public int? MinWidth { get; set; }

        public bool FieldIsSelectable { get; set; }
        public string FieldDisplayName { get; set; }

        //Note the total layout will be based on the FieldDisplay
        //the total value will be the FieldDisplay.DataField value but aggregated on all
        //records that match the screen filter
        public TotalType TotalType { get; set; }
        public string HeaderTooltip { get; set; }
    }

    public enum TotalType
    {
        None = 0,
        //Custom = 1, //Not available yet
        Sum = 2,
        Average = 3,
        CountDistinct = 4
    }

    //  public bool InFilterByDefault { get; set; }
    //  public bool InReferenceSearch { get; set; }



}
