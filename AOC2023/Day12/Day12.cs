using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace AOC2023
{
    /// <summary>
    /// Solution for day 12:
    /// https://adventofcode.com/2023/day/12
    /// </summary>
    [TestClass]
    public class Day12
    {
        /// <summary>
        /// Stores an arrangement of springs containing a pattern, and the
        /// groups of springs.
        /// </summary>
        /// <param name="Pattern">The pattern which we use to match springs.</param>
        /// <param name="Springs">The springs which we need to find in the pattern.</param>
        private record Arrangement(string Pattern, int[] Springs);

        /// <summary>
        /// A node which captures the current state of a search, so we can cache previous
        /// values and speed up the computation.
        /// </summary>
        /// <param name="PatternIndex">The index into the pattern string we're investigating.</param>
        /// <param name="PatternLength">The length of the current pattern we've found so far.</param>
        /// <param name="SpringIndex">The index into the spring we're currently looking for.</param>
        private record LookupCache(int PatternIndex, int PatternLength, int SpringIndex);

        /// <summary>
        /// Read the input from the source file.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <param name="unfoldInput">Whether to unfold the input for part 2.</param>
        /// <returns>The arrangements of springs.</returns>
        private static IEnumerable<Arrangement> ReadInput(string path, bool unfoldInput)
        {
            var grid = System.IO.File.ReadAllLines(path);

            foreach (var line in grid)
            {
                var splitLine = line.Split();

                var patternString = splitLine[0];
                var springsString = splitLine[1];
                if (unfoldInput)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        patternString += '?' + splitLine[0];
                        springsString += ',' + splitLine[1];
                    }
                }

                var pattern = patternString + '.'; // Append a dot to make matching easier
                var springs = springsString
                    .Split(',')
                    .Select(int.Parse)
                    .ToArray();

                yield return new(pattern, springs);
            }
        }

        /// <summary>
        /// This counts an empty spot which does not contain a spring. We need to close off
        /// the current group, and fail if we haven't matched the required group size.
        /// </summary>
        /// <param name="arrangement">The spring arrangement.</param>
        /// <param name="cache">The lookup cache containing previous values.</param>
        /// <param name="patternIndex">The index into the pattern string we're investigating.</param>
        /// <param name="patternLength">The length of the current pattern we've found so far.</param>
        /// <param name="springIndex">The index into the spring we're currently looking for.</param>
        /// <returns>The number of combinations.</returns>
        private static long CountEmpty(Arrangement arrangement,
            Dictionary<LookupCache, long> cache, int patternIndex,
            int patternLength, int springIndex)
        {
            // If we're reading a pattern, and we haven't matched the length, then
            // the pattern is invalid.
            if (patternLength != 0 && patternLength != arrangement.Springs[springIndex])
            {
                return 0;
            }

            // If the pattern matches, then we start looking for the next one.
            if (patternLength != 0 && patternLength == arrangement.Springs[springIndex])
            {
                springIndex++;
            }

            return CountCombinations(arrangement, cache, patternIndex + 1, 0, springIndex);
        }

        /// <summary>
        /// We count a space that contains a spring. We need to increment the current group, but
        /// fail if it grows larger than the spring group we're looking for.
        /// </summary>
        /// <param name="arrangement">The spring arrangement.</param>
        /// <param name="cache">The lookup cache containing previous values.</param>
        /// <param name="patternIndex">The index into the pattern string we're investigating.</param>
        /// <param name="patternLength">The length of the current pattern we've found so far.</param>
        /// <param name="springIndex">The index into the spring we're currently looking for.</param>
        /// <returns>The number of combinations.</returns>
        private static long CountSpring(Arrangement arrangement,
            Dictionary<LookupCache, long> cache, int patternIndex,
            int patternLength, int springIndex)
        {
            if (springIndex >= arrangement.Springs.Length)
            {
                return 0;
            }

            if (patternLength != 0 && patternLength > arrangement.Springs[springIndex])
            {
                return 0;
            }

            return CountCombinations(arrangement, cache, patternIndex + 1, patternLength + 1, springIndex);
        }

        /// <summary>
        /// Count the combinations that can be found by matching the pattern.
        /// </summary>
        /// <param name="arrangement">The spring arrangement.</param>
        /// <param name="cache">The lookup cache containing previous values.</param>
        /// <param name="patternIndex">The index into the pattern string we're investigating.</param>
        /// <param name="patternLength">The length of the current pattern we've found so far.</param>
        /// <param name="springIndex">The index into the spring we're currently looking for.</param>
        /// <returns>The number of combinations.</returns>
        private static long CountCombinations(Arrangement arrangement,
            Dictionary<LookupCache, long> cache, int patternIndex,
            int patternLength, int springIndex)
        {
            if (cache.TryGetValue(new(patternIndex, patternLength, springIndex), out var value))
            {
                return value;
            }

            // If we've exhausted both the pattern and the springs, we have found a match.
            // Otherwise, if we reach the end of the pattern, then we have not.
            if (patternIndex >= arrangement.Pattern.Length &&
                springIndex >= arrangement.Springs.Length)
            {
                return 1;
            }
            else if (patternIndex >= arrangement.Pattern.Length)
            {
                return 0;
            }

            long result;
            var currentChar = arrangement.Pattern[patternIndex];
            if (currentChar == '.')
            {
                result = CountEmpty(arrangement, cache, patternIndex, patternLength, springIndex);
            }
            else if (currentChar == '#')
            {
                result = CountSpring(arrangement, cache, patternIndex, patternLength, springIndex);
            }
            else
            {
                result = CountEmpty(arrangement, cache, patternIndex, patternLength, springIndex) +
                    CountSpring(arrangement, cache, patternIndex, patternLength, springIndex);
            }

            cache.Add(new(patternIndex, patternLength, springIndex), result);
            
            return result;
        }

        /// <summary>
        /// Count the combinations that can be found by matching the patterns in the input.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <param name="unfoldInput">Whether to unfold the input for part 2.</param>
        /// <returns>The number of combinations in the patterns.</returns>
        private static long CountCombinations(string path, bool unfoldInput)
        {
            long sum = 0;

            var arrangements = ReadInput(path, unfoldInput);
            foreach(var arrangement in arrangements)
            {
                var cache = new Dictionary<LookupCache, long>();
                sum += CountCombinations(arrangement, cache, 0, 0, 0);
            }

            return sum;
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(21, CountCombinations("AOC2023/Day12/Example.txt", unfoldInput: false));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(7407, CountCombinations("AOC2023/Day12/Input.txt", unfoldInput: false));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(525152, CountCombinations("AOC2023/Day12/Example.txt", unfoldInput: true));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(30568243604962, CountCombinations("AOC2023/Day12/Input.txt", unfoldInput: true));


        #endregion
    }
}
