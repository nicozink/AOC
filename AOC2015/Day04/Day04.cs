using Microsoft.VisualStudio.TestTools.UnitTesting;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace AOC2015
{
    /// <summary>
    /// Solution for day 4:
    /// https://adventofcode.com/2015/day/4
    /// </summary>
    [TestClass]
    public class Day04
    {
        /// <summary>
        /// Takes an input string, and searches for a number which
        /// when appended to the input produces an md5 hash starting
        /// with zeroes.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="zeroes">The number of zeroes to look for.</param>
        /// <returns>The answer.</returns>
        /// <exception cref="Exception">Thrown when an answer couldn't be found.</exception>
        private static int GetHashAnswer(string input, int zeroes)
        {
            string prefix = "".PadLeft(zeroes, '0');

            using (MD5 md5 = MD5.Create())
            {
                var lowest = 0;
                var highest = (int)Math.Pow(10, input.Length);
                for (int answer = lowest; answer < highest; answer++)
                {
                    var inputString = input + answer;

                    byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(inputString);
                    byte[] hashBytes = md5.ComputeHash(inputBytes);

                    string hexString = BitConverter.ToString(hashBytes).Replace("-", "");

                    if (hexString.StartsWith(prefix))
                    {
                        return answer;
                    }
                }
            }

            throw new Exception("No answer was found.");
        }

        #region Solve Problems

        [TestMethod]
        public void TestExample1()
        {
            Assert.AreEqual(609043, GetHashAnswer("abcdef", 5));
            Assert.AreEqual(1048970, GetHashAnswer("pqrstuv", 5));
        }

        [TestMethod]
        public void TestSolution1() => Assert.AreEqual(117946, GetHashAnswer("ckczppom", 5));

        [TestMethod]
        public void TestSolution2() => Assert.AreEqual(3938038, GetHashAnswer("ckczppom", 6));

        #endregion
    }
}
