
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Common
{
    /// <summary>
    /// Extension methods on objects. Designed to provide linq-like functionality
    /// </summary>
    public static class ObjectExtensions
    {

        //Handy for doing stuff inside a Func.
        public static T Do<T>(this T item, Action<T> action)
        {
            action(item);
            return item;
        }

        public static IEnumerable<T> Do<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items)
                action(item);
            return items;
        }


        public static IEnumerable<T> TraverseObjectTree<T>(this IEnumerable<T> rootList, Func<T, bool> predicate = null)
        {
            var done = new List<T>();
            foreach (var tRoot in rootList)
                foreach (var tChild in TraverseObjectTree<T>(tRoot, predicate, done))
                    yield return tChild;
        }

        public static IEnumerable<T> TraverseObjectTree<T>(this T rootObject, Func<T, bool> predicate = null)
        {
            return TraverseObjectTree(rootObject, predicate, new List<T>());
        }
        public static IEnumerable<T> TraverseObjectTree<T>(this T rootObject, Func<T, bool> predicate, List<T> done)
        {
            if (rootObject != null && rootObject is T && !done.Contains(rootObject) && (predicate == null || predicate(rootObject)))
            {
                yield return rootObject;
                done.Add(rootObject);
                foreach (var property in rootObject.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (typeof(T).IsAssignableFrom(property.PropertyType))
                    {
                        foreach (var child in TraverseObjectTree<T>((T)property.GetGetterFunc()(rootObject), predicate, done))
                            yield return child;
                    }
                    else if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(List<>) && typeof(T).IsAssignableFrom(property.PropertyType.GetGenericArguments().First()))
                    //TODO: Support other Enumerable types besides List<>
                    //(don't forget to not see string is an ienumerable!)
                    {
                        var listOfChildren = ((IEnumerable)property.GetGetterFunc()(rootObject));
                        if (listOfChildren != null)
                            foreach (var child in listOfChildren.OfType<T>().SelectMany(e => TraverseObjectTree(e, predicate, done)))
                                yield return child;
                    }
                    else if (property.PropertyType.IsArray)
                    {
                        var listOfChildren = ((Array)property.GetGetterFunc()(rootObject));
                        if (listOfChildren != null)
                            foreach (var child in listOfChildren.OfType<T>().SelectMany(e => TraverseObjectTree(e, predicate, done)))
                                yield return child;
                    }
                }
            }
        }

        /// <summary>
        /// Wraps this object instance into an IEnumerable<T>,
        /// consisting of one item.
        /// </summary>
        /// <typeparam name="T">The type of object.</typeparam>
        /// <param name="source">The existing type.</param>
        /// <returns>An IEnumerable<T>, consisting of single item .</returns>
        public static IEnumerable<T> ToSingletonCollection<T>(this T source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            yield return source;
        }


        /// <summary>
        /// Wraps this object instance into an IEnumerable<T>,
        /// consisting of two items.
        /// </summary>
        /// <typeparam name="T">The type of source object.</typeparam>
        /// <param name="source">The existing type. </param>
        /// <param name="value">The value of existing type. </param>
        /// <returns>An IEnumerable<T>, consisting of two items </returns>
        [Obsolete("This method is obsolete. Use ConcatSome with a single value to replicate the functionality.")]
        public static IEnumerable<T> UnionOne<T>(this T source, T value)
        {
            yield return source;
            yield return value;
        }
        public static IEnumerable<T> UnionSome<T>(this T source, params T[] values)
        {
            return source.Union((IEnumerable<T>)values);
        }

        /// <summary>
        /// Wraps this object instance into an IEnumerable<T>,
        /// consisting of more than two items.
        /// </summary>
        /// <typeparam name="T">The type of source object.</typeparam>
        /// <param name="source">The existing type. </param>
        /// <param name="value">IEnumerable value of existing type. </param>
        /// <returns>An IEnumerable<T>, consisting of more than two items </returns>
        public static IEnumerable<T> Union<T>(this T source, IEnumerable<T> value)
        {
            //Note we use the "normal" union method so we can let it 
            //deal with not returning the same value twice.
            return new T[] { source }.Union(value);
        }


        public static IEnumerable<T> ConcatSome<T>(this T source, params T[] values)
        {
            return source.Concat((IEnumerable<T>)values);
        }

        public static IEnumerable<T> Concat<T>(this T source, IEnumerable<T> value)
        {
            yield return source;
            foreach (var t in value)
                yield return t;
        }

        /// <summary>
        ///  Method which extends the LINQ methods to flatten a collection of 
        ///  items that have a property of children of the same type.
        /// </summary>
        /// <typeparam name = "T">Item type.</typeparam>
        /// <param name = "source">Source collection.</param>
        /// <param name = "getChildren">
        ///   Get children value selector delegate of each item. 
        ///   IEnumerable'T' getChildren(T itemBeingFlattened)
        /// </param>
        /// <returns>Returns a one level list of elements of type T.</returns>
        //public static IEnumerable<T> FlattenTree<T>(this T source, Func<T, IEnumerable<T>> getChildren)
        //{
        //    yield return source;
        //    var children = getChildren(source);
        //    if (children != null)
        //        foreach (var child in children)
        //            foreach (var descendant in child.FlattenTree(getChildren))
        //                yield return descendant;
        //}

        public static IEnumerable<T> FlattenTree<Tparent, T>(this Tparent source, Func<Tparent, IEnumerable<T>> getChildren)
            where Tparent : T
        {
            return FlattenTreeInternal<Tparent, T>((T)source, getChildren);
        }

        private static IEnumerable<T> FlattenTreeInternal<Tparent, T>(this T source, Func<Tparent, IEnumerable<T>> getChildren)
            where Tparent : T
        {
            yield return source;
            if (source is Tparent)
            {
                var children = getChildren((Tparent)source);
                if (children != null)
                    foreach (var child in children)
                        foreach (var descendant in child.FlattenTreeInternal(getChildren))
                            yield return descendant;
            }
        }

        public static IEnumerable<T> FlattenTree<Tparent, T>(this IEnumerable<T> source, Func<Tparent, IEnumerable<T>> getChildren)
            where Tparent : T
        {
            foreach (var item in source)
                foreach (var child in FlattenTreeInternal(item, getChildren))
                    yield return child;
        }

        public static IEnumerable<T> FlattenTree<Tparent, T>(this IEnumerable<Tparent> source, Func<Tparent, IEnumerable<T>> getChildren)
            where Tparent : T
        {
            foreach (var item in source)
                foreach (var child in FlattenTreeInternal(item, getChildren))
                    yield return child;
        }


        /// <summary>
        ///  Method which extends the LINQ methods to flatten a collection of 
        ///  items that have a property of children of the same type.
        /// </summary>
        /// <typeparam name = "T">Item type.</typeparam>
        /// <param name = "source">Source collection.</param>
        /// <param name = "getChild">
        ///   Get children value selector delegate of each item. 
        ///   'T' getChild(T itemBeingFlattened)
        /// </param>
        /// <returns>Returns a one level list of elements of type T.</returns>
        public static IEnumerable<T> FlattenTree<T>(this T source, Func<T, T> getChild)
        {
            yield return source;
            var child = getChild(source);
            if (child != null)
                foreach (var descendant in child.FlattenTree(getChild))
                    yield return descendant;
        }

        /// <summary>
        ///  Method which extends the LINQ methods to flatten a collection of 
        ///  items that have a property of Parent of the same type.
        /// </summary>
        /// <typeparam name = "T">Item type.</typeparam>
        /// <param name = "source">Source collection.</param>
        /// <param name = "getParent">
        ///   Get children value selector delegate of each item. 
        ///   'T' getParent(T )
        /// </param>
        /// <returns>Returns a one level list of elements of type T.</returns>

        public static IEnumerable<T> FlattenTreeUpwards<T>(this T source, Func<T, T> getParent)
        {
            yield return source;
            var parent = getParent(source);
            if (parent != null)
                foreach (var ancestor in parent.FlattenTreeUpwards(getParent))
                    yield return ancestor;
        }

    }
}
