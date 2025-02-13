using Damco.Model.Interfacing;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model
{
    public class RelationshipInfo
    {
        public PropertyInfo ForeignKeyProperty { get; internal set; }
        public PropertyInfo NavigatorPropertyFromChild { get; internal set; }
        public PropertyInfo NavigatorPropertyFromParent { get; internal set; }
        public bool OneToOne { get; internal set; }
        public bool CascadeDelete { get; internal set; }
        public bool IsRequired { get; set; }
    }

    public static class ModelUtils
    {
        public static Type FindAliasType(this Type entityType)
        {
            return entityType.Assembly.GetTypes()
                .Where(t => t.BaseType == typeof(AliasBase<>).MakeGenericType(entityType)).FirstOrDefault();
        }

        public static IEnumerable<RelationshipInfo> GetForeignKeys(this Type entityType)
        {
            var allProperties = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                .Where(x => x.GetCustomAttribute<NotMappedAttribute>() == null)
                .ToDictionary(p => p.Name, p => p);

            List<string> handledProperties = new List<string>();

            List<PropertyInfo> doneForeignKeys = new List<PropertyInfo>();
            foreach (var property in allProperties.OrderBy(x => x.Value.GetCustomAttribute<ForeignKeyAttribute>(true) != null ? 1 : 2)
            )
            {
                bool explicitForeignKey = false;
                PropertyInfo navigatorFromChild = null;
                PropertyInfo foreignKey = null;
                var foreignKeyAttribute = property.Value.GetCustomAttribute<ForeignKeyAttribute>(true);
                if (foreignKeyAttribute != null)
                {
                    explicitForeignKey = true;
                    if (property.Value.PropertyType == typeof(int?) || property.Value.PropertyType == typeof(int))
                    {
                        foreignKey = property.Value;
                        navigatorFromChild = allProperties[foreignKeyAttribute.Name];
                    }
                    else if (allProperties.TryGetValue(foreignKeyAttribute.Name, out foreignKey))
                        navigatorFromChild = property.Value;
                }
                else if (property.Key != "Id" && property.Key.EndsWith("Id"))
                {
                    foreignKey = property.Value;
                    allProperties.TryGetValue(property.Key.Substring(0, property.Key.Length - 2), out navigatorFromChild);
                }
                if (foreignKey != null && navigatorFromChild != null)
                {
                    if (explicitForeignKey || typeof(IEntity).IsAssignableFrom(navigatorFromChild.PropertyType))
                    {
                        bool oneToOne = false;
                        var allParentProperties = navigatorFromChild.PropertyType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy).Where(x => x.GetCustomAttribute<NotMappedAttribute>() == null);
                        var navigatorsFromParent = allParentProperties
                                .Where(p =>
                                     p.PropertyType == typeof(List<>).MakeGenericType(entityType) || p.PropertyType == entityType
                                ).ToList();
                        var specificNavigators = navigatorsFromParent.Where(p => p.GetCustomAttribute<ForeignKeyAttribute>(true)?.Name == foreignKey.Name).ToList();
                        if (specificNavigators.Any())
                            navigatorsFromParent = specificNavigators;
                        if (navigatorsFromParent.Count() == 1)
                            oneToOne = navigatorsFromParent.Single().PropertyType == entityType;
                        if (!doneForeignKeys.Contains(foreignKey))
                        {
                            yield return new RelationshipInfo()
                            {
                                CascadeDelete = navigatorFromChild.GetCustomAttribute<CascadeDeleteAttribute>(true) != null,
                                ForeignKeyProperty = foreignKey,
                                NavigatorPropertyFromChild = navigatorFromChild,
                                NavigatorPropertyFromParent = navigatorsFromParent.Count() == 1 ? navigatorsFromParent.Single() : null,
                                OneToOne = oneToOne,
                                IsRequired = (Nullable.GetUnderlyingType(foreignKey.PropertyType) == null)
                            };
                            doneForeignKeys.Add(foreignKey);
                        }
                    }
                }
            }
        }

        /*
        //Stopped development on these because not needed (for now)
        public static void GiveUniquePrimaryKeys(this IEnumerable<IEntity> entities)
        {
            foreach (var group in entities.GroupBy(e => e.GetType()))
            {
                var idProperty = group.Key.GetProperty("Id");
                var entityParam = Expression.Parameter(typeof(IEntity), "p");
                var idParam = Expression.Parameter(typeof(int), "i");
                Action<IEntity, int> keySetter = Expression.Lambda<Action<IEntity, int>>(
                    Expression.Assign(
                        Expression.Property(
                            Expression.Convert(entityParam, group.Key),
                            idProperty
                        ),
                        Expression.Convert(idParam, idProperty.PropertyType)
                    ),
                    entityParam,
                    idParam
                ).Compile();
                int id = 1;
                foreach (var entity in group)
                {
                    keySetter(entity, id);
                    id++;
                }
            }
        }
        public static IEnumerable<IEntity> GetFullEntityTree<T>(params IEntity[] entities)
        {
            return ((IEnumerable<IEntity>)entities).GetFullEntityTree();
        }
        public static IEnumerable<IEntity> GetFullEntityTree(this IEnumerable<IEntity> entities)
        {
            
               
        }
        */

        public static Expression GetPrimaryKeysPredicate(this IEnumerable<IEntity> entities, ParameterExpression entityParameter, bool keepEntityIfZeroKey = false)
        {
            if (!entities.Any())
                //throw new ArgumentException("At least one entity must be provided", nameof(entities));
                return Expression.Constant(false);
            else if (entities.Select(e => e.GetType()).Distinct().Count() > 1)
                throw new ArgumentException("Entities must all be the same type", nameof(entities));
            var entityType = entities.First().GetType();

            LambdaExpression keyGetter;
            Expression keyGetterBodyInTarget;
            Delegate keyGetterFuncInSource;
            if (entityType == entityParameter.Type)
            {
                keyGetter = GetKeyGetterLambda(entityType, entityParameter);
                keyGetterBodyInTarget = keyGetter.Body;
                keyGetterFuncInSource = keyGetter.Compile();
            }
            else
            {
                keyGetter = GetKeyGetterLambda(entityType, Expression.Parameter(entityType, "p"));
                keyGetterFuncInSource = keyGetter.Compile();
                keyGetterBodyInTarget = GetKeyGetterLambda(entityType, entityParameter).Body;
            }

            var keys = entities.Select(e => keyGetterFuncInSource.DynamicInvoke(e)).Cast<int>().ToList();
            Expression keyList;
            if (keepEntityIfZeroKey && keys.Contains(0) && keyGetter.Body is MemberExpression member && member.Expression is ParameterExpression)
            {
                List<Expression> keyExpressions = new List<Expression>();
                keyExpressions.AddRange(keys.Where(x => x != 0).Select(x => Expression.Constant(x)));
                keyExpressions.AddRange(entities
                    .Where(e => (int)keyGetterFuncInSource.DynamicInvoke(e) == 0)
                    .Select(e => Expression.PropertyOrField(Expression.Constant(e), member.Member.Name))
                );
                keyList = Expression.ListInit(
                    Expression.New(typeof(List<int>))
                    , keyExpressions
                );
            }
            else
                keyList = Expression.Constant(keys);

            return Expression.Call(
                keyList,
                "Contains",
                null,
                keyGetterBodyInTarget
            );
        }

        public static LambdaExpression GetPrimaryKeysPredicateLambda(this IEnumerable<IEntity> entities)
        {
            if (!entities.Any())
                throw new ArgumentException("At least one entity must be provided", nameof(entities));
            else if (entities.Select(e => e.GetType()).Distinct().Count() > 1)
                throw new ArgumentException("Entities must all be the same type", nameof(entities));
            var entityType = entities.First().GetType();

            var param = Expression.Parameter(entityType, "p");
            return Expression.Lambda(entities.GetPrimaryKeysPredicate(param), param);
        }

        public static LambdaExpression GetKeyGetterLambda(Type entityType, ParameterExpression entityParameter = null)
        {
            if (entityParameter == null)
                entityParameter = Expression.Parameter(entityType, "p");
            var property = GetPrimaryKeyProperties(entityType).Single();
            if (entityParameter.Type == entityType)
            {
                return Expression.Lambda(
                    Expression.Property(entityParameter, property),
                    entityParameter
                );
            }
            else if (entityParameter.Type != typeof(DynamicEntity))
                throw new ArgumentException($"Type of {nameof(entityParameter)} must equal {nameof(entityType)} or {nameof(DynamicEntity)}", nameof(entityParameter));
            else //Dynamic entity
            {
                return DynamicEntity.GetValueGetterLambda(entityParameter,
                    property.Name,
                    property.PropertyType
                );
            }
        }

        public static IEnumerable<PropertyInfo> GetPrimaryKeyProperties(Type entityType)
        {
            var allProperties = entityType.GetProperties();
            if (allProperties.Any(p => p.GetCustomAttribute<KeyAttribute>(true) != null))
                foreach (var prop in entityType.GetProperties().Where(p => p.GetCustomAttribute<KeyAttribute>(true) != null))
                    yield return prop;
            else
                yield return allProperties.Single(p => p.Name == "Id");
        }

        public static Expression<Func<IEntity, IEntity, bool>> GetByKeyComparer(Type entityType)
        {
            var keyProperties = GetPrimaryKeyProperties(entityType);
            if (keyProperties.Count() > 1)
                throw new NotSupportedException("GetCompareByKeyPredicate does not (yet) support multiple properties");
            var inputParameter1 = Expression.Parameter(typeof(IEntity));
            var inputParameter2 = Expression.Parameter(typeof(IEntity));
            return Expression.Lambda<Func<IEntity, IEntity, bool>>(
                Expression.Equal(
                    Expression.PropertyOrField(Expression.Convert(inputParameter1, entityType), keyProperties.Single().Name),
                    Expression.PropertyOrField(Expression.Convert(inputParameter2, entityType), keyProperties.Single().Name)
                ),
                inputParameter1, inputParameter2
            );
        }

    }
}
