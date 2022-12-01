using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AOC2020
{
    /// <summary>
    /// Solution for day 15:
    /// https://adventofcode.com/2020/day/15
    /// </summary>
    [TestClass]
    public class Day15
    {
        /// <summary>
        /// Gets the nth number, based on the starting numbers.
        /// </summary>
        /// <param name="starting">The starting numbers.</param>
        /// <param name="n">The turn n.</param>
        /// <returns>The nth number.</returns>
        public static int GetNthNumber(List<int> starting, int n)
        {
            // We process the starting numbers by putting them in a dictionary
            // that stores the last turn. We exclude the last number, since that
            // will need to be processed the same way we handle new numbers.
            var history = starting.GetRange(0, starting.Count - 1)
                .Select((num, index) => new Tuple<int, int>(num, index + 1))
                .ToDictionary(tuple => tuple.Item1, tuple => tuple.Item2);

            var lastNumber = starting.Last();
            var lastTurn = starting.Count;

            while (true)
            {
                var nextNumber = 0;

                // We get the next number based on the last number, and
                // the previous history based on the last turn.
                if (history.ContainsKey(lastNumber))
                {
                    nextNumber = lastTurn - history[lastNumber];
                }

                // We needed the history of the last number before the
                // last turn, but it's safe to update the hitory now.
                history[lastNumber] = lastTurn;

                var nextTurn = lastTurn + 1;
                if (nextTurn == n)
                {
                    return nextNumber;
                }

                lastNumber = nextNumber;
                lastTurn = nextTurn;
            }
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1()
        {
            Assert.AreEqual(436, GetNthNumber(new List<int> { 0, 3, 6 }, 2020));
            Assert.AreEqual(1, GetNthNumber(new List<int> { 1, 3, 2 }, 2020));
            Assert.AreEqual(10, GetNthNumber(new List<int> { 2, 1, 3 }, 2020));
            Assert.AreEqual(27, GetNthNumber(new List<int> { 1, 2, 3 }, 2020));
            Assert.AreEqual(78, GetNthNumber(new List<int> { 2, 3, 1 }, 2020));
            Assert.AreEqual(438, GetNthNumber(new List<int> { 3, 2, 1 }, 2020));
            Assert.AreEqual(1836, GetNthNumber(new List<int> { 3, 1, 2 }, 2020));
        }

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(496, GetNthNumber(new List<int> { 2, 0, 1, 7, 4, 14, 18 }, 2020));

        [TestMethod]
        public void SolveExample2()
        {
            Assert.AreEqual(175594, GetNthNumber(new List<int> { 0, 3, 6 }, 30000000));
            Assert.AreEqual(2578, GetNthNumber(new List<int> { 1, 3, 2 }, 30000000));
            Assert.AreEqual(3544142, GetNthNumber(new List<int> { 2, 1, 3 }, 30000000));
            Assert.AreEqual(261214, GetNthNumber(new List<int> { 1, 2, 3 }, 30000000));
            Assert.AreEqual(6895259, GetNthNumber(new List<int> { 2, 3, 1 }, 30000000));
            Assert.AreEqual(18, GetNthNumber(new List<int> { 3, 2, 1 }, 30000000));
            Assert.AreEqual(362, GetNthNumber(new List<int> { 3, 1, 2 }, 30000000));
        }

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(883, GetNthNumber(new List<int> { 2, 0, 1, 7, 4, 14, 18 }, 30000000));

        #endregion
    }
}
