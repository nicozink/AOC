using System;
using System.Collections.Generic;
using System.Linq;

namespace Common
{
    /// <summary>
    /// Equality comparer for array types.
    /// </summary>
    /// <typeparam name="T">The array type.</typeparam>
    public class ArrayComparer<T> : IEqualityComparer<T[]>
    {
        public bool Equals(T[] first, T[] second)
        {
            return Enumerable.SequenceEqual(first, second);
        }

        public int GetHashCode(T[] array)
        {
            int hash = 17;
            foreach (T element in array)
            {
                hash = hash * 31 + element.GetHashCode();
            }
            
            return hash;
        }
    }
}
