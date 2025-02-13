using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model.Workflow
{
    public class Counter : IEntity, ILogged<WorkflowSetupLog>
    {
        public int Id { get; set; }
        public string TagsAsString { get; set; }
        [NotMapped()]
        public string[] Tags
        {
            get { return this.TagsAsString?.FromJson<string[]>(); }
            set { this.TagsAsString = value?.ToJson(); }
        }
    }

    public class CounterValue : IEntity
    {
        public int Id { get; set; }
        public int CounterId { get; set; }
        public Counter Counter { get; set; }
        public int LastValue { get; set; }
    }
}
