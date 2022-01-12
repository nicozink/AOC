using Microsoft.VisualStudio.TestTools.UnitTesting;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AOC2015
{
    /// <summary>
    /// Solution for day 5:
    /// https://adventofcode.com/2015/day/5
    /// </summary>
    [TestClass]
    public class Day05
    {
        /// <summary>
        /// Checks whether a string is naughty or nice.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>True if the string is nice.</returns>
        public bool IsNice1(String str)
        {
            // Check for the required number of vowels

            var vowels = new char[] { 'a', 'e', 'i', 'o', 'u' };

            if (str.Count(x => vowels.Contains(x)) < 3)
            {
                return false;
            }

            // Make sure we have at least one consecutive repeating letter

            var letters = Enumerable.Range('a', 26)
                .Select(x => (char)x)
                .Select(x => "" + x + x);

            if (!letters.Any(x => str.Contains(x)))
            {
                return false;
            }

            // Make sure we don't have forbidden sequences

            var forbidden = new String[] { "ab", "cd", "pq", "xy" };

            if (forbidden.Any(x => str.Contains(x)))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Reads the input from a file, and count the number of nice stings.
        /// </summary>
        /// <param name="path">The input file path.</param>
        /// <returns>The number of nice strings.</returns>
        public int CountNice1(string path) => System.IO.File.ReadLines(path).Count(IsNice1);

        /// <summary>
        /// Checks wheter a string has a sequence of two letters that is
        /// repeated.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>True if it contains the pattern.</returns>
        public bool HasPattern1(String str)
        {
            bool foundPattern1 = false;

            for (int x = 0; x < str.Length - 2; ++x)
            {
                String combo = str.Substring(x, 2);

                if (str.Substring(x + 2).Contains(combo))
                {
                    foundPattern1 = true;
                    break;
                }
            }

            return foundPattern1;
        }

        /// <summary>
        /// Checks wheter a string has a letter that repeats with another
        /// letter in-between. Such as xyx.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>True if it matches the pattern.</returns>
        public bool HasPattern2(String str)
        {
            bool foundPattern2 = false;

            for (int x = 0; x < str.Length - 2; ++x)
            {
                if (str[x] == str[x + 2])
                {
                    foundPattern2 = true;
                    break;
                }
            }

            return foundPattern2;
        }

        /// <summary>
        /// Checks whether a string is naughty or nice.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>True if the string is nice.</returns>
        public bool IsNice2(String str)
        {
            if (!HasPattern1(str))
            {
                return false;
            }

            if (!HasPattern2(str))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Reads a list of strings from a file, and returns the number which are nice.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The number of nice strings.</returns>
        public int CountNice2(string path) => System.IO.File.ReadLines(path).Count(IsNice2);

        #region Solve Problems

        [TestMethod]
        public void TestExample1()
        {
            Assert.IsTrue(IsNice1("ugknbfddgicrmopn"));
            Assert.IsTrue(IsNice1("aaa"));
            Assert.IsFalse(IsNice1("jchzalrnumimnmhp"));
            Assert.IsFalse(IsNice1("haegwjzuvuyypxyu"));
            Assert.IsFalse(IsNice1("dvszwmarrgswjxmb"));
        }

        [TestMethod]
        public void TestSolution1() => Assert.AreEqual(238, CountNice1("Day05/Input.txt"));

        [TestMethod]
        public void TestExample2()
        {
            Assert.IsTrue(HasPattern1("xyxy"));
            Assert.IsTrue(HasPattern1("aabcdefgaa"));
            Assert.IsFalse(HasPattern1("aaa"));

            Assert.IsTrue(HasPattern2("xyx"));
            Assert.IsTrue(HasPattern2("abcdefeghi"));
            Assert.IsTrue(HasPattern2("aaa"));

            Assert.IsTrue(IsNice2("xyxy"));
            Assert.IsTrue(IsNice2("qjhvhtzxzqqjkmpb"));
            Assert.IsTrue(IsNice2("xxyxx"));
            Assert.IsFalse(IsNice2("uurcxstgmygtbstg"));
            Assert.IsFalse(IsNice2("ieodomkazucvgmuy"));
        }

        [TestMethod]
        public void TestSolution2() => Assert.AreEqual(69, CountNice2("Day05/Input.txt"));

        #endregion
    }
}
