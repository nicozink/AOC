using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Emit;

namespace AOC2023
{
    /// <summary>
    /// Solution for day 16:
    /// https://adventofcode.com/2023/day/16
    /// </summary>
    [TestClass]
    public class Day16
    {
        /// <summary>
        /// A position in the grid.
        /// </summary>
        /// <param name="Row">The row.</param>
        /// <param name="Col">The column.</param>
        private record Offset(int Row, int Col);

        /// <summary>
        /// Gets the offset based on a position and direction.
        /// </summary>
        /// <param name="position">The position in the grid.</param>
        /// <param name="direction">The direction of movement.</param>
        /// <returns>The new offset.</returns>
        /// <exception cref="ArgumentException">An invalid direction was passed in.</exception>
        private static Offset GetOffset(Offset position, char direction)
        {
            Offset offset = (direction) switch
            {
                '^' => new(-1, 0),
                '>' => new(0, 1),
                'v' => new(1, 0),
                '<' => new(0, -1),
                _ => throw new ArgumentException("Invalid enum", nameof(direction))
            };

            return new(position.Row + offset.Row, position.Col + offset.Col);
        }
        
        /// <summary>
        /// Checks whether a position on the grid is valid.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="position">The position.</param>
        /// <returns>True if the position is valid.</returns>
        private static bool IsValid(string[] grid, Offset position)
        {
            if (position.Row < 0 || position.Row >= grid.Length)
            {
                return false;
            }

            if (position.Col < 0 || position.Col >= grid[0].Length)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Check if a character represents a mirror.
        /// </summary>
        /// <param name="ch">The character.</param>
        /// <returns>True if the character is a mirror.</returns>
        private static bool IsMirrored(char ch)
        {
            return ch == '/' || ch == '\\';
        }

        /// <summary>
        /// Gets the direction if the light is reflected by a mirror.
        /// </summary>
        /// <param name="direction">The direction of the light.</param>
        /// <param name="ch">The type of the mirror.</param>
        /// <returns>The reflected direction.</returns>
        /// <exception cref="ArgumentException">Throws with an invalid direction.</exception>
        private static char GetMirrorDirection(char direction, char ch)
        {
            return (direction) switch
            {
                '^' => ch == '/' ? '>' : '<',
                '>' => ch == '/' ? '^' : 'v',
                'v' => ch == '/' ? '<' : '>',
                '<' => ch == '/' ? 'v' : '^',
                _ => throw new ArgumentException("Invalid enum", nameof(direction))
            };
        }

        /// <summary>
        /// Checks if the light is split for a direction and character.
        /// </summary>
        /// <param name="direction">The current light direction.</param>
        /// <param name="ch">The obstacle.</param>
        /// <returns>True if the light is split.</returns>
        private static bool IsSplit(char direction, char ch)
        {
            if (ch == '-')
            {
                return (direction == '^' || direction == 'v');
            }
            else if (ch == '|')
            {
                return (direction == '<' || direction == '>');
            }

            return false;
        }

        /// <summary>
        /// Returns the direction in which the light is split for a particular splitter.
        /// </summary>
        /// <param name="ch">The splitter.</param>
        /// <returns>The two new light directions.</returns>
        private static (char, char) GetSplit(char ch)
        {
            if (ch == '-')
            {
                return ('<', '>');
            }
            else
            {
                return ('^', 'v');
            }
        }

        /// <summary>
        /// Trace the light across the grid, and mark all energized tiles.
        /// </summary>
        /// <param name="grid">The grid containing the obstacles.</param>
        /// <param name="energizedTiles">Track the energized tiles.</param>
        /// <param name="position">The starting position.</param>
        /// <param name="direction">The light direction.</param>
        private static void MarkTiles(string[] grid, char[][] energizedTiles, Offset position, char direction)
        {
            var newPosition = position;
            if (!IsValid(grid, newPosition))
            {
                return;
            }

            // The path may contain loops, so check here to avoid them.
            if (energizedTiles[position.Row][position.Col] == direction)
            {
                return;
            }

            energizedTiles[position.Row][position.Col] = direction;

            var nextObstacle = grid[newPosition.Row][newPosition.Col];
            if (IsMirrored(nextObstacle))
            {
                var newDirection = GetMirrorDirection(direction, nextObstacle);
                MarkTiles(grid, energizedTiles, GetOffset(newPosition, newDirection), newDirection);
            }
            else if (IsSplit(direction, nextObstacle))
            {
                var (dir1, dir2) = GetSplit(nextObstacle);
                MarkTiles(grid, energizedTiles, GetOffset(newPosition, dir1), dir1);
                MarkTiles(grid, energizedTiles, GetOffset(newPosition, dir2), dir2);
            }
            else
            {
                MarkTiles(grid, energizedTiles, GetOffset(newPosition, direction), direction);
            }
        }

        /// <summary>
        /// Trace the light across the grid, mark any energized tiles, and then count them.
        /// </summary>
        /// <param name="grid">The grid containing the obstacles.</param>
        /// <param name="position">The position of the light.</param>
        /// <param name="direction">The direction fo the light.</param>
        /// <returns>The number of energized tiles.</returns>
        private static int CountEnergizedTiles(string[] grid, Offset position, char direction)
        {
            var energizedTiles = new char[grid.Length][];

            for (int r = 0; r < grid.Length; r++)
            {
                energizedTiles[r] = new char[grid[r].Length];
                Array.Fill(energizedTiles[r], '.');
            }

            MarkTiles(grid, energizedTiles, position, direction);

            return energizedTiles
                .Sum(r => r.Count(c => c != '.'));
        }

        /// <summary>
        /// Count the number of energized tiles starting at the top left and going right.
        /// </summary>
        /// <param name="input">The path to the input file.</param>
        /// <returns>The number of energized tiles.</returns>
        private static int CountEnergizedTiles(string input)
        {
            var grid = System.IO.File.ReadAllLines(input);
            return CountEnergizedTiles(grid, new(0, 0), '>');
        }

        /// <summary>
        /// Get the possible start locations for the lights along the grid.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <returns>The possible start positions.</returns>
        private static IEnumerable<(Offset position, char direction)> GetStartPositions(string[] grid)
        {
            for (int c = 0; c < grid[0].Length; c++)
            {
                yield return (new(0, c), 'v');
                yield return (new(grid.Length, c), '^');
            }

            for (int r = 0; r < grid.Length; r++)
            {
                yield return (new(r, 0), '>');
                yield return (new(r, grid[r].Length), '<');
            }
        }

        /// <summary>
        /// Count the most energized tiles by inspecting all possible starting locations.
        /// </summary>
        /// <param name="input">The path to the input file.</param>
        /// <returns>The most energized tiles.</returns>
        private static int CountMostEnergizedTiles(string input)
        {
            var grid = System.IO.File.ReadAllLines(input);

            int mostTiles = 0;
            foreach (var (position, direction) in GetStartPositions(grid))
            {
                int energizedTiles = CountEnergizedTiles(grid, position, direction);

                if (energizedTiles > mostTiles)
                {
                    mostTiles = energizedTiles;
                }
            }

            return mostTiles;
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(46, CountEnergizedTiles("AOC2023/Day16/Example.txt"));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(6361, CountEnergizedTiles("AOC2023/Day16/Input.txt"));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(51, CountMostEnergizedTiles("AOC2023/Day16/Example.txt"));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(6701, CountMostEnergizedTiles("AOC2023/Day16/Input.txt"));

        #endregion
    }
}
