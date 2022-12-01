using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AOC2020
{
    /// <summary>
    /// Solution for day 12:
    /// https://adventofcode.com/2020/day/12
    /// </summary>
    [TestClass]
    public class Day12
    {
        /// <summary>
        /// Stores a command for the movement of the ship.
        /// </summary>
        class ShipCommand
        {
            /// <summary>
            /// Stores the command. This includes 'N', 'E', 'S', 'W'
            /// for movement, L and R for rotation, and F to move
            /// forward.
            /// </summary>
            public char Command;

            /// <summary>
            /// Stores a value associated with the command,
            /// e.g. move a number of places, or rotate
            /// a number of degrees.
            /// </summary>
            public int Value;
        }

        /// <summary>
        /// Gets the ship movement commands from a file.
        /// </summary>
        /// <param name="path">The path to the movement commands.</param>
        /// <returns>The movement commands.</returns>
        IEnumerable<ShipCommand> GetShipCommands(String path)
        {
            var input = System.IO.File.ReadLines(path);

            foreach (var line in input)
            {
                char action = line[0];
                int value = int.Parse(line.Substring(1));

                yield return new ShipCommand()
                {
                    Command = action,
                    Value = value
                };
            }
        }

        /// <summary>
        /// Moves the position based on the direction and distance value.
        /// </summary>
        /// <param name="position">The original position.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="value">The distance to move.</param>
        /// <returns>The new position.</returns>
        Tuple<int, int> Move(Tuple<int, int> position, Tuple<int, int> direction, int value)
        {
            var positionX = position.Item1;
            var positionY = position.Item2;

            positionX += direction.Item1 * value;
            positionY += direction.Item2 * value;

            return new Tuple<int, int>(positionX, positionY);
        }

        /// <summary>
        /// Stores a mapping between movement codes, and direction.
        /// </summary>
        Dictionary<char, Tuple<int, int>> commandDirection = new Dictionary<char, Tuple<int, int>>()
        {
            { 'N', new Tuple<int, int>(0, 1) },
            { 'S', new Tuple<int, int>(0, -1) },
            { 'E', new Tuple<int, int>(1, 0) },
            { 'W', new Tuple<int, int>(-1, 0) }
        };

        /// <summary>
        /// Calculates a new position based on the command.
        /// </summary>
        /// <param name="position">The original position.</param>
        /// <param name="command">The move command.</param>
        /// <returns>The new position.</returns>
        Tuple<int, int> Move(Tuple<int, int> position, ShipCommand command)
        {
            var direction = commandDirection[command.Command];
            return Move(position, direction, command.Value);
        }

        /// <summary>
        /// Rotate the direction left or right based on the command.
        /// </summary>
        /// <param name="direction">The direction.</param>
        /// <param name="command">The command.</param>
        /// <returns>The new direction.</returns>
        Tuple<int, int> Rotate(Tuple<int, int> direction, ShipCommand command)
        {
            int degreeRotation = command.Value;

            // We transform a left rotation into a right rotation.
            if (command.Command == 'L')
            {
                degreeRotation = 360 - degreeRotation;
            }

            // Get the number of 90 degree rotations.
            int numRotations = degreeRotation / 90;

            // For simplicity, we just repeat 90 degree rotations.
            for (int i = 0; i < numRotations; ++i)
            {
                var newX = direction.Item2;
                var newY = -direction.Item1;

                direction = new Tuple<int, int>(newX, newY);
            }

            return direction;
        }

        /// <summary>
        /// The movemement type of the ship. We can
        /// move the ship only, or move a waypoint
        /// that is relative to the ship.
        /// </summary>
        enum MovementType
        {
            Ship,
            Waypoint
        }

        /// <summary>
        /// Moves and rotates the ship based on the commands provided.
        /// </summary>
        /// <param name="path">The path to the commands.</param>
        /// <param name="direction">The starting directino of the ship.</param>
        /// <param name="type">The movement type.</param>
        /// <returns>The manhattan distance travelled.</returns>
        int ExecuteShipCommands(String path, Tuple<int, int> direction, MovementType type)
        {
            var commands = GetShipCommands(path);

            var shipPosition = new Tuple<int, int>(0, 0);

            foreach (var command in commands)
            {
                if (commandDirection.ContainsKey(command.Command))
                {
                    if (type == MovementType.Ship)
                    {
                        shipPosition = Move(shipPosition, command);
                    }
                    else
                    {
                        direction = Move(direction, command);
                    }
                }
                else if (command.Command == 'L' || command.Command == 'R')
                {
                    direction = Rotate(direction, command);
                }
                else if (command.Command == 'F')
                {
                    shipPosition = Move(shipPosition, direction, command.Value);
                }
            }

            return Math.Abs(shipPosition.Item1) + Math.Abs(shipPosition.Item2);
        }

        public int GetSolution1(String path)
        {
            return ExecuteShipCommands(path, new Tuple<int, int>(1, 0), MovementType.Ship);
        }

        public long GetSolution2(String path)
        {
            return ExecuteShipCommands(path, new Tuple<int, int>(10, 1), MovementType.Waypoint);
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(25, GetSolution1("AOC2020/Day12/Example.txt"));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(319, GetSolution1("AOC2020/Day12/Input.txt"));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(286, GetSolution2("AOC2020/Day12/Example.txt"));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(50157, GetSolution2("AOC2020/Day12/Input.txt"));

        #endregion
    }
}
