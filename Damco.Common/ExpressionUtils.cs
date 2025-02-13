//Extension methods for dealing with linq expressions
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Common
{
    public static class ExpressionUtils
    {
        //Doesn't do anything, just here to quickly create an expression from a lamda
        //Optionally aligns the parameters used so multiple expressions can be created with the same parameter
        public class LambaGetter<T>
        {
            public ParameterExpression[] Parameters { get; set; }
            public LambaGetter() { }
            public LambaGetter(params ParameterExpression[] parameters)
            {
                Parameters = parameters;
            }
            public Expression<Func<T, TResult>> Get<TResult>(Expression<Func<T, TResult>> expression)
            {
                if (Parameters == null)
                    return expression;
                else
                    return GetReplacer(expression).VisitAndConvert<Expression<Func<T, TResult>>>(expression, "LambdaGetter.Get");
            }

            private ParameterReplacer GetReplacer(LambdaExpression expression)
            {
                var replacements = new Dictionary<ParameterExpression, ParameterExpression>();
                int index = 0;
                foreach (var param in Parameters)
                    replacements.Add(expression.Parameters[index], param);
                return new ParameterReplacer(replacements);
            }

            public Expression GetBody<TResult>(Expression<Func<T, TResult>> expression)
            {
                if (Parameters == null)
                    return expression.Body;
                else
                    return GetReplacer(expression).VisitAndConvert<Expression>(expression.Body, "LambdaGetter.Get");
            }

            private class ParameterReplacer : ExpressionVisitor
            {
                Dictionary<ParameterExpression, ParameterExpression> _replacements;
                public ParameterReplacer(Dictionary<ParameterExpression, ParameterExpression> replacements)
                {
                    _replacements = replacements;
                }

                protected override Expression VisitParameter(ParameterExpression node)
                {
                    ParameterExpression toUse;
                    if (_replacements.TryGetValue(node, out toUse))
                        return toUse;
                    else
                        return base.VisitParameter(node);
                }
            }

        }
        public static LambaGetter<T> Lamba<T>()
        {
            return new LambaGetter<T>();
        }
        public static LambaGetter<T> FixedParamLambda<T>()
        {
            return FixedParamLambda<T>(Expression.Parameter(typeof(T), "p"));
        }
        public static LambaGetter<T> FixedParamLambda<T>(ParameterExpression parameter)
        {
            return new LambaGetter<T>(parameter);
        }

        public static BinaryExpression CompareWith(this Expression left, Expression right, ExpressionType type)
        {
            if (type == ExpressionType.Equal)
                return Expression.Equal(left, right);
            else if (type == ExpressionType.NotEqual)
                return Expression.NotEqual(left, right);
            else if (type == ExpressionType.GreaterThan || type == ExpressionType.LessThan || type == ExpressionType.LessThanOrEqual || type == ExpressionType.GreaterThanOrEqual)
            {
                if (left.Type == typeof(string))
                {
                    return Expression.MakeBinary(
                        type,
                        Expression.Call(
                            left,
                            typeof(string).GetMethod("CompareTo", new[] { typeof(string) }),
                            right
                         ),
                         Expression.Constant(0)
                    );
                }
                else
                {
                    return Expression.MakeBinary(
                        type,
                        left,
                        right
                    );
                }
            }
            else
                throw new ArgumentException("type must be 'ExpressionType.Equal', 'ExpressionType.NotEqual', 'ExpressionType.GreaterThan', 'ExpressionType.LessThan', 'ExpressionType.LessThanOrEqual' or 'ExpressionType.GreaterThanOrEqual'", "type");
        }

        public static Expression<Action<T, T>> GetPropertiesCopier<T>(IEnumerable<string> properties)
        {
            ParameterExpression source = Expression.Parameter(typeof(T), "s");
            ParameterExpression target = Expression.Parameter(typeof(T), "t");
            return Expression.Lambda<Action<T, T>>(Expression.Block(
                from propName in properties
                select typeof(T).GetProperty(propName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.FlattenHierarchy | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic) into prop
                select Expression.Assign(
                    Expression.Property(target, prop),
                    Expression.Property(source, prop)
                )
            ), source, target);
        }

        public static Expression Replace(this Expression expression, Expression oldValue, Expression newValue)
        {
            return expression.Replace(e =>
            {
                if (e == oldValue)
                    return newValue;
                else
                    return null;
            });
        }

        public static object GetConstantValue(this Expression expression)
        {
            if (expression is ConstantExpression)
                return ((ConstantExpression)expression).Value;
            else
                return Expression.Lambda(expression).Compile().DynamicInvoke();
        }

        public static bool CanBeConvertedToConstant(this Expression expression)
        {
            var allParameters = expression.RecursiveWhere(x => x is ParameterExpression).ToList();
            if (allParameters.Any())
                return
                    !allParameters
                    .Except(
                        expression.RecursiveWhere(x => x is LambdaExpression)
                        .Cast<LambdaExpression>()
                        .SelectMany(x => x.Parameters)
                    ).Any(); //Check for parameters in the expression from a Lambda not in the expresssion
            else
                return true;
        }

        public static Expression<T> Replace<T>(this Expression<T> expression, Func<Expression, Expression> newValueFactory)
        {
            return (Expression<T>)new ReplaceVisitor(newValueFactory).Visit(expression);
        }

        public static Expression Replace(this Expression expression, Func<Expression, Expression> newValueFactory)
        {
            return new ReplaceVisitor(newValueFactory).Visit(expression);
        }

        private class FindVisitor : ExpressionVisitor
        {
            public FindVisitor(Func<Expression, bool> predicate)
            {
                _predicate = predicate;
            }
            Func<Expression, bool> _predicate;
            List<Expression> _found = new List<Expression>();
            bool _findingOne = false;

            public override Expression Visit(Expression node)
            {
                if (_predicate == null || _predicate(node))
                    _found.Add(node);
                if (_findingOne && _found.Any())
                    return node;
                else
                    return base.Visit(node);
            }
            public Expression FindFirst(Expression node)
            {
                _findingOne = true;
                _found = new List<Expression>();
                Visit(node);
                return _found.SingleOrDefault();
            }
            public IEnumerable<Expression> FindAll(Expression node)
            {
                _findingOne = false;
                _found = new List<Expression>();
                Visit(node);
                return _found;
            }
        }

        public static Expression RecursiveFirstOrDefault(this Expression expression, Func<Expression, bool> predicate)
        {
            return new FindVisitor(predicate).FindFirst(expression);
        }

        public static IEnumerable<Expression> RecursiveWhere(this Expression expression, Func<Expression, bool> predicate)
        {
            return new FindVisitor(predicate).FindAll(expression);
        }

        public static IEnumerable<Expression> Recurse(this Expression expression)
        {
            return RecursiveWhere(expression, x => true);
        }

        public static Expression BodyUsing(this LambdaExpression expression, params ParameterExpression[] parametersToUse)
        {
            return
                expression.Body.Replace(e =>
                {
                    if (e is ParameterExpression)
                    {
                        int index = expression.Parameters.IndexOf((ParameterExpression)e);
                        if (index == -1)
                            return null;
                        else if (parametersToUse.Length < index + 1)
                            return null;
                        return parametersToUse[index];
                    }
                    else
                        return null;
                });
        }
        private class ReplaceVisitor : ExpressionVisitor
        {
            Func<Expression, Expression> _newValueFactory;
            public ReplaceVisitor(Func<Expression, Expression> newValueFactory)
            {
                _newValueFactory = newValueFactory;
            }
            public override Expression Visit(Expression node)
            {
                var newValue = _newValueFactory(node);
                if (newValue != null)
                    return newValue;
                else
                    return base.Visit(node);
            }
        }

        public static Type GetEnumerableElementType(this Type enumerableType)
        {
            //if (enumerableType.IsArray)
            //  return enumerableType.GetElementType();
            //else
            return enumerableType.GetInterfaces().UnionSome(enumerableType)
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                ?.GetGenericArguments().FirstOrDefault();
        }

        private static Expression DoEnumerableMethodWithLambda(this Expression enumerableExpression, string methodName, LambdaExpression lambda)
        {
            //Type methodType;
            //Expression lambdaExpression;
            //AKO: Seems this is not needed, don't know why
            //if ((typeof(IQueryable).IsAssignableFrom(enumerableExpression.Type))
            //{
            //methodType = typeof(Queryable);
            //lambdaExpression = Expression.Quote(lambda);
            //}
            //else
            //{
            //methodType = typeof(Enumerable);
            //lambdaExpression = lambda;
            //}
            var method = typeof(Enumerable).GetMethods()
                .Where(m => m.Name == methodName && m.IsGenericMethodDefinition && m.GetGenericArguments().Length == 1)
                .Select(m => m.MakeGenericMethod(enumerableExpression.Type.GetEnumerableElementType()))
                .SingleOrDefault(m => m.GetParameters().Last().ParameterType == lambda.Type);
            if (method == null)
            {
                //Assume an overload with the return type
                method = typeof(Enumerable).GetMethods()
                    .Where(m => m.Name == methodName && m.IsGenericMethodDefinition && m.GetGenericArguments().Length == 2)
                    .Select(m => m.MakeGenericMethod(enumerableExpression.Type.GetEnumerableElementType(), lambda.Body.Type))
                    .SingleOrDefault(m => m.GetParameters().Last().ParameterType == lambda.Type);
                if (method == null)
                    throw new ArgumentException($"Method '{methodName}' not found for '{enumerableExpression.ToString()}', '{lambda.ToString()}'");
            }

            return Expression.Call(
                method,
                enumerableExpression,
                lambda
            );
        }

        private static Expression DoEnumerableMethodWithSelectorAndResultEnumerable(this Expression enumerableExpression, string methodName, LambdaExpression selector)
        {
            var method = typeof(Enumerable).GetMethods()
                .Single(m =>
                    m.Name == methodName
                    && m.IsGenericMethodDefinition
                    && m.GetParameters().Last().ParameterType.GetGenericTypeDefinition() == selector.Type.GetGenericTypeDefinition()
                 )
                .MakeGenericMethod(enumerableExpression.Type.GetEnumerableElementType(), selector.ReturnType);
            return Expression.Call(
                method,
                enumerableExpression,
                selector
            );
        }

        private static Expression DoEnumerableMethod(this Expression enumerableExpression, string methodName, params Expression[] parameters)
        {
            var method = typeof(Enumerable).GetMethods()
               .Single(m => m.Name == methodName && m.IsGenericMethodDefinition && m.GetParameters().Length == 1 + (parameters?.Length ?? 0))
               .MakeGenericMethod(enumerableExpression.Type.GetEnumerableElementType());
            List<Expression> realParams = new List<Expression>();
            foreach (var param in method.GetParameters().CombineWith(enumerableExpression.Concat(parameters ?? Enumerable.Empty<Expression>())))
            {
                if (param.Item1.ParameterType.IsAssignableFrom(param.Item2.Type))
                    realParams.Add(param.Item2);
                else
                    realParams.Add(Expression.Convert(param.Item2, param.Item1.ParameterType));
            }
            return Expression.Call(
                method,
                realParams // enumerableExpression.Concat(parameters ?? Enumerable.Empty<Expression>())
            );
        }

        public static Expression Sum(this Expression enumerableExpression, LambdaExpression selector) { return enumerableExpression.DoEnumerableMethodWithLambda("Sum", selector); }
        public static Expression Min(this Expression enumerableExpression, LambdaExpression selector) { return enumerableExpression.DoEnumerableMethodWithLambda("Min", selector); }
        public static Expression Max(this Expression enumerableExpression, LambdaExpression selector) { return enumerableExpression.DoEnumerableMethodWithLambda("Max", selector); }
        public static Expression Average(this Expression enumerableExpression, LambdaExpression selector) { return enumerableExpression.DoEnumerableMethodWithLambda("Average", selector); }
        public static Expression Select(this Expression enumerableExpression, LambdaExpression selector) { return enumerableExpression.DoEnumerableMethodWithSelectorAndResultEnumerable("Select", selector); }
        public static Expression ToList(this Expression enumerableExpression) { return enumerableExpression.DoEnumerableMethod("ToList"); }
        public static Expression First(this Expression enumerableExpression) { return enumerableExpression.DoEnumerableMethod("First"); }
        public static Expression FirstOrDefault(this Expression enumerableExpression) { return enumerableExpression.DoEnumerableMethod("FirstOrDefault"); }
        public static Expression Distinct(this Expression enumerableExpression) { return enumerableExpression.DoEnumerableMethod("Distinct"); }
        public static Expression Count(this Expression enumerableExpression) { return enumerableExpression.DoEnumerableMethod("Count"); }
        public static Expression Where(this Expression enumerableExpression, LambdaExpression predicate) { return enumerableExpression.DoEnumerableMethodWithLambda("Where", predicate); }
        public static Expression Any(this Expression enumerableExpression, LambdaExpression predicate) { return enumerableExpression.DoEnumerableMethodWithLambda("Any", predicate); }

        public static Expression Contains(this Expression enumerableExpression, Expression value) { return enumerableExpression.DoEnumerableMethod("Contains", value); }

        public static Expression ToLower(this Expression stringExpression)
        {
            if (stringExpression.Type != typeof(string))
                throw new ArgumentException($"'{nameof(stringExpression)}' must be of type 'string'", nameof(stringExpression));
            return Expression.Call(stringExpression, typeof(string).GetMethods().Single(m => m.Name == nameof(string.ToLower) && m.GetParameters().Length == 0));
        }

        public static Expression StringReplace(this Expression stringExpression, Expression oldValue, Expression newValue)
        {
            if (stringExpression.Type != typeof(string))
                throw new ArgumentException($"'{nameof(stringExpression)}' must be of type 'string'", nameof(stringExpression));
            if (oldValue.Type != typeof(string))
                throw new ArgumentException($"'{nameof(oldValue)}' must be of type 'string'", nameof(oldValue));
            if (newValue.Type != typeof(string))
                throw new ArgumentException($"'{nameof(newValue)}' must be of type 'string'", nameof(newValue));
            return Expression.Call(stringExpression,
                typeof(string).GetMethods().Single(m => m.Name == nameof(string.Replace) && m.GetParameters().Length == 2 && m.GetParameters().All(x => x.ParameterType == typeof(string))),
                oldValue, newValue
            );
        }


        public static Expression Trim(this Expression stringExpression)
        {
            if (stringExpression.Type != typeof(string))
                throw new ArgumentException($"'{nameof(stringExpression)}' must be of type 'string'", nameof(stringExpression));
            return Expression.Call(stringExpression, typeof(string).GetMethods().Single(m => m.Name == nameof(string.Trim) && m.GetParameters().Length == 0));
        }

        public static Expression GroupKey(this Expression groupingExpression)
        {
            return Expression.Property(groupingExpression, groupingExpression.Type.GetProperty("Key"));
        }

        public static Expression ToUpper(this Expression stringExpression)
        {
            if (stringExpression.Type != typeof(string))
                throw new ArgumentException($"'{nameof(stringExpression)}' must be of type 'string'", nameof(stringExpression));
            return Expression.Call(stringExpression, typeof(string).GetMethods().Single(m => m.Name == nameof(string.ToUpper) && m.GetParameters().Length == 0));
        }

        public static Expression AndAlso(this Expression firstExpression, params Expression[] moreExpressions)
        {
            return AndAlso(firstExpression?.ToSingletonCollection(), moreExpressions);
        }

        public static Expression AndAlso(this IEnumerable<Expression> expressions, params Expression[] moreExpressions)
        {
            Expression result = null;
            foreach (var expression in expressions.Concat(moreExpressions ?? new Expression[] { }))
            {
                if (expression != null)
                {
                    if (result == null)
                        result = expression;
                    else
                        result = Expression.AndAlso(result, expression);
                }
            }
            return result;
        }

        public static Expression OrElse(this Expression firstExpression, params Expression[] moreExpressions)
        {
            return OrElse(firstExpression?.ToSingletonCollection(), moreExpressions);
        }

        public static Expression OrElse(this IEnumerable<Expression> expressions, params Expression[] moreExpressions)
        {
            Expression result = null;
            foreach (var expression in expressions.Concat(moreExpressions ?? new Expression[] { }))
            {
                if (expression != null)
                {
                    if (result == null)
                        result = expression;
                    else
                        result = Expression.OrElse(result, expression);
                }
            }
            return result;
        }

        //-
        public static ConcurrentDictionary<PropertyInfo, Func<object, object>> propertyGetters = new ConcurrentDictionary<PropertyInfo, Func<object, object>>();
        public static Func<object, object> GetGetterFunc(this PropertyInfo property)
        {
            return propertyGetters.GetOrAdd(property, p =>
            {
                return GetGetterLamba(property).Compile();
            });
        }

        public static Expression<Func<object, object>> GetGetterLamba(this PropertyInfo property)
        {
            var param = Expression.Parameter(typeof(object), "p");
            return Expression.Lambda<Func<object, object>>(
                Expression.Convert(
                    Expression.Property(
                        Expression.Convert(param, property.DeclaringType),
                        property
                    ),
                    typeof(object)
                ),
                param
            );
        }

        //-
        public static ConcurrentDictionary<object, Action<object, object>> advancedPropertySetters = new ConcurrentDictionary<object, Action<object, object>>();
        public static Action<object, object> GetSetterAction(this Expression propertyExpression, ParameterExpression objectExpression, object cacheKey = null)
        {
            if (cacheKey == null)
                return propertyExpression.GetSetterLambda(objectExpression).Compile();
            else
                return advancedPropertySetters.GetOrAdd(cacheKey, k =>
                {
                    return propertyExpression.GetSetterAction(objectExpression);
                });
        }
        public static Expression<Action<object, object>> GetSetterLambda(this Expression propertyExpression, Expression objectExpression)
        {
            //if (objectExpression == null)
            //TODO: Maybe not this, but root expressoin instead?
            //    objectExpression = propertyExpression.FindAll(x => x is ParameterExpression).Single();
            var valueParameter = Expression.Parameter(typeof(object), "v");
            var unary = propertyExpression as UnaryExpression;
            if (unary != null && unary.NodeType == ExpressionType.Convert)
                propertyExpression = unary.Operand;
            var objectParameter = Expression.Parameter(typeof(object), "p");
            propertyExpression = propertyExpression.Replace(x =>
            {
                if (x == objectExpression)
                    return Expression.Convert(objectParameter, x.Type);
                else
                    return null;
            });
            return Expression.Lambda<Action<object, object>>(
                   Expression.Assign(
                       propertyExpression,
                       Expression.Convert(valueParameter, propertyExpression.Type)
                   ),
                   objectParameter,
                   valueParameter
               );
        }

        //-
        public static ConcurrentDictionary<object, Func<object, object>> advancedPropertyGetters = new ConcurrentDictionary<object, Func<object, object>>();
        public static Func<object, object> GetGetterFunc(this Expression propertyExpression, ParameterExpression objectExpression, object cacheKey = null)
        {
            if (cacheKey == null)
                return propertyExpression.GetGetterLambda(objectExpression).Compile();
            else
                return advancedPropertyGetters.GetOrAdd(cacheKey, k =>
                {
                    return propertyExpression.GetGetterFunc(objectExpression);
                });
        }
        public static Expression<Func<object, object>> GetGetterLambda(this Expression propertyExpression, Expression objectExpression)
        {
            var objectParameter = Expression.Parameter(typeof(object), "p");
            propertyExpression = propertyExpression.Replace(x =>
            {
                if (x == objectExpression)
                    return Expression.Convert(objectParameter, x.Type);
                else
                    return null;
            });
            return Expression.Lambda<Func<object, object>>(
                       Expression.Convert(propertyExpression, typeof(object)),
                       objectParameter
               );
        }

        //-
        public static ConcurrentDictionary<PropertyInfo, Action<object, object>> propertySetters = new ConcurrentDictionary<PropertyInfo, Action<object, object>>();
        public static Action<object, object> GetSetterAction(this PropertyInfo property)
        {
            return propertySetters.GetOrAdd(property, p =>
            {
                return GetSetterLamba(property).Compile();
            });
        }
        public static Expression<Action<object, object>> GetSetterLamba(this PropertyInfo property)
        {
            var paramObject = Expression.Parameter(typeof(object), "p");
            var paramValue = Expression.Parameter(typeof(object), "v");
            return Expression.Lambda<Action<object, object>>(
                Expression.Assign(
                    Expression.Property(
                        Expression.Convert(paramObject, property.DeclaringType),
                        property
                    ),
                    Expression.Convert(paramValue, property.PropertyType)
                ),
                paramObject,
                paramValue
            );
        }

        //-
        public static ConcurrentDictionary<FieldInfo, Action<object, object>> fieldSetters = new ConcurrentDictionary<FieldInfo, Action<object, object>>();
        public static Action<object, object> GetSetterAction(this FieldInfo field)
        {
            return fieldSetters.GetOrAdd(field, p =>
            {
                return GetSetterLamba(field).Compile();
            });
        }
        public static Expression<Action<object, object>> GetSetterLamba(this FieldInfo field)
        {
            var paramObject = Expression.Parameter(typeof(object), "p");
            var paramValue = Expression.Parameter(typeof(object), "v");
            return Expression.Lambda<Action<object, object>>(
                Expression.Assign(
                    Expression.Field(
                        Expression.Convert(paramObject, field.DeclaringType),
                        field
                    ),
                    Expression.Convert(paramValue, field.FieldType)
                ),
                paramObject,
                paramValue
            );
        }

        public static Expression GetValueOrDefault(this Expression expression)
        {
            return Expression.Call(
                expression,
                expression.Type
                    .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .Single(m => m.Name == nameof(Nullable<int>.GetValueOrDefault) && m.GetParameters().Length == 0)
            );
        }
        public static Expression GetValueOrDefault(this Expression expression, Expression defaultValue)
        {
            return Expression.Call(
                expression,
                expression.Type
                    .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .Single(m => m.Name == nameof(Nullable<int>.GetValueOrDefault) && m.GetParameters().Length == 1),
                defaultValue
            );
        }

        public static LambdaExpression OutputRealType<T>(this Expression<Func<T, object>> expression)
        {
            if (expression.Body.NodeType == ExpressionType.Convert) //The actual lambda returned a specific - other - type
                return Expression.Lambda(((UnaryExpression)expression.Body).Operand, expression.Parameters.First());
            else
                return expression;
        }

        public static PropertyInfo GetPropertyInfo(this Expression property)
        {
            if (property is LambdaExpression)
                property = ((LambdaExpression)property).Body;
            if (property is UnaryExpression && ((UnaryExpression)property).NodeType == ExpressionType.Convert)
                property = ((UnaryExpression)property).Operand;
            if (property is MemberExpression)
                return (PropertyInfo)((MemberExpression)property).Member;
            else
                throw new ArgumentException($"{nameof(property)} must be a simple property getter.", nameof(property));
        }

        public static LambdaExpression LambdaFromPropertyPath(this string[] path, ParameterExpression param)
        {
            Expression current = param;
            foreach (var item in path)
                current = Expression.Property(current, item);
            return Expression.Lambda(current, param);
        }

        public static Expression<Action<Tobject, Tvalue>> GetterToSetter<Tobject, Tvalue>(this Expression<Func<Tobject, Tvalue>> getter)
        {
            var valueParam = Expression.Parameter(typeof(Tvalue), "v");
            return Expression.Lambda<Action<Tobject, Tvalue>>(
                Expression.Assign(getter.Body, valueParam),
                getter.Parameters.First(), valueParam);
        }

        //This is needed if we use a paramter Expression<Func<T, object>> but actually want an expression
        //returning the appropriate type
        public static LambdaExpression RemoveConvertToObject(this LambdaExpression expression)
        {
            if (expression.Body.NodeType == ExpressionType.Convert && ((UnaryExpression)expression.Body).Type == typeof(object))
                return Expression.Lambda(((UnaryExpression)expression.Body).Operand, expression.Parameters);
            else
                return expression;
        }
    }
}
