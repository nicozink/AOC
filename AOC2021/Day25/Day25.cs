using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Common;

namespace AOC2021
{
    /// <summary>
    /// Solution for day 25:
    /// https://adventofcode.com/2021/day/25
    /// </summary>
    [TestClass]
    public class Day25
    {
        /// <summary>
        /// Stores a grid which represents the sea floor and two types
        /// of creature moving across (> and v).
        /// </summary>
        /// <param name="Grid"></param>
        record Board(char[][] Grid)
        {
            #region Public Methods

            /// <summary>
            /// Go through the creatures and move all that have an unobstructed path.
            /// </summary>
            /// <returns>True if some moves were made.</returns>
            public bool MakeMoves()
            {
                return MakeMoves('>') | MakeMoves('v');
            }

            /// <summary>
            /// Gets a string representation of the class.
            /// </summary>
            /// <returns>The string representation.</returns>
            public override String ToString()
            {
                string result = "";

                foreach (var row in Grid)
                {
                    result += string.Concat(row) + Environment.NewLine;
                }

                return result;
            }

            #endregion

            #region Private Methods

            /// <summary>
            /// Checls if a move is possible for the creature at the current position.
            /// </summary>
            /// <param name="position">The position.</param>
            /// <returns>True if a move can be made.</returns>
            private bool CanMove((int i, int j) position)
            {
                var (i, j) = GetNext(position);

                return Grid[i][j] == '.';
            }

            /// <summary>
            /// Gets the next position when a creature moves. Type
            /// > moves right, and v moves down.
            /// </summary>
            /// <param name="position">The position.</param>
            /// <returns>The next position.</returns>
            /// <exception cref="Exception">Throws if there is no creature.</exception>
            private (int i, int j) GetNext((int i, int j) position)
            {
                if (Grid[position.i][position.j] == '>')
                {
                    return position with
                    {
                        j = (position.j + 1) % Grid[position.i].Length
                    };
                }
                else if (Grid[position.i][position.j] == 'v')
                {
                    return position with
                    {
                        i = (position.i + 1) % Grid.Length
                    };
                }

                throw new Exception("Trying to get next of an unoccupied position.");
            }

            /// <summary>
            /// Gets all positions occupied by creatures of the given type.
            /// </summary>
            /// <param name="type">The creature type.</param>
            /// <returns>The positions occupied by that creature.</returns>
            private IEnumerable<(int i, int j)> GetOccupied(char type)
            {
                for (int i = 0; i < Grid.Length; i++)
                {
                    for (int j = 0; j < Grid[i].Length; j++)
                    {
                        if (Grid[i][j] == type)
                        {
                            yield return (i, j);
                        }
                    }
                }
            }

            /// <summary>
            /// Makes all possible moves by creatures of a certain type, where
            /// the creature isn't blocked.
            /// </summary>
            /// <param name="type">The type of creature.</param>
            /// <returns>True if any of them moved.</returns>
            public bool MakeMoves(char type)
            {
                var moves = GetOccupied(type)
                    .Where(x => CanMove(x))
                    .ToArray();

                foreach (var move in moves)
                {
                    char current = Grid[move.i][move.j];

                    var (i, j) = GetNext(move);
                    Grid[i][j] = current;

                    Grid[move.i][move.j] = '.';
                }

                return moves.Any();
            }

            #endregion
        }

        /// <summary>
        /// Reads the grid representing the sea floor from the file.
        /// Moves the creatures until they are all stuck, and returns
        /// the number of steps when that happens.
        /// </summary>
        /// <param name="input">The path to the input file.</param>
        /// <returns>The number of steps when creatures stop.</returns>
        private static int CountNumberOfMoves(string input)
        {
            var elements = System.IO.File.ReadLines(input)
                .Select(x => x.ToArray())
                .ToArray();

            var grid = new Board(elements);

            int count = 0;
            while (grid.MakeMoves())
            {
                count++;
            }

            // Need to add 1, since we currently have
            // the last step in which creatures moved,
            // and we want the first step when they no
            // longer move.
            return count + 1;
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample() => Assert.AreEqual(58, CountNumberOfMoves("Day25/Example.txt"));

        [TestMethod]
        public void SolvePart() => Assert.AreEqual(482, CountNumberOfMoves("Day25/Input.txt"));

        #endregion
    }
}
