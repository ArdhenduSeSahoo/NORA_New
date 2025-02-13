using Damco.Model.DataSourcing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model.Workflow
{
    [NotMapped()]
    public class LookupOperation : Operation
    {
        public int? DataSourceId { get; set; }
        public DataSource DataSource { get; set; }
        public string PredicateAsString { get; set; }
        public void SetPredicate<T>(Expression<Func<T, bool>> predicate) { this.PredicateAsString = predicate?.SerializeToString(); }
        public Expression<Func<T, bool>> GetPredicate<T>() { return this.PredicateAsString?.DeserializeExpression<Func<T, bool>>(); }
        public List<LookupOperationLinkField> LinkFields { get; set; } = new List<LookupOperationLinkField>();
        public List<LookupOperationMappingField> MappingFields { get; set; } = new List<LookupOperationMappingField>();
        public HashSet<string> TargetChildrenToLoad { get; } = new HashSet<string>();
        public bool SkipIfMultipleSourcesFound { get; set; }
        public int? CounterId { get; set; }
        public Counter Counter { get; set; }
        //Note: DefaultIfBlank and DefaultIfNoData, if used, are format (e.g. ILSE{0:000000}) for the counter in case counter is used
    }

    public class LookupOperationLinkField : IDataFieldLink
    {
        public int Id { get; set; }
        public int LookupOperationId { get; set; }
        public LookupOperation LookupOperation { get; set; }
        public int TargetDataFieldId { get; set; }
        public DataField TargetDataField { get; set; }
        public int LookupDataFieldId { get; set; }
        public DataField LookupDataField { get; set; }
        DataField IDataFieldLink.ParentDataField { get { return this.TargetDataField; } }
        DataField IDataFieldLink.ChildDataField { get { return this.LookupDataField; } }
    }

    public class LookupOperationMappingField
    {
        public int Id { get; set; }
        public int LookupOperationId { get; set; }
        public LookupOperation LookupOperation { get; set; }
        public int TargetDataFieldId { get; set; }
        public DataField TargetDataField { get; set; }
        public int? LookupDataFieldId { get; set; }
        public DataField LookupDataField { get; set; }
        public string DefaultIfNoDataAsString { get; set; }
        [NotMapped()]
        public object DefaultIfNoData
        {
            get { return (this.DefaultIfNoDataAsString == null ? null : JsonConvert.DeserializeObject(this.DefaultIfNoDataAsString)); }
            set { this.DefaultIfNoDataAsString = (value == null ? null : JsonConvert.SerializeObject(value)); }
        }
        public bool LeaveAsIsIfNoData { get; set; }
        public string DefaultIfBlankAsString { get; set; }
        [NotMapped()]
        public object DefaultIfBlank
        {
            get { return (this.DefaultIfBlankAsString == null ? null : JsonConvert.DeserializeObject(this.DefaultIfBlankAsString)); }
            set { this.DefaultIfBlankAsString = (value == null ? null : JsonConvert.SerializeObject(value)); }
        }
        public bool LeaveAsIsIfBlank { get; set; }
        public bool LeaveNonBlankData { get; set; }
        public string EndUserInputCode { get; set; }
    }



}
