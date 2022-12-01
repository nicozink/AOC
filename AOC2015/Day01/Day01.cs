using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AOC2015
{
    /// <summary>
    /// Solution for day 1:
    /// https://adventofcode.com/2015/day/1
    /// </summary>
    [TestClass]
    public class Day01
    {
        /// <summary>
        /// Gets the floor based on the input string. A
        /// '(' means we go up one floor, and ')' means
        /// we go down one floor.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The floor.</returns>
        public int GetFloor(String path)
        {
            var input = System.IO.File.ReadAllText(path);

            var open = input.Count(x => x == '(');
            var closed = input.Count(x => x == ')');

            return open - closed;
        }

        /// <summary>
        /// Gets the index of the first instruction when reaching
        /// the basement.
        /// </summary>
        /// <param name="path">The input path.</param>
        /// <returns>The instruction index.</returns>
        public int GetBasementInstruction(String path)
        {
            var input = System.IO.File.ReadAllText(path);

            int position = 0;

            for (int i = 0; i < input.Length; ++i)
            {
                if (input[i] == '(')
                {
                    ++position;
                }
                else
                {
                    --position;
                }

                if (position == -1)
                {
                    return i + 1;
                }
            }

            return 0;
        }

        #region Solve Problems

        [TestMethod]
        public void TestSolution1() => Assert.AreEqual(74, GetFloor("AOC2015/Day01/Input.txt"));

        [TestMethod]
        public void TestSolution2() => Assert.AreEqual(1795, GetBasementInstruction("AOC2015/Day01/Input.txt"));

        #endregion
    }
}
