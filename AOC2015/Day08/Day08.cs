using Microsoft.VisualStudio.TestTools.UnitTesting;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using Common.StringHelper;

namespace AOC2015
{
    /// <summary>
    /// Solution for day 8:
    /// https://adventofcode.com/2015/day/8
    /// </summary>
    [TestClass]
    public class Day08
    {
        /// <summary>
        /// Takes a string, and calculates the size difference after removing all special
        /// characters.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>The size difference.</returns>
        int CalculateStringDiff1(string str)
        {
            // We automatically remove the first and last quote, as that isn't part of the string.
            int whitespaceDiff = 2;

            for (int x = 1; x < str.Length - 1; x++)
            {
                if (str[x] == '\\')
                {
                    char next = str[x + 1];

                    switch (next)
                    {
                        // A slash followed by another slash or a quote indicates an escaped
                        // character - so we remove the slash.
                        case '\\':
                        case '\"':
                            {
                                whitespaceDiff++;
                                x++;

                                break;
                            }
                        // A slash followed by an x is an ascii character. The whole sequence,
                        // e.g. \x24 is replaced by a single character.
                        case 'x':
                            {
                                whitespaceDiff += 3;
                                x += 3;

                                break;
                            }
                    }
                }
            }

            return whitespaceDiff;
        }

        /// <summary>
        /// Reads a file, and calculates the size difference after removing all special
        /// characters.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The difference.</returns>
        int CalculateFileDiff1(string path) => System.IO.File.ReadLines(path).Sum(CalculateStringDiff1);

        /// <summary>
        /// Takes a string, and calculates the size difference after adding all required
        /// special characters.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>The size difference.</returns>
        int CalculateStringDiff2(string str)
        {
            // We add enclosing quotes.
            int whitespaceDiff = 2;

            for (int x = 0; x < str.Length; x++)
            {
                char next = str[x];

                // Slashes and quotes need to be escaped.
                if (next == '\\' || next == '\"')
                {
                    whitespaceDiff++;
                }
            }

            return whitespaceDiff;
        }

        /// <summary>
        /// Reads a file, and calculates the size difference after adding all required
        /// special characters.
        /// </summary>
        /// <param name="path">The input file.</param>
        /// <returns>The size difference.</returns>
        int CalculateFileDiff2(string path)
        {
            return System.IO.File.ReadLines(path)
                .Sum(CalculateStringDiff2);
        }

        #region Solve Problems

        [TestMethod]
        public void TestExample1()
        {
            Assert.AreEqual(2, CalculateStringDiff1("\"\""));
            Assert.AreEqual(2, CalculateStringDiff1("\"abc\""));
            Assert.AreEqual(3, CalculateStringDiff1("\"aaa\\\"aaa\""));
            Assert.AreEqual(5, CalculateStringDiff1("\"\\x27\""));
        }

        [TestMethod]
        public void TestSolution1() => Assert.AreEqual(1342, CalculateFileDiff1("Day08/Input.txt"));

        [TestMethod]
        public void TestExample2()
        {
            Assert.AreEqual(4, CalculateStringDiff2("\"\""));
            Assert.AreEqual(4, CalculateStringDiff2("\"abc\""));
            Assert.AreEqual(6, CalculateStringDiff2("\"aaa\\\"aaa\""));
            Assert.AreEqual(5, CalculateStringDiff2("\"\\x27\""));
        }

        [TestMethod]
        public void TestSolution2() => Assert.AreEqual(2074, CalculateFileDiff2("Day08/Input.txt"));

        #endregion
    }
}
