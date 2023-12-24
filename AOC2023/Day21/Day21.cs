using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace AOC2023
{
    /// <summary>
    /// Solution for day 21:
    /// https://adventofcode.com/2023/day/21
    /// </summary>
    [TestClass]
    public class Day21
    {
        /// <summary>
        /// Stores a position on the grid.
        /// </summary>
        /// <param name="Row">The row.</param>
        /// <param name="Col">The column.</param>
        record Position(int Row, int Col);

        /// <summary>
        /// Gets the midpoint along the grid, which is used to
        /// calculate the starting point.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <returns>The mid point.</returns>
        private static Position GetMidpoint(string[] grid)
        {
            return new(grid.Length / 2, grid[0].Length / 2);
        }

        /// <summary>
        /// Reads the grid from the input file.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The grid.</returns>
        private static string[] ReadInput(string path)
        {
            var lines = File.ReadAllLines(path);

            // We don't actually care where the starting point is, as it is always the
            // midpoint. So just make it an empty space.
            var startPos = GetMidpoint(lines);
            lines[startPos.Row] = lines[startPos.Row].Replace('S', '.');

            return lines;
        }

        /// <summary>
        /// Checks whether a position on the grid is valid.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="position">The position on the grid.</param>
        /// <returns>True if the position is valid.</returns>
        private static bool IsValid(string[] grid, Position position)
        {
            if (position.Row < 0 || position.Row >= grid.Length)
            {
                return false;
            }

            if (position.Col < 0 || position.Col >= grid[0].Length)
            {
                return false;
            }

            return grid[position.Row][position.Col] == '.';
        }

        /// <summary>
        /// Gets the neighbours of a position of a grid. This only counts garden plots,
        /// stones can't be traversed.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="position">The position on the grid.</param>
        /// <returns>The neighbours surrounding the position.</returns>
        private static IEnumerable<Position> GetNeighbours(string[] grid, Position position)
        {
            var top = position with { Row = position.Row + 1 };
            if (IsValid(grid, top))
            {
                yield return top;
            }

            var bottom = position with { Row = position.Row - 1 };
            if (IsValid(grid, bottom))
            {
                yield return bottom;
            }

            var left = position with { Col = position.Col - 1 };
            if (IsValid(grid, left))
            {
                yield return left;
            }

            var right = position with { Col = position.Col + 1 };
            if (IsValid(grid, right))
            {
                yield return right;
            }
        }

        /// <summary>
        /// Count the garden plots that are accessable from a starting position after the
        /// number of steps.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="start">The start position.</param>
        /// <param name="steps">The number of steps.</param>
        /// <returns>The number of positions which are reachable.</returns>
        private static long CountGardenPlots(string[] grid, Position start, int steps)
        {
            var positions = new HashSet<Position>()
            {
                start
            };

            for (int i = 0; i < steps; i++)
            {
                var newPositions = new HashSet<Position>();

                foreach (var position in positions)
                {
                    foreach (var neighbour in GetNeighbours(grid, position))
                    {
                        newPositions.Add(neighbour);
                    }
                }

                positions = newPositions;
            }

            return positions.Count;
        }

        /// <summary>
        /// Counts the number of garden plots which are accessible given the number of steps.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <param name="steps">The number of steps.</param>
        /// <returns></returns>
        private static long CountGardenPlots(string path, int steps)
        {
            var grid = ReadInput(path);
            var startPos = GetMidpoint(grid);

            return CountGardenPlots(grid, startPos, steps);
        }

        /// <summary>
        /// Count the garden plots which are accessible from the positions given the number of steps.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="positions">The input positions.</param>
        /// <param name="numSteps">The number of steps.</param>
        /// <returns>The numbers of accessible positions, one for each input position.</returns>
        private static long[] CountGardenPlots(string[] grid, Position[] positions, int numSteps)
        {
            var results = new long[positions.Length];
            for (var i = 0; i < positions.Length; i++)
            {
                results[i] = CountGardenPlots(grid, positions[i], numSteps);
            }

            return results;
        }

        /// <summary>
        /// Gets the plots that are accessable starting from the corners, after the number of steps.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="numSteps">The number of steps.</param>
        /// <returns>The numbers of accessable positions, one for each corner.</returns>
        private static long[] GetDiagonalPlots(string[] grid, int numSteps)
        {
            var corners = new Position[]
            {
                new(0, 0),
                new(0, grid.Length - 1),
                new(grid.Length - 1, 0),
                new(grid.Length - 1, grid.Length - 1)
            };

            return CountGardenPlots(grid, corners, numSteps);
        }

        /// <summary>
        /// Gets the plots that are accessable starting from the midpoints alkong the edges, after the number of steps.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="numSteps">The number of steps.</param>
        /// <returns>The numbers of accessable positions, one for each midpoint.</returns>
        private static long[] GetMidpointPlots(string[] grid, int numSteps)
        {
            var midpoint = GetMidpoint(grid);
            var midpoints = new Position[]
            {
                new(0, midpoint.Col),
                new(midpoint.Row, grid.Length - 1),
                new(grid.Length - 1, midpoint.Col),
                new(midpoint.Row, 0)
            };

            return CountGardenPlots(grid, midpoints, numSteps);
        }

        /// <summary>
        /// Count the repeating garden plots for an infinitely repeating garden, with a really large number of
        /// steps.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <param name="steps">The number of steps.</param>
        /// <returns>The number of accessable plots.</returns>
        private static long CountRepeatingGardenPlots(string path, int steps)
        {
            var grid = ReadInput(path);
            var startPos = GetMidpoint(grid);

            // This code makes some assumptions - mainly that the number of steps is a
            // mutliple of the grid size. This means we don't need to handle arbitrary
            // partially-traversed areas. The path grows out from the starting point
            // in a diamond shape, and the final step will expand the area right to the
            // edge of the grid.
            var numSteps = grid.Length - 1;
            var numGrids = steps / grid.Length;

            // The grid contains alternating entries on odd and even
            // rows/cols. It is assumed that the center is always odd, and the outermost
            // extremes are odd as well.
            //
            // The area makes an irregular four-sided polygon. If we flatten it out, we see that both
            // the odd and evens make squares.
            //
            //    E
            //   EOE      EEEE
            //  EOEOE     EEEE   OOO
            // EOEOEOE -> EEEE + OOO
            //  EOEOE     EEEE   OOO
            //   EOE
            //    E

            var evenFill = CountGardenPlots(grid, startPos, numSteps);
            var numEven = (long)numGrids * numGrids * evenFill;

            var oddFill = CountGardenPlots(grid, startPos, numSteps - 1);
            var numOdd = (long)(numGrids - 1) * (numGrids - 1) * oddFill;

            // Additionally, we have some incomplete grid patterns we need
            // to consider.
            //
            //      | /\ |
            //      |/  \|
            //     /|    |\
            // ----------------
            // | /  |  E |  \ |
            // |/   |    |   \|
            // ----------------
            //
            // The little bits of triangles are 1/4 of the area and 3/4 of the
            // area. Marching the grid length from a corner covers 1/2 the area,
            // so we adjust the steps accordingly.

            var midpointsFill = GetMidpointPlots(grid, grid.Length - 1);
            var numMidpoints = midpointsFill.Sum();

            var evenDiagonalFill = GetDiagonalPlots(grid, grid.Length / 2 - 1); // This marches 1/4 of the square.
            var numEvenDiagonals = evenDiagonalFill.Sum() * numGrids;

            var oddDiagonalFill = GetDiagonalPlots(grid, (3 * grid.Length / 2) - 1); // This marches 3/4 of the square.
            var numOddDiagonals = oddDiagonalFill.Sum() * (numGrids - 1);

            return numOdd + numEven + numMidpoints + numOddDiagonals + numEvenDiagonals;
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(16, CountGardenPlots("AOC2023/Day21/Example1.txt", steps: 6));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(3632, CountGardenPlots("AOC2023/Day21/Input.txt", steps: 64));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(625, CountRepeatingGardenPlots("AOC2023/Day21/Example2.txt", steps: 24));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(600336060511101, CountRepeatingGardenPlots("AOC2023/Day21/Input.txt", steps: 26501365));

        #endregion
    }
}
