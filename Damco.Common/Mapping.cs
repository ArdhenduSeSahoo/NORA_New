using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Common
{
    public static class Mapping
    {
        public static bool IsRequired(this PropertyInfo property)
        {
            return
                    (property.PropertyType.IsValueType && Nullable.GetUnderlyingType(property.PropertyType) == null)
                    || property.GetCustomAttribute<RequiredAttribute>(true) != null;
        }


        public static bool IsSimpleType(this Type type)
        {
            if (
                type == typeof(string)
                || type == typeof(sbyte)
                || type == typeof(short)
                || type == typeof(int)
                || type == typeof(long)
                || type == typeof(byte)
                || type == typeof(ushort)
                || type == typeof(uint)
                || type == typeof(ulong)
                || type == typeof(float)
                || type == typeof(double)
                || type == typeof(bool)
                || type == typeof(char)
                || type == typeof(decimal)
                || type == typeof(DateTime)
                || type.IsEnum
                || (Nullable.GetUnderlyingType(type) != null && IsSimpleType(Nullable.GetUnderlyingType(type)))
            )
                return true;
            else
                return false;
        }

        //Abandoned for now due to strange error
        //public static Func<Tin, Tout> GetCreatorAndMapper<Tin, Tout>()
        //    where Tout : new()
        //{
        //    ParameterExpression input = Expression.Parameter(typeof(Tin), "i");
        //    var expression = Expression.Lambda<Func<Tin, Tout>>(
        //        Expression.MemberInit(
        //            Expression.New(typeof(Tout)),
        //            GetPropertyMappings(typeof(Tin), typeof(Tout))
        //            .Select(m => Expression.Bind(m.Item2, Expression.Property(input, m.Item1)))
        //        ), input);
        //    return expression.Compile();
        //}

        public static Func<T, T, T> GetMapper<T>(string[] excludedProperties = null)
        {
            return GetMapper<T, T>(excludedProperties);
        }

        public static Func<Tin, Tout, Tout> GetMapper<Tin, Tout>(string[] excludedProperties = null)
        {
            ParameterExpression input = Expression.Parameter(typeof(Tin), "i");
            ParameterExpression output = Expression.Parameter(typeof(Tout), "o");
            return Expression.Lambda<Func<Tin, Tout, Tout>>(
                Expression.Block(
                    GetPropertyMappings(input.Type, output.Type, excludedProperties)
                    .Select(m => (Expression)Expression.Assign(
                        Expression.Property(output, m.Item2),
                        Expression.Convert(Expression.Property(input, m.Item1), m.Item2.PropertyType)
                    ))
                    .UnionSome((Expression)output)
                ), input, output).Compile();
        }

        private static IEnumerable<Tuple<PropertyInfo, PropertyInfo>> GetPropertyMappings(Type inputType, Type outputType, string[] excludedProperties = null)
        {
            return (
                from o in outputType.GetProperties().Where(p => IsSimpleType(p.PropertyType) && (excludedProperties == null || !excludedProperties.Contains(p.Name)))
                join i in inputType.GetProperties().Where(p => IsSimpleType(p.PropertyType) && (excludedProperties == null || !excludedProperties.Contains(p.Name)))
                    on o.Name.ToLower() equals i.Name.ToLower()
                select Tuple.Create(i, o)
            );
        }

        static MethodInfo _enumerableContainsOfStringMethod = typeof(Enumerable).GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Single(m => m.Name == "Contains" && m.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(string));
        public static Func<Tin, Tout, IEnumerable<string>, Tout> GetConditionalMapper<Tin, Tout>()
        {
            ParameterExpression input = Expression.Parameter(typeof(Tin), "i");
            ParameterExpression properties = Expression.Parameter(typeof(IEnumerable<string>), "p");
            ParameterExpression output = Expression.Parameter(typeof(Tout), "o");

            return Expression.Lambda<Func<Tin, Tout, IEnumerable<string>, Tout>>(
                Expression.Block(
                    GetPropertyMappings(input.Type, output.Type)
                    .Select(m => Expression.IfThen(
                        Expression.Or(
                            Expression.Equal(properties, Expression.Constant(null, typeof(IEnumerable<string>))),
                            Expression.Call(
                                _enumerableContainsOfStringMethod,
                                properties,
                                Expression.Constant(m.Item1.Name)
                            )
                        ),
                        (Expression)Expression.Assign(
                            Expression.Property(output, m.Item2),
                            Expression.Convert(Expression.Property(input, m.Item1), m.Item2.PropertyType)
                        )
                    ))
                    .UnionSome((Expression)output)
                ), input, output, properties).Compile();
        }
    }
}
