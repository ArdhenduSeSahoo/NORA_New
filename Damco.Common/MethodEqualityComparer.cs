using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Common
{
    public static class MethodEqualityComparer
    {
        public static MethodEqualityComparer<T> Get<T>(Func<T, T, bool> method)
        {
            return new Common.MethodEqualityComparer<T>(method);
        }
    }

    public class MethodEqualityComparer<T> : IEqualityComparer<T>
    {

        Func<T, T, bool> _method;
        public MethodEqualityComparer(Func<T, T, bool> method)
        {
            _method = method;
        }

        bool IEqualityComparer<T>.Equals(T x, T y)
        {
            return _method(x, y);
        }

        int IEqualityComparer<T>.GetHashCode(T obj)
        {
            return 0; //Let the Equals handle it
        }
    }
}
