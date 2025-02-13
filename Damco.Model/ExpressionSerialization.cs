//Classes to facilidate serilization of Expression<Func<T, Tresult>>
//Not all functionality is included but most functionality that would be used in 
//queries to the database - OrderBy, Where, Select etc. - is.
//The serialization is done in two steps: first the expression is converted to an object 
//array, second the object array is converted to a string using the Json
//Any part of the expression that does not use the T input is compiled and executed an its 
//return value is serialized as a constant expression.

//TOOD: Use ExpressionVisitor for the difficult stuff.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Damco.Model;
using System.Collections;
using System.Data;
using System.Reflection;

namespace Damco.Model
{
    public static class ExpressionSerialization
    {
        //Serialization to object arrays (which in turn are serialized to Json):
        //Lambda: [ "L", <body>, [ <paramid>, <paramname>, <paramtype> ], [ <paramid>, <paramname>, <paramtype> ] ]
        //Parameter: [ "P", <paramid> ] (<paramid> is the one in the lambda - unique index)
        //Constant - null or complex: [ <type>, <value>] //The fact that the type is more than 1 char means it is a constant
        //Member from parameter: [ "PM", <paramindex>, <membername>, <type> ]
        //   Can also be x.GetInt32(x.GetOrdinal("FieldName"))  for IDataRecord
        //   or          x.GetValue<int>("Id")                  for DynamicEntity
        //Member from something else: [ "M", <source>, <membername> ]
        //Static member something else: [ "SM", <containingtype>, <membername> ]
        //Binary: [ "B", <left>, <method>, <right> ]
        //Instance method call: [ "C", <object>, <fullmethodname>, <parameter1>, <parameter2>, ... ]
        //Static method call: [ "SC", <fullmethodname>, <parameter1>, <parameter2>, ... ]
        //Unary - Convert: [ "UC", <operand>, <type> ]
        //Unary - Not: [ "UN", <operand> ]
        //Unary - Quote: [ "UQ", <operand> ]
        //Unary - others: [ "U", <nodetype>, <operand>, <type> ]
        //New - [ "N", <type>, constructorparam1, constructorparam2, ... ]
        //Memberinit - [ "MI", newexpression, [ member1, value1 ], [member2, value2] , ... ]
        //New array - [ "NA", <elementtype>, value1, value2, ... ]
        //Data request (root) - [ "DR", <elementtype>|<datasourceid>, <optional_serviceclass>,<optional_methodname> ]
        //Conditional: [ "CN", <test>, <iftrue>, <iffalse> ]

        //public static string RemoveConstants(string value, List<object> constants)
        //{
        //    foreach (var constant in constants)
        //        value = value.Replace(constant.ToJson(), "");
        //    return value;
        //}

        public static string SerializeToString<T>(this Expression<T> expression)
        {
            return expression.SerializeToObjectArray().ToJson();
        }
        public static string SerializeToString(this LambdaExpression expression)
        {
            return expression.SerializeToObjectArray().ToJson();
        }
        public static string SerializeToStringBatch(this LambdaExpression expression, int? Batch_skip = null, int? Batch_take = null)
        {
            return expression.SerializeToObjectArray(Batch_skip: Batch_skip, Batch_take: Batch_take).ToJson();
        }
        public static string SerializeToString(this Expression expression, params ParameterExpression[] knownParameters)
        {
            return expression.SerializeToObjectArray(knownParameters: knownParameters).ToJson();
        }
        public static Expression<T> DeserializeExpression<T>(this string expressionAsString)
        {
            return expressionAsString.FromJson<object[]>().DeserializeExpression<T>();
        }
        public static LambdaExpression DeserializeExpression(this string expressionAsString, params ParameterExpression[] parameters)
        {
            return expressionAsString.FromJson<object[]>().DeserializeExpression(parameters);
        }
        public static Expression DeserializeExpression(this string expressionAsString)
        {
            return expressionAsString.FromJson<object[]>().DeserializeExpression();
        }

        public static object[] SerializeToObjectArray<T>(this Expression<T> expression)
        {
            return SerializeToObjectArray((LambdaExpression)expression);
        }
        public static object[] SerializeToObjectArray(this LambdaExpression expression, int? Batch_skip = null, int? Batch_take = null)
        {
            return ExpressionToObjectArray(expression.Body, expression.Parameters.Select((p, i) => new { Param = p, Id = i }).ToDictionary(p => p.Param, p => p.Id), Batch_skip: Batch_skip, Batch_take: Batch_take);
        }
        public static object[] SerializeToObjectArray(this Expression expression, int? Batch_skip = null, int? Batch_take = null, params ParameterExpression[] knownParameters)
        {
            return ExpressionToObjectArray(expression, knownParameters.Select((p, i) => new { Param = p, Id = i }).ToDictionary(p => p.Param, p => p.Id), Batch_skip: Batch_skip, Batch_take: Batch_take);
        }
        public static Expression<T> DeserializeExpression<T>(this object[] expressionAsObjectArray)
        {
            if (expressionAsObjectArray == null)
                return null;
            var parameters = typeof(T).GetMethod("Invoke").GetParameters()
                .Select((p, i) => new { Type = p.ParameterType, Index = i })
                .ToDictionary(s => s.Index, s => Expression.Parameter(s.Type, $"p{s.Index:######}")); //Note 0 will just become "p"
            var thisLambdaParams = parameters.Values.ToList(); //Can change in ExpressionFromObjectArray call so remember here.
            return
                Expression.Lambda<T>(
                    ExpressionFromObjectArray(expressionAsObjectArray, parameters),
                    thisLambdaParams);
        }
        public static LambdaExpression DeserializeExpression(this object[] expressionAsObjectArray, params ParameterExpression[] parameters)
        {
            return
                Expression.Lambda(
                    ExpressionFromObjectArray(expressionAsObjectArray, parameters
                        .Select((p, i) => new { Param = p, Index = i })
                        .ToDictionary(s => s.Index, s => s.Param)
                    ),
                    parameters);
        }

        public static Expression DeserializeExpression(this object[] expressionAsObjectArray)
        {
            return ExpressionFromObjectArray(expressionAsObjectArray, new Dictionary<int, ParameterExpression>());
        }

        private class FindNonConstantsVisitor : ExpressionVisitor
        {
            public bool Found { get; private set; }
            Dictionary<ParameterExpression, int> _parameters;
            public FindNonConstantsVisitor(Dictionary<ParameterExpression, int> parameters)
            {
                _parameters = parameters;
            }
            protected override Expression VisitParameter(ParameterExpression node)
            {
                if (_parameters.ContainsKey(node))
                    this.Found = true;
                return base.VisitParameter(node);
            }
            protected override Expression VisitUnary(UnaryExpression node)
            {
                if (node.NodeType == ExpressionType.Quote)
                    this.Found = true;
                return base.VisitUnary(node);
            }
            protected override Expression VisitMember(MemberExpression node)
            {
                if (node.Member.DeclaringType == typeof(DateTime)
                    && (node.Member.Name == nameof(DateTime.UtcNow) || node.Member.Name == nameof(DateTime.Now)))
                    this.Found = true;
                else if (node.Member.DeclaringType == typeof(DataSourcing.RunInfo) && node.Member.Name == nameof(DataSourcing.RunInfo.Current))
                    this.Found = true;
                return base.VisitMember(node);
            }

            public override Expression Visit(Expression node)
            {
                if (this.Found)
                    return node; //No point in visiting more
                else
                    return base.Visit(node);
            }
        }

        private static bool CanBeConstant(Expression expression, Dictionary<ParameterExpression, int> parameters)
        {
            //An expression can be converted to a constant in the serialization
            //if it does not use a known parameter
            //and it does not use an Expression
            //in these cases it is likely data from the provider will be used
            //in other cases it is likely only local data is required
            var visitor = new FindNonConstantsVisitor(parameters);
            visitor.Visit(expression);
            return !visitor.Found;
        }

        private static object[] ExpressionToObjectArray(Expression expression, Dictionary<ParameterExpression, int> parameterIds, int? Batch_skip = null, int? Batch_take = null)
        {
            if (expression == null)
                return null;
            if (expression is LambdaExpression)
            {
                var exp = (LambdaExpression)expression;
                foreach (var param in exp.Parameters)
                    if (!parameterIds.ContainsKey(param))
                        parameterIds.Add(param, parameterIds.Values.DefaultIfEmpty(-1).Max() + 1);
                var result = new object[]
                {
                    "L",
                    ExpressionToObjectArray(exp.Body, parameterIds, Batch_skip: Batch_skip, Batch_take: Batch_take),
                }
                .Union(exp.Parameters.Select(p => new object[]
                    {
                        parameterIds[p],
                        p.Name,
                        GetNameForType(p.Type)
                    }
                ))
                .ToArray();
                return result;
            }

            else if (expression is ParameterExpression)
                return new object[] { "P", parameterIds[(ParameterExpression)expression] };

            else if (CanBeConstant(expression, parameterIds))
            {
                object objValue;
                if (expression.Type.Name == "ObjectQuery`1") //Will happen when LinqToEntitiesQuery gets the key for the cached query
                    objValue = null;
                else
                    objValue = Expression.Lambda(expression).Compile().DynamicInvoke();

                var dr = objValue as DataRequest;
                if (dr != null)
                {
                    List<object> drInfo = new List<object>();
                    drInfo.Add("DR");
                    if (dr.DataSourceId != null)
                        drInfo.Add(dr.DataSourceId);
                    else
                    {
                        drInfo.Add(dr.EntityName);
                        if (dr.CodeDataSourceTag != null)
                        {
                            drInfo.Add(dr.CodeDataSourceTag);
                            drInfo.Add(dr.CodeDataSourceParameters);
                        }
                    }
                    return drInfo.ToArray();
                }
                if (Batch_skip != null && Batch_take != null)
                {
                    var obj_enumerable = objValue as IEnumerable;
                    if (obj_enumerable != null)
                    {
                        List<Object> list = new List<Object>();

                        list.AddRange(obj_enumerable.Cast<Object>().OrderByDescending(s => s).Skip(Batch_skip ?? 0).Take(Batch_take ?? 0));
                        objValue = list.ToArray();
                    }

                }

                Type typ = expression.Type;
                bool blnIsArray = false;
                bool blnIsList = false;
                if (!typeof(string).IsAssignableFrom(typ) && typeof(IEnumerable).IsAssignableFrom(typ))
                {
                    if (typ.IsArray)
                    {
                        blnIsArray = true;
                        typ = typ.GetElementType();
                    }
                    else
                    {
                        blnIsList = true;
                        typ = typ.GetGenericArguments()[0];
                    }
                }
                string strTypeName = GetNameForType(typ);
                if (blnIsArray)
                    strTypeName = $"ArrayOf{strTypeName}";
                else if (blnIsList)
                    strTypeName = $"ListOf{strTypeName}";
                return new object[] { strTypeName, objValue };
            }
            else if (expression is BinaryExpression)
            {
                BinaryExpression exp = (BinaryExpression)expression;
                //string binaryType;
                //if (!_expressionTypeToString.TryGetValue(exp.NodeType, out binaryType))
                //  binaryType = exp.NodeType.ToString();
                return new object[] {
                    "B",
                    ExpressionToObjectArray(exp.Left, parameterIds, Batch_skip: Batch_skip, Batch_take: Batch_take),
                    Convert.ToInt32(exp.NodeType),
                    ExpressionToObjectArray(exp.Right, parameterIds, Batch_skip: Batch_skip, Batch_take: Batch_take)
                };
            }
            else if (expression is MethodCallExpression && ((MethodCallExpression)expression).Object != null) //Instance method call
            {
                MethodCallExpression exp = (MethodCallExpression)expression;

                if (exp.Object is ParameterExpression && exp.Object.Type == typeof(IDataRecord) && exp.Method.Name.StartsWith("Get"))
                {
                    //IDataRecord
                    //E.g. x.GetInt32(x.GetOrdinal("FieldName"));
                    //Treat this as a member epxression.
                    return new object[] {
                        "PM",
                        parameterIds[(ParameterExpression)exp.Object],
                        Expression.Lambda(((MethodCallExpression)exp.Arguments.First()).Arguments.First()).Compile().DynamicInvoke().ToString(),
                        GetNameForType(exp.Method.ReturnType)
                    };
                }

                if (exp.Object is ParameterExpression && exp.Object.Type == typeof(DynamicEntity) &&
                    exp.Method.IsGenericMethod && exp.Method.GetGenericMethodDefinition() == DynamicEntity.TypedGetValueByNameMethod)
                {
                    //DynamicEntiy
                    //E.g. x.GetValue<int>("FieldName")
                    //Treat this as a member expression
                    return new object[] {
                        "PM",
                        parameterIds[(ParameterExpression)exp.Object],
                        Expression.Lambda(exp.Arguments.First()).Compile().DynamicInvoke().ToString(),
                        GetNameForType(exp.Method.ReturnType)
                    };
                }

                return new object[]
                {
                    "C",
                    ExpressionToObjectArray(exp.Object, parameterIds, Batch_skip: Batch_skip, Batch_take: Batch_take),
                    GetFullMethodName(exp.Method)
                }.Union(exp.Arguments.Select(expr => (object)ExpressionToObjectArray(expr, parameterIds, Batch_skip: Batch_skip, Batch_take: Batch_take))
                ).ToArray();
            }
            else if (expression is MethodCallExpression && ((MethodCallExpression)expression).Object == null) //Static method
            {
                MethodCallExpression exp = (MethodCallExpression)expression;
                return new object[]
                {
                    "SC",
                    GetFullMethodName(exp.Method),
                }.Union(exp.Arguments.Select(expr => (object)ExpressionToObjectArray(expr, parameterIds, Batch_skip: Batch_skip, Batch_take: Batch_take))
                ).ToArray();
            }
            else if (expression is MemberExpression)
            {
                MemberExpression exp = (MemberExpression)expression;
                if (exp.Expression is ParameterExpression)
                    return new object[] {
                        "PM",
                        parameterIds[(ParameterExpression)exp.Expression],
                        exp.Member.Name,
                        GetNameForType(
                            exp.Member is PropertyInfo prop ? prop.PropertyType
                            : exp.Member is FieldInfo fld ? fld.FieldType
                            : throw new ArgumentException("Member is property nor field")
                        )
                    };
                else if (exp.Member is PropertyInfo && ((PropertyInfo)exp.Member).GetGetMethod().IsStatic)
                    return new object[] {
                        "SM",
                        GetNameForType(exp.Member.DeclaringType),
                        exp.Member.Name
                    };
                else
                    return new object[] {
                        "M",
                        ExpressionToObjectArray(exp.Expression, parameterIds, Batch_skip: Batch_skip, Batch_take: Batch_take),
                        exp.Member.Name
                    };
            }
            else if (expression is ParameterExpression)
                return new object[] { };
            else if (expression is UnaryExpression && ((UnaryExpression)expression).NodeType == ExpressionType.Convert)
            {
                UnaryExpression exp = (UnaryExpression)expression;

                if (exp.Operand is MethodCallExpression
                        && ((MethodCallExpression)exp.Operand).Object is ParameterExpression
                        && ((MethodCallExpression)exp.Operand).Object.Type == typeof(IDataRecord)
                        && ((MethodCallExpression)exp.Operand).Method == typeof(IDataRecord).GetMethod("GetValue"))
                {
                    var methodCall = ((MethodCallExpression)exp.Operand);
                    //E.g. (long?)x.GetValue(x.GetOrdinal("FieldName"));
                    //Treat this as a member epxression.
                    return new object[] {
                        "PM",
                        parameterIds[(ParameterExpression)((MethodCallExpression)exp.Operand).Object],
                        Expression.Lambda(((MethodCallExpression)methodCall.Arguments.First()).Arguments.First()).Compile().DynamicInvoke().ToString(),
                        GetNameForType(exp.Type)
                    };
                }
                return new object[] { "UC", ExpressionToObjectArray(exp.Operand, parameterIds, Batch_skip: Batch_skip, Batch_take: Batch_take), GetNameForType(exp.Type) };
            }
            else if (expression is UnaryExpression && ((UnaryExpression)expression).NodeType == ExpressionType.Not)
            {
                UnaryExpression exp = (UnaryExpression)expression;
                return new object[] { "UN", ExpressionToObjectArray(exp.Operand, parameterIds, Batch_skip: Batch_skip, Batch_take: Batch_take) };
            }
            else if (expression is UnaryExpression && ((UnaryExpression)expression).NodeType == ExpressionType.Quote)
            {
                UnaryExpression exp = (UnaryExpression)expression;
                return new object[] { "UQ", ExpressionToObjectArray(exp.Operand, parameterIds, Batch_skip: Batch_skip, Batch_take: Batch_take) };
            }
            else if (expression is UnaryExpression) //Other unary
            {
                UnaryExpression exp = (UnaryExpression)expression;
                return new object[] { "U", exp.NodeType, ExpressionToObjectArray(exp.Operand, parameterIds, Batch_skip: Batch_skip, Batch_take: Batch_take), GetNameForType(exp.Type) };
            }

            else if (expression is MemberInitExpression)
            {
                var exp = (MemberInitExpression)expression;
                return (new object[]
                    {
                        "MI",
                        ExpressionToObjectArray(exp.NewExpression, parameterIds, Batch_skip: Batch_skip, Batch_take: Batch_take)
                    })
                    .Union(exp.Bindings.Cast<MemberAssignment>().Select(b =>
                        new object[]
                        {
                            b.Member.Name,
                            ExpressionToObjectArray(b.Expression, parameterIds, Batch_skip: Batch_skip, Batch_take: Batch_take)
                        }
                    ))
                    .ToArray();
            }

            else if (expression is NewExpression)
            {
                var exp = (NewExpression)expression;
                return (new object[]
                    {
                        "N",
                        GetNameForType(exp.Constructor.DeclaringType),
                    })
                    .Union(exp.Arguments.Select(e => ExpressionToObjectArray(e, parameterIds, Batch_skip: Batch_skip, Batch_take: Batch_take)))
                    .ToArray();
            }

            else if (expression is NewArrayExpression)
            {
                var exp = (NewArrayExpression)expression;
                return
                    new object[] { "NA", GetNameForType(exp.Type.GetElementType()) } //Note we can't rely on getting the type from the parameters because the array might be empty.
                    .Union(exp.Expressions.Select(e => ExpressionToObjectArray(e, parameterIds, Batch_skip: Batch_skip, Batch_take: Batch_take)))
                    .ToArray();
            }

            else if (expression is ConditionalExpression)
            {
                var exp = (ConditionalExpression)expression;
                return
                    new object[] { "CN", ExpressionToObjectArray(exp.Test, parameterIds, Batch_skip: Batch_skip, Batch_take: Batch_take), ExpressionToObjectArray(exp.IfTrue, parameterIds), ExpressionToObjectArray(exp.IfFalse, parameterIds, Batch_skip: Batch_skip, Batch_take: Batch_take) };
            }

            else
                throw new NotSupportedException("Expression type " + expression.GetType().Name + " is not supported");
        }
        public static string GetNameForType(this Type type)
        {
            if (type == null) return null;
            else if (type == typeof(object)) return "object";
            else if (type == typeof(string)) return "string";
            else if (type == typeof(sbyte)) return "sbyte";
            else if (type == typeof(short)) return "short";
            else if (type == typeof(int)) return "int";
            else if (type == typeof(long)) return "long";
            else if (type == typeof(byte)) return "byte";
            else if (type == typeof(ushort)) return "ushort";
            else if (type == typeof(uint)) return "uint";
            else if (type == typeof(ulong)) return "ulong";
            else if (type == typeof(float)) return "float";
            else if (type == typeof(double)) return "double";
            else if (type == typeof(bool)) return "bool";
            else if (type == typeof(char)) return "char";
            else if (type == typeof(decimal)) return "decimal";
            else if (type == typeof(DateTime)) return "datetime";
            else if (type == typeof(System.Linq.Enumerable)) return "enumerable";
            else if (type == typeof(System.Linq.Queryable)) return "queryable";
            else
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(System.Nullable<>))
                    return GetNameForType(Nullable.GetUnderlyingType(type)) + "?";
                string typeName;
                if (type.Namespace == "System" && type.Assembly.GetName().Name != "mscorlib")
                    typeName = type.Name;
                else
                {
                    typeName = $"{type.Namespace}.{type.Name}";
                    if (type.Assembly.GetName().Name != type.Namespace) //We will need the assembly
                        typeName = $"{typeName},{type.Assembly.GetName().Name}";
                }
                if (type.IsGenericType)
                    typeName = $"{typeName}[{string.Join(";", type.GetGenericArguments().Select(a => GetNameForType(a)))}]";
                return typeName;
            }
        }

        public static Type GetTypeForName(this string typeName)
        {
            if (typeName == null) return null;
            else if (typeName == "object") return typeof(object);
            else if (typeName == "string") return typeof(string);
            else if (typeName == "sbyte") return typeof(sbyte);
            else if (typeName == "short") return typeof(short);
            else if (typeName == "int") return typeof(int);
            else if (typeName == "long") return typeof(long);
            else if (typeName == "byte") return typeof(byte);
            else if (typeName == "ushort") return typeof(ushort);
            else if (typeName == "uint") return typeof(uint);
            else if (typeName == "ulong") return typeof(ulong);
            else if (typeName == "float") return typeof(float);
            else if (typeName == "double") return typeof(double);
            else if (typeName == "bool") return typeof(bool);
            else if (typeName == "char") return typeof(char);
            else if (typeName == "decimal") return typeof(decimal);
            else if (typeName == "datetime") return typeof(DateTime);
            else if (typeName == "enumerable") return typeof(System.Linq.Enumerable);
            else if (typeName == "queryable") return typeof(System.Linq.Queryable);
            else
            {
                //Format: <namespace>.<typename>,<assembly>[<genericparameters>]
                //"<namespace>." , ",<assembly>" and "[<genericparameters>]" can all be omitted
                if (typeName.EndsWith("?"))
                    return typeof(System.Nullable<>).MakeGenericType(GetTypeForName(typeName.Substring(0, typeName.Length - "?".Length)));
                var startGenerics = typeName.IndexOf('[');
                if (startGenerics > -1 && typeName.Substring(startGenerics, 2) == "[]") //No generics - it's an array
                    startGenerics = -1;
                var startAssembly = typeName.IndexOf(',');
                if (startAssembly > startGenerics) //Assembly is of a generic type
                    startAssembly = -1;

                string nameSpaceAndType;
                if (startAssembly > -1) //Assembly is provided
                    nameSpaceAndType = typeName.Substring(0, startAssembly);
                else if (startGenerics > -1)
                    nameSpaceAndType = typeName.Substring(0, startGenerics);
                else
                    nameSpaceAndType = typeName;

                string assembly;
                if (!nameSpaceAndType.Contains(".")) //"System." type in mscorlib
                {
                    assembly = "mscorlib";
                    nameSpaceAndType = $"System.{typeName}";
                }
                else if (startAssembly > -1 && startGenerics > -1)
                    assembly = typeName.Substring(startAssembly + 1, startGenerics - 1 - startAssembly);
                else if (startAssembly > -1)
                    assembly = typeName.Substring(startAssembly);
                else //No assembly, assume namespace
                    assembly = nameSpaceAndType.Substring(0, nameSpaceAndType.LastIndexOf('.'));

                Type[] genericArguments;
                if (startGenerics > -1)
                {
                    genericArguments = typeName
                        .Substring(startGenerics + 1, typeName.Length - startGenerics - 2)
                        .Split(';')
                        .Select(t => GetTypeForName(t))
                        .ToArray();
                }
                else
                    genericArguments = null;

                Type result = Type.GetType(nameSpaceAndType); //Simple technique first
                if (result == null)
                {
                    var loadedAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == assembly);
                    if (loadedAssembly == null)
                        throw new InvalidOperationException($"Assembly {assembly} not loaded");
                    result = loadedAssembly.GetType(nameSpaceAndType, true);
                }

                if (genericArguments != null)
                    result = result.MakeGenericType(genericArguments);

                return result;
            }
        }

        private static string GetFullMethodName(MethodInfo method)
        {
            var result = new StringBuilder();
            result.Append(GetNameForType(method.DeclaringType));
            result.Append(".");
            result.Append(method.Name);
            if (!MethodSupportsGenericArgumentResolution(method))
                result
                    .Append("[")
                    .Append(string.Join(";", method.GetGenericArguments().Select(t => GetNameForType(t))))
                    .Append("]");
            return result.ToString();
        }

        private static bool MethodSupportsGenericArgumentResolution(MethodInfo methodInfo)
        {
            if (!methodInfo.IsGenericMethod)
                return true;
            foreach (Type genericArgument in methodInfo.GetGenericMethodDefinition().GetGenericArguments())
                if (!methodInfo.GetGenericMethodDefinition().GetParameters().Any(p => HasType(p.ParameterType, genericArgument)))
                    return false;
            return true;
        }

        private static bool HasType(Type type, Type findType)
        {
            if (type == findType)
                return true;
            else if (type.IsGenericType)
                return type.GetGenericArguments().Any(t => HasType(t, findType));
            else
                return false;
        }

        static MethodInfo _stringConcatMethod = ((BinaryExpression)((Expression<Func<string, string, string>>)((a, b) => a + b)).Body).Method;
        private static MethodInfo GetMethod(string fullMethodName, List<Type> parameterTypes)
        {
            Type[] genericArguments;
            string methodName;
            if (fullMethodName.EndsWith("]"))
            {
                //Generics provided
                genericArguments = fullMethodName
                        .Substring(fullMethodName.IndexOf("[") + 1, fullMethodName.Length - fullMethodName.IndexOf("[") - 2)
                        .Split(';')
                        .Select(t => GetTypeForName(t))
                        .ToArray();
                methodName = fullMethodName.Substring(0, fullMethodName.IndexOf("["));
            }
            else
            {
                genericArguments = null;
                methodName = fullMethodName;
            }
            Type declaringType = GetTypeForName(methodName.Substring(0, methodName.LastIndexOf(".")));
            methodName = methodName.Substring(methodName.LastIndexOf(".") + 1);
            //Find correct generic method
            var methods = declaringType.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance)
                .Where(m => m.Name == methodName)
                .Select(m => GetMethodToUse(m, parameterTypes, genericArguments))
                .Where(m => m != null)
                .ToList();
            if (!methods.Any())
                throw new ArgumentException($"Cannot find method '{fullMethodName}' with argument types '{string.Join("', '", parameterTypes.Select(x => x.ToString()))}'");
            else if (methods.Count() == 1)
                return methods.Single();
            else
            {
                return
                    methods.FirstOrDefault(x => x.GetParameters().Select((y, i) => y.ParameterType == parameterTypes[i]).All(b => b)) //Exact match - e.g. string.Contact(string[])
                    ?? methods.First();
            }
        }

        private static MethodInfo GetMethodToUse(MethodInfo method, List<Type> parameterTypes, Type[] knownGenericArguments)
        {
            if (method.Name == "Sum")
                "".ToString();

            //Do generic argument & overload resolution
            var methodParameters = method.GetParameters();
            if (methodParameters.Length != parameterTypes.Count)
                return null; //We won't be able to make this work

            if (method.Name == "Sum")
                "".ToString();

            //Link parameter types against method counterparts
            var parameters = parameterTypes.Zip(methodParameters, (r, m) => new { TypeInMethod = m.ParameterType, RequestedType = r });

            if (method.IsGenericMethodDefinition)
            {
                Dictionary<Type, Type> genericArguments;
                if (knownGenericArguments == null)
                    genericArguments = method.GetGenericArguments().ToDictionary(g => g, g => default(Type));
                else
                    genericArguments = method.GetGenericArguments().Zip(knownGenericArguments, (g, k) => new KeyValuePair<Type, Type>(g, k)).ToDictionary(kv => kv.Key, kv => kv.Value);
                bool failure = false;
                foreach (var param in parameters)
                {
                    if (!CanMatchTypes(param.TypeInMethod, param.RequestedType, genericArguments))
                        return null;
                }

                if (failure || genericArguments.Any(g => g.Value == null))
                    return null;
                else
                    return method.MakeGenericMethod(genericArguments.Values.ToArray());
            }
            else
            {
                if (parameters.All(p => p.TypeInMethod.IsAssignableFrom(p.RequestedType)))
                    return method;
                else
                    return null;
            }
        }

        private static bool CanMatchTypes(Type expectedType, Type availableType, Dictionary<Type, Type> genericArgumentSubstitutions)
        {
            if (genericArgumentSubstitutions.ContainsKey(expectedType))
            {
                if (genericArgumentSubstitutions[expectedType] == null)
                {
                    genericArgumentSubstitutions[expectedType] = availableType;
                    return true;
                }
                else
                    expectedType = genericArgumentSubstitutions[expectedType];
            }
            if (expectedType.IsAssignableFrom(availableType)) //Same thing, so will work
                return true;
            else if (expectedType.IsGenericType && availableType.IsGenericType
                    && expectedType.GetGenericTypeDefinition().IsAssignableFrom(availableType.GetGenericTypeDefinition())
                )
            {
                //Assuming expectedType has some replaceable type arguments, we might be able to generate the same thing
                return expectedType.GetGenericArguments()
                    .Zip(availableType.GetGenericArguments(), (e, a) => CanMatchTypes(e, a, genericArgumentSubstitutions))
                    .All(b => b); //All sub types match
            }
            else if (expectedType.IsGenericType //&& availableType.IsGenericType
                    && availableType.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == expectedType.GetGenericTypeDefinition())
                )
            {
                var interfaceToUse = availableType.GetInterfaces().First(i => i.IsGenericType && i.GetGenericTypeDefinition() == expectedType.GetGenericTypeDefinition());
                //Assuming expectedType has some replaceable type arguments, we might be able to generate the same thing
                return expectedType.GetGenericArguments()
                    .Zip(interfaceToUse.GetGenericArguments(), (e, a) => CanMatchTypes(e, a, genericArgumentSubstitutions))
                    .All(b => b); //All sub types match
            }
            else
                return false;
        }

        private static Expression ExpressionFromObjectArray(object[] value, Dictionary<int, ParameterExpression> parameters)
        {
            if (((string)value[0]).Length > 2) //No specification of the type of expression, so its a constant
            {
                string typeName = (string)value[0];
                bool blnArray = false;
                bool blnList = false;
                if (typeName.StartsWith("ArrayOf"))
                {
                    blnArray = true;
                    typeName = typeName.Substring("ArrayOf".Length);
                }
                else if (typeName.StartsWith("ListOf"))
                {
                    blnList = true;
                    typeName = typeName.Substring("ListOf".Length);
                }
                Type typ = GetTypeForName(typeName);
                if (blnArray)
                    typ = typ.MakeArrayType();
                else if (blnList)
                    typ = typeof(List<>).MakeGenericType(typ);
                object constValue = value[1];
                if (constValue == null)
                    return Expression.Constant(null, typ);
                else if (typ.IsGenericType && typ.GetGenericTypeDefinition() == typeof(List<>))
                {
                    IList constValueList = (IList)typ.GetConstructor(System.Type.EmptyTypes).Invoke(new object[] { });
                    foreach (object obj in (IEnumerable)constValue)
                        constValueList.Add(obj);
                    return Expression.Constant(constValueList, typ);
                }
                else if (typ.IsArray)
                {
                    ArrayList constValueList = new ArrayList(); ;
                    foreach (object obj in (IEnumerable)constValue)
                        constValueList.Add(obj);
                    return Expression.Constant(constValueList.ToArray(typ.GetElementType()), typ);
                }
                else if (constValue.GetType() == Nullable.GetUnderlyingType(typ))
                    return Expression.Constant(constValue, typ);
                else
                    return Expression.Constant(ChangeType(constValue, typ), typ);
            }
            else if ((string)value[0] == "DR") //DataRequest
            {
                if (value[1] is int)
                    return Expression.Constant(DataRequest.Get((int)value[1]), typeof(IQueryable<DynamicEntity>));
                else
                {
                    var elementType = GetTypeForName((string)value[1]);
                    if (value.Length > 2)
                    {
                        //Code data source
                        return Expression.Constant(
                            typeof(DataRequest).GetMethod(nameof(DataRequest.Get), new Type[] { typeof(string), typeof(string) })
                                .MakeGenericMethod(elementType)
                                .Invoke(null, new object[] { value[2], (Dictionary<string, object>)value[3] }),
                            typeof(IQueryable<>).MakeGenericType(elementType)
                        );
                    }
                    else
                    {
                        return Expression.Constant(
                            typeof(DataRequest).GetMethod(nameof(DataRequest.Get), new Type[] { })
                                .MakeGenericMethod(elementType)
                                .Invoke(null, new object[] { }),
                            typeof(IQueryable<>).MakeGenericType(elementType)
                        );
                    }
                }
            }
            else if ((string)value[0] == "L") //Lambda
            {
                var thisLambdaParams = new List<ParameterExpression>();
                for (int i = 2; i < value.Length; i++)
                {
                    var paramInfo = (object[])value[i];
                    var param = Expression.Parameter(GetTypeForName((string)paramInfo[2]), (string)paramInfo[1]);
                    thisLambdaParams.Add(param);
                    parameters.Add((int)paramInfo[0], param);
                }
                return Expression.Lambda(ExpressionFromObjectArray((object[])value[1], parameters), thisLambdaParams);
            }
            else if ((string)value[0] == "P") //Parameter
                return parameters[(int)value[1]];
            else if ((string)value[0] == "B") //Binary
            {
                //ExpressionType binaryType;
                //if (!_stringToExpressionType.TryGetValue((string)value[2], out binaryType))
                //  binaryType = (ExpressionType)System.Enum.Parse(typeof(ExpressionType), (string)value[2], true);
                var expType = (ExpressionType)System.Enum.ToObject(typeof(ExpressionType), (int)value[2]);
                var left = ExpressionFromObjectArray((object[])value[1], parameters);
                var right = ExpressionFromObjectArray((object[])value[3], parameters);
                if (expType == ExpressionType.Add && left.Type == typeof(string) && right.Type == typeof(string))
                    //Concat
                    return Expression.MakeBinary(expType, left, right, false, _stringConcatMethod);
                else
                    return Expression.MakeBinary(expType, left, right);
            }
            else if ((string)value[0] == "SM")
            {
                return Expression.Property(null,
                    GetTypeForName((string)value[1]).GetProperty((string)value[2])
                );
            }
            else if ((string)value[0] == "M")
            {
                return Expression.PropertyOrField(
                    ExpressionFromObjectArray((object[])value[1], parameters),
                    (string)value[2]);
            }
            else if ((string)value[0] == "PM")
            {
                var param = parameters[(int)value[1]];
                if (typeof(IDataRecord).IsAssignableFrom(param.Type))
                {
                    var getMethod = typeof(System.Data.IDataRecord).GetMethod("Get" + GetTypeForName(value[3].ToString()).Name);
                    Type conversionType;
                    if (getMethod == null)
                    {
                        getMethod = typeof(System.Data.IDataRecord).GetMethod("GetValue");
                        conversionType = GetTypeForName(value[3].ToString());
                        if (conversionType == typeof(System.Object))
                            conversionType = null;
                    }
                    else
                        conversionType = null;
                    var getValueExpression = Expression.Call(
                        param,
                        getMethod,
                        Expression.Call(
                            param,
                            typeof(IDataRecord).GetMethod("GetOrdinal"),
                            Expression.Constant(value[2].ToString())
                        )
                    );
                    if (conversionType != null)
                        return Expression.Convert(getValueExpression, conversionType);
                    else
                        return getValueExpression;
                }
                else if (typeof(DynamicEntity).IsAssignableFrom(param.Type))
                    return DynamicEntity.GetValueGetter(param, value[2].ToString(), GetTypeForName(value[3].ToString()));
                else //Basic property or field
                    return Expression.PropertyOrField(param, (string)value[2]);
            }
            else if ((string)value[0] == "C")
            {
                List<Expression> lstArguments =
                    value.Skip(3).Cast<object[]>()
                    .Select(objarr => ExpressionFromObjectArray(objarr, parameters))
                    .ToList();
                Expression instance = ExpressionFromObjectArray((object[])value[1], parameters);
                return Expression.Call(
                    instance,
                    GetMethod((string)value[2], lstArguments.Select(arg => arg.Type).ToList()),
                    lstArguments.ToArray()
                );
            }
            else if ((string)value[0] == "SC") //Static call
            {
                List<Expression> lstArguments =
                    value.Skip(2).Cast<object[]>()
                    .Select(objarr => ExpressionFromObjectArray(objarr, parameters))
                    .ToList();
                return Expression.Call(
                    GetMethod((string)value[1], lstArguments.Select(arg => arg.Type).ToList()),
                    lstArguments.ToArray()
                );
            }
            else if ((string)value[0] == "UC")
                return Expression.Convert(ExpressionFromObjectArray((object[])value[1], parameters), GetTypeForName(value[2].ToString()));
            else if ((string)value[0] == "UN")
                return Expression.Not(ExpressionFromObjectArray((object[])value[1], parameters));
            else if ((string)value[0] == "UQ")
                return Expression.Quote(ExpressionFromObjectArray((object[])value[1], parameters));
            else if ((string)value[0] == "U")
                return Expression.MakeUnary((ExpressionType)value[1], ExpressionFromObjectArray((object[])value[2], parameters), GetTypeForName((string)value[3]));
            else if ((string)value[0] == "MI")
            {
                NewExpression newExpression = (NewExpression)ExpressionFromObjectArray((object[])value[1], parameters);
                return Expression.MemberInit(
                    newExpression,
                    value.Skip(2).Cast<object[]>()
                    .Select(b => Expression.Bind(
                        newExpression.Type.GetMember((string)b[0]).First(),
                        ExpressionFromObjectArray((object[])b[1], parameters)
                    ))
                );
            }
            else if ((string)value[0] == "N")
            {
                List<Expression> newParameters = value.Skip(2).Cast<object[]>().Select(a => ExpressionFromObjectArray(a, parameters)).ToList();
                return Expression.New(
                    GetTypeForName((string)value[1]).GetConstructor(newParameters.Select(p => p.Type).ToArray()),
                    newParameters
                );
            }

            else if ((string)value[0] == "NA")
                return Expression.NewArrayInit(
                    GetTypeForName((string)value[1]),
                    value.Skip(2).Cast<object[]>().Select(a => ExpressionFromObjectArray(a, parameters))
                );

            else if ((string)value[0] == "CN")
            {
                return Expression.Condition(
                    ExpressionFromObjectArray((object[])value[1], parameters),
                    ExpressionFromObjectArray((object[])value[2], parameters),
                    ExpressionFromObjectArray((object[])value[3], parameters)
                );
            }

            else
                throw new ArgumentException("Invalid data found in object array", "value");
        }

        public static object ChangeType(object value, Type conversionType)
        {
            //Fix issues that the normal Convert.ChangeType has with nullables
            if (Nullable.GetUnderlyingType(conversionType) != null) //it is a nullable type
            {
                if (value == null)
                    return null;
                else
                    conversionType = Nullable.GetUnderlyingType(conversionType);
            }
            if (conversionType.IsEnum && value is int)
                return Enum.ToObject(conversionType, value);
            else
                return Convert.ChangeType(value, conversionType);
        }

    }

}
