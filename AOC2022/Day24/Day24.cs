using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using System.Text.RegularExpressions;

namespace AOC2022
{
    /// <summary>
    /// Solution for day 24:
    /// https://adventofcode.com/2022/day/24
    /// </summary>
    [TestClass]
    public class Day24
    {
        /// <summary>
        /// Stores the position of a blizzard, and the party.
        /// </summary>
        /// <param name="X">The X location.</param>
        /// <param name="Y">The Y location.</param>
        record Position(int X, int Y);

        /// <summary>
        /// Stores the direction and position of a blizzard.
        /// </summary>
        /// <param name="Direction">T%he direction.</param>
        /// <param name="Position">The position.</param>
        record Blizzard(char Direction, Position Position);

        /// <summary>
        /// Stores a grid with blizzards that the party is trying to make its way through.
        /// </summary>
        class Grid
        {
            /// <summary>
            /// Stores the grid containing the blizzards.
            /// </summary>
            private readonly Blizzard[] grid;

            /// <summary>
            /// Stores the maximum X coordinate.
            /// </summary>
            private readonly int maxX;

            /// <summary>
            /// Stores the maximum Y coordinate.
            /// </summary>
            private readonly int maxY;

            /// <summary>
            /// Reads the grid from the file.
            /// </summary>
            /// <param name="path">The path to the input file.</param>
            public Grid(string path)
            {
                grid = ReadInput(path).ToArray();
                maxX = grid.Max(x => x.Position.X);
                maxY = grid.Max(x => x.Position.Y);
            }

            /// <summary>
            /// Find the fastest way through the blizzard, and return the fastest time in minutes.
            /// </summary>
            /// <param name="trips">The number of trips to make between the start and end points.</param>
            /// <returns>The fastest time in minutes.</returns>
            public int Solve(int trips)
            {
                // Initialise the goals based on the number of trips between the start and end points.

                var goals = new Queue<Position>();
                goals.Enqueue(new Position(maxX, maxY + 1));

                while (goals.Count < trips)
                {
                    goals.Enqueue(new Position(0, -1));
                    goals.Enqueue(new Position(maxX, maxY + 1));
                }

                // Keep track of all valid positions each turn.
                var frontier = new HashSet<Position>
                {
                    new Position(0, -1)
                };

                int numSteps = 0;
                while (goals.Count != 0)
                {
                    numSteps++;
                    Advance();

                    var newFrontier = new HashSet<Position>();
                    var empty = GetEmpty();

                    bool foundGoal = false;
                    foreach (var item in frontier)
                    {
                        foreach (var neighbour in GetNeighbours(empty, item))
                        {
                            if (neighbour == goals.Peek())
                            {
                                foundGoal = true;
                            }

                            newFrontier.Add(neighbour);
                        }
                    }

                    if (foundGoal)
                    {
                        var goal = goals.Dequeue();
                        newFrontier = new HashSet<Position>()
                        {
                            goal
                        };
                    }

                    frontier = newFrontier;
                }

                return numSteps;
            }

            /// <summary>
            /// Advance time by moving all blizzards.
            /// </summary>
            private void Advance()
            {
                for (int i = 0; i < grid.Length; i++)
                {
                    var blizzard = grid[i];
                    grid[i] = GetNextPosition(blizzard);
                }
            }

            /// <summary>
            /// Gets all neighbours that can be moved to where they are not covered by blizzards.
            /// </summary>
            /// <param name="empty">The empty spaces.</param>
            /// <param name="position">The current positions.</param>
            /// <returns>All reachable positions.</returns>
            private static IEnumerable<Position> GetNeighbours(HashSet<Position> empty, Position position)
            {
                if (empty.Contains(position))
                {
                    yield return position;
                }

                var up = position with { Y = position.Y - 1 };
                if (empty.Contains(up))
                {
                    yield return up;
                }

                var down = position with { Y = position.Y + 1 };
                if (empty.Contains(down))
                {
                    yield return down;
                }

                var left = position with { X = position.X - 1 };
                if (empty.Contains(left))
                {
                    yield return left;
                }

                var right = position with { X = position.X + 1 };
                if (empty.Contains(right))
                {
                    yield return right;
                }
            }

            /// <summary>
            /// Gets the next position for a blizzard after advancing time.
            /// </summary>
            /// <param name="blizzard">The blizzard.</param>
            /// <returns>The blizzard with the new position.</returns>
            private Blizzard GetNextPosition(Blizzard blizzard)
            {
                var position = blizzard.Position;

                var posX = position.X;
                var posY = position.Y;

                switch (blizzard.Direction)
                {
                    case '<':
                        {
                            posX--;
                            break;
                        }
                    case '>':
                        {
                            posX++;
                            break;
                        }
                    case '^':
                        {
                            posY--;
                            break;
                        }
                    case 'v':
                        {
                            posY++;
                            break;
                        }
                }

                posX = (posX + maxX + 1) % (maxX + 1);
                posY = (posY + maxY + 1) % (maxY + 1);

                return blizzard with { Position = new Position(posX, posY) };
            }

            /// <summary>
            /// Gets the empty spaces on the grid not covered by the blizzard.
            /// </summary>
            /// <returns>The empty spaces.</returns>
            private HashSet<Position> GetEmpty()
            {
                var blizzards = grid
                    .Select(x => x.Position)
                    .ToHashSet();

                var empty = new HashSet<Position>();
                for (int i = 0; i <= maxX; i++)
                {
                    for (int j = 0; j <= maxY; j++)
                    {
                        var position = new Position(i, j);
                        if (!blizzards.Contains(position))
                        {
                            empty.Add(position);
                        }
                    }
                }

                // These two positions are never covered by a blizzard, so always return them.
                empty.Add(new Position(0, -1));
                empty.Add(new Position(maxX, maxY + 1));

                return empty;
            }

            /// <summary>
            /// Reads the input from the file and returns it as blizzard positions.
            /// </summary>
            /// <param name="path">The path to the input file.</param>
            /// <returns>The blizzard positions.</returns>
            private static IEnumerable<Blizzard> ReadInput(string path)
            {
                var lines = System.IO.File.ReadAllLines(path);

                for (int i = 1; i < lines.Length - 1; i++)
                {
                    for (int j = 1; j < lines[i].Length - 1; j++)
                    {
                        if (lines[i][j] != '.')
                        {
                            var direction = lines[i][j];
                            var position = new Position(j - 1, i - 1);
                            yield return new(direction, position);
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Reads the input as blizzard positions, and returns the fastest time
        /// through the grid.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <param name="trips">The number of trips to take between start and end positions.</param>
        /// <returns>The fastet time through the blizzards.</returns>
        private static int GetSolution(string path, int trips)
        {
            var solver = new Grid(path);
            return solver.Solve(trips);
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(18, GetSolution("AOC2022/Day24/Example.txt", 1));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(253, GetSolution("AOC2022/Day24/Input.txt", 1));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(54, GetSolution("AOC2022/Day24/Example.txt", 3));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(794, GetSolution("AOC2022/Day24/Input.txt", 3));

        #endregion
    }
}
