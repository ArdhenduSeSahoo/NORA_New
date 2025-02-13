//Species specific changes on one or more entities so pass to the service for saving.
using Damco.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model
{
    [Serializable()]
    public enum ChangeType
    {
        Add = 1,
        Update = 2,
        Delete = 3,
        None = 4
    }

    public static class ChangeInfoExtensions
    {
        public static EntityChangeInfo<T> GetChangeInfo<T>(this T entity)
            where T : IEntity
        {
            return new EntityChangeInfo<T>(entity, ChangeType.None);
        }
        public static EntityChangeInfo<T> GetChangeInfo<T>(this T entity, ChangeType changeType)
            where T : IEntity
        {
            return new EntityChangeInfo<T>(entity, changeType);
        }

        public static EntityChangeInfo<T> GetChangeInfo<T>(this T entity, params string[] updatedProperties)
            where T : IEntity
        {
            return new EntityChangeInfo<T>(entity, updatedProperties);
        }
        public static EntityChangeInfo<T> GetChangeInfo<T>(this T entity, IEnumerable<string> updatedProperties)
            where T : IEntity
        {
            return new EntityChangeInfo<T>(entity, updatedProperties);
        }

        public static EntityChangeInfo<T> GetChangeInfo<T>(this T entity, params Expression<Func<T, object>>[] updatedProperties)
            where T : IEntity
        {
            return new EntityChangeInfo<T>(entity, updatedProperties);
        }
        public static EntitySetChangeInfo<T> GetChangeInfo<T>(this IEnumerable<T> entities, params string[] updatedProperties)
            where T : IEntity
        {
            return new EntitySetChangeInfo<T>(entities, updatedProperties);
        }
        public static EntitySetChangeInfo<T> GetChangeInfo<T>(this IEnumerable<T> entities, IEnumerable<string> updatedProperties)
            where T : IEntity
        {
            return new EntitySetChangeInfo<T>(entities, updatedProperties);
        }
        public static EntitySetChangeInfo<T> GetChangeInfo<T>(this IEnumerable<T> entities, ChangeType changeType)
            where T : IEntity
        {
            return new EntitySetChangeInfo<T>(entities, changeType);
        }
        public static EntitySetChangeInfo<T> GetChangeInfo<T>(this IEnumerable<T> entities, params Expression<Func<T, object>>[] updatedProperties)
            where T : IEntity
        {
            return new EntitySetChangeInfo<T>(entities, updatedProperties);
        }
    }
    public abstract class ChangeInfo
    {
        public ChangeType ChangeType { get; set; }
        public List<string> UpdatedProperties { get; set; } = new List<string>();
        public string TemporaryKey { get; set; }

        public abstract IEnumerable<IEntity> GetEntities();

        static internal List<string> GetProperties<T>(IEnumerable<Expression<Func<T, object>>> expressions)
        {
            return GetPropertyNames<T, object>(expressions);
        }

        static internal List<string> GetPropertyNames<T, Tproperty>(IEnumerable<Expression<Func<T, Tproperty>>> expressions)
        {
            List<string> properties = new List<string>();
            foreach (var property in expressions)
                properties.Add(GetPropertyName(property.Body));
            return properties;
        }

        static internal string GetPropertyName(Expression prop)
        {
            if (prop is LambdaExpression)
                prop = ((LambdaExpression)prop).Body;
            if (prop is UnaryExpression && ((UnaryExpression)prop).NodeType == ExpressionType.Convert)
                prop = ((UnaryExpression)prop).Operand;
            if (prop is MemberExpression)
            {
                var chain = new List<string>();
                while (prop is MemberExpression || prop is MethodCallExpression)
                {
                    if (prop is MemberExpression)
                    {
                        chain.Insert(0, ((MemberExpression)prop).Member.Name);
                        prop = ((MemberExpression)prop).Expression;
                    }
                    else if(prop is MethodCallExpression)
                    {
                        chain.Insert(0, $"{((MethodCallExpression)prop).Method.Name}()");
                        prop = ((MethodCallExpression)prop).Object ?? ((MethodCallExpression)prop).Arguments.First();
                    }
                }
                return string.Join(".", chain.ToArray());
            }
            else
                throw new ArgumentException($"{nameof(prop)} must be a simple property getter.", nameof(prop));
        }

    }

    public abstract class ChangeInfo<T> : ChangeInfo
    {
        public void AddChangedProperty(Expression property)
        {
            this.UpdatedProperties.Add(GetPropertyName(property));
        }

    }

    [Serializable()]
    public class EntityChangeInfo<T> : ChangeInfo<T>
        where T : IEntity
    {
        public EntityChangeInfo()
        {
        }

        public EntityChangeInfo(T entity, ChangeType changeType)
        {
            this.Entity = entity;
            this.ChangeType = changeType;
        }
        public EntityChangeInfo(T entity, IEnumerable<string> updatedProperties) : this(entity, ChangeType.Update)
        {
            this.UpdatedProperties = updatedProperties.ToList();
        }
        public EntityChangeInfo(T entity, params string[] updatedProperties) : this(entity, (IEnumerable<string>)updatedProperties)
        {
        }
        public EntityChangeInfo(T entity, params Expression<Func<T, object>>[] updatedProperties) : this(entity, ChangeType.Update)
        {
            this.UpdatedProperties = GetProperties(updatedProperties);
        }

        public T Entity { get; set; }

        public override IEnumerable<IEntity> GetEntities()
        {
            yield return this.Entity;
        }
    }

    [Serializable()]
    public class EntitySetChangeInfo<T> : ChangeInfo<T>
        where T : IEntity
    {
        public EntitySetChangeInfo(IEnumerable<T> entities, ChangeType changeType)
        {
            this.Entities = entities;
            this.ChangeType = changeType;
        }
        public EntitySetChangeInfo(IEnumerable<T> entities, IEnumerable<string> updatedProperties) : this(entities, ChangeType.Update)
        {
            this.UpdatedProperties = updatedProperties.ToList();
        }
        public EntitySetChangeInfo(IEnumerable<T> entities, params string[] updatedProperties) : this(entities, (IEnumerable<string>)updatedProperties)
        {
        }
        public EntitySetChangeInfo(IEnumerable<T> entities, params Expression<Func<T, object>>[] updatedProperties) : this(entities, ChangeType.Update)
        {
            if (!updatedProperties.Any())
                throw new ArgumentException("Please specify at least one updated property", nameof(updatedProperties));
            this.UpdatedProperties = EntityChangeInfo<T>.GetProperties(updatedProperties);
        }

        public IEnumerable<T> Entities { get; set; }
        public override IEnumerable<IEntity> GetEntities()
        {
            return this.Entities.Cast<IEntity>();
        }
    }

    public static class ChangeTrackerExtensions
    {
        public static EntityChangeTracker<T> GetChangeTracker<T>(this T entity) where T : IEntity
        {
            return entity.GetChangeTracker(false);
        }
        public static EntityChangeTracker<T> GetChangeTracker<T>(this T entity, bool isAdd) where T : IEntity
        {
            return new EntityChangeTracker<T>(entity, isAdd);
        }
    }

    public class EntityChangeTracker<T> where T : IEntity
    {
        static Action<T, Dictionary<string, object>> _valueSaver;
        static Func<T, Dictionary<string, object>, string[]> _changeGetter; //Note the array will have nulls for unchanged properties, it's just easier.

        static EntityChangeTracker()
        {
            ParameterExpression paramT = Expression.Parameter(typeof(T), "t");
            ParameterExpression paramD = Expression.Parameter(typeof(Dictionary<string, object>), "d");

            var addItemMethod = typeof(Dictionary<string, object>).GetMethod("Add");
            _valueSaver = Expression.Lambda<Action<T, Dictionary<string, object>>>(Expression.Block(
                typeof(T).GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.FlattenHierarchy)
                .Select(p => Expression.Call(paramD, addItemMethod, Expression.Constant(p.Name), Expression.Convert(Expression.Property(paramT, p), typeof(object))))
            ), paramT, paramD)
            .Compile();

            var equalsMethod = typeof(object).GetMethod("Equals", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            var getItemMethod = typeof(Dictionary<string, object>).GetMethod("get_Item");
            _changeGetter = Expression.Lambda<Func<T, Dictionary<string, object>, string[]>>(Expression.NewArrayInit(typeof(string),
                typeof(T).GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.FlattenHierarchy)
                .Select(p =>
                    Expression.Condition(
                        Expression.Call(equalsMethod,
                            Expression.Call(paramD, getItemMethod, Expression.Constant(p.Name)),
                            Expression.Convert(Expression.Property(paramT, p), typeof(object))
                        ),
                        Expression.Constant(default(string), typeof(string)),
                        Expression.Constant(p.Name)
                    )
                )
            ), paramT, paramD)
            .Compile();
        }

        Dictionary<string, object> _originalValues = new Dictionary<string, object>();
        public T Entity { get; private set; }
        bool _isAdd;

        public EntityChangeTracker(T entity, bool isAdd)
        {
            Entity = entity;
            _isAdd = isAdd;
            _valueSaver(entity, _originalValues);
        }

        public EntityChangeInfo<T> GetChangeInfo()
        {
            if (_isAdd)
                return Entity.GetChangeInfo(ChangeType.Add);
            else
                return Entity.GetChangeInfo(_changeGetter(Entity, _originalValues).Where(p => p != null).ToArray());
        }
    }

}
