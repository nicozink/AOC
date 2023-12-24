using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;

namespace AOC2023
{
    /// <summary>
    /// Solution for day 23:
    /// https://adventofcode.com/2023/day/23
    /// </summary>
    [TestClass]
    public class Day23
    {
        /// <summary>
        /// Stores a position on the grid.
        /// </summary>
        /// <param name="Row">The row.</param>
        /// <param name="Col">The column.</param>
        record Position(int Row, int Col);

        /// <summary>
        /// Stores the directions in which movement is possible.
        /// </summary>
        private enum Direction
        {
            North = '^',
            East = '>',
            South = 'v',
            West = '<'
        }

        /// <summary>
        /// Gets the position from a grod node based on the direction.
        /// </summary>
        /// <param name="position">The position on the grid.</param>
        /// <param name="direction">The direction in which to move.</param>
        /// <returns>The new position.</returns>
        private static Position GetPosition(Position position, Direction direction)
        {
            return (direction) switch
            {
                Direction.North => position with { Row = position.Row - 1 },
                Direction.East => position with { Col = position.Col + 1 },
                Direction.South => position with { Row = position.Row + 1 },
                _ => position with { Col = position.Col - 1 }
            };
        }

        /// <summary>
        /// Checks whether a move is possible from the position in the grid in the
        /// direction. Movement is blocked by walls, tiles which have already been
        /// traversed, and slopes.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="position">The position on the grid.</param>
        /// <param name="direction">The direction in which to move.</param>
        /// <returns>True if a move is possible.</returns>
        private static bool CanMove(char[][] grid, Position position, Direction direction)
        {
            var newPosition = GetPosition(position, direction);
            if (newPosition.Row < 0 || newPosition.Row >= grid.Length)
            {
                return false;
            }

            if (newPosition.Col < 0 || newPosition.Col >= grid[0].Length)
            {
                return false;
            }

            // Check if movement is possible. We can move into a slope when
            // the path out of the slope allows a further move, but are
            // blocked when the slope is opposite to the direction of
            // movement.
            var newChar = grid[newPosition.Row][newPosition.Col];
            if ("^>v<".Contains(newChar))
            {
                var opposite = (direction) switch
                {
                    Direction.North => Direction.South,
                    Direction.East => Direction.West,
                    Direction.South => Direction.North,
                    _ => Direction.East
                };

                if ((int)opposite == newChar)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            
            return newChar == '.';
        }

        /// <summary>
        /// Gets the neighbours which can be moved to from a position.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="position">The current positions.</param>
        /// <returns>The directions in which movement is possible.</returns>
        private static IEnumerable<Direction> GetNeighbours(char[][] grid, Position position)
        {
            if (CanMove(grid, position, Direction.North))
            {
                yield return Direction.North;
            }

            if (CanMove(grid, position, Direction.East))
            {
                yield return Direction.East;
            }

            if (CanMove(grid, position, Direction.South))
            {
                yield return Direction.South;
            }

            if (CanMove(grid, position, Direction.West))
            {
                yield return Direction.West;
            }
        }

        /// <summary>
        /// Count the longest parth from the position to the finish node.
        /// </summary>
        /// <param name="distances">The distance between each node.</param>
        /// <param name="visited">The positions which have been visisted, to avoid retreading movement.</param>
        /// <param name="position">The current position.</param>
        /// <param name="finish">The finish position.</param>
        /// <returns>The longest path to the finish.</returns>
        private static int CountLongestPath(Dictionary<int, int>[] distances, HashSet<int> visited, int position, int finish)
        {
            if (position == finish)
            {
                return 0;
            }

            var longestPath = -1; // This stays -1 if we can't find a path to the finish.
            var distancesToNext = distances[position];
            foreach (var (nextPosition, distance) in distancesToNext)
            {
                if (visited.Contains(nextPosition))
                {
                    continue;
                }

                visited.Add(nextPosition);
                var recursiveDistance = CountLongestPath(distances, visited, nextPosition, finish);
                visited.Remove(nextPosition);

                if (recursiveDistance != -1)
                {
                    recursiveDistance += distance;
                }

                if (recursiveDistance > longestPath)
                {
                    longestPath = recursiveDistance;
                }
            }

            return longestPath;
        }

        /// <summary>
        /// Gets the column with a single empty position, so that we can find the start
        /// and end nodes.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="row">The row in which to look.</param>
        /// <returns>The start or end node.</returns>
        private static Position GetSinglePosition(char[][] grid, int row)
        {
            var specialColumn = grid[row]
                .Select((x, i) => (x, i))
                .Where(x => x.x == '.')
                .Select(x => x.i)
                .Single();

            return new(row, specialColumn);
        }

        /// <summary>
        /// Follow a parth from a start position, in a specific direction. Mark the path along the way,
        /// and return if we find a fork in the road. Return the position and path length of the path
        /// we followed.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="position">The start position.</param>
        /// <param name="direction">The direction of movement.</param>
        /// <returns>The next node, and the path length.</returns>
        private static (Position, int) FollowPath(char[][] grid, Position position, Direction direction)
        {
            position = GetPosition(position, direction);
            var cache = new Dictionary<Position, char>
            {
                { position, grid[position.Row][position.Col] }
            };
            grid[position.Row][position.Col] = 'O';

            int pathCount = 0;
            while (true)
            {
                var neighbours = GetNeighbours(grid, position)
                    .ToArray();

                // If we have one direction to follow, we do so. If we run out of
                // path, then we automatically have to stop. If we reach a fork with
                // multiple options, then we need to return as well - this is a node
                // fow which we need to work out the alternative paths.
                if (neighbours.Length == 1)
                {
                    position = GetPosition(position, neighbours[0]);

                    cache.Add(position, grid[position.Row][position.Col]);
                    grid[position.Row][position.Col] = 'O';
                    
                    pathCount++;
                }
                else
                {
                    break;
                }
            }

            // Restore any characters we marked as occupied so that we can
            // lave the grid blank for the next traversals.
            foreach (var (cachedPosition, cachedChar) in cache)
            {
                grid[cachedPosition.Row][cachedPosition.Col] = cachedChar;
            }

            return (position, pathCount + 1);
        }

        /// <summary>
        /// Calculate an array of distances between pairs of nodes (where
        /// traversal is possible).
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <returns>The array of differences between pairs.</returns>
        private static Dictionary<int, int>[] CalculateDistances(char[][] grid)
        {
            var start = GetSinglePosition(grid, 0);
            var finish = GetSinglePosition(grid, grid.Length - 1);

            var positions = new List<Position>()
            {
                start,
                finish
            };

            var positionKeys = new Dictionary<Position, int>()
            {
                { start, 0 },
                { finish, 1 }
            };

            var distances = new Dictionary<(int, int), int>();

            var frontier = new Queue<int>();
            frontier.Enqueue(positionKeys[start]);

            while (frontier.Count > 0)
            {
                var positionIndex = frontier.Dequeue();
                var position = positions[positionIndex];

                grid[position.Row][position.Col] = 'O';

                var outgoing = GetNeighbours(grid, position);
                foreach (var outgoingDirection in outgoing)
                {
                    var (nextPosition, pathLength) = FollowPath(grid, position, outgoingDirection);

                    if (!positionKeys.TryGetValue(nextPosition, out var nextPositionIndex))
                    {
                        nextPositionIndex = positionKeys.Count;
                        positions.Add(nextPosition);
                        positionKeys[nextPosition] = nextPositionIndex;

                        frontier.Enqueue(nextPositionIndex);
                    }

                    // It can happens that two different paths lead to the same node.
                    // If we have a collision, then we choose the larger of the two
                    // values.
                    var key = (positionIndex, nextPositionIndex);
                    if (distances.TryGetValue(key, out var existingDistance))
                    {
                        if (existingDistance > pathLength)
                        {
                            pathLength = existingDistance;
                        }
                    }

                    distances[(positionIndex, nextPositionIndex)] = pathLength;
                }

                grid[position.Row][position.Col] = '.';
            }

            var processedDistances = new Dictionary<int, int>[positions.Count];
            for (int i = 0; i < processedDistances.Length; i++)
            {
                processedDistances[i] = new ();
            }

            foreach (var (pair, distance) in distances)
            {
                var (from, to) = pair;
                processedDistances[from][to] = distance;
            }

            return processedDistances;
        }

        /// <summary>
        /// Find the longest path between the start and end points. We can optionally climb
        /// peaks for more movement options.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <param name="climbSlopes">Whether we can climb slopes for more movement options.</param>
        /// <returns>The longest path.</returns>
        private static int Solve(string path, bool climbSlopes)
        {
            var grid = System.IO.File.ReadLines(path)
                .Select(x => x.ToArray())
                .ToArray();

            var start = GetSinglePosition(grid, 0);
            var finish = GetSinglePosition(grid, grid.Length - 1);

            if (climbSlopes)
            {
                for (int r = 0; r < grid.Length; r++)
                {
                    for (int c = 0; c < grid.Length; c++)
                    {
                        if ("^>v<".Contains(grid[r][c]))
                        {
                            grid[r][c] = '.';
                        }
                    }
                }
            }

            var distances = CalculateDistances(grid);

            var visited = new HashSet<int>() { 0 };
            return CountLongestPath(distances, visited, 0, 1);
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(94, Solve("AOC2023/Day23/Example.txt", climbSlopes: false));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(2134, Solve("AOC2023/Day23/Input.txt", climbSlopes: false));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(154, Solve("AOC2023/Day23/Example.txt", climbSlopes: true));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(6298, Solve("AOC2023/Day23/Input.txt", climbSlopes: true));

        #endregion
    }
}
