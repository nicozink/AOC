using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class MathUtil
    {
        /// <summary>
        /// See the Euclidean Algorithm here:
        /// https://en.wikipedia.org/wiki/Greatest_common_divisor
        /// </summary>
        /// <param name="a">An integer.</param>
        /// <param name="b">An integer.</param>
        /// <returns>The greatest common divisor of the two integers.</returns>
        public static long GreatestCommonDivisor(long a, long b)
        {
            while (b != 0)
            {
                (a, b) = (b, a % b);
            }
            return a;
        }

        /// <summary>
        /// See the forula for calculating the lease common multiple:
        /// https://en.wikipedia.org/wiki/Least_common_multiple
        /// </summary>
        /// <param name="a">An integer.</param>
        /// <param name="b">An integer.</param>
        /// <returns>The least common multiple of the two integers.</returns>
        public static long LeastCommonMultiple(long a, long b)
        {
            return (a * b) / GreatestCommonDivisor(a, b);
        }
    }
}
