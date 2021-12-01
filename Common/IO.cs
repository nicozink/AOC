using System;
using System.Collections.Generic;
using System.Linq;

namespace Common
{
    /// <summary>
    /// Contains various IO and file read operations.
    /// </summary>
    public class IO
    {
        /// <summary>
        /// Reads a collection of numbers from a file.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <returns>The collection of numbers.</returns>
        public static IEnumerable<int> ReadNumbers(String path)
        {
            var input = System.IO.File.ReadLines(path);

            return input.Select(x => int.Parse(x));
        }
    }
}
