using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model
{
    public interface IChangeLog : IEntity
    {
        ChangeType ChangeType { set; get; }
        Type EntityType { set; get; }
        int EntityId { set; get; }
        DateTime DateTime { set; get; }
        int? UserId { set; get; }
        Dictionary<string, object> NewValues { set; get; }
        int? OperationId { get; set; }
        int? BackgroundProcessId { get; set; }
    }

    public interface ILogged : IEntity
    {
        int Id { get; }
    }
    public interface IConditionallyLogged: ILogged
    {
        bool ShouldBeLogged();
    }

    public interface ILogged<T> : ILogged
        where T : IChangeLog, IEntity, new()
    {
    }

    public interface IConditionallyLogged<T> : ILogged<T>, IConditionallyLogged
        where T : IChangeLog, IEntity, new()
    {
    }

    public abstract class ChangeLogBase : IChangeLog
    {
        public int Id { get; set; }
        public ChangeType ChangeType { get; set; }
        [MaxLength(200), Required()]
        public string EntityTypeAsString { get; set; }
        [NotMapped(), Required()]
        public Type EntityType
        {
            get { return ExpressionSerialization.GetTypeForName(this.EntityTypeAsString); }
            set { this.EntityTypeAsString = ExpressionSerialization.GetNameForType(value); }
        }
        public int EntityId { get; set; }
        public DateTime DateTime { get; set; }
        public int? UserId { get; set; }
        public UserManagement.User User { get; set; }
        public string NewValuesAsString { get; set; }
        [NotMapped]
        public Dictionary<string, object> NewValues
        {
            get { return this.NewValuesAsString?.FromJson<Dictionary<string, object>>(); }
            set { this.NewValuesAsString = value?.ToJson(); }
        }
        [NotMapped()]
        public TextPlaceHolder Info
        {
            get { return this.InfoAsString.FromJson<TextPlaceHolder>(); }
            set { this.InfoAsString = value.ToJson(); }
        }
        public string InfoAsString { get; set; }
        public string TechnicalInfo { get; set; }
        public int? OperationId { get; set; }
        public int? BackgroundProcessId { get; set; }
    }

    public abstract class EntityHistory
    {
        public int Id { get; set; }
        public int EntityId { get; set; }
        public DateTime DateTime { get; set; }
        public int? UserId { get; set; }
        public List<string> ChangedProperties { get; set; } = new List<string>();
    }

    public class EntityHistory<T> : EntityHistory
        where T: IEntity 
    {
        public T Entity { get; set; }
    }

}
