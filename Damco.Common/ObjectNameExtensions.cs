using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Damco.Common
{
    public static class ObjectNameExtensions
    {
        public static string GetPropertyName<TObject>(this TObject value, Expression<Func<TObject, object>> propertyExpression)
        {
            return propertyExpression.GetPropertyName();
        }

        public static string GetPropertyName<TObject>(this Expression<Func<TObject, object>> propertyExpression)
        {
            return (propertyExpression as LambdaExpression).GetPropertyName();
        }

        public static string GetPropertyName(this LambdaExpression propertyExpression)
        {
            if (propertyExpression == null)
            {
                throw new ArgumentException(@"The parameter should be added", "propertyExpression");
            }

            var unaryExpression = propertyExpression.Body as UnaryExpression;
            var memberExpression =
                (unaryExpression == null ? propertyExpression.Body : unaryExpression.Operand) as MemberExpression;

            if (memberExpression == null)
            {
                throw new ArgumentException(@"The lambda expression should be set to proper value", "propertyExpression");
            }

            var name = memberExpression.Member.Name;
            return name;
        }

        public static string GetPropertyName(this object obj, string name)
        {
            PropertyInfo propInfo = obj.GetType().GetProperty(name);
            if (propInfo == null)
            {
                return null;
            }

            return propInfo.Name;
        }

        public static object GetPropValueByName(this object obj, string name)
        {
            if (name == null)
            {
                return null;
            }
            PropertyInfo propInfo = obj.GetType().GetProperty(name);
            if (propInfo == null)
            {
                return null;
            }

            object value = propInfo.GetValue(obj, null);
            return value;
        }
    }
}