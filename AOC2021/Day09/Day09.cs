using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace AOC2021
{
    /// <summary>
    /// Solution for day 9:
    /// https://adventofcode.com/2021/day/9
    /// </summary>
    [TestClass]
    public class Day09
    {
        /// <summary>
        /// Gets the valid neighbours surrounding an index of the grid.
        /// </summary>
        /// <param name="input">The input grid.</param>
        /// <param name="idx">The index.</param>
        /// <returns>The neighbours.</returns>
        IEnumerable<(int x, int y)> GetNeighbours(string[] input, (int x, int y) idx)
        {
            if (idx.x > 0)
            {
                yield return (idx.x - 1, idx.y);
            }

            if (idx.x < input.Length - 1)
            {
                yield return (idx.x + 1, idx.y);
            }

            if (idx.y > 0)
            {
                yield return (idx.x, idx.y - 1);
            }

            if (idx.y < input[0].Length - 1)
            {
                yield return (idx.x, idx.y + 1);
            }
        }

        /// <summary>
        /// Gets the lowest local points in the grid. This is each point that only
        /// has larger neighbours.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>The lowest local points.</returns>
        private IEnumerable<(int x, int y)> GetLowPoints(string[] input)
        {
            for (int x = 0; x < input.Length; x++)
            {
                for (int y = 0; y < input[0].Length; y++)
                {
                    char c = input[x][y];
                    var neigbours = GetNeighbours(input, (x, y));
                    var lowerNeighbours = neigbours.Where(idx => input[idx.x][idx.y] <= c);

                    if (!lowerNeighbours.Any())
                    {
                        yield return (x, y);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the sum of the lowest points in the input.
        /// </summary>
        /// <param name="path">The input file path.</param>
        /// <returns>The sum of the lowest points.</returns>
        private int SumLowPoints(String path)
        {
            var input = System.IO.File.ReadAllLines(path);
            
            return GetLowPoints(input)
                .Sum(idx => 1 + int.Parse(input[idx.x][idx.y].ToString()));
        }

        /// <summary>
        /// Creates the basin from a lowest point, and returns the area. A
        /// basin spreads out from the lowest point until it reaches the highest
        /// point.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="lowPoint">The low point.</param>
        /// <returns>The area of the basin.</returns>
        private int CountBasin(string[] input, (int x, int y) lowPoint)
        {
            var basin = new HashSet<(int x, int y)>();
            var frontier = new Stack<(int x, int y)>();

            frontier.Push(lowPoint);

            while (frontier.Count > 0)
            {
                var next = frontier.Pop();

                basin.Add(next);
                
                var neighbours = GetNeighbours(input, next)
                    .Where(idx => !basin.Contains(idx) && input[idx.x][idx.y] != '9');

                foreach (var neighbour in neighbours)
                {
                    frontier.Push(neighbour);
                }
            }

            return basin.Count;
        }

        /// <summary>
        /// Finds all the lowest points and their basins, and returns the product
        /// of the three largest areas.
        /// </summary>
        /// <param name="path">The input file path.</param>
        /// <returns>The product of the areas.</returns>
        private int MultiplyBasins(String path)
        {
            var input = System.IO.File.ReadAllLines(path);

            var lowPoints = GetLowPoints(input).ToHashSet();

            var basinSizes = new List<int>();
            foreach (var lowPoint in lowPoints)
            {
                basinSizes.Add(CountBasin(input, lowPoint));
            }
            var topThree = basinSizes.OrderByDescending(x => x).Take(3);
            return topThree.Aggregate((n1, n2) => n1 * n2);
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(15, SumLowPoints("AOC2021/Day09/Example.txt"));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(436, SumLowPoints("AOC2021/Day09/Input.txt"));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(1134, MultiplyBasins("AOC2021/Day09/Example.txt"));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(1317792, MultiplyBasins("AOC2021/Day09/Input.txt"));

        #endregion
    }
}
