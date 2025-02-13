using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model.DataSourcing
{
    /// <summary>
    /// Maintains the details/references of the datasources.
    /// </summary>
    public class DataSource: IEntity, ILogged<DataSourcingMasterDataLog>
    {
        public int Id { get; set; }
        public int? StoreDataSourceId { get; set; }
        public StoreDataSource StoreDataSource { get; set; }
        public string CodeDataSourceTag { get; set; }
        public string EntityTypeAsString { get; set; }
        [NotMapped()]
        public Type EntityType
        {
            get { return ExpressionSerialization.GetTypeForName(this.EntityTypeAsString) ?? typeof(DynamicEntity); }
            set { this.EntityTypeAsString = value == typeof(DynamicEntity) ? null : ExpressionSerialization.GetNameForType(value); }
        }
        public List<DataField> Fields { get; set; } = new List<DataField>();

        [NotMapped()]
        public Type RealEntityType
        {
            get
            {
                if (this.StoreDataSource != null)
                    return this.StoreDataSource.MirroredEntityType;
                else
                    return this.EntityType;
            }
        }

        [NotMapped()]
        public string[] Tags
        {
            get { return this.TagsAsString?.FromJson<string[]>() ?? new string[] { }; }
            set { this.TagsAsString = (value == null || value.Length == 0 ? null : value.ToJson()); }
        }
        public string TagsAsString { get; set; }
        //[MaxLength(100), Required()]
        //public string Name { get; set; }
    }

    /// <summary>
    /// Maintains the details of the datafields of the specified datasource.
    /// </summary>
    public class DataField: IEntity, ILogged<DataSourcingMasterDataLog>
    {
        /// <summary>
        /// The Id of the datafield.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The Id of the datasource.
        /// </summary>
        public int DataSourceId { get; set; }

        /// <summary>
        /// Datasource of the respective Datafield.
        /// </summary>
        public DataSource DataSource { get; set; }

        /// <summary>
        /// The source details of datafield in string.
        /// </summary>
        [Required()]
        public string SourceAsString { get; set; }

        /// <summary>
        /// Get the source lambdaexpression.
        /// </summary>
        /// <param name="parameter">Given parameter expression </param>
        /// <returns>Returns Lambda expression</returns>
        public LambdaExpression GetSource(ParameterExpression parameter)
        {
            return this.SourceAsString.DeserializeExpression(parameter);
        }
        [NotMapped()]
        public Type FieldType
        {
            get
            {
                Type paramType;
                if (this.DataSource?.EntityType != null)
                    paramType = this.DataSource?.EntityType;
                else
                    paramType = typeof(DynamicEntity);
                if(this.AggregateField)
                    paramType = typeof(IEnumerable<>).MakeGenericType(paramType);
                return this.SourceAsString.DeserializeExpression(Expression.Parameter(paramType, "p")).Body.Type;
            }
        }

        /// <summary>
        /// Return the source for given lambda expression of the datafield.
        /// </summary>
        /// <param name="lambda"></param>
        public void SetSource(LambdaExpression lambda)
        {
            this.SourceAsString = lambda.SerializeToString();
        }


        /// <summary>
        /// Gets or Sets boolean which indicates whether the Datafield is Aggreagated field or Not.
        /// </summary>
        public bool AggregateField { get; set; }

        /// <summary>
        /// Get or Sets the pivotal type of the datafield.
        /// </summary>
        //public PivotType? PivotType { get; set; }

        /// <summary>
        /// Get or Sets the pivotal column source of the datafield.
        /// </summary>
        //public PivotColumnSource? PivotColumnSource { get; set; }
        [Required(), MaxLength(100)]
        public string Name { get; set; }
        [MaxLength(100)]
        public string TechnicalName { get; set; }
        [NotMapped]
        public string RealTechnicalName
        {
            get
            {
                if (this.TechnicalName != null)
                    return this.TechnicalName;
                else
                    return RootTextProvider.DisplayTextToPascal(this.Name);
            }
        }

        public string TagsAsString { get; set; }
        [NotMapped()]
        public string[] Tags
        {
            get { return this.TagsAsString?.FromJson<string[]>(); }
            set { this.TagsAsString = value?.ToJson(); }
        }

        public string TechnicalDetails { get; set; }
    }

    public class DataRelation
    {
        public int ParentDataSourceId { get; set; }
        public DataSource ParentDataSource { get; set; }
        public int ChildDataSourceId { get; set; }
        public DataSource ChildDataSource { get; set; }
        /// <summary>
        /// Blank if the relationship cannot be used from the parent POV.
        /// </summary>
        [MaxLength(100)]
        public string NameOfChildFromParent { get; set; }
        public string TechnicalNameOfChildFromParent { get; set; }
        /// <summary>
        /// Blank if the relationship cannot be used from the child POV.
        /// </summary>
        [MaxLength(100)]
        public string NameOfParentFromChild { get; set; }
        public string TechnicalNameOfParentFromChild { get; set; }
        public bool OneChildOnly { get; set; }
        public List<DataRelationField> Fields { get; set; } = new List<DataRelationField>();
    }

    public class DataRelationField : IDataFieldLink
    {
        public int DataRelationId { get; set; }
        public DataRelation DataRelation { get; set; }
        public int ParentDataFieldId { get; set; }
        public DataField ParentDataField { get; set; }
        public int ChildDataFieldId { get; set; }
        public DataField ChildDataField { get; set; }
    }

    public enum PivotType
    {
        Column = 1,
        Data = 2,
        DataWithDedicatedRow = 3
    }

    public enum PivotColumnSource
    {
        Past7Days = 1,
        MonthsCurrentYear = 2
    }

    public interface IDataFieldLink
    {
        DataField ParentDataField { get; }
        DataField ChildDataField { get; }
    }
}
