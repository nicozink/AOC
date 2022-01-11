using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace AOC2021
{
    /// <summary>
    /// Solution for day 2:
    /// https://adventofcode.com/2021/day/2
    /// </summary>
    [TestClass]
    public class Day02
    {
        /// <summary>
        /// An enum for the directions in which the sub can move.
        /// </summary>
        enum Direction
        {
            Up,
            Down,
            Forward
        }

        /// <summary>
        /// Gets the commands from a file. Note that distance is
        /// modified as positive/negative based on up/down.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>The commands.</returns>
        IEnumerable<(Direction direction, int distance)> GetCommands(string path)
        {
            var lines = System.IO.File.ReadLines(path);

            foreach (var line in lines)
            {
                var info = line.Split();

                var distance = int.Parse(info[1]);

                if (info[0] == "forward")
                {
                    yield return (Direction.Forward, distance);
                }
                else if (info[0] == "up")
                {
                    yield return (Direction.Up, -distance);
                }
                else if (info[0] == "down")
                {
                    yield return (Direction.Down, distance);
                }
            }
        }

        /// <summary>
        /// Follow the commands. This is done in two ways. When not
        /// tracking aim, up/down moves the sub up/down horizontally.
        /// When tracking aim, up/down rotates the sub instead.
        /// Moving forward is thus adjusted accordingly.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="trackAim">Whether to track aim.</param>
        /// <returns>The product of total movement.</returns>
        int FollowCommands(string path, bool trackAim)
        {
            var commands = GetCommands(path);

            int position = 0;
            int depth = 0;
            int aim = 0;

            foreach (var command in commands)
            {
                if (command.direction == Direction.Forward)
                {
                    position += command.distance;
                    depth += command.distance * aim;
                }
                else if (trackAim)
                {
                    aim += command.distance;
                }
                else
                {
                    depth += command.distance;
                }
            }

            return position * depth;
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1()
        {
            Assert.AreEqual(150, FollowCommands("Day02/Example.txt", false));
        }

        [TestMethod]
        public void SolvePart1()
        {
            Assert.AreEqual(1507611, FollowCommands("Day02/Input.txt", false));
        }

        [TestMethod]
        public void SolveExample2()
        {
            Assert.AreEqual(900, FollowCommands("Day02/Example.txt", true));
        }

        [TestMethod]
        public void SolvePart2()
        {
            Assert.AreEqual(1880593125, FollowCommands("Day02/Input.txt", true));
        }

        #endregion
    }
}
