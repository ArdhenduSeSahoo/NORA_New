using Damco.Model.DataSourcing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model.ScreenTemplating
{
    public class FrameTemplate : Template
    {
        public int Height { get; set; }
        public int Width { get; set; }
        public GridPositions DisplayPosition { get; set; }
        public int DataSourceId { get; set; }
        public DataSource DataSource { get; set; }
        public List<FrameTemplateField> Fields { get; set; } = new List<FrameTemplateField>();

        [Required()]
        public string ContentUrl { get; set; } //http://dfadsfasfda/adfajfklas?asdfasdfa={0}&dafkjl={1}
    }

    public class FrameTemplateField
    {
        public int Id { get; set; }
        //[Translatable(), Required()]
        //public string DisplayName { get; set; }
        public int DataFieldId { get; set; }
        public DataField DataField { get; set; }
    }

}
