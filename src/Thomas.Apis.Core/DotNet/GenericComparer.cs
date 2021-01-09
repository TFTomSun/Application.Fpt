using System;
using System.Collections.Generic;

namespace Thomas.Apis.Core.DotNet
{
    internal class GenericComparer<T> : IComparer<T>
    {
        private readonly Func<T, T, int> m_compareFunction;

        public GenericComparer(Func<T, T, int> compareFunction)
        {
            m_compareFunction = compareFunction;
        }

        public int Compare(T x, T y)
        {
            return m_compareFunction(x, y);
        }
    }
}