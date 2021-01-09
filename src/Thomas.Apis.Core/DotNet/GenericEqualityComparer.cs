using System;
using System.Collections.Generic;

namespace Thomas.Apis.Core.DotNet
{
    internal class GenericEqualityComparer<T> : IEqualityComparer<T>
    {
        private readonly Func<T, T, bool> m_compareFunction;

        private readonly Func<T, int> m_getHashCode;

        public GenericEqualityComparer(Func<T, T, bool> compareFunction, Func<T, int> getHashCode = null)
        {
            m_compareFunction = compareFunction;
            m_getHashCode = getHashCode;
        }

        public bool Equals(T x, T y)
        {
            var equals = m_compareFunction(x, y);
            return equals;
        }

        public int GetHashCode(T obj)
        {
            if (m_getHashCode == null)
            {
                throw new ArgumentException("m_getHashcode was null");
            }
            else
            {
                return m_getHashCode(obj);
            }
        }
    }
}