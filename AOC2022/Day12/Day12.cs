using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace AOC2022
{
    /// <summary>
    /// Solution for day 12:
    /// https://adventofcode.com/2022/day/12
    /// </summary>
    [TestClass]
    public class Day12
    {
        /// <summary>
        /// Stores the terrain, and some functions to calculate the shortest path.
        /// </summary>
        private class Terrain
        {
            /// <summary>
            /// We calculate the movement in two ways - either traversing forward from
            /// the start to the end, oir in reverse from the end.
            /// </summary>
            private enum Direction
            {
                Forward,
                Reverse
            }

            /// <summary>
            /// The lines representing the height map.
            /// </summary>
            private readonly char[][] lines;

            /// <summary>
            /// The number of rows.
            /// </summary>
            private readonly int numRows;

            /// <summary>
            /// The number of columns.
            /// </summary>
            private readonly int numCols;

            /// <summary>
            /// The start position in the height map.
            /// </summary>
            private readonly (int, int) startPosition;

            /// <summary>
            /// The end position in the height map.
            /// </summary>
            private readonly (int, int) endPosition;

            /// <summary>
            /// Creates a new terrain object by reading in the input file.
            /// </summary>
            /// <param name="path">The path to the file.</param>
            public Terrain(string path)
            {
                lines = System.IO.File.ReadLines(path)
                    .Select(x => x.ToArray())
                    .ToArray();

                numRows = lines.Length;
                numCols = lines[0].Length;
                for (int r = 0; r < numRows; r++)
                {
                    for (int c = 0; c < numCols; c++)
                    {
                        // We keep track of the start and end so we can track them, but
                        // replace them with the values they represent to simplify the
                        // calculations.

                        if (lines[r][c] == 'S')
                        {
                            startPosition = (r, c);
                            lines[r][c] = 'a';
                        }
                        else if (lines[r][c] == 'E')
                        {
                            endPosition = (r, c);
                            lines[r][c] = 'z';
                        }
                    }
                }
            }

            /// <summary>
            /// Calculate the shortest path from the starting point to the end.
            /// </summary>
            /// <returns>The shortest path.</returns>
            public int GetShortestPathToEnd()
            {
                var shortestPaths = GetShortestPaths(startPosition, Direction.Forward);
                return shortestPaths[endPosition.Item1, endPosition.Item2];
            }

            /// <summary>
            /// Calculate the shortest path to the end from any starting point at a
            /// lowest point.
            /// </summary>
            /// <returns>The shortest path to the end.</returns>
            public int GetShortestPathFromEnd()
            {
                var shortestPaths = GetShortestPaths(endPosition, Direction.Reverse);

                int shortestPath = int.MaxValue;
                for (int r = 0; r < numRows; r++)
                {
                    for (int c = 0; c < numCols; c++)
                    {
                        if (lines[r][c] == 'a' && shortestPaths[r, c] < shortestPath)
                        {
                            shortestPath = shortestPaths[r, c];
                        }
                    }
                }

                return shortestPath;
            }

            /// <summary>
            /// Checks whether a hiker can move from the previous to the next position.
            /// </summary>
            /// <param name="prev">The previous position.</param>
            /// <param name="next">The next position.</param>
            /// <returns>True if the move is possible.</returns>
            private bool CanMove((int, int) prev, (int, int) next)
            {
                var prevChar = lines[prev.Item1][prev.Item2];
                var nextChar = lines[next.Item1][next.Item2];

                return nextChar <= prevChar + 1;
            }

            /// <summary>
            /// Checks whether the move between the previous and next position is possible, but in reverse
            /// used for the reverse lookup.
            /// </summary>
            /// <param name="prev">The previous position.</param>
            /// <param name="next">The next position.</param>
            /// <returns>True if the move is possible.</returns>
            private bool CanMoveReverse((int, int) prev, (int, int) next)
            {
                return CanMove(next, prev);
            }

            /// <summary>
            /// Gets the neighbours for a position.
            /// </summary>
            /// <param name="position">The position.</param>
            /// <returns>The neighbours.</returns>
            private IEnumerable<(int, int)> GetNeighbours((int, int) position)
            {
                if (position.Item1 > 0)
                {
                    yield return position with { Item1 = position.Item1 - 1 };
                }

                if (position.Item1 < lines.Length - 1)
                {
                    yield return position with { Item1 = position.Item1 + 1 };
                }

                if (position.Item2 > 0)
                {
                    yield return position with { Item2 = position.Item2 - 1 };
                }

                if (position.Item2 < lines[0].Length - 1)
                {
                    yield return position with { Item2 = position.Item2 + 1 };
                }
            }

            /// <summary>
            /// Gets the shortest paths from a position. Takes a direction, which
            /// determines whether the paths are from the start position leading out, or
            /// to a destination.
            /// </summary>
            /// <param name="position">The position.</param>
            /// <param name="direction">The direction.</param>
            /// <returns>The shortest paths.</returns>
            private int[,] GetShortestPaths((int, int) position, Direction direction)
            {
                var shortestPaths = new int[numRows, numCols];
                for (int r = 0; r < numRows; r++)
                {
                    for (int c = 0; c < numCols; c++)
                    {
                        if (r == position.Item1 && c == position.Item2)
                        {
                            shortestPaths[r, c] = 0;
                        }
                        else
                        {
                            shortestPaths[r, c] = int.MaxValue;
                        }
                    }
                }

                var queue = new Queue<(int, int)>();
                queue.Enqueue(position);

                while (queue.Count > 0)
                {
                    var next = queue.Dequeue();
                    var nextCost = shortestPaths[next.Item1, next.Item2] + 1;

                    foreach (var neighbour in GetNeighbours(next))
                    {
                        bool canMove = (direction == Direction.Forward) ? CanMove(next, neighbour) : CanMoveReverse(next, neighbour);
                        if (canMove && shortestPaths[neighbour.Item1, neighbour.Item2] > nextCost)
                        {
                            shortestPaths[neighbour.Item1, neighbour.Item2] = nextCost;
                            queue.Enqueue(neighbour);
                        }
                    }
                }

                return shortestPaths;
            }
        }

        private static int SolvePart1(string path)
        {
            var terrain = new Terrain(path);
            return terrain.GetShortestPathToEnd();
        }

        private static int SolvePart2(string path)
        {
            var terrain = new Terrain(path);
            return terrain.GetShortestPathFromEnd();
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(31, SolvePart1("AOC2022/Day12/Example.txt"));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(412, SolvePart1("AOC2022/Day12/Input.txt"));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(29, SolvePart2("AOC2022/Day12/Example.txt"));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(402, SolvePart2("AOC2022/Day12/Input.txt"));

        #endregion
    }
}
