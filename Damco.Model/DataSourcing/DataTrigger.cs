using Damco.Model.Workflow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model.DataSourcing
{
    public class DataTrigger
    {
        public int Id { get; set; }
        public List<DataTriggerEntity> Entities { get; set; }
        public int OperationId { get; set; }
        public Operation Operation { get; set; }
    }

    public class DataTriggerEntity
    {
        public int Id { get; set; }
        public int DataTriggerId { get; set; }
        public DataTrigger DataTrigger { get; set; }

    }

    public class DataTriggerProperty
    {
        public int Id { get; set; }
        public string PropertyName { get; set; }
    }



}
