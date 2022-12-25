using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace AOC2022
{
    /// <summary>
    /// Solution for day 18:
    /// https://adventofcode.com/2022/day/19
    /// </summary>
    [TestClass]
    public class Day20
    {
        /// <summary>
        /// Stores a number which includes the index and value.
        /// </summary>
        /// <param name="Index">The index.</param>
        /// <param name="Value">The value.</param>
        record Number(int Index, long Value);

        /// <summary>
        /// Gets the solution by decrypting the message.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <param name="encryptionKey">The encryption key.</param>
        /// <param name="numIterations">The number of iterations to perform.</param>
        /// <returns>The decrypted result.</returns>
        private static long GetSolution(string path, int encryptionKey, int numIterations)
        {
            var baseNumbers = System.IO.File.ReadLines(path)
                .Select((val, index) => new Number(index, long.Parse(val) * encryptionKey))
                .ToArray();

            var list = baseNumbers.ToList();

            for (int x = 0; x < numIterations; x++)
            {
                foreach (var number in baseNumbers)
                {
                    var index = list.IndexOf(number);
                    int newIndex = (int)((index + number.Value) % (baseNumbers.Length - 1));

                    if (newIndex <= 0)
                    {
                        newIndex += (baseNumbers.Length - 1);
                    }

                    list.RemoveAt(index);
                    list.Insert(newIndex, number);
                }
            }

            var zeroIndex = list.IndexOf(baseNumbers.First(x => x.Value == 0));

            long num1 = list[(zeroIndex + 1000) % list.Count].Value;
            long num2 = list[(zeroIndex + 2000) % list.Count].Value;
            long num3 = list[(zeroIndex + 3000) % list.Count].Value;

            return num1 + num2 + num3;
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(3, GetSolution("AOC2022/Day20/Example.txt", 1, 1));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(13522, GetSolution("AOC2022/Day20/Input.txt", 1, 1));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(1623178306, GetSolution("AOC2022/Day20/Example.txt", 811589153, 10));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(17113168880158, GetSolution("AOC2022/Day20/Input.txt", 811589153, 10));

        #endregion
    }
}
