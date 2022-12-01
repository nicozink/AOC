using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace AOC2021
{
    internal static class Day01Helper
    {
        /// <summary>
        /// Takes a set of numbers, and returns them as
        /// adjacent pairs.
        /// </summary>
        /// <param name="numbers">The numbers</param>
        /// <returns>The pairs.</returns>
        public static IEnumerable<(int num1, int num2)> GetPairs(this IEnumerable<int> numbers)
        {
            var num1 = numbers.ElementAt(0);

            foreach (var num2 in numbers.Skip(1))
            {
                yield return (num1, num2);
                num1 = num2;
            }
        }

        /// <summary>
        /// Takes a set of numbers, and returns them as
        /// adjacent triples.
        /// </summary>
        /// <param name="numbers">The numbers</param>
        /// <returns>The triples.</returns>
        public static IEnumerable<(int num1, int num2, int num3)> GetTriples(this IEnumerable<int> numbers)
        {
            var num1 = numbers.ElementAt(0);
            var num2 = numbers.ElementAt(1);

            foreach (var num3 in numbers.Skip(2))
            {
                yield return (num1, num2, num3);
                num1 = num2;
                num2 = num3;
            }
        }
    }

    /// <summary>
    /// Solution for day 1:
    /// https://adventofcode.com/2021/day/1
    /// </summary>
    [TestClass]
    public class Day01
    {
        /// <summary>
        /// Reads in a file, and counts the the number of
        /// times a value increases from the previous one.
        /// </summary>
        /// <param name="path">The input file path.</param>
        /// <returns>The number of times a value increased.</returns>
        private int CountSingleIncreased(String path)
        {
            var numbers = IO.ReadNumbers(path);
            var pairs = numbers.GetPairs();

            return pairs.Count(x => x.num1 < x.num2);
        }

        /// <summary>
        /// Reads in a file. Considers a rolling window of
        /// three values. Counts the the number of times
        /// the sum of those values increases from the
        /// previous one.
        /// </summary>
        /// <param name="path">The input file path.</param>
        /// <returns>The number of times a value increased.</returns>
        private int CountTripleIncreased(String path)
        {
            var numbers = IO.ReadNumbers(path);
            var tripleSums = numbers.GetTriples().Select(x => x.num1 + x.num2 + x.num3);
            var pairs = tripleSums.GetPairs();

            return pairs.Count(x => x.num1 < x.num2);
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(7, CountSingleIncreased("AOC2021/Day01/Example.txt"));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(1583, CountSingleIncreased("AOC2021/Day01/Input.txt"));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(5, CountTripleIncreased("AOC2021/Day01/Example.txt"));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(1627, CountTripleIncreased("AOC2021/Day01/Input.txt"));

        #endregion
    }
}
