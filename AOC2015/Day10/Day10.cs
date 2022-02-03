using Microsoft.VisualStudio.TestTools.UnitTesting;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AOC2015
{
    /// <summary>
    /// Solution for day 10:
    /// https://adventofcode.com/2015/day/10
    /// </summary>
    [TestClass]
    public class Day10
    {
        /// <summary>
        /// Play a game of look and say. Parse a string, and
        /// return the consecutive count of each char.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>The output string.</returns>
        string PlayLookAndSay(string input)
        {
            StringBuilder output = new();

            char currentChar = input[0];
            int currentCount = 0;

            foreach (char c in input)
            {
                if (c == currentChar)
                {
                    currentCount++;
                }
                else
                {
                    output.Append(Convert.ToString(currentCount));
                    output.Append(currentChar);

                    currentChar = c;
                    currentCount = 1;
                }
            }

            output.Append(Convert.ToString(currentCount));
            output.Append(currentChar);

            return output.ToString();
        }

        /// <summary>
        /// Plays look and say by repeatedly handling the same string.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="numTimes">The number of times.</param>
        /// <returns>The output.</returns>
        string PlayRepeatedLookAndSay(string input, int numTimes)
        {
            for (int i = 0; i < numTimes; i++)
            {
                input = PlayLookAndSay(input);
            }

            return input;
        }

        #region Solve Problems

        [TestMethod]
        public void TestExample1()
        {
            Assert.AreEqual("11", PlayLookAndSay("1"));
            Assert.AreEqual("21", PlayLookAndSay("11"));
            Assert.AreEqual("1211", PlayLookAndSay("21"));
            Assert.AreEqual("111221", PlayLookAndSay("1211"));
            Assert.AreEqual("312211", PlayLookAndSay("111221"));
        }

        [TestMethod]
        public void TestExample2() => Assert.AreEqual("312211", PlayRepeatedLookAndSay("11", 4));

        [TestMethod]
        public void TestSolution1() => Assert.AreEqual(360154, PlayRepeatedLookAndSay("1113122113", 40).Length);

        [TestMethod]
        public void TestSolution2() => Assert.AreEqual(5103798, PlayRepeatedLookAndSay("1113122113", 50).Length);

        #endregion
    }
}
