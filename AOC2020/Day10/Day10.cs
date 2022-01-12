using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AOC2020
{
    /// <summary>
    /// Solution for day 10:
    /// https://adventofcode.com/2020/day/10
    /// </summary>
    [TestClass]
    public class Day10
    {
        /// <summary>
        /// Reads the list of adapters. This adds 0 as the
        /// first input, and adds 3 to the maximum. The
        /// list is sorted.
        /// </summary>
        /// <param name="path">The input file</param>
        /// <returns>The output list.</returns>
        private List<int> ReadAdapterList(string path)
        {
            var numbers = System.IO.File.ReadLines(path)
                            .Select(x => int.Parse(x))
                            .ToList();

            numbers.Sort();

            numbers.Insert(0, 0);
            numbers.Add(numbers.Last() + 3);
            return numbers;
        }

        /// <summary>
        /// Calculates a valid sequence of adapters given
        /// the input of jolts, and returns the produce
        /// of 1-jolt differences multiplied by the number
        /// of 3-jolt differences
        /// </summary>
        /// <param name="path">The input giving the jolts</param>
        /// <returns>The product of differences.</returns>
        public int GetJoltProduct(String path)
        {
            List<int> numbers = ReadAdapterList(path);

            int countDiff1 = 0;
            int countDiff3 = 0;

            for (int i = 1; i < numbers.Count; ++i)
            {
                int difference = numbers[i] - numbers[i - 1];

                if (difference == 1)
                {
                    ++countDiff1;
                }
                else if (difference == 3)
                {
                    ++countDiff3;
                }
            }

            return countDiff1 * countDiff3;
        }

        /// <summary>
        /// Given a list of adapters, and a current and new position,
        /// determines whether the two positions are valid.
        /// </summary>
        /// <param name="adapters">The list of adapters.</param>
        /// <param name="currentPosition">The current position.</param>
        /// <param name="newPosition">The next position.</param>
        /// <returns>True if these are compatible.</returns>
        private bool CanAdvance(List<int> adapters, int currentPosition, int newPosition)
        {
            // False if the new position is beyond the end.
            if (newPosition > adapters.Count - 1)
            {
                return false;
            }

            var last = adapters[currentPosition];
            var next = adapters[newPosition];

            // False if the next one is much larger than the current one.
            if (next - last > 3)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Count the number of valid arrangements, given a list of adapters.
        /// </summary>
        /// <param name="adapters">The list of adapters.</param>
        /// <param name="currentPosition">The current position.</param>
        /// <param name="lookupCache">A cache to store previous results.</param>
        /// <returns>The number of valid combinations of adapters.</returns>
        private long CountValdidArrangements(List<int> adapters, int currentPosition, Dictionary<int, long> lookupCache)
        {
            // Return a previous calculation, if it's available.
            if (lookupCache.ContainsKey(currentPosition))
            {
                return lookupCache[currentPosition];
            }

            // If we reach the end of the list, we've found a valid combination.
            if (currentPosition == adapters.Count - 1)
            {
                return 1;
            }

            long result = 0;

            // We start at the current position, and check all subsequent adapters
            // that are within range.

            int nextPosition = currentPosition + 1;
            while (CanAdvance(adapters, currentPosition, nextPosition))
            {
                // Recursively check all combinations starting at the next position.

                result += CountValdidArrangements(adapters, nextPosition, lookupCache);
                ++nextPosition;
            }

            // Save the result for the current position for the next time.
            lookupCache.Add(currentPosition, result);

            return result;
        }

        /// <summary>
        /// Counts the number of valid arrangements in the sequence.
        /// </summary>
        /// <param name="path">The input path.</param>
        /// <returns>The number of valid arrangements</returns>
        public long CountValidArrangements(string path)
        {
            var numbers = ReadAdapterList(path);
            var lookupCache = new Dictionary<int, long>();

            return CountValdidArrangements(numbers, 0, lookupCache);
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(35, GetJoltProduct("Day10/Example1.txt"));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(220, GetJoltProduct("Day10/Example2.txt"));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(2277, GetJoltProduct("Day10/Input.txt"));

        [TestMethod]
        public void SolveExample3() => Assert.AreEqual(8, CountValidArrangements("Day10/Example1.txt"));

        [TestMethod]
        public void SolveExample4() => Assert.AreEqual(19208, CountValidArrangements("Day10/Example2.txt"));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(37024595836928, CountValidArrangements("Day10/Input.txt"));

        #endregion
    }
}
