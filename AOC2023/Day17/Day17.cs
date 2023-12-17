using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AOC2023
{
    /// <summary>
    /// Solution for day 17:
    /// https://adventofcode.com/2023/day/17
    /// </summary>
    [TestClass]
    public class Day17
    {
        /// <summary>
        /// A direction of travel along the grid.
        /// </summary>
        private enum Direction
        {
            North, East, South, West
        }

        /// <summary>
        /// The orientation of possible follow-up travel when reaching a position on the grid.
        /// </summary>
        private enum Orientation
        {
            Horizontal, Vertical
        }

        /// <summary>
        /// A position on the grid.
        /// </summary>
        /// <param name="Row">The row.</param>
        /// <param name="Col">The column.</param>
        record Position(int Row, int Col);

        /// <summary>
        /// Get the position along a direction after doing the number of steps.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="steps">The number of steps.</param>
        /// <returns>The position.</returns>
        /// <exception cref="ArgumentException">Thrown with an invalid direction.</exception>
        private static Position GetPosition(Position position, Direction direction, int steps)
        {
            Position offset = (direction) switch
            {
                Direction.North => new(-steps, 0),
                Direction.East => new(0, steps),
                Direction.South => new(steps, 0),
                Direction.West => new(0, -steps),
                _ => throw new ArgumentException("Invalid enum", nameof(direction))
            };

            return new(position.Row + offset.Row, position.Col + offset.Col);
        }

        /// <summary>
        /// Checks whether the position on the grid is valid.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="position">The position.</param>
        /// <returns>True if the position is valid.</returns>
        private static bool IsValid(int[][] grid, Position position)
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
        /// Gets the directions that are valid for an orientation.
        /// </summary>
        /// <param name="orientation">The orientation.</param>
        /// <returns>The directions.</returns>
        private static IEnumerable<Direction> GetDirections(Orientation orientation)
        {
            if (orientation == Orientation.Horizontal)
            {
                yield return Direction.East;
                yield return Direction.West;
            }
            else
            {
                yield return Direction.North;
                yield return Direction.South;
            }
        }

        /// <summary>
        /// Solve the best path that minimises heat loss.
        /// </summary>
        /// <param name="input">The path to the input file.</param>
        /// <param name="ultraCrucible">Whether to use the ultra crucible vehicle.</param>
        /// <returns>The best path.</returns>
        private static int SolveBestPath(string input, bool ultraCrucible)
        {
            int minimumMoves = ultraCrucible ? 4 : 1;
            int maximumMoves = ultraCrucible ? 10 : 3;

            var grid = System.IO.File.ReadAllLines(input)
                .Select(x => x.Select(y => y - '0').ToArray())
                .ToArray();

            var topLeft = new Position(0, 0);

            var bestRoute = new Dictionary<(Position, Orientation), int>()
            {
                { (topLeft, Orientation.Horizontal), 0 },
                { (topLeft, Orientation.Vertical), 0 }
            };
            
            var frontier = new Queue<(Position, Orientation)>();
            frontier.Enqueue((topLeft, Orientation.Horizontal));
            frontier.Enqueue((topLeft, Orientation.Vertical));

            while (frontier.Count > 0)
            {
                var key = frontier.Dequeue();
                var currentRoute = bestRoute[key];

                var (position, orientation) = key;
                var nextOrientation = orientation == Orientation.Vertical ? Orientation.Horizontal : Orientation.Vertical;
                foreach (var direction in GetDirections(orientation))
                {
                    int directionHeatLoss = 0;
                    for (int steps = 1; steps < minimumMoves; steps++)
                    {
                        var newPosition = GetPosition(position, direction, steps);
                        if (!IsValid(grid, newPosition))
                        {
                            break;
                        }

                        directionHeatLoss += grid[newPosition.Row][newPosition.Col];
                    }

                    for (int steps = minimumMoves; steps <= maximumMoves; steps++)
                    {
                        var newPosition = GetPosition(position, direction, steps);
                        if (!IsValid(grid, newPosition))
                        {
                            break;
                        }

                        directionHeatLoss += grid[newPosition.Row][newPosition.Col];
                        var totalHeatLoss = currentRoute + directionHeatLoss;

                        var newKey = (newPosition, nextOrientation);
                        if (!bestRoute.TryGetValue(newKey, out var existingRoute) || existingRoute > totalHeatLoss)
                        {
                            bestRoute[newKey] = totalHeatLoss;
                            frontier.Enqueue(newKey);
                        }
                    }
                }
            }

            var bottomRight = new Position(grid.Length - 1, grid[0].Length - 1);
            return Math.Min(bestRoute[(bottomRight, Orientation.Horizontal)],
                bestRoute[(bottomRight, Orientation.Vertical)]);
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(102, SolveBestPath("AOC2023/Day17/Example.txt", ultraCrucible: false));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(635, SolveBestPath("AOC2023/Day17/Input.txt", ultraCrucible: false));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(94, SolveBestPath("AOC2023/Day17/Example.txt", ultraCrucible: true));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(734, SolveBestPath("AOC2023/Day17/Input.txt", ultraCrucible: true));

        #endregion
    }
}
