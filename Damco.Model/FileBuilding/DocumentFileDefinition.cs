using Damco.Model.DataSourcing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model.FileBuilding
{
    public class DocumentFileDefinition : DataSourcedFileBuildingDefinition
    {
        public int LayoutId { get; set; }
        public DocumentLayoutDefinition Layout { get; set; }
        public DocumentType Type { get; set; }
        public int? TimeZoneFieldId { get; set; }
        public DataExtractField TimeZoneField { get; set; }
        public string DefaultTimeZoneId { get; set; }
        public List<DocumentFileDefinitionParameter> Parameters { get; set; } = new List<DocumentFileDefinitionParameter>();
    }

    public class DocumentFileDefinitionParameter
    {
        public int Id { get; set; }
        public int DocumentFileDefinitionId { get; set; }
        public DocumentFileDefinition DocumentFileDefinition { get; set; }
        [MaxLength(50), Required()]
        public string Name { get; set; }
        public string ValueAsString { get; set; }
        [NotMapped()]
        public object Value
        {
            get { return this.ValueAsString?.FromJson<object>(); }
            set { this.ValueAsString = value?.ToJson(); }
        }
    }

    public class DocumentLayoutDefinition : IEntity
    {
        public int Id { get; set; }
        [Required(), MaxLength(100)]
        public string Name { get; set; }
        [Required()]
        public byte[] LayoutDefinition { get; set; }
        [NotMapped()]
        public string[] Tags
        {
            get { return this.TagsAsString?.FromJson<string[]>() ?? new string[] { }; }
            set { this.TagsAsString = (value == null || value.Length == 0 ? null : value.ToJson()); }
        }
        public string TagsAsString { get; set; }
    }

    public enum DocumentType
    {
        Pdf = 1,
        Word = 2
    }


}
