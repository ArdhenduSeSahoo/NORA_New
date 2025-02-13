using Damco.Model.DataSourcing;
using Damco.Model.Workflow;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model.FileImporting
{
    [NotMapped()]
    public class DataListFileImportOperation : Operation
    {
        public DataListFileImportType Type { get; set; }
        public int RootTableId { get; set; }
        public FileImportOperationTable RootTable { get; set; }
        public string FileName { get; set; }
    }
    public enum DataListFileImportType
    {
        Excel = 1
    }

    public enum DataListFileImportExistingItemHandling
    {
        Update = 0,
        Error = 1,
        Skip = 2
    }

    public class FileImportOperationTable
    {
        public List<FileImportOperationField> Fields { get; set; }
        public List<FileImportOperationTable> Children { get; set; }
        public DataField ParentToChild { get; set; }
        public bool DeleteMissingItemsWithinParent { get; set; }
        public DataListFileImportExistingItemHandling ExistingItemHandling { get; set; }
        //DataField that gets the navigation property from the parent to the child
        //c => c.Aliases
        public bool SkipEmptyRowValidation { get; set; }
        //public int? FollowUpOperationId { get; set; }
        public Operation SaveFollowUpOperation { get; set; }
        //public int? DeleteFollowUpOperationId { get; set; }
        public Operation DeleteFollowUpOperation { get; set; }
    }

    public class FileImportOperationField
    {
        public bool IsMandatory { get; set; }

        public string DefaultValueAsString { get; set; }
        [NotMapped()]
        public object DefaultValue
        {
            get { return this.DefaultValueAsString == null ? null : JsonConvert.DeserializeObject(this.DefaultValueAsString); }
            set { this.DefaultValueAsString = value == null ? null : JsonConvert.SerializeObject(value); }
        }
        public int? DataFieldId { get; set; }
        public DataField DataField { get; set; }
        public string NameInFile { get; set; }
        public List<FileImportOperationFieldLookup> Lookups { get; set; } = new List<FileImportOperationFieldLookup>();
        public bool LookupKey { get; set; }
        public int SequenceNo { get; set; }
        public bool HasDefaultValue { get; set; }
        public bool DeletionKey { get; set; }

        public string Format { get; set; }
        public bool IsPositive { get; set; }
        public int FileImportOperationTableId { get; set; }
        public FileImportOperationTable FileImportOperationTable { get; set; }
    }

    public class FileImportOperationFieldLookup
    {
        public int LookupDataFieldId { get; set; }
        public DataField LookupDataField { get; set; } //E.g. "Countries.Code" or "CountryAliases.Alias"
        public int ResultDataFieldId { get; set; }
        public DataField ResultDataField { get; set; } //E.g. "Countries.Id" or "CountryAliases.ItemId"
        public List<FileImportOperationFieldLookupLinkField> LinkFields { get; set; } = new List<FileImportOperationFieldLookupLinkField>();
        public string FixedPredicateAsString { get; set; }
        public Expression<Func<Tentity, bool>> GetFixedPredicate<Tentity>() { return this.FixedPredicateAsString?.DeserializeExpression<Func<Tentity, bool>>(); }
        public void SetFixedPredicate<Tentity>(Expression<Func<Tentity, bool>> value) { this.FixedPredicateAsString = value?.SerializeToString(); }
        public LambdaExpression GetFixedPredicate(ParameterExpression parameter) { return this.FixedPredicateAsString?.DeserializeExpression(parameter); }
    }

    public class FileImportOperationFieldLookupLinkField
    {
        public int SourceDataFieldId { get; set; }
        public DataField SourceDataField { get; set; }
        public int TargetFilterDataFieldId { get; set; }
        public DataField TargetFilterDataField { get; set; } //Field to filter the target data source on RateContractId
    }

}
