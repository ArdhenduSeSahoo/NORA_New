using Damco.Model.DataSourcing;
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
    public class FormTemplate : Template
    {
        public int DataSourceId { get; set; }
        public DataSource DataSource { get; set; }
        public List<FormTemplateField> Fields { get; set; }
        public bool ShowCheckboxes { get; set; } 
    }

    public class FormTemplateField
    {
        public int Id { get; set; }
        public int OrderNumber { get; set; }
        [Translatable(), Required()]
        public string DisplayName { get; set; }
        public int FieldDisplayId { get; set; }
        public FieldDisplay FieldDisplay { get; set; }
        public GridPositions LabelPosition { get; set; }
        public GridPositions DisplayPosition { get; set; }
    }
}
