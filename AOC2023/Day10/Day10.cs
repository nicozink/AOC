using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace AOC2023
{
    /// <summary>
    /// Solution for day 10:
    /// https://adventofcode.com/2023/day/10
    /// </summary>
    [TestClass]
    public class Day10
    {
        record Position(int Row, int Col);

        enum Direction
        {
            North,
            East,
            South,
            West
        }

        /// <summary>
        /// Stores the directions which are reachable by a node int eh grid.
        /// </summary>
        /// <param name="Dir1">The first direction.</param>
        /// <param name="Dir2">The second direction.</param>
        record GridValue(Direction Dir1, Direction Dir2);

        /// <summary>
        /// Stores a lookup of the directions which are reachable by each type of node.
        /// </summary>
        private static readonly Dictionary<char, GridValue> gridValues = new()
        {
            { '|', new(Direction.North, Direction.South) },
            { '-', new(Direction.East, Direction.West) },
            { 'L', new(Direction.North, Direction.East) },
            { 'J', new(Direction.North, Direction.West) },
            { '7', new(Direction.South, Direction.West) },
            { 'F', new(Direction.South, Direction.East) }
        };

        /// <summary>
        /// Checks whether it's possible to move from a point in the grid in a particular direction.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="position">The position in the grid.</param>
        /// <param name="direction">The direction in which it's moving.</param>
        /// <returns>True if it's possible to move in that direction.</returns>
        private static bool CanMove(char[,] grid, Position position, Direction direction)
        {
            // We first check if it's possible to move in a particular direction from the source.
            if (IsDirectionValid(grid, position, direction))
            {
                // We then check the neighbour node and check that it's possible to move into
                // that position.
                var neighbour = GetNeighbour(position, direction);
                var reverse = Reverse(direction);
                if (IsValid(grid, neighbour) && IsDirectionValid(grid, neighbour, reverse))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Count the enclosed spaces. We first solve the grid to discard any
        /// random places that aren't part of the loop.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The number of enclosed spaces.</returns>
        private static long CountEnclosedSpaces(string path)
        {
            var grid = ReadInput(path);

            var rows = grid.GetLength(0);
            var cols = grid.GetLength(1);

            var distances = new int[rows, cols];
            foreach (var position in GetPositions(grid))
            {
                distances[position.Row, position.Col] = -1;
            }

            // Solve the path first.
            SolvePaths(grid, distances);

            // We now clear all positions that are not on the path - especially
            // taking care to clear all paths that are not on the main loop as
            // those are confusing.
            foreach (var position in GetPositions(grid))
            {
                if (distances[position.Row, position.Col] == -1)
                {
                    grid[position.Row, position.Col] = 'E';
                }
            }

            // Now we can count the enclosed spaces.
            SolveEnclosedSpaces(grid);

            return GetPositions(grid).Count(x => grid[x.Row, x.Col] == 'I');
        }

        /// <summary>
        /// Count the farthest location along the loop.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The number of steps to reach the farthest point.</returns>
        private static long CountFarthestSteps(string path)
        {
            var grid = ReadInput(path);

            var rows = grid.GetLength(0);
            var cols = grid.GetLength(1);

            var distances = new int[rows, cols];
            foreach (var position in GetPositions(grid))
            {
                distances[position.Row, position.Col] = -1;
            }

            SolvePaths(grid, distances);

            return GetPositions(grid).Max(x => distances[x.Row, x.Col]);
        }

        /// <summary>
        /// Get the neighbour point from a given point in a location.
        /// </summary>
        /// <param name="position">The current location.</param>
        /// <param name="direction">The direction.</param>
        /// <returns>The neighbour.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown with an invalid direction.</exception>
        private static Position GetNeighbour(Position position, Direction direction)
        {
            return direction switch
            {
                Direction.North => position with { Row = position.Row - 1 },
                Direction.East => position with { Col = position.Col + 1 },
                Direction.South => position with { Row = position.Row + 1 },
                Direction.West => position with { Col = position.Col - 1 },
                _ => throw new ArgumentOutOfRangeException(nameof(direction))
            };
        }

        /// <summary>
        /// Gets the neighbours which are accessible from a point.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="position">A position on the grid.</param>
        /// <returns>The neighbouring locations surrounding the point.</returns>
        private static IEnumerable<Position> GetNeightbours(char[,] grid, Position position)
        {
            foreach (var direction in (Direction[])Enum.GetValues(typeof(Direction)))
            {
                if (CanMove(grid, position, direction))
                {
                    yield return GetNeighbour(position, direction);
                }
            }
        }

        /// <summary>
        /// Enumerates all positions on the grid.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <returns>The positions.</returns>
        private static IEnumerable<Position> GetPositions(char[,] grid)
        {
            var rows = grid.GetLength(0);
            var cols = grid.GetLength(1);

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    yield return new(r, c);
                }
            }
        }

        /// <summary>
        /// Gets the start position in the grid.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <returns>The start position.</returns>
        private static Position GetStartPosition(char[,] grid)
        {
            return GetPositions(grid).Single(x => IsStart(grid, x));
        }

        /// <summary>
        /// Checks if a direction is valid as outgoing for a certain point.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="position">The source position.</param>
        /// <param name="direction">The movement direction.</param>
        /// <returns>True if the direction is valid for the point.</returns>
        private static bool IsDirectionValid(char[,] grid, Position position, Direction direction)
        {
            var currentChar = grid[position.Row, position.Col];

            if (currentChar == 'S')
            {
                return true;
            }

            if (gridValues.TryGetValue(currentChar, out var value))
            {
                return direction == value.Dir1 ||
                    direction == value.Dir2;
            }

            return false;
        }

        /// <summary>
        /// Checks if a position is the start location.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="position">The position to check.</param>
        /// <returns>True if the position is the starting position.</returns>
        private static bool IsStart(char[,] grid, Position position)
        {
            return grid[position.Row, position.Col] == 'S';
        }

        /// <summary>
        /// Checks wether a position is calid, and inside the bounds of the grid.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="position">The position to check.</param>
        /// <returns>True if the position is valid.</returns>
        private static bool IsValid(char[,] grid, Position position)
        {
            if (position.Row < 0 ||
                position.Row >= grid.GetLength(0))
            {
                return false;
            }

            if (position.Col < 0 ||
                position.Col >= grid.GetLength(1))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Read the input grid from the file.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The grid.</returns>
        private static char[,] ReadInput(string path)
        {
            var input = System.IO.File.ReadAllLines(path);

            var rows = input.Length;
            var cols = input[0].Length;

            var grid = new char[rows, cols];
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    grid[r, c] = input[r][c];
                }
            }

            return grid;
        }

        /// <summary>
        /// Reverse the direction.
        /// </summary>
        /// <param name="direction">The direction.</param>
        /// <returns>The reversed direction.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if given an invalid direction.</exception>
        private static Direction Reverse(Direction direction)
        {
            return direction switch
            {
                Direction.North => Direction.South,
                Direction.East => Direction.West,
                Direction.South => Direction.North,
                Direction.West => Direction.East,
                _ => throw new ArgumentOutOfRangeException(nameof(direction))
            };
        }

        /// <summary>
        /// Solve the enclosed spaces. Exterior spaces 'E' are marked as 'I' when they
        /// are enclosed by the path. The path is a loop which we interpret as a polygon.
        /// We count the intersections from each point to the outside. An odd number of
        /// intersections means that the point is enclosed by the path.
        /// </summary>
        /// <param name="grid">The input grid.</param>
        private static void SolveEnclosedSpaces(char[,] grid)
        {
            foreach (var position in GetPositions(grid))
            {
                if (grid[position.Row, position.Col] == 'E')
                {
                    int numIntersections = 0;

                    for (int i = position.Col + 1; i < grid.GetLength(1); i++)
                    {
                        var newPosition = position with { Col = i };

                        // We have to take a bit of care here that we only count intersections. As
                        // we are traversing east, intersections go from north to south. An intersection
                        // may run horizontally, so we follow it, and one end needs to go north and the
                        // other south. If we only have nort-north, then that's the end of a loop which
                        // we don't consider as an intersection.

                        bool canMoveNorth = CanMove(grid, newPosition, Direction.North);
                        bool canMoveSouth = CanMove(grid, newPosition, Direction.South);

                        if (canMoveNorth || canMoveSouth)
                        {
                            while (CanMove(grid, newPosition, Direction.East))
                            {
                                newPosition = GetNeighbour(newPosition, Direction.East);
                                i++;
                            }

                            canMoveNorth |= CanMove(grid, newPosition, Direction.North);
                            canMoveSouth |= CanMove(grid, newPosition, Direction.South);
                        }

                        if (canMoveNorth && canMoveSouth)
                        {
                            numIntersections++;
                        }
                    }

                    if (numIntersections % 2 != 0)
                    {
                        grid[position.Row, position.Col] = 'I';
                    }
                }
            }
        }

        /// <summary>
        /// Solve the quickest path starting from the start location to every
        /// other location along the path.
        /// </summary>
        /// <param name="grid">The input grid.</param>
        /// <param name="distances">The grid containing the distances to all other points on the path.</param>
        private static void SolvePaths(char[,] grid, int[,] distances)
        {
            var startPos = GetStartPosition(grid);
            distances[startPos.Row, startPos.Col] = 0;

            var queue = new Queue<Position>();
            queue.Enqueue(startPos);

            while (queue.Count > 0)
            {
                var next = queue.Dequeue();
                var distance = distances[next.Row, next.Col];
                foreach (var neighbour in GetNeightbours(grid, next))
                {
                    var existingDistance = distances[neighbour.Row, neighbour.Col];
                    var newDistance = distance + 1;
                    if (existingDistance == -1 || existingDistance > newDistance)
                    {
                        distances[neighbour.Row, neighbour.Col] = newDistance;
                        queue.Enqueue(neighbour);
                    }
                }
            }
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(4, CountFarthestSteps("AOC2023/Day10/Example1.txt"));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(8, CountFarthestSteps("AOC2023/Day10/Example2.txt"));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(6786, CountFarthestSteps("AOC2023/Day10/Input.txt"));

        [TestMethod]
        public void SolveExample3() => Assert.AreEqual(4, CountEnclosedSpaces("AOC2023/Day10/Example3.txt"));

        [TestMethod]
        public void SolveExample4() => Assert.AreEqual(4, CountEnclosedSpaces("AOC2023/Day10/Example4.txt"));

        [TestMethod]
        public void SolveExample5() => Assert.AreEqual(8, CountEnclosedSpaces("AOC2023/Day10/Example5.txt"));

        [TestMethod]
        public void SolveExample6() => Assert.AreEqual(10, CountEnclosedSpaces("AOC2023/Day10/Example6.txt"));
        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(495, CountEnclosedSpaces("AOC2023/Day10/Input.txt"));

        #endregion
    }
}
