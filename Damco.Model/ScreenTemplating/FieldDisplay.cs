//https://docs.angularjs.org/api/ng/filter/date
//http://stackoverflow.com/questions/14419651/filters-on-ng-model-in-an-input
using Damco.Model.DataSourcing;
using Damco.Model.Workflow;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model.ScreenTemplating
{
    /// <summary>
    /// Maintains details of a field to be displayed
    /// </summary>
    public class FieldDisplay
    {
        /// <summary>
        /// The Id of field.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The Id of the field in the datasource.
        /// </summary>
        public int DataFieldId { get; set; }

        /// <summary>
        /// Field to be mapped to the field in the datasource.
        /// </summary>
        public DataField DataField { get; set; }

        /// <summary>
        /// Format of the field.
        /// </summary>
        /// <remarks>
        /// For example, for a date type field , the format is - "yyyy-MM-dd".
        /// </remarks>
        public string Format { get; set; }

        /// <summary>
        /// The Id of the template Link.
        /// </summary>
        public int? TemplateLinkId { get; set; }

        /// <summary>
        /// Used when a field is to be displayed as a hyperlink.
        /// </summary>
        /// <remarks>
        /// For example, a value in a table to displayed as a hyper link.
        /// </remarks>
        public TemplateLink TemplateLink { get; set; }

        [NotMapped()]
        public Expression<Func<DynamicEntity, bool>> HiddenPredicate //ng-hide
        {
            get { return this.HiddenPredicateAsString?.DeserializeExpression<Func<DynamicEntity, bool>>(); }
            set { this.HiddenPredicateAsString = value?.SerializeToString(); }
        }

        public string HiddenPredicateAsString { get; set; }

        public bool FieldIsSelectable { get; set; }

        public string FieldDisplayName { get; set; }

        public DataField FieldSelectionReferenceField { get; set; }

        public bool ShowAsHyperlink { get; set; }

        public FieldDisplay PopupDisplay { get; set; }

    }

    public interface IFieldWithTranslation
    {
        bool RequiresTranslation { get; }
        DataField ValueField { get; }
        DataField DisplayField { get; }
        DataField DataField { get; }
        Dictionary<string, object> DataSourceParameters { get; set; }
    }


    public class TranslatedFieldDisplay : FieldDisplay, IFieldWithTranslation
    {
        bool IFieldWithTranslation.RequiresTranslation { get { return true; } }

        public int ValueFieldId { get; set; }
        public DataField ValueField { get; set; }
        public int DisplayFieldId { get; set; }
        public DataField DisplayField { get; set; }
        public string DataSourceParametersAsString { get; set; }
        public Dictionary<string, object> DataSourceParameters
        {
            get { return this.DataSourceParametersAsString?.FromJson<Dictionary<string, object>>(); }
            set { this.DataSourceParametersAsString = value?.ToJson(); }
        }
    }

    //Simple stuff:
    //Boolean value: https://css-tricks.com/indeterminate-checkboxes/ ("Rotating amongst the states")
    //  maybe an angular directive to make this
    //String: TextBox
    //Date / number: Textbox with client-side type/format validation

    /// <summary>
    /// Used to indicate the field as mandatory or to make the field readonly through an expression. 
    /// </summary>
    /// <remarks>
    /// Adds an attribute "ng-readonly" based upon the expression for a readOnly scenario.
    /// Adds an attribute "ng-required" based upon the expression for a mandatory scenario.
    /// </remarks>
    public class FieldEdit : FieldDisplay
    {
        /// <summary>
        /// An expression to be evaluated for a field to be readOnly.
        /// </summary>
        [NotMapped()]
        public Expression<Func<DynamicEntity, bool>> ReadonlyPredicate //ng-readonly
        {
            get { return this.ReadonlyPredicateAsString?.DeserializeExpression<Func<DynamicEntity, bool>>(); }
            set { this.ReadonlyPredicateAsString = value?.SerializeToString(); }
        }

        /// <summary>
        /// Expression as a string.
        /// </summary>
        public string ReadonlyPredicateAsString { get; set; }

        /// <summary>
        /// An expression to be evaluated for a field to be Mandatory.
        /// </summary>
        [NotMapped()]
        public Expression<Func<DynamicEntity, bool>> MandatoryPredicate //ng-required
        {
            get { return this.MandatoryPredicateAsString?.DeserializeExpression<Func<DynamicEntity, bool>>(); }
            set { this.MandatoryPredicateAsString = value?.SerializeToString(); }
        }
        /// <summary>
        /// Expression as a string.
        /// </summary>
        public string MandatoryPredicateAsString { get; set; }

        public int? NumberOfRows { get; set; }

        public int? OnChangeOperationId { get; set; }
        public Operation OnChangeOperation { get; set; }

        public bool IsPositive { get; set; }

        public int? MaxLength { get; set; }
        public int? MinLength { get; set; }
        public string ValidationRegularExpression { get; set; }
        public string ValidationRegularExpressionErrorMessage { get; set; }
        public bool ShowNumberOfCharacters { get; set; }
        public int? MaxLengthPerLine { get; set; }
    }

    //Validation:
    //- Show list of errors somewhere
    //- We need to be able to give it its own error messages

    /// <summary>
    /// Represents an enumeration which has Dropdown =1 , AutoComplete =2 and PopUp =4.
    /// </summary>
    [Flags()]
    public enum ListType
    {
        Dropdown = 1, //Still uses Json to fill it
        AutoComplete = 2, //Angular and/or bootstrap might have a good one - it needs to be able to save another value than you search on
        Popup = 4, //Layout can be in the layout.cshtml or javascript
                   /*
                       - Angular and/or bootstrap might have a good one
                       - Listbox with records (max 200)
                       - Textbox above it search for the text you type as soon you as you
                       - Filter button uses the typed text to get filter server-side
                       - On selection, text of the texbox can get another one value, another value (not displayed) can be stored in the model
                   */
                   //Autocomplete and popup can be combined
    }


    /// <summary>
    /// Used to indicate the field as a list.
    /// </summary>
    /// <remarks>
    /// Types of lists can be Dropdown =1 , AutoComplete =2 and PopUp =4.
    /// </remarks>
    public class FieldEditList : FieldEdit, IFieldWithTranslation
    {
        bool IFieldWithTranslation.RequiresTranslation { get { return this.ListType == ListType.Popup; } }

        //public List<KeyValuePair<int, string>> lstData { get; set; } //AKO: Seems obsolete.

        public ListType ListType { get; set; }

        public int ValueFieldId { get; set; }
        public DataField ValueField { get; set; }
        public int DisplayFieldId { get; set; }
        public DataField DisplayField { get; set; }
        public int SortFieldId { get; set; }
        public DataField SortField { get; set; }
        public string ListFixedPredicateAsString { get; set; }
        public Expression<Func<Tentity, bool>> GetListFixedPredicate<Tentity>() { return this.ListFixedPredicateAsString?.DeserializeExpression<Func<Tentity, bool>>(); }
        public void SetListFixedPredicate<Tentity>(Expression<Func<Tentity, bool>> value) { this.ListFixedPredicateAsString = value?.SerializeToString(); }
        public LambdaExpression GetListFixedPredicate(ParameterExpression parameter) { return this.ListFixedPredicateAsString?.DeserializeExpression(parameter); }
        public string TenantFieldAsString { get; set; }
        public Expression<Func<Tentity, int?>> GetTenantField<Tentity>() { return this.TenantFieldAsString?.DeserializeExpression<Func<Tentity, int?>>(); }
        public void SetTenantField<Tentity>(Expression<Func<Tentity, int?>> value) { this.TenantFieldAsString = value?.SerializeToString(); }
        public LambdaExpression GetTenantField(ParameterExpression parameter) { return this.TenantFieldAsString?.DeserializeExpression(parameter); }

        [NotMapped()]
        public Expression<Func<DynamicEntity, bool>> ListFilter
        {
            get { return this.ListFilterAsString?.DeserializeExpression<Func<DynamicEntity, bool>>(); }
            set { this.ListFilterAsString = value?.SerializeToString(); }
        }
        public string ListFilterAsString { get; set; }
        public string DataSourceParametersAsString { get; set; }
        public Dictionary<string, object> DataSourceParameters
        {
            get { return this.DataSourceParametersAsString?.FromJson<Dictionary<string, object>>(); }
            set { this.DataSourceParametersAsString = value?.ToJson(); }
        }
        public List<FieldEditListLinkField> LinkFields { get; set; } = new List<FieldEditListLinkField>();
    }

    public class FieldEditListLinkField
    {
        public int Id { get; set; }
        public int FieldEditListId { get; set; }
        public FieldEditList FieldEditList { get; set; }
        public int SourceDataFieldId { get; set; }
        public DataField SourceDataField { get; set; }
        public int? SourceDataSourceId { get; set; }
        public PageDesignDataSource SourceDataSource { get; set; }
        public int TargetDataFieldId { get; set; }
        public DataField TargetDataField { get; set; }
        public bool UseSelectedItemsOnly { get; set; }
    }

    public class FieldEditFile : FieldEdit
    {
        //public int? FileNameDataFieldId { get; set; }
        //public DataField FileNameDataField { get; set; }
        public string DefaultFileName { get; set; }
    }

}
