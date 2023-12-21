using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace AOC2023
{
    /// <summary>
    /// Solution for day 14:
    /// https://adventofcode.com/2023/day/14
    /// </summary>
    [TestClass]
    public class Day14
    {
        /// <summary>
        /// Stores an offset for the direction in which the rocks can move.
        /// </summary>
        /// <param name="Row">The row offset.</param>
        /// <param name="Col">The column offset.</param>
        record Direction(int Row, int Col);

        /// <summary>
        /// The directions in which the rocks can move, in the order that tilts happen.
        /// </summary>
        private static readonly List<Direction> directions = new()
        {
            new(-1, 0),
            new(0, -1),
            new(1, 0),
            new(0, 1),
        };

        /// <summary>
        /// Move the rocks in a particular direction.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="direction">The input direction.</param>
        /// <returns>True if any rocks were moved.</returns>
        private static bool MoveRocks(char[][] grid, Direction direction)
        {
            bool moved = false;
            for (int r = 0; r < grid.Length; r++)
            {
                for (int c = 0; c < grid[r].Length; c++)
                {
                    int dr = r + direction.Row;
                    int dc = c + direction.Col;

                    if (dr < 0 || dr >= grid.Length)
                    {
                        continue;
                    }

                    if (dc < 0 || dc >= grid[r].Length)
                    {
                        continue;
                    }

                    if (grid[r][c] == 'O' && grid[dr][dc] == '.')
                    {
                        grid[r][c] = '.';
                        grid[dr][dc] = 'O';

                        moved = true;
                    }
                }
            }

            return moved;
        }

        /// <summary>
        /// Calculates the load of a particular rock configuration.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <returns>The load of the rocks.</returns>
        private static int CalculateLoad(char[][] grid)
        {
            int sum = 0;
            for (int r = 0; r < grid.Length; r++)
            {
                int weight = grid.Length - r;
                int numRocks = grid[r].Count(x => x == 'O');
                sum += numRocks * weight;
            }

            return sum;
        }

        /// <summary>
        /// Calculates the load for part 1, after moving all rocks north.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The load.</returns>
        private static int CalculateLoadPart1(string path)
        {
            var input = System.IO.File.ReadAllLines(path)
                .Select(x => x.ToArray())
                .ToArray();

            while (MoveRocks(input, directions[0]))
            {

            }

            return CalculateLoad(input);
        }

        /// <summary>
        /// Do a full cycle where the grid is tilted north, west, south and east.
        /// </summary>
        /// <param name="grid">The grid.</param>
        private static void DoFullCycle(char[][] grid)
        {
            foreach (var direction in directions)
            {
                while (MoveRocks(grid, direction))
                {

                }
            }
        }

        /// <summary>
        /// Calculate the load for part 2, after doing full cycles.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The load.</returns>
        private static int CalculateLoadPart2(string path)
        {
            var input = System.IO.File.ReadAllLines(path)
                .Select(x => x.ToArray())
                .ToArray();

            int cycleOffset = 0;
            int cycleLength = 0;

            var previousCycles = new Dictionary<string, int>();
            int currentCycle = 0;
            while (true)
            {
                var currentGrid = new string(input.SelectMany(x => x).ToArray());
                if (previousCycles.TryGetValue(currentGrid, out var cycleStart))
                {
                    cycleOffset = cycleStart;
                    cycleLength = currentCycle - cycleOffset;

                    break;
                }

                previousCycles.Add(currentGrid, currentCycle++);
                DoFullCycle(input);
            }

            int numCycles = 1000000000 - cycleOffset;
            int remainingCycles = numCycles % cycleLength;

            for (int i = 0; i < remainingCycles; i++)
            {
                DoFullCycle(input);
            }

            return CalculateLoad(input);
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(136, CalculateLoadPart1("AOC2023/Day14/Example.txt"));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(110677, CalculateLoadPart1("AOC2023/Day14/Input.txt"));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(64, CalculateLoadPart2("AOC2023/Day14/Example.txt"));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(90551, CalculateLoadPart2("AOC2023/Day14/Input.txt"));


        #endregion
    }
}
