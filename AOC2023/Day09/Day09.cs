using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace AOC2023
{
    /// <summary>
    /// Solution for day 9:
    /// https://adventofcode.com/2023/day/9
    /// </summary>
    [TestClass]
    public class Day09
    {
        /// <summary>
        /// Reads the sequences of numbers from the input file.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The sequences from the file.</returns>
        private static IEnumerable<int[]> ReadInput(string path)
        {
            var lines = System.IO.File.ReadAllLines(path);

            foreach (var line in lines)
            {
                yield return line
                    .Split()
                    .Select(int.Parse)
                    .ToArray();
            }
        }

        /// <summary>
        /// Take a sequence of numbers, and extrapolate the previous and next numbers.
        /// </summary>
        /// <param name="sequence">The sequence of numbers.</param>
        /// <returns>The previous and next numbers.</returns>
        private static (int Prev, int Next) Extrapolate(int[] sequence)
        {
            if (sequence.All(x => x == 0))
            {
                return (0, 0);
            }

            var newSequence = new int[sequence.Length - 1];
            for (int i = 0; i < newSequence.Length; i++)
            {
                newSequence[i] = sequence[i + 1] - sequence[i];
            }

            var (prev, next) = Extrapolate(newSequence);
            return (sequence[0] - prev, sequence[^1] + next);
        }

        /// <summary>
        /// Read the input, and create a sum of each extrapolated pair of numebrs.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The sum of each extrapolated pair of numbers.</returns>
        private static (int Prev, int Next) SumExtrapolatedValues(string path)
        {
            var input = ReadInput(path);

            int prevSum = 0;
            int nextSum = 0;
            foreach (var sequence in input)
            {
                var (prev, next) = Extrapolate(sequence);
                prevSum += prev;
                nextSum += next;
            }

            return (prevSum, nextSum);
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample()
        {
            var (prev, next) = SumExtrapolatedValues("AOC2023/Day09/Example.txt");
            Assert.AreEqual(114, next); // Part 1
            Assert.AreEqual(2, prev); // Part 2
        }

        [TestMethod]
        public void SolveInput()
        {
            var (prev, next) = SumExtrapolatedValues("AOC2023/Day09/Input.txt");
            Assert.AreEqual(1980437560, next); // Part 1
            Assert.AreEqual(977, prev); // Part 2
        }

        #endregion
    }
}
