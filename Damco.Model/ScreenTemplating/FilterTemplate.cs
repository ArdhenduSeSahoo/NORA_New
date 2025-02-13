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
    public class FilterTemplate : Template
    {
        public string DefaultAsString { get; set; }
        [NotMapped()]
        public Expression<Func<DynamicEntity, bool>> Default
        {
            get { return this.DefaultAsString?.DeserializeExpression<Func<DynamicEntity, bool>>(); }
            set { this.DefaultAsString = value?.SerializeToString(); }
        }

        public List<FilterTemplateField> Fields { get; set; }
        public FilterDisplayType FilterDisplayType { get; set; }

    }

    public enum FilterDisplayType
    {
        Basic = 1,
        Advanced = 2
    }

    public class FilterTemplateField
    {
        public int Id { get; set; }
        public int FilterTemplateId { get; set; }
        public FilterTemplate FilterTemplate { get; set; }
        //TODO: Remove DataField once fielddisplay takes
        public int FieldDisplayId { get; set; }
        public FieldEdit FieldEdit { get; set; }

        public bool DefaultVisible { get; set; }
        public DataFilterOperator DefaultOperator { get; set; } = DataFilterOperator.Is;
        [Translatable(), Required()]
        public string DisplayName { get; set; }
    }
}
