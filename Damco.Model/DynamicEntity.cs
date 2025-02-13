using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
//using System.Runtime.Remoting.Contexts;
using System.Runtime.Serialization;

namespace Damco.Model
{
    public class NamedExpression
    {
        public NamedExpression(string name, Expression expression)
        {
            this.Name = name;
            this.Expression = expression;
        }
        public string Name { get; set; }
        public Expression Expression { get; set; }
    }

    [Serializable()]
    [JsonConverter(typeof(DynamicEntityJsonConverter))]
    public class DynamicEntity
    {

        public bool CreateMissingProperties { get; set; }

        //public virtual void GetObjectData(SerializationInfo info, StreamingContext Context)
        //{
        //    // Use the AddValue method to specify serialized values.
        //    info.AddValue("Values", this.Values, typeof(object[]));
        //    info.AddValue("PropertyMapping", this.PropertyMapping, typeof(Dictionary<string, int>));
        //}

        //public DynamicEntity(SerializationInfo info, StreamingContext context)
        //    : this(
        //        (Dictionary<string, int>)info.GetValue("PropertyMapping", typeof(Dictionary<string, int>)),
        //        (object[])info.GetValue("Values", typeof(object[]))
        //    )
        //{
        //}

        #region Constructors
        public DynamicEntity()
        {
        }
        public DynamicEntity(params object[] values)
        {
            this.Values = values;
        }

        public DynamicEntity(IDictionary<string, int> propertyMapping, params object[] values) : this(values)
        {
            this.PropertyMapping = propertyMapping;
        }

        public DynamicEntity(string[] names, object[] values) :
            this(names.Select((n, i) => new KeyValuePair<string, int>(n, i)).ToDictionary(k => k.Key, k => k.Value), values)
        {
        }
        #endregion

        #region Static helpers
        public static Expression<Func<T, DynamicEntity>> GetConstructorLambda<T>()
        {
            var parameter = Expression.Parameter(typeof(T), "p");
            return Expression.Lambda<Func<T, DynamicEntity>>(GetConstructor(typeof(T).GetProperties().Select(p => new NamedExpression(p.Name, Expression.Property(parameter, p)))), parameter);
        }

        public static MethodInfo TypedGetValueByNameMethod { get; } = typeof(DynamicEntity).GetMethods().First(m => m.Name == "GetValue" && m.IsGenericMethod && m.GetParameters().First().ParameterType == typeof(string));
        public static MethodInfo TypedGetValueByIndexMethod { get; } = typeof(DynamicEntity).GetMethods().First(m => m.Name == "GetValue" && m.IsGenericMethod && m.GetParameters().First().ParameterType == typeof(int));

        public static Expression<Func<T, DynamicEntity>> GetConstructorLambda<T>(ParameterExpression inputParameter, params Expression[] values)
        {
            return GetConstructorLambda<T>(inputParameter, (IEnumerable<Expression>)values);
        }

        public static Expression GetValueGetter(Expression dynamicEntityExpression, string name, Type dataType)
        {
            return Expression.Call(dynamicEntityExpression, TypedGetValueByNameMethod.MakeGenericMethod(dataType), Expression.Constant(name));
        }

        public static Expression GetValueGetter(Expression dynamicEntityExpression, int index, Type dataType)
        {
            return Expression.Call(dynamicEntityExpression, TypedGetValueByIndexMethod.MakeGenericMethod(dataType), Expression.Constant(index));
        }

        public static Expression<Func<DynamicEntity, T>> GetValueGetterLambda<T>(string name)
        {
            return GetValueGetterLambda<T>(Expression.Parameter(typeof(DynamicEntity), "p"), name);
        }

        public static Expression<Func<DynamicEntity, T>> GetValueGetterLambda<T>(ParameterExpression parameter, string name)
        {
            return Expression.Lambda<Func<DynamicEntity, T>>(GetValueGetter(parameter, name, typeof(T)), parameter);
        }

        public static LambdaExpression GetValueGetterLambda(string name, Type dataType)
        {
            return GetValueGetterLambda(Expression.Parameter(typeof(DynamicEntity), "p"), name, dataType);
        }
        public static LambdaExpression GetValueGetterLambda(ParameterExpression parameter, string name, Type dataType)
        {
            return Expression.Lambda(GetValueGetter(parameter, name, dataType), parameter);
        }

        public static Expression<Func<Tinput, DynamicEntity>> GetConstructorLambda<Tinput>(ParameterExpression inputParameter, IEnumerable<Expression> values)
        {
            return Expression.Lambda<Func<Tinput, DynamicEntity>>(GetConstructor(values), inputParameter);
        }
        public static Expression<Func<Tinput, DynamicEntity>> GetConstructorLambda<Tinput>(ParameterExpression inputParameter, params NamedExpression[] values)
        {
            return GetConstructorLambda<Tinput>(inputParameter, (IEnumerable<NamedExpression>)values);
        }
        public static Expression<Func<Tinput, DynamicEntity>> GetConstructorLambda<Tinput>(ParameterExpression inputParameter, IEnumerable<NamedExpression> values)
        {
            return Expression.Lambda<Func<Tinput, DynamicEntity>>(GetConstructor(values), inputParameter);
        }


        //public static Expression<Func<T, DynamicEntity>> GetConstructorLambda<T>(ParameterExpression inputParameter, IEnumerable<Expression> values)
        //{
        //    return Expression.Lambda<Func<T, DynamicEntity>>(GetConstructor(values), inputParameter);
        //}

        //public static Expression GetConstructor<T>(Expression input)
        //{
        //    return GetConstructor(typeof(T).GetProperties().Select(p => Expression.Property(input, p)));
        //}

        //public static Expression GetConstructor(params Expression[] values)
        //{
        //    return GetConstructor((IEnumerable<Expression>)values);
        //}

        public static Expression GetConstructor(params Expression[] values)
        {
            return GetConstructor((IEnumerable<Expression>)values);
        }

        public static Expression GetConstructor(IEnumerable<Expression> values)
        {
            return Expression.New(
                typeof(DynamicEntity).GetConstructor(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance, Type.DefaultBinder, new Type[] { typeof(object[]) }, null),
                Expression.NewArrayInit(typeof(object), values.Select(v => Expression.Convert(v, typeof(object))))
            );
        }
        public static Expression GetConstructor(IEnumerable<NamedExpression> values)
        {
            if (values.Any(n => n.Expression is LambdaExpression))
                throw new ArgumentException($"'{nameof(values)}' should not have LambdaExpressions. Please use the bodies of the expressions instead (using the same parameters for each expression)", nameof(values));
            return Expression.New(
                typeof(DynamicEntity).GetConstructor(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance, Type.DefaultBinder, new Type[] { typeof(string[]), typeof(object[]) }, null),
                Expression.NewArrayInit(typeof(string), values.Select(v => Expression.Constant(v.Name))),
                Expression.NewArrayInit(typeof(object), values.Select(v => Expression.Convert(v.Expression, typeof(object))))
            );
        }


        #endregion

        #region Propeties
        public IDictionary<string, int> PropertyMapping { get; set; }
        public object[] Values { get; set; }
        #endregion

        #region Instance methods

        public Expression<Func<T, bool>> ToPredicate<T>()
        {
            if (typeof(T) == typeof(DynamicEntity))
                return ToPredicate<T>((f, t, p) => DynamicEntity.GetValueGetter(p, f, t));
            else
                return ToPredicate<T>((f, t, p) => Expression.PropertyOrField(p, f));
        }

        public Expression<Func<T, bool>> ToPredicate<T>(Func<string, Type, ParameterExpression, Expression> fieldExpressionBuilder)
        {
            return (Expression<Func<T, bool>>)ToPredicate(fieldExpressionBuilder, Expression.Parameter(typeof(T), "p"));
        }

        public LambdaExpression ToPredicate(Func<string, Type, ParameterExpression, Expression> fieldExpressionBuilder, ParameterExpression entityParameter)
        {
            if (this.PropertyMapping == null)
                throw new InvalidOperationException("This method requires a property mapping");
            Expression predicate = null;
            foreach (var linkValue in this.AllProperties())
            {
                //var thisField = DynamicEntity.GetValueGetter(param, linkValue.Key, linkValue.Value == null ? typeof(object) : linkValue.Value.GetType());
                var thisField = fieldExpressionBuilder(linkValue.Key, linkValue.Value == null ? typeof(object) : linkValue.Value.GetType(), entityParameter);
                Expression predicateThisField;
                if (thisField.Type.IsGenericType && (thisField.Type.GetGenericTypeDefinition() == typeof(IEnumerable<>) || thisField.Type.GetGenericTypeDefinition() == typeof(List<>)))
                {
                    //"Contains"
                    var elementType = thisField.Type.GetGenericArguments().First();
                    predicateThisField = Expression.Call(
                        typeof(Enumerable).GetMethods(BindingFlags.Public | BindingFlags.Static)
                            .Single(m => m.Name == "Contains" && m.IsGenericMethodDefinition && m.GetGenericArguments().Length == 1 && m.GetParameters().Length == 2)
                            .MakeGenericMethod(elementType),
                        thisField,
                        Expression.Constant(ExpressionSerialization.ChangeType(linkValue.Value, elementType), elementType)
                    );
                }
                else
                    predicateThisField = Expression.Equal(thisField, Expression.Constant(ExpressionSerialization.ChangeType(linkValue.Value, thisField.Type), thisField.Type));
                if (predicate == null)
                    predicate = predicateThisField;
                else
                    predicate = Expression.AndAlso(predicate, predicateThisField);
            }
            if (predicate == null)
                predicate = Expression.Constant(true, typeof(bool));
            return Expression.Lambda(predicate, entityParameter);
        }


        public IEnumerable<KeyValuePair<string, object>> AllProperties()
        {
            if (this.PropertyMapping == null)
            {
                for (int f = 0; f < this.FieldCount; f++)
                    yield return new KeyValuePair<string, object>("", this.Values[f]);
            }
            else
            {
                foreach (var field in this.PropertyMapping)
                    yield return new KeyValuePair<string, object>(field.Key, this.Values[field.Value]);
            }
        }

        public void RemoveField(string name)
        {
            this.SetValue<object>(name, null);
            this.PropertyMapping.Remove(name);
        }

        public void RemoveField(int index)
        {
            if (this.Values.Length == (index + 1)) //Last field
            {
                var arr = this.Values;
                Array.Resize(ref arr, index);
                this.Values = arr;
            }
            else
                this.SetValue<object>(index, null);
        }

        public DynamicEntity SetValue<T>(int index, T value)
        {
            if (this.Values == null)
                this.Values = new object[index + 1];

            if (index >= this.Values.Length)
            {
                var array = this.Values;
                Array.Resize(ref array, index + 1);
                this.Values = array;
            }

            //Decimals in our database are up to 20 digits after point. To properly show them on the UI we have convert them from decimal type to string.
            //Casting from T to Decimal and using ToString with provided format allows us to make sure that given decimal will be properly shown on the UI
            if (value != null && value.GetType() == typeof(decimal))
            {
                this.Values[index] = ((decimal)(value as object)).ToString("0.####################");
            }
            else
            {
                this.Values[index] = value;
            }

            return this;
        }

        public T GetValue<T>(int index)
        {
            try
            {
                var result = this.GetValue(index);
                if (result is T)
                    return (T)result;
                else
                    return (T)ChangeType(result, typeof(T));
            }
            catch (Exception ex)
            {
                ex.Data["GetValue<T>-index"] = index;
                throw;
            }
        }

        private static object ChangeType(object value, Type conversionType)
        {
            //Fix issues that the normal Convert.ChangeType has with nullables
            if (Nullable.GetUnderlyingType(conversionType) != null) //it is a nullable type
            {
                if (value == null)
                    return null;
                else
                    conversionType = Nullable.GetUnderlyingType(conversionType);
            }
            if (conversionType.IsEnum)
            {
                if (value is string)
                    return Enum.Parse(conversionType, (string)value);
                else
                    return Enum.ToObject(conversionType, ChangeType(value, conversionType.GetEnumUnderlyingType()));
            }
            else if (conversionType != null && value != null && conversionType.IsAssignableFrom(value.GetType()))
                return value;
            else if (conversionType.IsArray && value is IEnumerable)
            {
                var arrayType = conversionType.GetElementType();
                var result = new ArrayList();
                foreach (var elementValue in ((IEnumerable)value))
                    result.Add(ChangeType(elementValue, arrayType));
                return result.ToArray(arrayType);
            }
            else
                return Convert.ChangeType(value, conversionType);
        }

        public int GetIndex<T>(string name)
        {
            if (this.PropertyMapping == null)
                throw new InvalidOperationException("Property mapping is not known - getting values by name is not supported");
            int index;
            if (!this.PropertyMapping.TryGetValue(name, out index))
            {
                if (this.CreateMissingProperties)
                {
                    index = this.Values.Length;
                    this.PropertyMapping[name] = index;
                    this.SetValue<T>(index, default(T));
                }
                else
                    throw new ArgumentException($"Name '{name}' is not mapped");
            }
            return index;
        }

        private int GetIndexOrMinusOne(string name)
        {
            if (this.PropertyMapping == null)
                throw new InvalidOperationException("Property mapping is not known - getting values by name is not supported");
            int index;
            if (!this.PropertyMapping.TryGetValue(name, out index))
                return -1;
            return index;
        }

        public DynamicEntity SetValue<T>(string name, T value)
        {
            if (this.PropertyMapping == null)
                this.PropertyMapping = new Dictionary<string, int>();
            int index = GetIndexOrMinusOne(name);
            if (index == -1)
            {
                index = this.PropertyMapping.Values.DefaultIfEmpty().Max() + 1;
                this.PropertyMapping[name] = index;
            }
            return SetValue<T>(index, value);
        }

        static MethodInfo _getValueByNameMethod = typeof(DynamicEntity)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Single(m =>
                m.Name == "GetValue"
                && m.IsGenericMethodDefinition
                && m.GetGenericArguments().Length == 1
                && m.GetParameters().Length == 1
                && m.GetParameters().First().ParameterType == typeof(string)
            );
        public T GetValue<T>(string name)
        {
            try
            {
                return GetValue<T>(GetIndex<T>(name));
            }
            catch (Exception ex)
            {
                ex.Data["GetValue<T>-name"] = name;
                throw;
            }
        }
        public object GetValue(string name)
        {
            return GetValue(GetIndex<object>(name));
        }

        public T GetValueOrDefault<T>(string name, T defaultValue)
        {
            try
            {
                int index = GetIndexOrMinusOne(name);
                if (index == -1)
                    return defaultValue;
                else
                    return GetValue<T>(GetIndex<object>(name));
            }
            catch (Exception ex)
            {
                ex.Data[$"{nameof(GetValueOrDefault)}<T>-{nameof(name)}"] = name;
                throw;
            }
        }
        public object GetValueOrDefault(string name, object defaultValue)
        {
            try
            {
                int index = GetIndexOrMinusOne(name);
                if (index == -1)
                    return defaultValue;
                else
                    return GetValue(GetIndex<object>(name));
            }
            catch (Exception ex)
            {
                ex.Data[$"{nameof(GetValueOrDefault)}-{nameof(name)}"] = name;
                throw;
            }
        }
        public T GetValueOrDefault<T>(string name)
        {
            return GetValueOrDefault<T>(name, default(T));
        }
        public object GetValueOrDefault(string name)
        {
            return GetValueOrDefault<object>(name, default(object));
        }

        public object GetValue(int index)
        {
            return this.Values[index];
        }

        public int FieldCount { get { return this.Values?.Length ?? 0; } }

        public override string ToString()
        {
            if (this.Values == null || this.Values.Length == 0)
                return "<empty>";
            else
                return string.Join(", ", this.Values);
        }
        #endregion

        #region Equality on value
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj.GetType() != this.GetType()) //Not a DynamicEntity or incorrect generic params
                return false;
            DynamicEntity other = ((DynamicEntity)obj);
            if (this.Values == null && other.Values == null)
                return true;
            if (this.Values == null || other.Values == null)
                return false;
            if (other.Values.Length != this.Values.Length)
                return false;
            for (int i = 0; i < this.Values.Length; i++)
                if (!object.Equals(this.Values[i], other.Values[i]))
                    return false;
            return true;
        }

        public bool EqualsOn(DynamicEntity other, IEnumerable<string> fieldNames)
        {
            if (other == null)
                return false;
            if (this.Values == null && other.Values == null)
                return true;
            if (this.Values == null || other.Values == null)
                return false;
            foreach (var fieldName in fieldNames)
                if (!object.Equals(this.GetValue(fieldName), other.GetValue(fieldName)))
                    return false;
            return true;
        }

        public override int GetHashCode()
        {
            if (this.Values == null || this.Values.Length == 0)
                return 0;
            int result = 0;
            foreach (object value in this.Values)
                result ^= (value?.GetHashCode() ?? 0);
            return result;
        }

        public static string GetFieldName(Expression expression)
        {
            if (expression is LambdaExpression)
                expression = ((LambdaExpression)expression).Body;

            var methodCall = expression as MethodCallExpression;
            if (methodCall != null
                && methodCall.Method.IsGenericMethod
                && methodCall.Method.GetGenericMethodDefinition() == _getValueByNameMethod)
            {
                var fieldNameExpression = methodCall.Arguments.First();
                if (fieldNameExpression is ConstantExpression)
                    return ((ConstantExpression)fieldNameExpression).Value?.ToString();
                else
                    return Expression.Lambda(fieldNameExpression).Compile().DynamicInvoke()?.ToString();
            }
            else
                return null;
        }

        #endregion
    }

}