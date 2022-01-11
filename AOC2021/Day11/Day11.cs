using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using AOC2021.Day11Utils;

namespace AOC2021
{
    namespace Day11Utils
    {
        internal static class Helper
        {
            /// <summary>
            /// Gets all neighbours surrounding a specific index.
            /// </summary>
            /// <param name="input">The grid.</param>
            /// <param name="idx">The index.</param>
            /// <returns>The neighbours.</returns>
            public static IEnumerable<(int x, int y)> GetNeighbours(this int[][] input, (int x, int y) idx)
            {
                int startx = Math.Max(idx.x - 1, 0);
                int endx = Math.Min(idx.x + 1, input.Length - 1);

                int starty = Math.Max(idx.y - 1, 0);
                int endy = Math.Min(idx.y + 1, input.Length - 1);

                for (int x = startx; x <= endx; x++)
                {
                    for (int y = starty; y <= endy; y++)
                    {
                        if (x != idx.x || y != idx.y)
                        {
                            yield return (x, y);
                        }
                    }
                }
            }

            /// <summary>
            /// Enumerates all indices in the grid.
            /// </summary>
            /// <param name="input">The input.</param>
            /// <returns>The indices.</returns>
            public static IEnumerable<(int x, int y)> GetAll(this int[][] input)
            {
                for (int x = 0; x < input.Length; x++)
                {
                    for (int y = 0; y < input[0].Length; y++)
                    {
                        yield return (x, y);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Solution for day 11:
    /// https://adventofcode.com/2021/day/11
    /// </summary>
    [TestClass]
    public class Day11
    {
        /// <summary>
        /// Gets the number of octupus which flash each turn.
        /// Stops when flashing starts to synchronise.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>The octupus which flash.</returns>
        IEnumerable<int> GetFlashes(int[][] input)
        {
            while (true)
            {
                var flashing = new HashSet<(int x, int y)>();

                foreach (var (x, y) in input.GetAll())
                {
                    input[x][y]++;
                }

                int currentlyFlashing;
                do
                {
                    currentlyFlashing = flashing.Count;

                    foreach (var idx in input.GetAll())
                    {
                        if (input[idx.x][idx.y] > 9 && !flashing.Contains(idx))
                        {
                            foreach (var (x, y) in input.GetNeighbours(idx))
                            {
                                input[x][y]++;
                            }

                            flashing.Add(idx);
                        }
                    }
                }
                while (currentlyFlashing != flashing.Count);

                foreach (var (x, y) in flashing)
                {
                    input[x][y] = 0;
                }

                yield return flashing.Count;

                // We stop when all octupus were flashing.
                if (flashing.Count == input.Length * input[0].Length)
                {
                    yield break;
                }
            }
        }

        /// <summary>
        /// Count the number of flashes which happen in 100 steps.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The number of flashes.</returns>
        int CountFlashes(String path)
        {
            var input = System.IO.File.ReadLines(path)
                .Select(x => x.Select(x => int.Parse(x.ToString())).ToArray())
                .ToArray();

            return GetFlashes(input).Take(100).Sum();
        }

        /// <summary>
        /// Count the steps until all flashing synchronises.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The number of steps.</returns>
        int CountSteps(String path)
        {
            var input = System.IO.File.ReadLines(path)
                .Select(x => x.Select(x => int.Parse(x.ToString())).ToArray())
                .ToArray();

            return GetFlashes(input).Count();
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(1656, CountFlashes("Day11/Example.txt"));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(1743, CountFlashes("Day11/Input.txt"));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(195, CountSteps("Day11/Example.txt"));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(364, CountSteps("Day11/Input.txt"));

        #endregion
    }
}
