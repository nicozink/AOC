using Microsoft.VisualStudio.TestTools.UnitTesting;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AOC2015
{
    /// <summary>
    /// Solution for day 6:
    /// https://adventofcode.com/2015/day/6
    /// </summary>
    [TestClass]
    public class Day06
    {
        /// <summary>
        /// Stores a command which modifies the lights.
        /// </summary>
        enum Command
        {
            On,
            Off,
            Toggle
        };

        /// <summary>
        /// A point on the grid which represents a light.
        /// </summary>
        /// <param name="X">The x coordinate.</param>
        /// <param name="Y">The y coordinate.</param>
        record Point(int X, int Y);

        /// <summary>
        /// Stores an instruction which changes all lights in
        /// a rectangle between two points.
        /// </summary>
        /// <param name="cmd">The command.</param>
        /// <param name="From">The rectangle min bounds.</param>
        /// <param name="To">The rectangle max bounds.</param>
        record Instruction(Command cmd, Point From, Point To);

        /// <summary>
        /// Gets a command from a string.
        /// </summary>
        /// <param name="line">The string.</param>
        /// <returns>The command.</returns>
        Command GetCommand(string line)
        {
            if (line.Substring(0, 6) == "toggle")
            {
                return Command.Toggle;
            }
            else if (line.Substring(0, 7) == "turn on")
            {
                return Command.On;
            }
            else
            {
                return Command.Off;
            }
        }

        /// <summary>
        /// Reads the input from a file, and returns all commands.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The commands.</returns>
        IEnumerable<Instruction> ReadInput(string path)
        {
            var lines = System.IO.File.ReadLines(path);

            foreach (var line in lines)
            {
                Command cmd = GetCommand(line);

                var split = line.Replace("toggle ", "")
                    .Replace("turn on ", "")
                    .Replace("turn off ", "")
                    .Replace(" through ", ",")
                    .Split(',')
                    .Select(int.Parse)
                    .ToArray();

                Point from = new(split[0], split[1]);
                Point to = new(split[2], split[3]);

                yield return new(cmd, from, to);
            }
        }

        /// <summary>
        /// Gets all points in the rectange between the bounds.
        /// </summary>
        /// <param name="from">The min bounds.</param>
        /// <param name="to">The max bounds.</param>
        /// <returns>The points.</returns>
        IEnumerable<Point> GetPoints(Point from, Point to)
        {
            for (int x = from.X; x <= to.X; x++)
            {
                for (int y = from.Y; y <= to.Y; y++)
                {
                    yield return new(x, y);
                }
            }
        }

        /// <summary>
        /// Counts the number of lights after all instructions have been
        /// executed. Elvish commands are different from normal commands.
        /// </summary>
        /// <param name="path">The path to the commands.</param>
        /// <param name="elvish">Whether to interpret the commands as elvish.</param>
        /// <returns>The number of lights/brightness.</returns>
        int CountLights(string path, bool elvish)
        {
            int[,] lights = new int[1000, 1000];

            var commands = ReadInput(path);
            foreach (var cmd in commands)
            {
                foreach (var point in GetPoints(cmd.From, cmd.To))
                {
                    ref int value = ref lights[point.X, point.Y];

                    if (cmd.cmd == Command.On && (elvish || value == 0))
                    {
                        value++;
                    }
                    else if (cmd.cmd == Command.Off && value > 0)
                    {
                        value--;
                    }
                    else if (cmd.cmd == Command.Toggle && elvish)
                    {
                        value += 2;
                    }
                    else if (cmd.cmd == Command.Toggle && !elvish)
                    {
                        value = value == 0 ? 1 : 0;
                    }
                }
            }

            int sum = 0;
            foreach (var light in lights)
            {
                if (elvish)
                {
                    sum += light;
                }
                else if (light != 0)
                {
                    sum++;
                }
            }
            return sum;
        }

        #region Solve Problems

        [TestMethod]
        public void TestSolution1() => Assert.AreEqual(400410, CountLights("Day06/Input.txt", elvish: false));

        [TestMethod]
        public void TestSolution2() => Assert.AreEqual(15343601, CountLights("Day06/Input.txt", elvish: true));

        #endregion
    }
}
