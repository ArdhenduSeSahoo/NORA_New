using Damco.Model.Workflow;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model.ScreenTemplating
{
    public class PageDesign
    {
        public int Id { get; set; }
        public int MenuOptionId { get; set; }
        public MenuOption MenuOption { get; set; }
        public int ContentsGroupTemplateId { get; set; }
        public GroupTemplate ContentsGroupTemplate { get; set; }
        public List<PageDesignDataSource> DataSources { get; set; }
        public List<PageDesignStructure> FullStructure { get; set; }
        public string TagsAsString { get; set; }
        [NotMapped()]
        public string[] Tags
        {
            get { return this.TagsAsString?.FromJson<string[]>(); }
            set { this.TagsAsString = value?.ToJson(); }
        }
        public string PageTitle { get; set; }
    }

    public class PageDesignStructure
    {
        public int Id { get; set; }
        public int PageDesignId { get; set; }
        public PageDesign PageDesign { get; set; }
        public int? ParentId { get; set; }
        public PageDesignStructure Parent { get; set; }
        public int? GroupTemplatePartId { get; set; }
        public GroupTemplatePart GroupTemplatePart { get; set; }
        public List<PageDesignStructureDataSource> DataSources { get; set; } = new List<PageDesignStructureDataSource>();
        public List<PageDesignStructure> Children { get; set; } = new List<PageDesignStructure>();
    }

    public class PageDesignStructureDataSource
    {
        public int Id { get; set; }
        public int PageDesignStructureId { get; set; }
        public PageDesignStructure PageDesignStructure { get; set; }
        public int PageDesignDataSourceId { get; set; }
        public PageDesignDataSource PageDesignDataSource { get; set; }
        public PageDesignStructureDataSourceRoles Role { get; set; }
        public int? PageDesignOperationId { get; set; }
        public PageDesignOperation PageDesignOperation { get; set; }
        public bool IsVisible { get; set; }
    }

    [Flags()]
    public enum PageDesignStructureDataSourceRoles
    {
        Main = 1,
        Add = 2,
        Save = 4,
        Delete = 8,
        RefreshAfterSave = 16,
        SaveWithRefresh = Save | RefreshAfterSave,
        Refresh = 32,
        RefreshAfterDelete = 64,
        DeleteWithRefresh = Delete | RefreshAfterDelete,
        SoftDelete = 128,
        DoCustomOperation = 256,
        RefreshAfterCustomOperation = 512,
        DoCustomOperationAndRefresh = DoCustomOperation | RefreshAfterCustomOperation,
        ExportToExcel = 1024,
        AddDefaults = 2048,
        Copy = 4096,
        SelectFields = 8192
    }

    public class PageDesignOperation
    {
        public int Id { get; set; }
        public int OperationId { get; set; }
        //public Operation Operation { get; set; } TODO: Re-enable once this goes to the DB
        public string ButtonText { get; set; }
        public ButtonType ButtonType { get; set; }
        public string ConfirmationMessage { get; set; }

        public PopupWidthType PopupWidthType { get; set; } = PopupWidthType.Unspecified;

        public bool SaveChangesFirst { get; set; }
        public bool WorkOnAllData { get; set; }
        public int? MaximumDataCount { get; set; }
        public bool FullRefresh { get; set; }
        public bool AllowSelectAll { get; set; }

        public string TagsAsString { get; set; }
        [NotMapped()]
        public string[] Tags
        {
            get { return this.TagsAsString?.FromJson<string[]>(); }
            set { this.TagsAsString = value?.ToJson(); }
        }
        public int? DialogPageDesignId { get; set; }
        public PageDesign DialogPageDesign { get; set; }

        public string ConfigsAsString { get; set; }
        public Dictionary<string, object> Configs
        {
            get { return this.ConfigsAsString == null ? null : JsonConvert.DeserializeObject<Dictionary<string, object>>(this.ConfigsAsString); }
            set { this.ConfigsAsString = value == null ? null : JsonConvert.SerializeObject(value); }
        }
    }

    public enum ButtonType
    {
        Default = 0,
        Primary = 1,
        Success = 2,
        Info = 3,
        Warning = 4,
        Danger = 5
    }



}
