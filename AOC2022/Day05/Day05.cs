using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace AOC2022
{
    /// <summary>
    /// Solution for day 5:
    /// https://adventofcode.com/2022/day/5
    /// </summary>
    [TestClass]
    public class Day05
    {
        /// <summary>
        /// Stores a move where the crane picks up a number oif objects, and
        /// moves them from one stack to the other.
        /// </summary>
        /// <param name="Num">The number of objects to move.</param>
        /// <param name="From">The source stack.</param>
        /// <param name="To">The destination stack.</param>
        private record Move(int Num, int From, int To);

        /// <summary>
        /// Stores the stacks that the crane operates on, and the move instructions
        /// which the crane will follow.
        /// </summary>
        private class Stacks
        {
            /// <summary>
            /// The move instructions for the crane to follow.
            /// </summary>
            private readonly List<Move> moves;

            /// <summary>
            /// The stacks the crane operates on.
            /// </summary>
            private readonly Stack<char>[] stacks;

            /// <summary>
            /// Creates the stacks and gets the moves from the input file.
            /// </summary>
            /// <param name="path">The path to the input file.</param>
            public Stacks(string path)
            {
                (stacks, moves) = ReadInput(path);
            }

            /// <summary>
            /// Gets the final message consisting of the top crate for each stack.
            /// </summary>
            /// <returns>The message.</returns>
            public string GetMessage()
            {
                string output = "";

                foreach (var stack in stacks)
                {
                    output += stack.Peek();
                }

                return output;
            }

            /// <summary>
            /// Get the crane to follow the instructions, and move the crates around. Crane version
            /// 9000 moves one crate at a time, but crane version 9001 moves all crates at one
            /// (preserving their order).
            /// </summary>
            /// <param name="craneVersion">The crane version (9000 or 9001).</param>
            public void Solve(int craneVersion)
            {
                foreach (var move in moves)
                {
                    var from = stacks[move.From];
                    var to = stacks[move.To];

                    if (craneVersion == 9000)
                    {
                        MoveObjects(from, to, move.Num);
                    }
                    else
                    {
                        var crane = new Stack<char>();
                        MoveObjects(from, crane, move.Num);
                        MoveObjects(crane, to, move.Num);
                    }
                }
            }

            /// <summary>
            /// Read the stack layout and list of moves. See the problem description for
            /// the format.
            /// /// </summary>
            /// <param name="path">The file path.</param>
            /// <returns>The stack layout and list of moves.</returns>
            private static (Stack<char>[] , List<Move>) ReadInput(string path)
            {
                var lines = System.IO.File.ReadAllLines(path);

                // Get the number of stacks. Each line has the format
                // [c] [c] [c] [c], so we pad the length with an extra
                // space at the end and divide by four characters to
                // get the number of stacks.
                var numStacks = (lines[0].Length + 1) / 4;
                var stacks = Enumerable.Range(0, numStacks)
                    .Select(x => new Stack<char>())
                    .ToArray();
                
                // Find the new line that seperates the stack layout
                // from the list of moves.
                int newLineIndex = 0;
                for (; newLineIndex < lines.Length; newLineIndex++)
                {
                    if (string.IsNullOrEmpty(lines[newLineIndex]))
                    {
                        break;
                    }
                }

                // Read the stacks from the first part of the file
                for (int i = newLineIndex - 2; i >= 0; i--)
                {
                    for (int s = 0; s < numStacks; s++)
                    {
                        var stackPos = s * 4 + 1;

                        var ch = lines[i][stackPos];
                        if (ch != ' ')
                        {
                            stacks[s].Push(ch);
                        }
                    }
                }

                // Read the moves from the second part of the file
                var moves = new List<Move>();
                for (var i = newLineIndex + 1; i < lines.Length; i++)
                {
                    var line = lines[i];

                    // The line is something like "move 19 from 7 to 8" so
                    // edit it so we can easily parse it
                    var moveDesc = line
                        .Replace("move ", "")
                        .Replace(" from ", " ")
                        .Replace(" to ", " ")
                        .Split()
                        .Select(x => int.Parse(x))
                        .ToArray();

                    moves.Add(new Move(moveDesc[0], moveDesc[1] - 1, moveDesc[2] - 1));
                }

                return (stacks, moves);
            }

            /// <summary>
            /// Use the crane to move objects from one stack to the other.
            /// </summary>
            /// <param name="from">The source stack.</param>
            /// <param name="to">The destination stack.</param>
            /// <param name="numMoves">The number of objects to move.</param>
            private static void MoveObjects(Stack<char> from, Stack<char> to, int numMoves)
            {
                for (var i = 0; i < numMoves; i++)
                {
                    var ch = from.Pop();
                    to.Push(ch);
                }
            }
        }

        /// <summary>
        /// Parse the input file, apply the move with the crane, and
        /// get the message from the top crates.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="craneVersion">The crate version (either 9000 or 9001).</param>
        /// <returns>The message from the top crates.</returns>
        private static string GetTopCrates(string path, int craneVersion)
        {
            var stacks = new Stacks(path);
            stacks.Solve(craneVersion);
            return stacks.GetMessage();
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual("CMZ", GetTopCrates("AOC2022/Day05/Example.txt", 9000));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual("SBPQRSCDF", GetTopCrates("AOC2022/Day05/Input.txt", 9000));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual("MCD", GetTopCrates("AOC2022/Day05/Example.txt", 9001));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual("RGLVRCQSB", GetTopCrates("AOC2022/Day05/Input.txt", 9001));

        #endregion
    }
}
