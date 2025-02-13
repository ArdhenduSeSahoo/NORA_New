using Damco.Model.Interfacing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Damco.Model.Workflow
{
    public class OperationInput
    {
        public OperationInput()
        {
        }

        private OperationInput(Type entityType, Dictionary<string, object> endUserInputs = null, Dictionary<string, object> runningInformation = null)
        {
            this.EntityType = entityType;
            this.RunningInformation = runningInformation ?? new Dictionary<string, object>();
            this.EndUserInputs = endUserInputs ?? new Dictionary<string, object>();
        }
        
        public OperationInput(Type entityType, IEnumerable<ChangeInfo> entityChanges, Dictionary<string, object> endUserInputs = null, Dictionary<string, object> runningInformation = null)
            : this(entityType
                  , entityChanges.SelectMany(c => c.GetEntities()).ToList()
                  , (from c in entityChanges from e in c.GetEntities() select new { c, e }).ToDictionary(x => x.e, x => x.c)
                  , endUserInputs, runningInformation)

        {
        }

        public OperationInput(Type entityType, List<IEntity> entities, Dictionary<IEntity, ChangeInfo> entityChanges = null, Dictionary<string, object> endUserInputs = null, Dictionary<string, object> runningInformation = null)
            : this(entityType, endUserInputs, runningInformation)
        {
            this.Entities = entities;
            if (entityChanges != null)
                this.EntityChanges = entityChanges;
            AddEntitiesToEntityChanges(entities);
        }

        public OperationInput(Type entityType, LambdaExpression entityPredicate, Dictionary<string, object> endUserInputs = null, Dictionary<string, object> runningInformation = null)
            : this(entityType, endUserInputs, runningInformation)
        {
            this.EntityPredicate = entityPredicate;
        }

        public OperationInput(OperationInput parent, Dictionary<string, object> runningInformation)
            : this(parent.EntityType, parent.EndUserInputs, runningInformation)
        {
            this.OverlookedErrors = parent.OverlookedErrors;
            this.Entities = parent.Entities;
            this.EntityChanges = parent.EntityChanges;
            this.EntityPredicate = parent.EntityPredicate;
        }

        public OperationInput(OperationInput parent, string runningInformationKey)
            : this(parent,
                  runningInformationKey == ""
                  ? parent.RunningInformation
                  : (parent.RunningInformation != null && parent.RunningInformation.ContainsKey(runningInformationKey) ? (Dictionary<string, object>)parent.RunningInformation[runningInformationKey] : null)
            )
        {
        }

        public OperationInput(OperationInput parent, Type entityType, List<IEntity> entities, LambdaExpression entityPredicate, string runningInformationKey)
            : this(parent, runningInformationKey)
        {
            this.EntityType = entityType;
            this.Entities = entities;
            AddEntitiesToEntityChanges(entities);
            this.EntityPredicate = entityPredicate;
        }

        public OperationInput(OperationInput parent, Type entityType, List<IEntity> entities, string runningInformationKey)
            : this(parent, entityType, entities, default(LambdaExpression), runningInformationKey)
        {

        }

        public OperationInput(OperationInput parent, Type entityType, LambdaExpression entityPredicate, string runningInformationKey)
            : this(parent, entityType, default(List<IEntity>), entityPredicate, runningInformationKey)
        {
        }
        private void AddEntitiesToEntityChanges(List<IEntity> entities)
        {
            if (entities != null)
                foreach (var entity in entities)
                    if (!this.EntityChanges.ContainsKey(entity))
                        this.EntityChanges.Add(entity, entity.GetChangeInfo());
        }

        [JsonIgnore()]
        public Type EntityType { get; private set; }
        public string EntityTypeAsString
        {
            get { return this.EntityType?.GetNameForType(); }
            set { this.EntityType = value?.GetTypeForName(); }
        }

        [JsonIgnore()]
        public List<IEntity> Entities { get; private set; }
        Dictionary<IEntity, ChangeInfo> _entityChanges = new Dictionary<IEntity, ChangeInfo>();
        [JsonIgnore()]
        public Dictionary<IEntity, ChangeInfo> EntityChanges
        {
            get { return _entityChanges; }
            private set
            {
                if (value == null)
                    "".ToString();
                _entityChanges = value;
            }
        }

        [JsonIgnore()]
        public LambdaExpression EntityPredicate { get; set; }
        public string EntityPredicateForSerialization
        {
            //Note since we don't serialize the entities, they get serialized as the predicate instead
            get { return (this.EntityPredicate ?? ModelUtils.GetPrimaryKeysPredicateLambda(this.Entities))?.SerializeToString(); }
            set { this.EntityPredicate = value?.DeserializeExpression(Expression.Parameter(this.EntityType, "p")); }
        }

        public Dictionary<string, object> EndUserInputs { get; set; }
        public Dictionary<string, object> RunningInformation { get; set; }
        public List<string> OverlookedErrors { get; set; } = new List<string>();
    }

    public class OperationResult
    {
        public Dictionary<string, byte[]> Files { get; set; } = new Dictionary<string, byte[]>();
        public List<FoundError> FoundErrors { get; } = new List<FoundError>();
        public List<string> FoundWarnings { get; } = new List<string>();
        public Dictionary<string, object> DelayedProcessingRunningInformation { get; set; } = new Dictionary<string, object>();
        public bool RequiresDelayedProcessing { get; set; }

        public void AddErrorsFromException(BusinessException exception)
        {
            this.FoundErrors.AddRange(FoundError.GetFromException(exception));
        }
    }

    public class FoundError
    {
        public static IEnumerable<FoundError> GetFromException(BusinessException exception)
        {
            if (exception is AggregateBusinessException)
                foreach (var child in ((AggregateBusinessException)exception).Children)
                    foreach (var err in GetFromException(child))
                        yield return err;
            else
                yield return new FoundError(exception.Message, exception.CanBeOverlooked, exception.OverlookRequiresAdministrator);
        }

        public FoundError()
        {
            
        }
        public FoundError(string errorMessage, bool canBeOverlooked, bool overlookRequiresAdministrator) : this(null, errorMessage, canBeOverlooked, overlookRequiresAdministrator)
        {
        }
        public FoundError(IEntity entity, string errorMessage, bool canBeOverlooked, bool overlookRequiresAdministrator)
        {
            this.Entity = entity;
            this.ErrorMessage = errorMessage;
            this.CanBeOverlooked = canBeOverlooked;
            this.OverlookRequiresAdministrator = overlookRequiresAdministrator;
        }
        [JsonIgnore()]
        public IEntity Entity { get; set; } 
        public string ErrorMessage { get; set; } // TODO check if custom json converter is better solution than public setters
        public bool CanBeOverlooked { get; set; }
        public bool OverlookRequiresAdministrator { get; set; }

        public InboundMessageError ToInboundMessageError(int? statusId = null)
        {
            return new InboundMessageError()
            {
                Description = ErrorMessage,
                Overlook = false,
                StatusId = statusId,
                DelegatedToId = null,
                CanBeOverlooked = CanBeOverlooked,
                OverlookRequiresAdministrator = OverlookRequiresAdministrator,
                IsSolved = false
            };
        }
        
    }

    public static class FoundErrorExtensions
    {
        public static void ThrowExceptionIfAny(this IEnumerable<FoundError> errors)
        {
            if (errors != null)
            {
                if (errors.Count() == 1)
                    throw new BusinessException((TextPlaceHolder)errors.First().ErrorMessage);
                else if (errors.Count() > 1)
                    throw new AggregateBusinessException(errors.Select(e =>
                        new BusinessException((TextPlaceHolder)e.ErrorMessage)
                    ));
            }
        }
    }

}
