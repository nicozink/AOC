using System;
using System.Collections.Generic;
using System.Linq;

namespace Common
{
    /// <summary>
    /// Contains list extension operations.
    /// </summary>
    public class ListExtension
    {
        /// <summary>
        /// Gets all pairs in a list.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>The pairs.</returns>
        public static IEnumerable<Tuple<T, T>> GetPairs<T>(List<T> input)
        {
            for (int x = 0; x < input.Count; ++x)
            {
                for (int y = 0; y < input.Count; ++y)
                {
                    if (x == y)
                    {
                        continue;
                    }

                    yield return Tuple.Create(input[x], input[y]);
                }
            }
        }
    }
}
