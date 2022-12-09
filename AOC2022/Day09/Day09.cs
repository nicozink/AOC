using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace AOC2022
{
    /// <summary>
    /// Solution for day 9:
    /// https://adventofcode.com/2022/day/9
    /// </summary>
    [TestClass]
    public class Day09
    {
        /// <summary>
        /// Stores a direction in which the head of the rope can move.
        /// </summary>
        enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }

        /// <summary>
        /// Stores a command which moves the head in the direction by a given value.
        /// </summary>
        /// <param name="Direction">The direction the head moves.</param>
        /// <param name="NumMoves">The number of units the head moves.</param>
        record Command(Direction Direction, int NumMoves);

        /// <summary>
        /// The position of a node in the string.
        /// </summary>
        /// <param name="X">The x coordinate.</param>
        /// <param name="Y">The y coordinate.</param>
        record Position(int X, int Y);

        /// <summary>
        /// Read the commands from the input file.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The commands.</returns>
        /// <exception cref="Exception">Throws an exception if we have invalid data.</exception>
        private static IEnumerable<Command> ReadInput(string path)
        {
            var lines = System.IO.File.ReadLines(path);

            foreach (var line in lines)
            {
                var command = line.Split();

                Direction direction = command[0] switch
                {
                    "U" => Direction.Up,
                    "D" => Direction.Down,
                    "L" => Direction.Left,
                    "R" => Direction.Right,
                    _ => throw new Exception("Invalid di8rection value")
                };

                var numMoves = int.Parse(command[1]);

                yield return new Command(direction, numMoves);
            }
        }

        /// <summary>
        /// Move the head of the chain in a direction, and move
        /// all subsequent nodes along the chain.
        /// </summary>
        /// <param name="chain">The chain.</param>
        /// <param name="direction">The direction in which the head moves.</param>
        /// <exception cref="Exception">Throws an exception for an invalid direction.</exception>
        private static void MoveHead(Position[] chain, Direction direction)
        {
            var head = chain[0];
            head = direction switch
            {
                Direction.Up => head with { Y = head.Y - 1 },
                Direction.Down => head with { Y = head.Y + 1 },
                Direction.Left => head with { X = head.X - 1 },
                Direction.Right => head with { X = head.X + 1 },
                _ => throw new Exception("Invalid direction value")
            };
            chain[0] = head;

            for (int i = 1; i < chain.Length; i++)
            {
                var prev = chain[i - 1];
                var next = chain[i];

                var xDiff = prev.X - next.X;
                var yDiff = prev.Y - next.Y;

                if (Math.Abs(xDiff) == 2)
                {
                    next = next with { X = next.X + Math.Sign(xDiff) };

                    if (Math.Abs(yDiff) > 0)
                    {
                        next = next with { Y = next.Y + Math.Sign(yDiff) };
                    }
                }
                else if (Math.Abs(yDiff) == 2)
                {
                    next = next with { Y = next.Y + Math.Sign(yDiff) };

                    if (Math.Abs(xDiff) > 0)
                    {
                        next = next with { X = next.X + Math.Sign(xDiff) };
                    }
                }

                chain[i] = next;
            }
        }

        /// <summary>
        /// Takes a chain and evaluates the commands by moving the nodes in the chain.
        /// </summary>
        /// <param name="chain">The nodes in the chain.</param>
        /// <param name="commands">The movement commands.</param>
        /// <returns>The modified nodes in the chain.</returns>
        private static IEnumerable<Position[]> ExecuteCommands(Position[] chain, IEnumerable<Command> commands)
        {
            foreach (var command in commands)
            {
                for (int i = 0; i < command.NumMoves; i++)
                {
                    MoveHead(chain, command.Direction);
                    yield return chain;
                }
            }    
        }

        /// <summary>
        /// Reads the commands from the file, and modifies the chain.
        /// Calculates the unique locations of the tail of the chain.
        /// </summary>
        /// <param name="path">The path containing the commands.</param>
        /// <param name="numLinks">The number of links in the chain.</param>
        /// <returns>The number of unique tail positions.</returns>
        private static int CountPlacesVisited(string path, int numLinks = 2)
        {
            var defaultStart = new Position(0, 0);
            var chain = Enumerable.Range(0, numLinks)
                .Select(i => defaultStart)
                .ToArray();

            var commands = ReadInput(path);

            return ExecuteCommands(chain, commands)
                .Select(x => x[numLinks - 1])
                .Distinct()
                .Count();
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(13, CountPlacesVisited("AOC2022/Day09/Example1.txt"));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(5683, CountPlacesVisited("AOC2022/Day09/Input.txt"));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(1, CountPlacesVisited("AOC2022/Day09/Example1.txt", 10));

        [TestMethod]
        public void SolveExample3() => Assert.AreEqual(36, CountPlacesVisited("AOC2022/Day09/Example2.txt", 10));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(2372, CountPlacesVisited("AOC2022/Day09/Input.txt", 10));

        #endregion
    }
}
