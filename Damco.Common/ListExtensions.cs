using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Common
{
    public static class ListExtensions
    {
        public static void AddRangeDistinct(this IList list, IEnumerable items)
        {
            foreach (var item in items)
                if (!list.Contains(item))
                    list.Add(item);
        }
        public static void AddRangeDistinct<T>(this List<T> list, IEnumerable<T> items)
        {
            list.AddRange(items.Except(list).Distinct());
        }
        public static void AddDistinct<T>(this List<T> list, T item)
        {
            if (!list.Contains(item))
                list.Add(item);
        }

        public static void RemoveRange<T>(this List<T> list, IEnumerable<T> items)
        {
            if (!(items is List<T>) || items == list) //It's not a separate list, so there is a good change the range is based on the input which means removals will fail
                items = items.ToList();
            foreach (var item in items.ToList())
                list.Remove(item);
        }
    }
}
