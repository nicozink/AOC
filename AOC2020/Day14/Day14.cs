using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AOC2020
{
    /// <summary>
    /// Solution for day 14:
    /// https://adventofcode.com/2020/day/14
    /// </summary>
    [TestClass]
    public class Day14
    {
        /// <summary>
        /// Modifies a value given a special bitmask.
        /// </summary>
        /// <param name="value">The input value</param>
        /// <param name="mask">The bitmask</param>
        /// <returns>The modified value.</returns>
        private long GetMaskedValue(long value, String mask)
        {
            var binary = Convert.ToString(value, 2).PadLeft(36, '0');

            StringBuilder sb = new StringBuilder(binary);
            for (int i = 0; i < 36; ++i)
            {
                if (mask[i] != 'X')
                {
                    sb[i] = mask[i];
                }
            }

            return Convert.ToInt64(sb.ToString(), 2);
        }

        /// <summary>
        /// Takes an input mask, and returns all combinations where
        /// 'X' is replaced with either '0' or '1'.
        /// </summary>
        /// <param name="mask">The input mask.</param>
        /// <returns>The collection containing each combination.</returns>
        private IEnumerable<String> GetMaskCombinations(String mask)
        {
            // If the mask contains no more 'X', then return it.
            if (!mask.Contains('X'))
            {
                yield return mask;
                yield break;
            }

            var xIndex = mask.IndexOf('X');

            StringBuilder sb = new StringBuilder(mask);

            // Replace the first X with '0', and recursively return all combinations.

            sb[xIndex] = '0';

            foreach (var newMask in GetMaskCombinations(sb.ToString()))
            {
                yield return newMask;
            }

            // Replace the first X with '1', and recursively return all combinations.

            sb[xIndex] = '1';

            foreach (var newMask in GetMaskCombinations(sb.ToString()))
            {
                yield return newMask;
            }
        }

        /// <summary>
        /// For a given input mask and value, find all values that can be generated
        /// from the mask by treating 'X' as a wildcard.
        /// </summary>
        /// <param name="value">The input value.</param>
        /// <param name="mask">The input mask.</param>
        /// <returns>The modified values.</returns>
        private IEnumerable<long> GetMaskedValues(long value, string mask)
        {
            String binary = Convert.ToString(value, 2).PadLeft(36, '0');

            StringBuilder sb = new StringBuilder(binary);
            for (int i = 0; i < 36; ++i)
            {
                if (mask[i] == 'X')
                {
                    sb[i] = 'X';
                }
                else if (mask[i] == '1' || binary[i] == '1')
                {
                    sb[i] = '1';
                }
                else
                {
                    sb[i] = '0';
                }
            }

            return GetMaskCombinations(sb.ToString()).Select(x => Convert.ToInt64(x, 2));
        }

        /// <summary>
        /// Executes the commands read from the file. The program can either set a bitmask,
        /// or modify memory. The last bitmask is used to control how the value is written
        /// to memory.
        /// </summary>
        /// <param name="path">The input path.</param>
        /// <returns>The sum of values in memory</returns>
        public long ExecuteBitmaskProgram(String path, int programVersion)
        {
            var lines = System.IO.File.ReadLines(path);

            String mask = "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX";
            var values = new Dictionary<long, long>();

            foreach (var line in lines)
            {
                if (line.Contains("mask = "))
                {
                    mask = line.Substring(7);
                }
                else
                {
                    var memAddressStart = line.IndexOf("[") + 1;
                    var memAddressEnd = line.IndexOf("]");
                    var valueStart = line.IndexOf("=") + 1;

                    var memAddress = int.Parse(line.Substring(memAddressStart, memAddressEnd - memAddressStart));
                    var value = int.Parse(line.Substring(valueStart));

                    if (programVersion == 1)
                    {
                        values[memAddress] = GetMaskedValue(value, mask);
                    }
                    else
                    {
                        foreach (var maskedAddress in GetMaskedValues(memAddress, mask))
                        {
                            values[maskedAddress] = value;
                        }
                    }
                }
            }

            return values.Values.Sum();
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(165, ExecuteBitmaskProgram("AOC2020/Day14/Example1.txt", 1));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(4297467072083, ExecuteBitmaskProgram("AOC2020/Day14/Input.txt", 1));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(208, ExecuteBitmaskProgram("AOC2020/Day14/Example2.txt", 2));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(5030603328768, ExecuteBitmaskProgram("AOC2020/Day14/Input.txt", 2));

        #endregion
    }
}
