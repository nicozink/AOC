using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace AOC2023
{
    /// <summary>
    /// Solution for day 11:
    /// https://adventofcode.com/2023/day/11
    /// </summary>
    [TestClass]
    public class Day11
    {
        /// <summary>
        /// Look through the grid and return the positions of all galaxies.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <returns>The galaxy positions.</returns>
        private static IEnumerable<(int Row, int Col)> GetGalaxies(string[] grid)
        {
            for (int r = 0; r < grid.Length; r++)
            {
                for (int c = 0; c < grid[r].Length; c++)
                {
                    if (grid[r][c] != '.')
                    {
                        yield return (r, c);
                    }
                }
            }
        }

        /// <summary>
        /// Get a tally of the expansions along the rows of the grid.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <returns>The tally of areas for expansion.</returns>
        private static int[] GetRowExpansions(string[] grid)
        {
            var rowExpansions = new int[grid.Length];
            int rowExpansionsTotal = 0;
            for (int r = 0; r < grid.Length; r++)
            {
                if (grid[r].All(x => x == '.'))
                {
                    rowExpansionsTotal++;
                }

                rowExpansions[r] = rowExpansionsTotal;
            }

            return rowExpansions;
        }

        /// <summary>
        /// Get a tally of the expansions along the columns of the grid.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <returns>The tally of areas for expansion.</returns>
        private static int[] GetColExpansions(string[] grid)
        {
            var colExpansions = new int[grid[0].Length];
            int colExpansionsTotal = 0;
            for (int c = 0; c < grid[0].Length; c++)
            {
                var column = Enumerable.Range(0, grid.Length)
                    .Select(x => grid[x][c]);

                if (column.All(x => x == '.'))
                {
                    colExpansionsTotal++;
                }

                colExpansions[c] = colExpansionsTotal;
            }

            return colExpansions;
        }

        /// <summary>
        /// Calculate the distance between two points considering areas being expanded.
        /// </summary>
        /// <param name="val1">The position of the first galaxy.</param>
        /// <param name="val2">The position of the second galaxy.</param>
        /// <param name="expansions">The tally of expansions.</param>
        /// <param name="expansionFactor">The number to add for expansions.</param>
        /// <returns>The distance between the galaxies.</returns>
        private static int CalculateDistance(int val1, int val2, int[] expansions, int expansionFactor)
        {
            int expansionDiff = Math.Abs(expansions[val1] - expansions[val2]);
            return Math.Abs(val1 - val2) + expansionDiff * (expansionFactor - 1);
        }

        /// <summary>
        /// Get the sum of all lengths between each pair of galaxies.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <param name="expansionFactor">The factor to add for expansion.</param>
        /// <returns>The sums of the distances.</returns>
        private static long SumLengths(string path, int expansionFactor)
        {
            var grid = System.IO.File.ReadAllLines(path);

            var rowExpansions = GetRowExpansions(grid);
            var colExpansions = GetColExpansions(grid);

            long sum = 0;
            foreach (var (row1, col1) in GetGalaxies(grid))
            {
                foreach (var (row2, col2) in GetGalaxies(grid))
                {
                    sum += CalculateDistance(row1, row2, rowExpansions, expansionFactor);
                    sum += CalculateDistance(col1, col2, colExpansions, expansionFactor);
                }
            }

            return sum / 2;
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(374, SumLengths("AOC2023/Day11/Example.txt", 2));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(10165598, SumLengths("AOC2023/Day11/Input.txt", 2));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(1030, SumLengths("AOC2023/Day11/Example.txt", 10));

        [TestMethod]
        public void SolveExample3() => Assert.AreEqual(8410, SumLengths("AOC2023/Day11/Example.txt", 100));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(678728808158, SumLengths("AOC2023/Day11/Input.txt", 1000000));


        #endregion
    }
}
