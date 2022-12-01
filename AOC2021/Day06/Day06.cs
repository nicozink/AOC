using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace AOC2021
{
    /// <summary>
    /// Solution for day 6:
    /// https://adventofcode.com/2021/day/6
    /// </summary>
    [TestClass]
    public class Day06
    {
        /// <summary>
        /// Read the fish from the input - fish are a sequence
        /// of numbers indicating the spawning time. Convert that
        /// into an array storing the count by index.
        /// </summary>
        /// <param name="path">The input path.</param>
        /// <returns>Count of fish by index.</returns>
        private long[] ReadInput(String path)
        {
            var fishInput = System.IO.File.ReadAllText(path)
                .Split(',')
                .Select(int.Parse);

            var fish = new long[7];

            foreach (var item in fishInput)
            {
                fish[item]++;
            }

            return fish;
        }

        /// <summary>
        /// Count the number of fish after the number of days.
        /// </summary>
        /// <param name="path">The input file.</param>
        /// <param name="numDays">The number of days.</param>
        /// <returns>The number of fish.</returns>
        private long CountFishAfterDays(String path, int numDays)
        {
            var fish = ReadInput(path);

            // Assume there are no new fish still in the
            // 8-day cycle.
            var newFish = new Queue<long>();
            newFish.Enqueue(0);
            newFish.Enqueue(0);

            for (int i = 0; i < numDays; i++)
            {
                int day = i % 7;
                newFish.Enqueue(fish[day]);
                fish[day] += newFish.Dequeue();
            }

            return fish.Sum() + newFish.Sum();
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(5934, CountFishAfterDays("AOC2021/Day06/Example.txt", 80));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(390923, CountFishAfterDays("AOC2021/Day06/Input.txt", 80));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(26984457539, CountFishAfterDays("AOC2021/Day06/Example.txt", 256));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(1749945484935, CountFishAfterDays("AOC2021/Day06/Input.txt", 256));

        #endregion
    }
}
