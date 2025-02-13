using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model.Workflow
{
    [NotMapped()]
    public class ApplicationOperation : Operation
    {
        [Required()]
        public string MethodTag { get; set; }
        [NotMapped()]
        public Dictionary<string, object> Setup
        {
            get { return this.SetupAsString?.FromJson<Dictionary<string, object>>(); }
            set { this.SetupAsString = value?.ToJson(); }
        }
        public string SetupAsString { get; set; }
        //public int ReturnValueOperationId { get; set; }
        public Operation ReturnValueOperation { get; set; }
    }
}
