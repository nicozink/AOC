using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace AOC2022
{
    /// <summary>
    /// Solution for day 4:
    /// https://adventofcode.com/2022/day/4
    /// </summary>
    [TestClass]
    public class Day04
    {
        /// <summary>
        /// Stores the indices for two pairs of cleaning crew. These
        /// may overlap, which means that elves will clean the same
        /// areas.
        /// </summary>
        /// <param name="Start1">The start for the first elf.</param>
        /// <param name="End1">The end for the first elf.</param>
        /// <param name="Start2">The start for the second elf.</param>
        /// <param name="End2">The end for the second elf.</param>
        record CleaningPair(int Start1, int End1, int Start2, int End2)
        {
            /// <summary>
            /// Checks whether one schedule is fully contained in the other.
            /// </summary>
            /// <returns>True if it is fully contained.</returns>
            internal bool IsFullyContained()
            {
                return (Start1 <= Start2 && End1 >= End2) || (Start1 >= Start2 && End1 <= End2);
            }

            /// <summary>
            /// Checks whether the two pairs are overlapping in any way.
            /// </summary>
            /// <returns>True if they are overlapping.</returns>
            internal bool IsOverlapping()
            {
                if (IsFullyContained())
                {
                    return true;
                }

                return (Start1 >= Start2 && Start1 <= End2) || (End1 >= Start2 && End1 <= End2);
            }
        }

        /// <summary>
        /// Reads the pairs to get the cleaning schedule for the elves. The schedule
        /// is stored in the format s1,e1-s2,e2 over multiple lines.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The cleaning schedule.</returns>
        private static IEnumerable<CleaningPair> ReadInput(string path)
        {
            var lines = System.IO.File.ReadLines(path);

            foreach (var line in lines)
            {
                var pairs = line.Split(',', '-')
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                yield return new CleaningPair(pairs[0], pairs[1], pairs[2], pairs[3]);
            }
        }

        /// <summary>
        /// Read the schedule from the file, and read the number of pairs
        /// where one is fully contained in the other.
        /// </summary>
        /// <param name="path">The input file.</param>
        /// <returns>The number of fully contained pairs.</returns>
        private static int CountFullyContainedPairs(string path)
        {
            var cleaningPairs = ReadInput(path);

            var fullyContained = cleaningPairs
                .Where(x => x.IsFullyContained());

            return fullyContained.Count();
        }

        /// <summary>
        /// Read the schedule from the file, and read the number of pairs
        /// where one is fully contained in the other.
        /// </summary>
        /// <param name="path">The input file.</param>
        /// <returns>The number of overlapping pairs.</returns>
        private static int CountOverlappingPairs(string path)
        {
            var cleaningPairs = ReadInput(path);

            var overlapping = cleaningPairs
                .Where(x => x.IsOverlapping());

            return overlapping.Count();
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(2, CountFullyContainedPairs("AOC2022/Day04/Example.txt"));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(556, CountFullyContainedPairs("AOC2022/Day04/Input.txt"));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(4, CountOverlappingPairs("AOC2022/Day04/Example.txt"));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(876, CountOverlappingPairs("AOC2022/Day04/Input.txt"));

        #endregion
    }
}
