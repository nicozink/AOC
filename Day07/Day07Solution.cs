using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace Solutions
{
    /// <summary>
    /// Solution for day 7:
    /// https://adventofcode.com/2021/day/7
    /// </summary>
    [SolutionClass(Day = 7)]
    public class Day07Solution
    {
        /// <summary>
        /// Calculate the fuel used to travel a certain distance. The
        /// calculation can either be linear, or the sum of the sequence
        /// of consecutive numbers.
        /// </summary>
        /// <param name="distance">The distance.</param>
        /// <param name="linear">Whether do a linear calculation.</param>
        /// <returns>The fuel needed.</returns>
        private long CalculateFuel(int distance, bool linear)
        {
            if (linear)
            {
                return distance;
            }
            else
            {
                return distance * (distance + 1) / 2;
            }
        }

        /// <summary>
        /// Calculate the minimum fuel used to move all crabs to the same location.
        /// </summary>
        /// <param name="path">The input.</param>
        /// <param name="linear">Whether do a linear calculation.</param>
        /// <returns>The minimum fuel needed.</returns>

        private long CountMinimumFuel(String path, bool linear)
		{
            var numbers = System.IO.File.ReadAllText(path)
                .Split(',')
                .Select(int.Parse)
                .ToList();

            var min = numbers.Min();
            var max = numbers.Max();

            var count = max - min + 1;

            var fuelCosts = Enumerable.Range(min, count)
                .Select(x =>
                    numbers.Select(y =>
                        CalculateFuel(Math.Abs(x - y), linear))
                    .Sum());

            return fuelCosts.Min();
		}

        #region Solve Problems

        public long SolveExample1() => CountMinimumFuel("Day07/Example.txt", linear: true);

        [SolutionMethod(Part = 1)]
        public long SolvePart1() => CountMinimumFuel("Day07/Input.txt", linear: true);

        public long SolveExample2() => CountMinimumFuel("Day07/Example.txt", linear: false);

        [SolutionMethod(Part = 2)]
        public long SolvePart2() => CountMinimumFuel("Day07/Input.txt", linear: false);

        #endregion
    }
}
