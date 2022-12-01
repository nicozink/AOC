using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace AOC2021
{
    /// <summary>
    /// Solution for day 15:
    /// https://adventofcode.com/2021/day/15
    /// </summary>
    [TestClass]
    public class Day15
    {
        /// <summary>
        /// Stores an index to a value in the grid.
        /// </summary>
        /// <param name="X">The x coordinate.</param>
        /// <param name="Y">The y coordinate.</param>
        record Index(int X, int Y);

        /// <summary>
        /// Gets the valid neighbours surrounding an index of the grid.
        /// </summary>
        /// <param name="input">The input grid.</param>
        /// <param name="idx">The index.</param>
        /// <returns>The neighbours.</returns>
        IEnumerable<Index> GetNeighbours(int[,] input, Index idx)
        {
            if (idx.X > 0)
            {
                yield return new(idx.X - 1, idx.Y);
            }

            if (idx.X < input.GetLength(0) - 1)
            {
                yield return new(idx.X + 1, idx.Y);
            }

            if (idx.Y > 0)
            {
                yield return new(idx.X, idx.Y - 1);
            }

            if (idx.Y < input.GetLength(0) - 1)
            {
                yield return new(idx.X, idx.Y + 1);
            }
        }

        /// <summary>
        /// Count the shortest path from the top left of the grid
        /// to the bottom right. Returns the sum of the risk
        /// of each grid position.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <returns>The shortest path.</returns>
        int CountShortestPath(int[,] grid)
        {
            var totalRisk = new int[grid.GetLength(0), grid.GetLength(0)];
            
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(0); j++)
                {
                    totalRisk[i, j] = int.MaxValue;
                }
            }

            totalRisk[0, 0] = 0;

            PriorityQueue<Index, int> frontier = new();
            frontier.Enqueue(new(0, 0), 0);

            while (frontier.Count != 0)
            {
                var next = frontier.Dequeue();

                foreach (var neighbour in GetNeighbours(grid, next))
                {
                    int oldRisk = totalRisk[neighbour.X, neighbour.Y];
                    int newRisk = totalRisk[next.X, next.Y] + grid[neighbour.X, neighbour.Y];

                    if (newRisk < oldRisk)
                    {
                        totalRisk[neighbour.X, neighbour.Y] = newRisk;
                        frontier.Enqueue(neighbour, newRisk);
                    }
                }
            }

            return totalRisk[totalRisk.GetLength(0) - 1, totalRisk.GetLength(1) - 1];
        }

        /// <summary>
        /// Count the sum of the risks along the shortest path in the input.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <param name="numTimesLarger">The number of times to repeat the grid.</param>
        /// <returns>The sum along the shortest path.</returns>
        int CountShortestPath(String path, int numTimesLarger)
        {
            var lines = System.IO.File.ReadAllLines(path);

            var grid = new int[lines.Length * numTimesLarger, lines.Length * numTimesLarger];

            for (int i = 0; i < numTimesLarger; i++)
            {
                for (int j = 0; j < numTimesLarger; j++)
                {
                    int rowOffset = i * lines.Length;
                    int colOffset = j * lines.Length;

                    for (int x = 0; x < lines.Length; x++)
                    {
                        for (int y = 0; y < lines.Length; y++)
                        {
                            int numValue = lines[x][y] - '0' + i + j;

                            while (numValue > 9)
                            {
                                numValue -= 9;
                            }

                            grid[rowOffset + x, colOffset + y] = numValue;
                        }
                    }
                }
            }

            return CountShortestPath(grid);
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(40, CountShortestPath("AOC2021/Day15/Example.txt", 1));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(386, CountShortestPath("AOC2021/Day15/Input.txt", 1));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(315, CountShortestPath("AOC2021/Day15/Example.txt", 5));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(2806, CountShortestPath("AOC2021/Day15/Input.txt", 5));

        #endregion
    }
}
