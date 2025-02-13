using Damco.Model.Emails;
using Damco.Model.Interfacing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model.Workflow
{
    public abstract class Operation : IEntity
    {
        public int Id { get; set; }
        [MaxLength(100), Required()]
        public string Name { get; set; }
        public string Description { get; set; }
        [NotMapped()]
        public string[] Tags
        {
            get { return this.TagsAsString?.FromJson<string[]>() ?? new string[] { }; }
            set { this.TagsAsString = (value == null || value.Length == 0 ? null : value.ToJson()); }
        }
        public string TagsAsString { get; set; }

        public string ConditionAsString { get; set; }
        public LambdaExpression GetCondition(ParameterExpression parameter) { return this.ConditionAsString?.DeserializeExpression(parameter); }
        public void SetCondition(LambdaExpression condition) { this.ConditionAsString = condition?.SerializeToString(); }
        [ForeignKey(nameof(OperationAlias.ItemId))]
        public List<OperationAlias> Aliases { get; set; } = new List<OperationAlias>();
        public bool FindIdsFirst { get; set; }
        [MaxLength(100)]
        public string BackgroundProcessCategoryCode { get; set; }
        public string BackgroundProcessingDoneMessage { get; set; }
        [NotMapped()]
        public int? MaximumRecords { get; set; }
        [NotMapped()]
        public string TechnicalInfo { get; set; }
        [NotMapped()]
        public bool CacheConditionQuery { get; set; }
    }

    //public class DummyOperation : Operation, IExntity
    //{
        //TODO: Remove this class once we have at least one Damco operation class in the DB
        //We need this now for FinOps to work, otherwise we get message
        //  The abstract type 'Damco.Model.Workflow.Operation' has no mapped descendants and so cannot be mapped. 
        //  Either remove 'Damco.Model.Workflow.Operation' from the model or add one or more types deriving from 'Damco.Model.Workflow.Operation' to the model. 
    //}

    public class OperationAlias : AliasBase<Operation>, IEntity
    {
    }

}
