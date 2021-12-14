using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace Solutions
{
    /// <summary>
    /// Solution for day 14:
    /// https://adventofcode.com/2021/day/14
    /// </summary>
    [SolutionClass(Day = 14)]
    public class Day14Solution
    {
        /// <summary>
        /// A class used to count the number of characters, after the polymer
        /// has been expanded multiple times.
        /// </summary>
        private class PolymerSolver
        {
            /// <summary>
            /// Create the polymer solver. Takes the lookup used
            /// to expands strings.
            /// </summary>
            /// <param name="lookup">The lookup.</param>
            internal PolymerSolver(Dictionary<string, char> lookup)
            {
                this.lookup = lookup;

                // Create an empty dictionary for convenience, containing
                // the zero counts for each character.
                foreach (var c in lookup.Values.GroupBy(x => x))
                {
                    emptyDictionary.Add(c.Key, 0);
                }
            }

            /// <summary>
            /// Expand the polymer string, and return the counts per character.
            /// </summary>
            /// <param name="expansionString">The string to expand.</param>
            /// <param name="depth">The depth.</param>
            /// <returns>The count per character.</returns>
            internal Dictionary<char, long> CountExpanded(string expansionString, int depth)
            {
                if (depth == 0)
                {
                    return new(emptyDictionary);
                }

                Dictionary<char, long> output = new(emptyDictionary);

                for (int i = 0; i < expansionString.Length - 1; i++)
                {
                    String subString = "" + expansionString[i] + expansionString[i + 1];
                    
                    Dictionary<char, long> subStringCount;
                    if (cache.ContainsKey((subString, depth)))
                    {
                        subStringCount = cache[(subString, depth)];
                    }
                    else
                    {
                        var newChar = lookup[subString];
                        var newString = "" + subString[0] + newChar + subString[1];

                        subStringCount = CountExpanded(newString, depth - 1);

                        subStringCount[newChar]++;

                        cache.Add((subString, depth), subStringCount);
                    }

                    foreach (var count in subStringCount)
                    {
                        output[count.Key] += count.Value;
                    }
                }

                return output;
            }

            /// <summary>
            /// A cache of previous strings expanded at a particulat depth.
            /// </summary>
            private Dictionary<(string, int), Dictionary<char, long>> cache = new();

            /// <summary>
            /// An empty dictionary with a count of zero for each character.
            /// Can be used to easily create a new dictionary.
            /// </summary>
            private Dictionary<char, long> emptyDictionary = new();

            /// <summary>
            /// The lookup, for each pair of characters.
            /// </summary>
            private Dictionary<string, char> lookup;
        }

        /// <summary>
        /// Reads the string and lookup rules, and returns the difference between
        /// the min and max counts for the expanded string.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <param name="depth">The desired expansion depth.</param>
        /// <returns>The difference of the min and max character counts.</returns>
        long CountExpanded(String path, int depth)
        {
            var input = System.IO.File.ReadLines(path);

            var enumerator = input.GetEnumerator();
            enumerator.MoveNext();

            String expansionString = enumerator.Current;

            enumerator.MoveNext();

            var lookup = new Dictionary<String, char>();
            while (enumerator.MoveNext())
            {
                var split = enumerator.Current.Replace(" -> ", " ").Split();

                lookup.Add(split[0], split[1][0]);
            }

            var solver = new PolymerSolver(lookup);
            var charactersCount = solver.CountExpanded(expansionString, depth);

            // Don't forget to add the original characters!
            foreach (var c in expansionString.GroupBy(x => x))
            {
                charactersCount[c.Key] += c.Count();
            }

            return charactersCount.Values.Max() - charactersCount.Values.Min();
        }

        #region Solve Problems

        public long SolveExample1() => CountExpanded("Day14/Example.txt", 10);

        [SolutionMethod(Part = 1)]
        public long SolvePart1() => CountExpanded("Day14/Input.txt", 10);

        public long SolveExample2() => CountExpanded("Day14/Example.txt", 40);

        [SolutionMethod(Part = 2)]
        public long SolvePart2() => CountExpanded("Day14/Input.txt", 40);

        #endregion
    }
}
