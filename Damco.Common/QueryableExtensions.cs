using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Common
{
    public static class QueryableExtensions
    {
        public static Type GetEnumeratedType(this Type type)
        {
            return
                type.GetInterfaces().UnionSome(type).SingleOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IQueryable<>))?.GetGenericArguments().Single()
                ?? type.GetInterfaces().UnionSome(type).SingleOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>))?.GetGenericArguments().Single();
        }
        public static IQueryable Take(this IQueryable source, int count)
        {
            return typeof(Queryable).CallDynamic<IQueryable>(nameof(Queryable.Take), GetEnumeratedType(source.GetType()), source, count);
        }


        public static IQueryable Where(this IQueryable source, LambdaExpression predicate)
        {
            return typeof(Queryable).CallDynamic<IQueryable>(nameof(Queryable.Where), GetEnumeratedType(source.GetType()), source, predicate);
        }
        public static IQueryable Select(this IQueryable source, LambdaExpression selector)
        {
            var elementType = GetEnumeratedType(source.GetType());
            if (selector.GetType().GetGenericArguments().Single().GetGenericArguments().First() != elementType)
                throw new ArgumentException($"'{nameof(selector)}' does not expect the element type of '{nameof(source)}' as its input");
            return typeof(Queryable).CallDynamic<IQueryable>(nameof(Queryable.Select), new Type[] { GetEnumeratedType(source.GetType()), selector.Body.Type }, source, selector);
        }

        public static IQueryable Distinct(this IQueryable source)
        {
            var elementType = GetEnumeratedType(source.GetType());
            return typeof(Queryable).CallDynamic<IQueryable>(nameof(Queryable.Distinct), new Type[] { GetEnumeratedType(source.GetType()) }, source);
        }
        public static IQueryable SelectMany(this IQueryable source, LambdaExpression selector)
        {
            return typeof(Queryable).CallDynamic<IQueryable>(nameof(Queryable.SelectMany), new Type[] { GetEnumeratedType(source.GetType()), GetEnumeratedType(selector.Body.Type) }, source, selector);
        }
        public static IQueryable GroupBy(this IQueryable source, LambdaExpression keySelector)
        {
            return typeof(Queryable).CallDynamic<IQueryable>(nameof(Queryable.GroupBy), new Type[] { GetEnumeratedType(source.GetType()), keySelector.Body.Type }, source, keySelector);
        }
        public static IQueryable GroupBy(this IQueryable source, LambdaExpression keySelector, LambdaExpression elementSelector)
        {
            return typeof(Queryable).CallDynamic<IQueryable>(nameof(Queryable.GroupBy), new Type[] { GetEnumeratedType(source.GetType()), keySelector.Body.Type }, source, keySelector);
        }

        public static object First(this IQueryable source)
        {
            return typeof(Queryable).CallDynamic(nameof(Queryable.First), GetEnumeratedType(source.GetType()), source);
        }
        public static object First(this IQueryable source, LambdaExpression predicate)
        {
            return typeof(Queryable).CallDynamic(nameof(Queryable.First), GetEnumeratedType(source.GetType()), source, predicate);
        }
        public static object FirstOrDefault(this IQueryable source)
        {
            return typeof(Queryable).CallDynamic(nameof(Queryable.FirstOrDefault), GetEnumeratedType(source.GetType()), source);
        }
        public static object FirstOrDefault(this IQueryable source, LambdaExpression predicate)
        {
            return typeof(Queryable).CallDynamic(nameof(Queryable.FirstOrDefault), GetEnumeratedType(source.GetType()), source, predicate);
        }
        public static IEnumerable UntypedToList(this IEnumerable source)
        {
            return typeof(Enumerable).CallDynamic<IEnumerable>(nameof(Enumerable.ToList), GetEnumeratedType(source.GetType()), source);
        }
    }
}
