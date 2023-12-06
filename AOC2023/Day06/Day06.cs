using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AOC2023
{
    /// <summary>
    /// Solution for day 6:
    /// https://adventofcode.com/2023/day/6
    /// </summary>
    [TestClass]
    public class Day06
    {
        /// <summary>
        /// Reads the numbers from the input. Given the option, the seperate numbers
        /// are concatenated into one large number.
        /// </summary>
        /// <param name="line">The input line containing the numbers.</param>
        /// <param name="concatNumbers">The option whether to concatenate.</param>
        /// <returns>The numbers.</returns>
        private static long[] ReadNumbers(string line, bool concatNumbers)
        {
            // If we want to concatenate the numbers, just remove all spaces.
            if (concatNumbers)
            {
                line = line.Replace(" ", "");
            }
            else
            {
                // First remove all double spaces
                int length = 0;
                do
                {
                    length = line.Length;
                    line = line.Replace("  ", " ");
                }
                while (length != line.Length);

                // Then remove the space after the colon, which we're using as a seperator.
                line = line.Replace(": ", ":");
            }

            var splitLine = line.Split(":");
            var numberString = splitLine[1];
            var numbers = numberString.Split();

            return numbers
                .Select(long.Parse)
                .ToArray();
        }

        /// <summary>
        /// Count the number of possible winning races given the time and distance.
        /// </summary>
        /// <param name="time">The time available in the race.</param>
        /// <param name="distance">The distance needing to be covered.</param>
        /// <returns>The number of winning races.</returns>
        private static long CountWinningRaces(long time, long distance)
        {
            long sum = 0;
            for (int waitTime = 0; waitTime <= time; waitTime++)
            {
                long timeRemaining = time - waitTime;
                long distanceTravelled = waitTime * timeRemaining;

                if (distanceTravelled > distance)
                {
                    sum++;
                }
            }

            return sum;
        }

        /// <summary>
        /// Count the number of winning combinations, given that races can be won in different
        /// ways.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <param name="concatNumbers">Whether to concatenate all numbers into a single race.</param>
        /// <returns>The number of winning race combinations.</returns>
        private static long FindWinningCombinations(string path, bool concatNumbers)
        {
            var raceLines = System.IO.File.ReadAllLines(path);

            var times = ReadNumbers(raceLines[0], concatNumbers);
            var distances = ReadNumbers(raceLines[1], concatNumbers);

            long product = 1;
            for (long i = 0; i < times.Length; i++)
            {
                var time = times[i];
                var distance = distances[i];

                product *= CountWinningRaces(time, distance);
            }

            return product;
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(288, FindWinningCombinations("AOC2023/Day06/Example.txt", concatNumbers: false));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(32076, FindWinningCombinations("AOC2023/Day06/Input.txt", concatNumbers: false));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(71503, FindWinningCombinations("AOC2023/Day06/Example.txt", concatNumbers: true));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(34278221, FindWinningCombinations("AOC2023/Day06/Input.txt", concatNumbers: true));

        #endregion
    }
}
