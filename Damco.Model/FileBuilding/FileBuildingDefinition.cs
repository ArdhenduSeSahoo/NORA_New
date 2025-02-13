using Damco.Model.DataSourcing;
using Damco.Model.Workflow;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model.FileBuilding
{
    [NotMapped()]
    public abstract class FileBuildingDefinition : Operation
    {
        public string FileName { get; set; }
    }

    public abstract class DataSourcedFileBuildingDefinition : FileBuildingDefinition, IRequiresDataExtract
    {
        public int DataExtractId { get; set; }
        public DataExtract DataExtract { get; set; }
        public ArchiveType ArchiveTypeMultipleFiles { get; set; }
        public bool AlwaysUseArchiveType { get; set; }
        public List<DataSourcedFileBuildingDefinitionGroupingField> GroupingFields { get; set; } = new List<DataSourcedFileBuildingDefinitionGroupingField>();
        public bool GroupPerItem { get; set; }
        public string SamplePredicateAsString { get; set; }
        public Expression<Func<Tentity, bool>> GetSamplePredicate<Tentity>() { return this.SamplePredicateAsString?.DeserializeExpression<Func<Tentity, bool>>(); }
        public LambdaExpression GetSamplePredicate() { return (LambdaExpression)this.SamplePredicateAsString?.DeserializeExpression(Expression.Parameter(this.SampleEntityType, "p")); }
        public void SetSamplePredicate<Tentity>(Expression<Func<Tentity, bool>> value)
        {
            this.SampleEntityType = typeof(Tentity);
            this.SamplePredicateAsString = value?.SerializeToString();
        }
        public string SampleEntityTypeAsString { get; set; }
        [NotMapped()]
        public Type SampleEntityType
        {
            get { return ExpressionSerialization.GetTypeForName(this.SampleEntityTypeAsString) ?? typeof(DynamicEntity); }
            set { this.SampleEntityTypeAsString = value == typeof(DynamicEntity) ? null : ExpressionSerialization.GetNameForType(value); }
        }

        public string FileNameTemplate { get; set; }
        [NotMapped()]
        public int FileNameDataExtractFieldId { get; set; }
        [NotMapped()]
        public DataExtractField FileNameDataExtractField { get; set; }
    }

    public class DataSourcedFileBuildingDefinitionGroupingField
    {
        public int Id { get; set; }
        public int DataSourcedFileBuildingDefinitionId { get; set; }
        public DataSourcedFileBuildingDefinition DataSourcedFileBuildingDefinition { get; set; }
        public int DataExtractFieldId { get; set; }
        public DataExtractField DataExtractField { get; set; }
    }

}
