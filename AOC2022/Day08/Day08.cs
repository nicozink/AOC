using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace AOC2022
{
    /// <summary>
    /// Solution for day 8:
    /// https://adventofcode.com/2022/day/8
    /// </summary>
    [TestClass]
    public class Day08
    {
        /// <summary>
        /// Enum for the various directions in which we can find trees.
        /// </summary>
        private enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }

        /// <summary>
        /// Gets all trees above a position.
        /// </summary>
        /// <param name="lines">The grid of trees.</param>
        /// <param name="row">The position.</param>
        /// <param name="col">The position.</param>
        /// <returns>All trees above the position.</returns>
        private static IEnumerable<char> GetTreesUp(string[] lines, int row, int col)
        {
            for (int i = row - 1; i >= 0; i--)
            {
                yield return lines[i][col];
            }
        }

        /// <summary>
        /// Gets all trees below a position.
        /// </summary>
        /// <param name="lines">The grid of trees.</param>
        /// <param name="row">The position.</param>
        /// <param name="col">The position.</param>
        /// <returns>All trees below the position.</returns>
        private static IEnumerable<char> GetTreesDown(string[] lines, int row, int col)
        {
            for (int i = row + 1; i < lines.Length; i++)
            {
                yield return lines[i][col];
            }
        }

        /// <summary>
        /// Gets all trees left of a position.
        /// </summary>
        /// <param name="lines">The grid of trees.</param>
        /// <param name="row">The position.</param>
        /// <param name="col">The position.</param>
        /// <returns>All trees left of the position.</returns>
        private static IEnumerable<char> GetTreesLeft(string[] lines, int row, int col)
        {
            for (int i = col - 1; i >= 0; i--)
            {
                yield return lines[row][i];
            }
        }

        /// <summary>
        /// Gets all trees right of a position.
        /// </summary>
        /// <param name="lines">The grid of trees.</param>
        /// <param name="row">The position.</param>
        /// <param name="col">The position.</param>
        /// <returns>All trees right of the position.</returns>
        private static IEnumerable<char> GetTreesRight(string[] lines, int row, int col)
        {
            for (int i = col + 1; i < lines[row].Length; i++)
            {
                yield return lines[row][i];
            }
        }

        /// <summary>
        /// Gets all trees at the position in the direction.
        /// </summary>
        /// <param name="lines">The grid of trees.</param>
        /// <param name="row">The position.</param>
        /// <param name="col">The position.</param>
        /// <param name="dir">The direction.</param>
        /// <returns>All trees at the position in the direction.</returns>
        private static IEnumerable<char> GetTrees(string[] lines, int row, int col, Direction dir)
        {
            return dir switch
            {
                Direction.Up => GetTreesUp(lines, row, col),
                Direction.Down => GetTreesDown(lines, row, col),
                Direction.Left => GetTreesLeft(lines, row, col),
                Direction.Right => GetTreesRight(lines, row, col),
                _ => throw new Exception("Shouldn't be here!")
            };
        }

        /// <summary>
        /// Checks whether a tree is visible from the outside, meaning
        /// it has no trees that are same size or larger from one of the
        /// directions.
        /// </summary>
        /// <param name="lines">The grid of trees.</param>
        /// <param name="row">The position of the tree.</param>
        /// <param name="col">The position of the tree.</param>
        /// <returns>True if the tree is visible.</returns>
        private static bool IsVisible(string[] lines, int row, int col)
        {
            var height = lines[row][col];

            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
            {
                if (GetTrees(lines, row, col, direction)
                    .All(x => x < height))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns the number of trees that are visible from
        /// outside the grid, meaning they don't have any trees
        /// same size or larger in the line of sight.
        /// </summary>
        /// <param name="path">The path to the grid of trees.</param>
        /// <returns>The number of visible trees.</returns>
        private static int GetVisible(string path)
        {
            var lines = System.IO.File.ReadAllLines(path);

            int numVisible = 0;
            for (int row = 0; row < lines.Length; row++)
            {
                for (int col = 0; col < lines[row].Length; col++)
                {
                    if (IsVisible(lines, row, col))
                    {
                        numVisible++;
                    }
                }
            }

            return numVisible;
        }

        /// <summary>
        /// Gets the visibility score of the line of trees. This is the
        /// number of trees that are visible until the view is blocked
        /// by a larger one..
        /// </summary>
        /// <param name="trees">The trees in a direction.</param>
        /// <param name="height">The height of the current tree.</param>
        /// <returns>The number of visible trees.</returns>
        private static int GetScore(IEnumerable<char> trees, char height)
        {
            int numVisible = 0;
            foreach (var tree in trees)
            {
                numVisible++;

                if (tree >= height)
                {
                    break;
                }
            }

            return numVisible;
        }

        /// <summary>
        /// Gets the scenic score or a particular tree, which is the product of
        /// visible trees in each direction.
        /// </summary>
        /// <param name="lines">The grid of trees.</param>
        /// <param name="row">The position of the current tree.</param>
        /// <param name="col">The position of the current tree.</param>
        /// <returns></returns>
        private static int GetScenicScore(string[] lines, int row, int col)
        {
            var height = lines[row][col];

            int scoreProduct = 1;
            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
            {
                int score = GetScore(GetTrees(lines, row, col, direction), height);
                scoreProduct *= score;
            }

            return scoreProduct;
        }

        /// <summary>
        /// Gets the best scenic score of the trees in the grid.
        /// </summary>
        /// <param name="path">The path to the file containing the grid.</param>
        /// <returns>The best scenic score.</returns>
        private static int GetBestScenicScore(string path)
        {
            var lines = System.IO.File.ReadAllLines(path);

            int bestScore = 0;
            for (int row = 1; row < lines.Length; row++)
            {
                for (int col = 2; col < lines[row].Length; col++)
                {
                    var scenicScore = GetScenicScore(lines, row, col);

                    if (scenicScore > bestScore)
                    {
                        bestScore = scenicScore;
                    }
                }
            }

            return bestScore;
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(21, GetVisible("AOC2022/Day08/Example.txt"));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(1794, GetVisible("AOC2022/Day08/Input.txt"));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(8, GetBestScenicScore("AOC2022/Day08/Example.txt"));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(199272, GetBestScenicScore("AOC2022/Day08/Input.txt"));

        #endregion
    }
}
