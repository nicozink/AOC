using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using System.Text.RegularExpressions;

namespace AOC2022
{
    /// <summary>
    /// Solution for day 25:
    /// https://adventofcode.com/2022/day/25
    /// </summary>
    [TestClass]
    public class Day25
    {
        /// <summary>
        /// Takes a number in SNAFU format, and decodes to to decimal.
        /// </summary>
        /// <param name="input">The input SNAFU string.</param>
        /// <returns>The output decimal.</returns>
        private static long DecodeSNAFU(string input)
        {
            long decoded = 0;

            long multipleFive = 1;
            for (int i = input.Length - 1; i >= 0; i--)
            {
                decoded += multipleFive * input[i] switch
                {
                    '=' => -2,
                    '-' => -1,
                    _ => input[i] - '0'
                };

                multipleFive *= 5;
            }

            return decoded;
        }

        /// <summary>
        /// Takes an input digital number, and encodes it in SNAFU.
        /// </summary>
        /// <param name="input">The input decimal.</param>
        /// <returns>The output SNAFU number.</returns>
        private static string EncodeSNAFU(long input)
        {
            string result = "";
            while (input != 0)
            {
                long remainder = input % 5;
                if (remainder > 2)
                {
                    remainder -= 5;
                    input -= remainder;
                }
                input /= 5;

                char c = remainder switch
                {
                    -2 => '=',
                    -1 => '-',
                    _ => (char)(remainder + '0')
                };
                result = c + result;
            }
            return result;
        }

        /// <summary>
        /// Reads a list of SNAFU numbers from the file, and returns the sum
        /// as SNAFU.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The sum as SNAFU.</returns>
        private static string GetSolution(string path)
        {
            var lines = System.IO.File.ReadLines(path);
            var total = lines.Sum(DecodeSNAFU);

            return EncodeSNAFU(total);
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample() => Assert.AreEqual("2=-1=0", GetSolution("AOC2022/Day25/Example.txt"));

        [TestMethod]
        public void SolvePart() => Assert.AreEqual("2-1=10=1=1==2-1=-221", GetSolution("AOC2022/Day25/Input.txt"));

        #endregion
    }
}
