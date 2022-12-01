using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Common;

namespace AOC2021
{
    /// <summary>
    /// Solution for day 24:
    /// https://adventofcode.com/2021/day/24
    /// </summary>
    [TestClass]
    public class Day24
    {
        /// <summary>
        /// Takes a command and input state, and executes a part of the program.
        /// This was modified from the input, so all we need are the three constants
        /// which change from each subroutine.
        /// </summary>
        /// <param name="commands">The constants for the command.</param>
        /// <param name="state">The input state (current command, and input z).</param>
        /// <param name="input">The input for the current command to process.</param>
        /// <returns>The new z value.</returns>
        private static int ExecuteCommands((int divZ, int addX, int addY)[] commands, (int commandIndex, int z) state, int input)
        {
            var (divZ, addX, addY) = commands[state.commandIndex];

            int w = input;
            int z = state.z;
            
            int x = state.z % 26 + addX;

            if (x == w)
            {
                x = 0;
            }
            else
            {
                x = 1;
            }

            z /= divZ;
            z *= 25 * x + 1;
            z += (w + addY) * x;

            return z;
        }

        /// <summary>
        /// Executes the command for the given state, and returns all input sequences which
        /// result in a final value of 0.
        /// </summary>
        /// <param name="commands">The commands.</param>
        /// <param name="state">The current state.</param>
        /// <param name="lookupCache">A lookup cache to find previous known combinations.</param>
        /// <returns>The numbers which lead to a valid z.</returns>
        private static List<string> ExecuteCommands((int divZ, int addX, int addY)[] commands, (int commandIndex, int z) state, ref Dictionary<(int commandIndex, int z), List<string>> lookupCache)
        {
            if (lookupCache.ContainsKey(state))
            {
                return lookupCache[state];
            }

            List<string> result = new();

            for (int i = 1; i <= 9; i++)
            {
                var newZ = ExecuteCommands(commands, state, i);

                if (state.commandIndex == 13)
                {
                    if (newZ == 0)
                    {
                        result.Add(Convert.ToString(i));
                    }
                }
                else
                {
                    foreach (var validString in ExecuteCommands(commands, (state.commandIndex + 1, newZ), ref lookupCache))
                    {
                        result.Add(Convert.ToString(i) + validString);
                    }
                }
            }

            lookupCache.Add(state, result);

            return result;
        }

        /// <summary>
        /// Reads the program inoput from the file. Executes the command
        /// for all input combinations, and returns the list of inputs that
        /// are valid.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The valid input numbers.</returns>
        static List<string> FindNumbers(string path)
        {
            // The input is basically 18 lines which are repeated several
            // times with three constants that differ for each section.
            // We scan each of these groups, and read out just the constants.
            // The operations have been converted to c# for better speed solving
            // the combinations.

            var rawCommands = System.IO.File.ReadLines(path);

            int i = 0;
            var groupedCommands = from command in rawCommands
                                  group command by i++ / 18 into part
                                  select part.ToArray();

            var commands = groupedCommands
                .Select(x => (
                    int.Parse(x[4].Split()[2]),
                    int.Parse(x[5].Split()[2]),
                    int.Parse(x[15].Split()[2])
                )).ToArray();

            var lookupCache = new Dictionary<(int commandIndex, int z), List<string>>();
            var validNumbers = ExecuteCommands(commands, (0, 0), ref lookupCache);

            validNumbers.Sort();
            return validNumbers;
        }

        #region Solve Problems

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual("53999995829399", FindNumbers("AOC2021/Day24/Input.txt").Last());

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual("11721151118175", FindNumbers("AOC2021/Day24/Input.txt").First());

        #endregion
    }
}
