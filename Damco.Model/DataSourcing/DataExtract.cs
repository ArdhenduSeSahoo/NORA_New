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
    public class DataExtract
    {
        public int Id { get; set; }
        [Required(), MaxLength(100)]
        public string Name { get; set; }
        public string Description { get; set; }
        public int RootDataSourceId { get; set; }
        public DataSource RootDataSource { get; set; }
        public int RootDataExtractTableId { get; set; }
        public DataExtractTable RootDataExtractTable { get; set; }
    }

    public class DataExtractTable
    {
        public int Id { get; set; }
        public int? ParentId { get; set; } //null for root
        public DataExtractTable Parent { get; set; }
        public int? DataRelationId { get; set; } //null for root
        public DataRelation DataRelation { get; set; }
        public bool RelationChildToParent { get; set; }
        public List<DataExtractTable> Children { get; set; } = new List<DataExtractTable>();
        public List<DataExtractField> Fields { get; set; } = new List<DataExtractField>();
        [NotMapped()]
        public string RealTechnicalName
        {
            get
            {
                if (this.RelationChildToParent)
                    return this.DataRelation.TechnicalNameOfParentFromChild ?? TextProviderBase.DisplayTextToPascal(this.DataRelation.NameOfParentFromChild);
                else
                    return this.DataRelation.TechnicalNameOfChildFromParent ?? TextProviderBase.DisplayTextToPascal(this.DataRelation.NameOfChildFromParent);
            }
        }
        [NotMapped()]
        public bool OneChildOnly
        {
            get
            {
                if (this.RelationChildToParent)
                    return true;
                else
                    return this.DataRelation.OneChildOnly;
            }
        }
        public string TableTechnicalName { get; set; }
        public string TableFieldPrefix { get; set; }
        public string PredicateAsString { get; set; }
        public Expression<Func<Tentity, bool>> GetPredicate<Tentity>() { return this.PredicateAsString?.DeserializeExpression<Func<Tentity, bool>>(); }
        public void SetPredicate<Tentity>(Expression<Func<Tentity, bool>> value)
        {
            this.PredicateAsString = value?.SerializeToString();
        }
    }

    public class DataExtractField
    {
        public int Id { get; set; }
        public int TableId { get; set; }
        public DataExtractTable Table { get; set; }
        public int DataFieldId { get; set; }
        public DataField DataField { get; set; }
        [NotMapped]
        public string RealTechnicalName
        {
            get
            {
                return this.DataField.RealTechnicalName;
            }
        }
        [NotMapped]
        public Type FieldType
        {
            get
            {
                return this.DataField.FieldType;
            }
        }
    }

    public interface IRequiresDataExtract
    {
        int DataExtractId { get; set; }
        DataExtract DataExtract { get; set; }
    }

}
