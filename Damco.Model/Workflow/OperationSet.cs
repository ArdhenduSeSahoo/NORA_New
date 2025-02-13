using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model.Workflow
{
    [NotMapped()]
    public class OperationSet : Operation
    {
        public string EntitySelectorAsString { get; set; }
        public LambdaExpression GetEntitySelector(ParameterExpression parameter) { return this.EntitySelectorAsString?.DeserializeExpression(parameter); }
        public void SetEntitySelector(LambdaExpression selector) { this.EntitySelectorAsString = selector?.SerializeToString(); }
        public List<OperationSetChild> Children { get; set; }
    }

    public class OperationSetChild
    {
        public int Id { get; set; }
        public int OperationSetId { get; set; }
        public OperationSet OperationSet { get; set; }
        public int OperationId { get; set; }
        public Operation Operation { get; set; }
        public int Sequence { get; set; } //TODO: Sort on this when executing
        public bool DoIfErrorsFound { get; set; }
        public OperationExecutionTime ExecutionTime { get; set; }
        public bool SaveChangesBefore { get; set; }
        public bool SaveChangesAfter { get; set; }
        public bool SkipIfNoDataChanges { get; set; }
        public string EndUserInputsAsString { get; set; }
        public bool GetEverythingFromDb { get; set; }
        public Dictionary<string, object> EndUserInputs
        {
            get { return this.EndUserInputsAsString == null ? null : JsonConvert.DeserializeObject<Dictionary<string, object>>(this.EndUserInputsAsString); }
            set { this.EndUserInputsAsString = value == null ? null : JsonConvert.SerializeObject(value); }
        }
    }

    public enum OperationExecutionTime
    {
        Default = 0,
        Immediate = 1,
        Delayed = 2
    }
}
