//Utility methods for IEnumerable<T>
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Common
{

    /// <summary>
    /// Utility methods for IEnumerable<T>
    /// </summary>
    public static class EnumerableExtensions
    {
        public static IEnumerable<Ttarget> CopyUniqueItems<Tsource, Ttarget>(this IEnumerable<Tsource> sources, IList<Ttarget> targets, Func<Ttarget, Ttarget, bool> targetComparer, bool alsoComplexProperties = false, IEnumerable<string> excludedProperties = null, Action<Tsource, Ttarget> finishCopy = null)
            where Ttarget : new()
        {
            List<Ttarget> result = new List<Ttarget>();
            foreach (var item in sources)
            {
                var itemToUse = item.CopyMatchingPropertiesToNew<Ttarget>(alsoComplexProperties, excludedProperties);
                var existingItem = targets.FirstOrDefault(t => targetComparer(t, itemToUse));
                if (existingItem == null)
                {
                    targets.Add(itemToUse);
                    result.Add(itemToUse);
                }
                else
                    itemToUse = existingItem;
                finishCopy?.Invoke(item, itemToUse);
            }
            return result;
        }

        /// <summary>
        /// Picks the best item from a list.
        /// </summary>
        /// <typeparam name="T">Type of item in the list.</typeparam>
        /// <param name="source">A list</param>
        /// <param name="predicates">The predicates that determine which items are preferred, in order of importance.</param>
        /// <returns>The best item based on the predicates. Logic being that items that match earlier predicates are always preferred but if multiple items match the earlier predicates, the items that also match later predicates win. Note if none of the items of the list match any predicate, the first item of the list is returned. If multiple items are "best", the first of those is returned.</returns>
        /// <remarks>Unless the source is empty, this method will always return an item even if none of the predicates are met. If there are mandatory predicates, use .Where first to filter those out.</remarks>
        public static T BestOrDefault<T>(this IEnumerable<T> source, params Func<T, bool>[] predicates)
        {
            List<T> inTheRunning = source.ToList();
            foreach(var predicate in predicates)
            {
                var found = inTheRunning.Where(predicate).ToList();
                if (found.Count() == 1) //Must be the best
                    return found.Single();
                else if (found.Any()) //>= 2
                    inTheRunning = found;
                //Note if we didn't find anything we skip the predicate and move on the the next one with the same items
            }
            return inTheRunning.FirstOrDefault();
        }

        public static IEnumerable<Tresult> Join<T1, T2, Tresult>(this IEnumerable<T1> list1, IEnumerable<T2> list2, Func<T1, T2, bool> join, Func<T1, T2, Tresult> resultSelector)
        {
            foreach (var t1 in list1)
                foreach (var t2 in list2)
                    if (join(t1, t2))
                        yield return resultSelector(t1, t2);
        }

        public static IEnumerable<T> Substitute<T>(this IEnumerable<T> value, T searchValue, params T[] substitutions)
        {
            foreach (var t in value)
            {
                if (object.Equals(t, searchValue))
                {
                    foreach (var t2 in substitutions)
                        yield return t2;
                }
                else
                    yield return t;
            }
        }

        public static IList ToListOfType(this IEnumerable value, Type targetType)
        {
            var result = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(targetType));
            foreach (var item in value)
                result.Add(item);
            return result;
        }

        public static IEnumerable<T> DefaultIfIfNull<T>(this IEnumerable<T> value, T nullValue)
        {
            if (value == null)
                return nullValue.ToSingletonCollection();
            else
                return value;
        }

        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> value)
        {
            if (value == null)
                return Enumerable.Empty<T>();
            else
                return value;
        }

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> items)
        {
            return items.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
        public static ConcurrentDictionary<TKey, TValue> ToConcurrentDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> items)
        {
            return items.ToConcurrentDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<Tuple<TKey, TValue>> items)
        {
            return items.ToDictionary(kvp => kvp.Item1, kvp => kvp.Item2);
        }
        public static ConcurrentDictionary<TKey, TValue> ToConcurrentDictionary<TKey, TValue>(this IEnumerable<Tuple<TKey, TValue>> items)
        {
            return items.ToConcurrentDictionary(kvp => kvp.Item1, kvp => kvp.Item2);
        }

        /// <summary>
        /// Retrieve the distinct values of type T from IEnumerable<T> using the selector delegate.
        /// </summary>
        /// <typeparam name="T">Type of the source object.</typeparam>
        /// <typeparam name="Tvalue">Type of the object returned by selector delegate. </typeparam>
        /// <param name="input">Current instance IEnumerable object from which distinct values will be retrieved.</param>
        /// <param name="selector">Get the Tvalue using selector delegate, for each item.  </param>
        /// <returns> Distinct items of type T.</returns>
        public static IEnumerable<T> DistinctBy<T, Tvalue>(this IEnumerable<T> input, Func<T, Tvalue> selector)
        {
            List<Tvalue> doneValues = new List<Tvalue>();
            foreach (T t in input)
            {
                Tvalue value = selector(t);
                if (!doneValues.Contains(value))
                {
                    doneValues.Add(value);
                    yield return t;
                }
            }
        }

        public static IEnumerable<T> DistinctOn<T>(this IEnumerable<T> input, Func<T, T, bool> comparer)
        {
            return input.Distinct(new MethodEqualityComparer<T>(comparer));
        }

        /// <summary>
        /// Divides data into batches based on a batchSize of the number of child items that can be in a batch
        /// </summary>
        /// <typeparam name="Tparent">Parent object type - the type that is batched</typeparam>
        /// <typeparam name="Tchild">Child object type - the type that is used for the batch count</typeparam>
        /// <param name="input">All parent items to be included</param>
        /// <param name="getChildren">Method to get the children from the parent</param>
        /// <param name="batchSize">Number of child items that can be in a batch</param>
        /// <returns>
        /// Batches of parent items each of which will not exceed the specified number of child items. 
        /// There is one exception: if a single parent item has more children than the batch size, it alone is returned
        /// in a batch of one item.
        /// </returns>
        public static IEnumerable<IEnumerable<Tuple<Tparent, IEnumerable<Tchild>>>> BatchOnChildren<Tparent, Tchild>(this IEnumerable<Tparent> input, Func<Tparent, IEnumerable<Tchild>> getChildren, int batchSize, bool tryToKeepParentsWhole)
        {
            if (!tryToKeepParentsWhole)
                throw new NotSupportedException($"It is not supported yet for {nameof(tryToKeepParentsWhole)} to be false");

            if (batchSize == int.MaxValue) //They don't want to batch
            {
                if (!input.Any())
                    yield break;
                else
                    yield return input.Select(i => Tuple.Create(i, getChildren(i)));
            }
            else
            {
                var batch = new List<Tuple<Tparent, IEnumerable<Tchild>>>();
                foreach (Tparent parent in input)
                {
                    var leftOverChildren = (IEnumerable<Tchild>)getChildren(parent).ToList();
                    while (leftOverChildren.Any())
                    {
                        var childrenOneBatch = leftOverChildren.Take(batchSize);
                        if ((batch.SelectMany(t => t.Item2).Count() + childrenOneBatch.Count()) > batchSize)
                        {
                            yield return batch;
                            batch = new List<Tuple<Tparent, IEnumerable<Tchild>>>();
                        }
                        batch.Add(Tuple.Create(parent, leftOverChildren.Take(batchSize)));
                        leftOverChildren = leftOverChildren.Skip(batchSize);
                    }
                }
                if (batch.Any())
                    yield return batch;
            }
        }

        /// <summary>
        ///  Retrieve the lists of items from the current instance with specified batch size.
        /// </summary>
        /// <typeparam name="T">Type of the source object.</typeparam>
        /// <param name="input">Items of type T which are required in various batches.</param>
        /// <param name="batchSize">No.of Items to be in one batch.</param>
        /// <returns> Batches of items from the current instance.</returns>
        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> input, int batchSize)
        {
            if (batchSize == int.MaxValue) //They don't want to batch
            {
                if (!input.Any())
                    yield break;
                else
                    yield return input;
            }
            else
            {
                List<T> batch = new List<T>();
                foreach (T t in input)
                {
                    if (batch.Count >= batchSize)
                    {
                        yield return batch;
                        batch = new List<T>();
                    }
                    batch.Add(t);
                }
                if (batch.Any())
                    yield return batch;
            }
        }
        /// <summary>
        /// Combines the two objects of different types.
        /// </summary>
        /// <typeparam name="T1">Type of the 1st source object.</typeparam>
        /// <typeparam name="T2">Type of the 2nd source object.</typeparam>
        /// <param name="source1">Current instance of type T1 to be combined. </param>
        /// <param name="source2">Second object of type T2 to be combined.</param>
        /// <returns>Combined object of type Tuple<T1,T2></T1></returns>
        public static IEnumerable<Tuple<T1, T2>> CombineWith<T1, T2>(this IEnumerable<T1> source1, IEnumerable<T2> source2)
        {
            return source1.CombineWith(source2, false);
        }

        public static IEnumerable<Tuple<T1, T2>> CombineWith<T1, T2>(this IEnumerable<T1> source1, IEnumerable<T2> source2, bool allowMismatch)
        {
            using (var enumerator2 = source2.GetEnumerator())
            {
                bool enum2Done = false;
                foreach (var t1 in source1)
                {
                    if (!enumerator2.MoveNext())
                    {
                        if (allowMismatch)
                            enum2Done = true;
                        else
                            throw new ArgumentException("source2 has less items than source1", "source2");
                    }
                    yield return Tuple.Create(t1, enum2Done ? default(T2) : enumerator2.Current);
                }
                if (!enum2Done && enumerator2.MoveNext())
                {
                    if (allowMismatch)
                    {
                        yield return Tuple.Create(default(T1), enumerator2.Current);
                        while (enumerator2.MoveNext())
                            yield return Tuple.Create(default(T1), enumerator2.Current);
                    }
                    else
                        throw new ArgumentException("source2 has more items than source1");
                }
            }
        }

        public static IEnumerable<Tuple<T1, T2>> FullJoin<T1, T2>(this IEnumerable<T1> source1, IEnumerable<T2> source2, Func<T1, T2, bool> comparison)
        {
            List<T2> done2 = new List<T2>();
            foreach (var value1 in source1)
            {
                bool aValueFound = false;
                foreach (var value2 in source2.Where(v2 => comparison(value1, v2)))
                {
                    yield return Tuple.Create(value1, value2);
                    done2.Add(value2);
                    aValueFound = true;
                }
                if (!aValueFound)
                    yield return Tuple.Create(value1, default(T2));
            }
            foreach (var value2 in source2.Except(done2))
                yield return Tuple.Create(default(T1), value2);
        }

        //TODO: remove this method alltogether (after all its usages have been changed to ContactSome or UnionSome)
        /// <summary>
        /// Wraps this object instance into an IEnumerable<T>,
        /// consisting of more than two items of same type.
        /// </summary>
        /// <typeparam name="T">The type of source object.</typeparam>
        /// <param name="source">The IEnumerable of existing type T. </param>
        /// <param name="value">Other item of existing type T. </param>
        /// <returns>An IEnumerable<T>, consisting of more than two items </returns>
        [Obsolete("This method is obsolete. Use ConcatSome with a single value to replicate the functionality.")]
        public static IEnumerable<T> UnionOne<T>(this IEnumerable<T> source, T value)
        {
            foreach (T t in source)
                yield return t;
            yield return value;
        }

        public static IEnumerable<T> UnionSome<T>(this IEnumerable<T> source, params T[] values)
        {
            return source.Union((IEnumerable<T>)values);
        }

        public static IEnumerable<T> ConcatSome<T>(this IEnumerable<T> source, params T[] values)
        {
            return source.Concat((IEnumerable<T>)values);
        }

        /// <summary>
        /// Create the ConcurrentDictionary using existing instance & Keys selector delegate.
        /// </summary>
        /// <typeparam name="TSource">Type of the source object.</typeparam>
        /// <typeparam name="TKey">Type of the Key object.</typeparam>
        /// <param name="source">Source of Key and its elements for creating ConcurrentDictionary.</param>
        /// <param name="keySelector"> Get key value selector delegate, for each item.</param>
        /// <returns>ConcurrentDictionary <TKey, TSource>.  </returns>
        ///
        public static ConcurrentDictionary<TKey, TSource> ToConcurrentDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return ToConcurrentDictionary(source, keySelector, (item => item), default(IEqualityComparer<TKey>));
        }

        /// <summary>
        /// Create the ConcurrentDictionary using existing instance, Keys selector delegate & Element selector delegate.
        /// </summary>
        /// <typeparam name="TSource">Type of the source object</typeparam>
        /// <typeparam name="TKey">Type of the Key object.</typeparam>
        /// <typeparam name="TElement">Type of the Element object.</typeparam>
        /// <param name="source">Source of Key and its elements for creating ConcurrentDictionary.</param>
        /// <param name="keySelector"> Get key value selector delegate, for each item. </param>
        /// <param name="elementSelector">Get element value selector delegate, for each item.</param>
        /// <returns>ConcurrentDictionary <TKey, TElement>.</returns>
        public static ConcurrentDictionary<TKey, TElement> ToConcurrentDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
        {
            return ToConcurrentDictionary(source, keySelector, elementSelector, default(IEqualityComparer<TKey>));
        }

        /// <summary>
        /// Create the ConcurrentDictionary using existing instance, Keys selector delegate & Object Comparer.
        /// </summary>
        /// <typeparam name="TSource">Type of the source object</typeparam>
        /// <typeparam name="TKey">Type of the Key object.</typeparam>
        /// <param name="source">Source of Key and its elements for creating ConcurrentDictionary.</param>
        /// <param name="keySelector"> Get key value selector delegate, for each item. </param>
        /// <param name="comparer">Used for comparision Keys for equality in ConcurrentDictionary .</param>
        /// <returns>ConcurrentDictionary<TKey,TSource></returns>
        public static ConcurrentDictionary<TKey, TSource> ToConcurrentDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            return ToConcurrentDictionary(source, keySelector, (item => item), comparer);
        }

        /// <summary>
        /// Create the ConcurrentDictionary using existing instance, Keys, Elements & object comparison.
        /// </summary>
        /// <typeparam name="TSource">Type of the source object</typeparam>
        /// <typeparam name="TKey">Type of the Key object.</typeparam>
        /// <typeparam name="TElement">Type of the Element object.</typeparam>
        /// <param name="source">Source of Key and its elements for creating ConcurrentDictionary.</param>
        /// <param name="keySelector"> Get key value using selector delegate, for each item. </param>
        /// <param name="elementSelector">Get element value using selector delegate, for each item.</param>
        /// <param name="comparer">Used for comparision Keys for equality in ConcurrentDictionary.</param>
        /// <returns>ConcurrentDictionary<TKey,TElement>.</returns>
        public static ConcurrentDictionary<TKey, TElement> ToConcurrentDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
        {
            var result = (comparer == null ? new ConcurrentDictionary<TKey, TElement>() : new ConcurrentDictionary<TKey, TElement>(comparer));
            foreach (var sourceItem in source)
                result[keySelector(sourceItem)] = elementSelector(sourceItem);
            return result;
        }

        public static Dictionary<TKey, TElement> DistinctToDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
        {
            var result = new Dictionary<TKey, TElement>();
            foreach (var sourceItem in source)
                result[keySelector(sourceItem)] = elementSelector(sourceItem); //Not using add, so won't go wrong with duplicate key
            return result;
        }

        public static T UniqueOrDefault<T>(this IEnumerable<T> source)
        {
            var distinct = source.Where(v => !object.Equals(v, default(T))).Distinct();
            if (distinct.Count() == 1)
                return distinct.First();
            else
                return default(T);
        }

    }
}
