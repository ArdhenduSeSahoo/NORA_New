using Damco.Model.DataSourcing;
using Damco.Model.Workflow;
using Newtonsoft.Json;
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
    public class PageDesignDataSource
    {
        public int Id { get; set; }
        [Required()]
        public string Description { get; set; }
        public int PageDesignId { get; set; }
        public PageDesign PageDesign { get; set; }
        public int DataSourceId { get; set; }
        public DataSource DataSource { get; set; }
        public bool RequiresLink { get; set; }
        public string FixedPredicateAsString { get; set; }
        public Expression<Func<Tentity, bool>> GetFixedPredicate<Tentity>() { return this.FixedPredicateAsString?.DeserializeExpression<Func<Tentity, bool>>(); }
        public void SetFixedPredicate<Tentity>(Expression<Func<Tentity, bool>> value) { this.FixedPredicateAsString = value?.SerializeToString(); }
        public LambdaExpression GetFixedPredicate(ParameterExpression parameter) { return this.FixedPredicateAsString?.DeserializeExpression(parameter); }
        public string TenantFieldAsString { get; set; }
        public Expression<Func<Tentity, int?>> GetTenantField<Tentity>() { return this.TenantFieldAsString?.DeserializeExpression<Func<Tentity, int?>>(); }
        public void SetTenantField<Tentity>(Expression<Func<Tentity, int?>> value) { this.TenantFieldAsString = value?.SerializeToString(); }
        public LambdaExpression GetTenantField(ParameterExpression parameter) { return this.TenantFieldAsString?.DeserializeExpression(parameter); }
        public int? PageSize { get; set; }
        public int? MaximumItemsExported { get; set; }
        public List<PageDesignDataSourceOrderField> OrderFields { get; set; }
        public List<PageDesignDataSourceLink> ChildDataSourceLinks { get; set; } = new List<PageDesignDataSourceLink>();
        public int? StatusUsageProfileId { get; set; }
        public StatusUsageProfile StatusUsageProfile { get; set; }
        public int? StatusDataFieldId { get; set; }
        public DataField StatusDataField { get; set; }
        public int? StatusDataSourceId { get; set; }
        public PageDesignDataSource StatusDataSource { get; set; }
        public bool OnlyForAdditions { get; set; }
        public string EndUserInputTagsAsString { get; set; }
        [NotMapped()]
        public string[] EndUserInputTags
        {
            get { return this.EndUserInputTagsAsString?.FromJson<string[]>(); }
            set { this.EndUserInputTagsAsString = value?.ToJson(); }
        }
        public bool IsScalarEndUserInputs { get; set; }
        public PageDesignDataSourceDefaultItem DefaultValues { get; set; }
        public List<PageDesignDataSourceDefaultItem> DefaultItems { get; set; } = new List<PageDesignDataSourceDefaultItem>();
        public bool AutomaticAddition { get; set; }
        public List<PageDesignDataSourceFieldSetup> FieldSetups { get; set; } = new List<PageDesignDataSourceFieldSetup>();
        public int? SaveOperationId { get; set; }
        //public Operation SaveOperation { get; set; }
        public int? DeleteOperationId { get; set; }
        //public Operation DeleteOperation { get; set; }

        public string DataSourceParametersAsString { get; set; }
        public Dictionary<string, object> DataSourceParameters
        {
            get { return this.DataSourceParametersAsString?.FromJson<Dictionary<string, object>>(); }
            set { this.DataSourceParametersAsString = value?.ToJson(); }
        }
        public bool NoLock { get; set; }

        /// <summary>
        /// Optional name or connection string to change the default one for the query
        /// </summary>
        public string NameOrConnectionString { get; set; }
    }
    public class PageDesignDataSourceFieldSetup
    {
        public int DataFieldId { get; set; }
        public DataField DataField { get; set; }
        public bool ExcludeFromCopy { get; set; }
    }

    public class PageDesignDataSourceDefaultItem
    {
        public List<PageDesignDataSourceDefaultValue> DefaultValues { get; set; } = new List<PageDesignDataSourceDefaultValue>();
    }
    public class PageDesignDataSourceDefaultValue
    {
        public int DataFieldId { get; set; }
        public DataField DataField { get; set; }
        public string DefaultValueAsString { get; set; }
        [NotMapped()]
        public object DefaultValue
        {
            get { return this.DefaultValueAsString?.FromJson<object>(); }
            set { this.DefaultValueAsString = value?.ToJson(); }
        }
    }

    public class PageDesignDataSourceOrderField
    {
        public int PageDesignDataSourceId { get; set; }
        public PageDesignDataSource PageDesignDataSource { get; set; }
        public int DataFieldId { get; set; }

        public DataField DataField { get; set; }

        public OrderByType OrderByType { get; set; }
        public int? OrderByIndex { get; set; }
    }

    public class PageDesignDataSourceLink
    {
        public int ParentDataSourceId { get; set; }
        public PageDesignDataSource ParentDataSource { get; set; }
        public int ChildDataSourceId { get; set; }
        public PageDesignDataSource ChildDataSource { get; set; }
        public List<PageDesignDataSourceLinkField> Fields { get; set; }
    }

    public class PageDesignDataSourceLinkField
    {
        public int PageDesignDataSourceLinkId { get; set; }
        public PageDesignDataSourceLink PageDesignDataSourceLink { get; set; }
        public int DataFieldParentId { get; set; }
        public DataField DataFieldParent { get; set; }
        public int DataFieldChildId { get; set; }
        public DataField DataFieldChild { get; set; }
    }

    [Serializable()]
    public enum OrderByType
    {
        Ascending = 1,
        Descending = 2
    }
}
