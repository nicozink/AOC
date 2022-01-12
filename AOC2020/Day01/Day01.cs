using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AOC2020
{
    using Pair = Tuple<int, int>;
    using Triple = Tuple<int, int, int>;

    /// <summary>
    /// Solution for day 1:
    /// https://adventofcode.com/2020/day/1
    /// </summary>
    [TestClass]
    public class Day01
    {
        public int GetSolution1(String path)
        {
            var numbers = Common.IO.ReadNumbers(path).ToList();

            var pair = GetPairs(numbers)
                .First(x => x.Item1 + x.Item2 == 2020);

            return pair.Item1 * pair.Item2;
        }

        public int GetSolution2(String path)
        {
            var numbers = Common.IO.ReadNumbers(path).ToList();

            var pair = GetTriples(numbers)
                .First(x => x.Item1 + x.Item2 + x.Item3 == 2020);

            return pair.Item1 * pair.Item2 * pair.Item3;
        }

        /// <summary>
        /// Gets all pairs of numbers from the list.
        /// </summary>
        /// <param name="list">The list of numbers.</param>
        /// <returns>The collection of pairs.</returns>
        private IEnumerable<Pair> GetPairs(List<int> list)
        {
            for (int x = 0; x < list.Count; ++x)
            {
                for (int y = 0; y < list.Count; ++y)
                {
                    if (x == y)
                    {
                        continue;
                    }

                    yield return Tuple.Create(list[x], list[y]);
                }
            }
        }

        /// <summary>
        /// Gets all triples of numbers from the list.
        /// </summary>
        /// <param name="list">The list of numbers.</param>
        /// <returns>The collection of triples.</returns>
        private IEnumerable<Triple> GetTriples(List<int> list)
        {
            for (int x = 0; x < list.Count; ++x)
            {
                for (int y = 0; y < list.Count; ++y)
                {
                    for (int z = 0; z < list.Count; ++z)
                    {
                        if (x == y || x == z || y == z)
                        {
                            continue;
                        }

                        yield return Tuple.Create(list[x], list[y], list[z]);
                    }
                }
            }
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(514579, GetSolution1("Day01/Example.txt"));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(927684, GetSolution1("Day01/Input.txt"));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(241861950, GetSolution2("Day01/Example.txt"));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(292093004, GetSolution2("Day01/Input.txt"));

        #endregion
    }
}
