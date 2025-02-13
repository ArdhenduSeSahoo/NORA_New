using Damco.Model.DataSourcing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model.ScreenTemplating
{
    public class TemplateLink
    {
        public int Id { get; set; }
        public string TargetPageDesignTag { get; set; }
        public PageDesign TargetPageDesign { get; set; }
        public List<TemplateLinkTarget> Targets { get; set; }
        public ContextualStyle? SelectedItemStyle { get; set; }
    }

    public class TemplateLinkTarget
    {
        public int Id { get; set; }
        public int TemplateLinkId { get; set; }
        public TemplateLink TemplateLink { get; set; }
        public int TargetPageDesignDataSourceId { get; set; }
        public PageDesignDataSource TargetPageDesignDataSource { get; set; }
        public int? TargetIncludePageDesignStructureId { get; set; }
        public PageDesignStructure TargetIncludePageDesignStructure { get; set; }
        public List<TemplateLinkField> Fields { get; set; }
        public bool SetFields { get; set; } //If true, fields in the target data source are set to the source values, instead of filtering the data
    }

    public class TemplateLinkField
    {
        public int Id { get; set; }
        public int SourceDataFieldId { get; set; }
        public DataField SourceDataField { get; set; }
        public int TargetDataFieldId { get; set; }
        public DataField TargetDataField { get; set; }
        public int TemplateLinkTargetId { get; set; }
        public TemplateLinkTarget TemplateLinkTarget { get; set; }
    }

}
