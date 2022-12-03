using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace AOC2022
{
    /// <summary>
    /// Solution for day 3:
    /// https://adventofcode.com/2022/day/3
    /// </summary>
    [TestClass]
    public class Day03
    {
        /// <summary>
        /// Gets the priority of the rucksack item so that
        /// item types a through z have priorities 1 through 26,
        /// and uppercase item types A through Z have priorities
        /// 27 through 52.
        /// </summary>
        /// <param name="c">The character.</param>
        /// <returns>The priority.</returns>
        private static int GetPriority(char c)
        {
            if (char.IsUpper(c))
            {
                return c - 'A' + 27;
            }

            return c - 'a' + 1;
        }

        /// <summary>
        /// Gets the priority of the items that are placed incorrectly
        /// in both compartments of each rucksack.
        /// </summary>
        /// <param name="path">The file input describing the rucksacks.</param>
        /// <returns>The sum of incorrect item priorities.</returns>
        private static int GetDoubleItemPriority(string path)
        {
            var lines = System.IO.File.ReadLines(path);

            var priorities = new List<int>();
            foreach (var line in lines)
            {
                var comparmentSize = line.Length / 2;
                var compartment1 = line[..comparmentSize];
                var comparment2 = line[comparmentSize..];

                var commonItem = compartment1.Intersect(comparment2);
                var priority = GetPriority(commonItem.First());
                priorities.Add(priority);
            }

            return priorities.Sum();
        }

        /// <summary>
        /// Gets the badge for each group of three rucksacks, and
        /// retuns the sum of the badge priorities.
        /// </summary>
        /// <param name="path">The file input describing the rucksacks.</param>
        /// <returns>The sum of the badge priorities.</returns>
        private static int GetBadgeSum(string path)
        {
            var lines = System.IO.File.ReadLines(path).ToArray();

            var badges = new List<int>();
            for (var i = 0; i < lines.Length; i+= 3)
            {
                var commonItem = lines[i]
                    .Intersect(lines[i + 1])
                    .Intersect(lines[i + 2]);

                var priority = GetPriority(commonItem.First());
                badges.Add(priority);
            }

            return badges.Sum();
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(157, GetDoubleItemPriority("AOC2022/Day03/Example.txt"));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(7691, GetDoubleItemPriority("AOC2022/Day03/Input.txt"));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(70, GetBadgeSum("AOC2022/Day03/Example.txt"));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(2508, GetBadgeSum("AOC2022/Day03/Input.txt"));

        #endregion
    }
}
