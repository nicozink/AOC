using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace AOC2022
{
    /// <summary>
    /// Solution for day 10:
    /// https://adventofcode.com/2022/day/10
    /// </summary>
    [TestClass]
    public class Day10
    {
        /// <summary>
        /// Reads the commands from the input file.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The commands.</returns>
        private static IEnumerable<int> ReadInput(string path)
        {
            var lines = System.IO.File.ReadLines(path);

            foreach (var line in lines)
            {
                if (line == "noop")
                {
                    yield return 0; // A no-op takes one cycle to complete and does nothing.
                }
                else
                {
                    yield return 0; // The add takes two cycles, in which the first does nothing.

                    var cmd = line.Split();
                    yield return int.Parse(cmd[1]); // Return the value to add in the second cycle.
                }
            }    
        }

        /// <summary>
        /// Executes the commands given the input.
        /// </summary>
        /// <param name="start">The start value of the x register.</param>
        /// <param name="cmds">The commands to execute.</param>
        /// <returns>The list of values.</returns>
        private static IEnumerable<int> Execute(int start, IEnumerable<int> cmds)
        {
            int x = start;
            foreach (var cmd in cmds)
            {
                yield return x; // The value changes at the end of the cycle, so return the value first.
                x += cmd; // Then apply the value at the end of the cycle.
            }

            yield return x; // Need to return the final value as well.
        }

        /// <summary>
        /// Gets the sm of the signal strenghts at cycle 20, 60, 100, etc. in 40 cycle intervals.
        /// </summary>
        /// <param name="path">The paht to the input file.</param>
        /// <returns>The sum of signal strengths.</returns>
        private static int GetSignalStrengthSum(string path)
        {
            var cmds = ReadInput(path);
            var results = Execute(1, cmds).ToArray();

            int sum = 0;
            for (int i = 19; i < results.Length; i+=40)
            {
                sum += results[i] * (i + 1);
            }

            return sum;
        }

        /// <summary>
        /// Prints the output to the CRT display.
        /// </summary>
        /// <param name="path">The paht to the input file.</param>
        private static void PrintSolution(string path)
        {
            var cmds = ReadInput(path);
            var results = Execute(1, cmds).ToArray();

            for (int row = 0; row < 6; row++)
            {
                for (int pixel = 0; pixel < 40; pixel++)
                {
                    int index = row * 40 + pixel;
                    var value = results[index];

                    if (value >= pixel - 1 && value <= pixel + 1)
                    {
                        Console.Write("#");
                    }
                    else
                    {
                        Console.Write(".");
                    }
                }

                Console.WriteLine();
            }
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(13140, GetSignalStrengthSum("AOC2022/Day10/Example.txt"));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(14820, GetSignalStrengthSum("AOC2022/Day10/Input.txt"));

        [TestMethod]
        public void SolveExample2() => PrintSolution("AOC2022/Day10/Example.txt");

        [TestMethod]
        public void SolvePart2() => PrintSolution("AOC2022/Day10/Input.txt");

        #endregion
    }
}
