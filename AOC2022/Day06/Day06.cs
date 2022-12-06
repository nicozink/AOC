using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace AOC2022
{
    /// <summary>
    /// Solution for day 6:
    /// https://adventofcode.com/2022/day/6
    /// </summary>
    [TestClass]
    public class Day06
    {
        /// <summary>
        /// Gets the position of the first sequence of unique characters
        /// of a given lenght. Returns marker position which is the first
        /// index after the first sequence.
        /// </summary>
        /// <param name="input">The input message.</param>
        /// <param name="markerLength">The length of unique characters.</param>
        /// <returns>The start of the character.</returns>
        /// <exception cref="Exception">An exception is thrown is a sequence is not found.</exception>
        private static int GetMarkerPosition(string input, int markerLength)
        {
            for (int i = 0; i < input.Length - markerLength; i++)
            {
                var distinctCount = input.Substring(i, markerLength)
                    .Distinct()
                    .Count();

                if (distinctCount == markerLength)
                {
                    return i + markerLength;
                }
            }

            throw new Exception("Could no find a marker.");
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1()
        {
            Assert.AreEqual(7, GetMarkerPosition("mjqjpqmgbljsphdztnvjfqwrcgsmlb", 4));
            Assert.AreEqual(5, GetMarkerPosition("bvwbjplbgvbhsrlpgdmjqwftvncz", 4));
            Assert.AreEqual(6, GetMarkerPosition("nppdvjthqldpwncqszvftbrmjlhg", 4));
            Assert.AreEqual(10, GetMarkerPosition("nznrnfrfntjfmvfwmzdfjlvtqnbhcprsg", 4));
            Assert.AreEqual(11, GetMarkerPosition("zcfzfwzzqfrljwzlrfnpqdbhtmscgvjw", 4));
        }

        [TestMethod]
        public void SolvePart1()
        {
            var input = System.IO.File.ReadAllText("AOC2022/Day06/Input.txt");
            Assert.AreEqual(1578, GetMarkerPosition(input, 4));
        }

        [TestMethod]
        public void SolveExample2()
        {
            Assert.AreEqual(19, GetMarkerPosition("mjqjpqmgbljsphdztnvjfqwrcgsmlb", 14));
            Assert.AreEqual(23, GetMarkerPosition("bvwbjplbgvbhsrlpgdmjqwftvncz", 14));
            Assert.AreEqual(23, GetMarkerPosition("nppdvjthqldpwncqszvftbrmjlhg", 14));
            Assert.AreEqual(29, GetMarkerPosition("nznrnfrfntjfmvfwmzdfjlvtqnbhcprsg", 14));
            Assert.AreEqual(26, GetMarkerPosition("zcfzfwzzqfrljwzlrfnpqdbhtmscgvjw", 14));
        }

        [TestMethod]
        public void SolvePart2()
        {
            var input = System.IO.File.ReadAllText("AOC2022/Day06/Input.txt");
            Assert.AreEqual(2178, GetMarkerPosition(input, 14));
        }

        #endregion
    }
}
