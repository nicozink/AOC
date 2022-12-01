using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace AOC2022
{
    /// <summary>
    /// Solution for day 1:
    /// https://adventofcode.com/2022/day/1
    /// </summary>
    [TestClass]
    public class Day01
    {
        /// <summary>
        /// Reads the total calories carried by each elf from the file.
        /// The input consists of a number of calories per line, separated
        /// by a newline for each elf.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The calories per elf.</returns>
        private static IEnumerable<int> GetTotalCalories(string path)
        {
            string[] lines = System.IO.File.ReadAllLines(path);
            
            var enumerator = lines.GetEnumerator();
            while (enumerator.MoveNext())
            {
                int total = 0;
                do
                {
                    total += Convert.ToInt32(enumerator.Current);
                }
                while (enumerator.MoveNext() && !string.IsNullOrEmpty((string)enumerator.Current));

                yield return total;
            }
        }

        /// <summary>
        /// Gets the total number of calories carried by the number of
        /// elves that are carrying the most.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <param name="numElves">The number of elves.</param>
        /// <returns>The total calories carried by the top elves.</returns>
        private static int GetLargetsTotalCalories(string path, int numElves = 1)
        {
            return GetTotalCalories(path)
                .OrderByDescending(x => x)
                .Take(numElves)
                .Sum();
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(24000, GetLargetsTotalCalories("AOC2022/Day01/Example.txt"));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(71471, GetLargetsTotalCalories("AOC2022/Day01/Input.txt"));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(45000, GetLargetsTotalCalories("AOC2022/Day01/Example.txt", 3));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(211189, GetLargetsTotalCalories("AOC2022/Day01/Input.txt", 3));

        #endregion
    }
}
