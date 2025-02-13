using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Common
{   /// <summary>
    /// Extension methods on Dictionary.
    /// </summary>
    public static class DictionaryExtensions
    {

        /// <summary>
        /// Return value associated with the specified key.
        /// </summary>
        /// <typeparam name="TKey">Key of specified type.</typeparam>
        /// <typeparam name="TValue"> Value of specified type.</typeparam>
        /// <param name="source">Dictionary from which values needs to be retrieved.</param>
        /// <param name="key">Key used for retrieving the value from dictionary. </param>
        /// <returns>The value for the key in dictionary. Oherwise, the default value for the type of the value parameter</returns>
        public static TValue TryGetValue<TKey, TValue>(this Dictionary<TKey, TValue> source, TKey key)
        {
            TValue result;
            if (source.TryGetValue(key, out result))
                return result;
            else
                return default(TValue);
        }

        public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> source, TKey key, Func<TKey, TValue> valueFactory)
        {
            TValue result;
            if (!source.TryGetValue(key, out result))
            {
                result = valueFactory(key);
                source[key] = result;
            }
            return result;
        }
        /// <summary>
        /// Return value associated with the specified key.
        /// </summary>
        /// <typeparam name="TKey">Key of specified type.</typeparam>
        /// <typeparam name="TValue">Value of specified type</typeparam>
        /// <param name="source">Dictionary from which values needs to be retrieved</param>
        /// <param name="key">Key used for retrieving the value from dictionary.</param>
        /// <param name="valueIfMissingFactory">Delegate returning the Value  of type TValue.</param>
        /// <returns>The value for the key in dictionary. Oherwise, delegate valueIfMissingFactory return the value of type TValue. </returns>
        public static TValue TryGetValue<TKey, TValue>(this Dictionary<TKey, TValue> source, TKey key, Func<TValue> valueIfMissingFactory)
        {
            TValue result;
            if (source.TryGetValue(key, out result))
                return result;
            else if (valueIfMissingFactory == null)
                return default(TValue);
            else
                return valueIfMissingFactory();
        }


        /// <summary>
        /// Return value associated with the specified key.
        /// </summary>
        /// <typeparam name="TKey">Key of specified type.</typeparam>
        /// <typeparam name="TValue">Value of specified type</typeparam>
        /// <param name="source">Dictionary from which values needs to be retrieved</param>
        /// <param name="key">Key used for retrieving the value from dictionary.</param>
        /// <param name="valueIfMissing"> Value  of type TValue.</param>
        /// <returns>The value for the key in dictionary. Oherwise, parameter valueIfMissing of type TValue is returned. </returns>
        public static TValue TryGetValue<TKey, TValue>(this Dictionary<TKey, TValue> source, TKey key, TValue valueIfMissing)
        {
            TValue result;
            if (source.TryGetValue(key, out result))
                return result;
            else
                return valueIfMissing;
        }

        public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IEnumerable<KeyValuePair<TKey, TValue>> items)
        {
            foreach (var item in items)
                dictionary.Add(item.Key, item.Value);
        }
        public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IEnumerable<Tuple<TKey, TValue>> items)
        {
            foreach (var item in items)
                dictionary.Add(item.Item1, item.Item2);
        }

    }
}
