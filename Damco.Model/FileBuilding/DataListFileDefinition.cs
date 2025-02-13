using Damco.Model.DataSourcing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model.FileBuilding
{
    public class DataListFileDefinition : DataSourcedFileBuildingDefinition
    {
        public DataListFileType Type { get; set; }
        public int LayoutId { get; set; }
        public DataListLayoutDefinition Layout { get; set; }

    }

    public class DataListLayoutDefinition
    {
        public int Id { get; set; }
        [Required(), MaxLength(100)]
        public string Name { get; set; }
        public bool AutoFilter { get; set; }
        public List<DataListLayoutDefinitionField> Fields { get; set; } = new List<DataListLayoutDefinitionField>();
    }

    public class DataListLayoutDefinitionField
    {
        public int Id { get; set; }
        public int DataListLayoutDefinitionId { get; set; }
        public DataListLayoutDefinition DataListLayoutDefinition { get; set; }
        public int DataExtractFieldId { get; set; }
        public DataExtractField DataExtractField { get; set; }
        [Translatable(), Required()]
        public string ColumnName { get; set; }
        public string ValueFormat { get; set; }
        public bool HasTranslation { get; set; }
        public DataField TranslationValueField { get; set; }
        public DataField TranslationDisplayField { get; set; }

    }

    public enum DataListFileType
    {
        Excel = 1,
        Csv = 2
    }
}
