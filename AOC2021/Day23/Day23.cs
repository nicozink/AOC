using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;

namespace AOC2021
{
    /// <summary>
    /// Solution for day 23:
    /// https://adventofcode.com/2021/day/23
    /// </summary>
    [TestClass]
    public class Day23
    {
        /// <summary>
        /// This is the line length of the input - with a few bits of unnecessary
        /// bits removed.
        /// </summary>
        private const int LineLength = 11;

        /// <summary>
        /// These are the columns which are the final positions for amphipods grouped
        /// by type.
        /// </summary>
        private static readonly Dictionary<char, int> finalPositions = new()
        {
            { 'A', 2 },
            { 'B', 4 },
            { 'C', 6 },
            { 'D', 8 }
        };

        /// <summary>
        /// These are the costs for moving each amphipods one square.
        /// </summary>
        private static readonly Dictionary<char, int> moveCosts = new()
        {
            { 'A', 1 },
            { 'B', 10 },
            { 'C', 100 },
            { 'D', 1000 }
        };

        /// <summary>
        /// Stores the state of the game at each step - each amphipod can be
        /// moved around until they are all at theier destination.
        /// </summary>
        /// <param name="Board">The input board.</param>
        record GameState(
            string Board)
        {
            #region Properties

            /// <summary>
            /// Returns the number of rows ion the board.
            /// </summary>
            public int NumRows => Board.Length / LineLength;

            #endregion

            #region Public Methods

            /// <summary>
            /// Gets all valid moves which are possible for each amphipod from the current
            /// board layout.
            /// </summary>
            /// <returns>The valid moves - containing the previous and new position.</returns>
            public IEnumerable<((int X, int Y) Prev, (int X, int Y) Next)> GetValidMoves()
            {
                var positions = GetPositions();

                foreach (var prevPos in positions)
                {
                    if (!char.IsLetter(GetAt(prevPos)))
                    {
                        continue;
                    }

                    foreach (var nextPos in positions)
                    {
                        if (IsValidMove(prevPos, nextPos))
                        {
                            yield return (prevPos, nextPos);
                        }
                    }
                }
            }

            /// <summary>
            /// Returns true if this is the final layout - with each amphipod at the correct
            /// location.
            /// </summary>
            /// <returns>True if this is a final layout.</returns>
            public bool IsFinalLayout()
            {
                foreach (var (ch, idx) in finalPositions)
                {
                    for (int i = 1; i < NumRows; i++)
                    {
                        if (GetAt((idx, i)) != ch)
                        {
                            return false;
                        }
                    }
                }

                return true;
            }

            /// <summary>
            /// Makes a move for the amphipod at the previous position to the next position.
            /// Returns the new game state and new cost.
            /// </summary>
            /// <param name="prevPos">The previous position.</param>
            /// <param name="newPos">The new position.</param>
            /// <returns>The new state and cost of the move.</returns>
            public (GameState state, int cost) MakeMove((int X, int Y) prevPos, (int X, int Y) newPos)
            {
                var newAmphipods = Board;
                var pod = GetAt(prevPos);

                newAmphipods = ReplaceChar(newAmphipods, GetIndex(prevPos.X, prevPos.Y), '.');
                newAmphipods = ReplaceChar(newAmphipods, GetIndex(newPos.X, newPos.Y), GetAt(prevPos));

                var horizMoved = Math.Abs(newPos.X - prevPos.X);
                var vertMoved = prevPos.Y + newPos.Y;

                var cost = (horizMoved + vertMoved) * moveCosts[pod];

                return (new(newAmphipods), cost);
            }

            #endregion

            #region Private Methods

            /// <summary>
            /// Get the amphipod at the position.
            /// </summary>
            /// <param name="pos">The position.</param>
            /// <returns>The amphipod type.</returns>
            public char GetAt((int X, int Y) pos)
            {
                return Board[GetIndex(pos.X, pos.Y)];
            }

            /// <summary>
            /// Converts the position in the board to an index in the string storing the board.
            /// </summary>
            /// <param name="x">The x position.</param>
            /// <param name="y">The y position.</param>
            /// <returns>The index.</returns>
            private static int GetIndex(int x, int y)
            {
                return y * LineLength + x;
            }

            /// <summary>
            /// Gets the path along the board from the previous position to a new position.
            /// </summary>
            /// <param name="prev">The previous position.</param>
            /// <param name="next">The next position.</param>
            /// <returns>The path between the two points.</returns>
            private static IEnumerable<(int X, int Y)> GetPath((int X, int Y) prev, (int X, int Y) next)
            {
                if (prev.Y > 0)
                {
                    for (int y = prev.Y - 1; y >= 0; y--)
                    {
                        yield return (prev.X, y);
                    }
                }

                if (prev.X < next.X)
                {
                    for (int x = prev.X + 1; x <= next.X; x++)
                    {
                        yield return (x, 0);
                    }
                }

                if (prev.X > next.X)
                {
                    for (int x = prev.X - 1; x >= next.X; x--)
                    {
                        yield return (x, 0);
                    }
                }

                if (next.Y > 0)
                {
                    for (int y = 1; y <= next.Y; y++)
                    {
                        yield return (next.X, y);
                    }
                }
            }

            /// <summary>
            /// Gets all positions on the board which amphipods can move from or to.
            /// </summary>
            /// <returns>The positions on the board.</returns>
            private IEnumerable<(int X, int Y)> GetPositions()
            {
                yield return (0, 0);

                for (int i = 1; i <= 9; i += 2)
                {
                    yield return (i, 0);
                }

                yield return (10, 0);

                for (int i = 1; i < NumRows; i++)
                {
                    for (int j = 2; j <= 8; j += 2)
                    {
                        yield return (j, i);
                    }
                }
            }

            /// <summary>
            /// Checks wheter a move between two points is valid according to the rules..
            /// </summary>
            /// <param name="prev">The previous point.</param>
            /// <param name="next">The next point.</param>
            /// <returns>True if the move is valid.</returns>
            private bool IsValidMove((int X, int Y) prev, (int X, int Y) next)
            {
                // There is no reason to move vertically.
                if (prev.X == next.X)
                {
                    return false;
                }

                // Can not move into a spot which isn't empty!
                if (GetAt(next) != '.')
                {
                    return false;
                }

                // Once an amphipod has moved out, it cannot move
                // again except into it's home position. So we
                // can't move horizontally.
                if (prev.Y == 0 && next.Y == 0)
                {
                    return false;
                }

                var pod = GetAt(prev);
                var finalPosition = finalPositions[pod];
                var numRows = NumRows;

                // An amphipod that is in it's final position can
                // only move in a few cases where it's blocking the
                // exit for others.
                if (prev.Y > 0 && prev.X == finalPosition)
                {
                    // Check all cells that are below the previous cell. If we don't have any
                    // of a different type, then the move is illegal.
                    var previousCells = Enumerable.Range(prev.Y + 1, numRows - prev.Y - 1)
                        .Select(y => GetAt((prev.X, y)));

                    if (!previousCells.Any(x => x != pod))
                    {
                        return false;
                    }
                }

                // Similar rules apply when moving into a new cell, as we can't block any empty cells
                // or ones still holding a different type.
                if (next.Y > 0)
                {
                    if (next.X != finalPosition)
                    {
                        return false;
                    }

                    var previousCells = Enumerable.Range(next.Y + 1, numRows - next.Y - 1)
                        .Select(y => GetAt((next.X, y)));

                    if (previousCells.Any(x => x != pod))
                    {
                        return false;
                    }
                }

                // Assuming all other checks have passed, we need to inspect the path taken
                // to make sure it doesn't cross any other occupied spots.
                foreach (var pos in GetPath(prev, next))
                {
                    if (GetAt(pos) != '.')
                    {
                        return false;
                    }
                }

                return true;
            }

            /// <summary>
            /// When making a move, replace the character at a specific index with a different one.
            /// </summary>
            /// <param name="str">The input string.</param>
            /// <param name="index">The index.</param>
            /// <param name="c">The new character.</param>
            /// <returns>The new string with the replaced character.</returns>
            public static string ReplaceChar(string str, int index, char c)
            {
                StringBuilder stringBuilder = new(str);
                stringBuilder[index] = c;

                return stringBuilder.ToString();
            }

            #endregion
        }

        /// <summary>
        /// Process all possible moves to find the combination of moves that moves all
        /// amphipods to the correct spot with the least amount of energy.
        /// </summary>
        /// <param name="gameState">The current game state.</param>
        /// <param name="cache">A lookup cache to store previous results.</param>
        /// <returns>The minimum required energy.</returns>
        private static int FindLeastEnergy(GameState gameState, ref Dictionary<string, int> cache)
        {
            if (cache.ContainsKey(gameState.Board))
            {
                return cache[gameState.Board];
            }

            if (gameState.IsFinalLayout())
            {
                return 0;
            }

            var minCost = int.MaxValue;

            var validMoves = gameState.GetValidMoves();
            foreach (var (prev, next) in validMoves)
            {
                var (newState, cost) = gameState.MakeMove(prev, next);

                var newCost = FindLeastEnergy(newState, ref cache);

                if (newCost != int.MaxValue)
                {
                    var totalCost = cost + newCost;

                    if (totalCost < minCost)
                    {
                        minCost = totalCost;
                    }
                }
            }

            cache.Add(gameState.Board, minCost);

            return minCost;
        }

        /// <summary>
        /// Reads the input from the file, and finds the minimum required energy
        /// to move all amphipods into the correct location.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>The minimum energy.</returns>
        private static int FindLeastEnergy(string path)
        {
            // Remove nonsense from input string. This includes extra lines of ####
            // and padding for the first and last columns. Also pads any lines to the
            // desited line count, so that we get a uniform string.
            var lines = System.IO.File.ReadLines(path)
                .Select(x => x.Substring(1, Math.Min(x.Length - 2, LineLength)).PadRight(LineLength))
                .ToList();

            lines.RemoveAt(0);
            lines.RemoveAt(lines.Count - 1);

            GameState inputState = new(string.Concat(lines));

            Dictionary<string, int> cache = new();
            return FindLeastEnergy(inputState, ref cache);
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(12521, FindLeastEnergy("Day23/Example1.txt"));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(15109, FindLeastEnergy("Day23/Input1.txt"));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(44169, FindLeastEnergy("Day23/Example2.txt"));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(53751, FindLeastEnergy("Day23/Input2.txt"));

        #endregion
    }
}
