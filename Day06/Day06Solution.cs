using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace Solutions
{
    /// <summary>
    /// Solution for day 6:
    /// https://adventofcode.com/2021/day/6
    /// </summary>
    [SolutionClass(Day = 6)]
    public class Day06Solution
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

        public long SolveExample1() => CountFishAfterDays("Day06/Example.txt", 80);

        [SolutionMethod(Part = 1)]
        public long SolvePart1() => CountFishAfterDays("Day06/Input.txt", 80);

        public long SolveExample2() => CountFishAfterDays("Day06/Example.txt", 256);

        [SolutionMethod(Part = 2)]
        public long SolvePart2() => CountFishAfterDays("Day06/Input.txt", 256);

        #endregion
    }
}
