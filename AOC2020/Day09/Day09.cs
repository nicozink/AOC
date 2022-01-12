using Microsoft.VisualStudio.TestTools.UnitTesting;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AOC2020
{
    /// <summary>
    /// Solution for day 9:
    /// https://adventofcode.com/2020/day/9
    /// </summary>
    [TestClass]
    public class Day09
    {
        /// <summary>
        /// Contains an implementation of the eXchange-Masking
        /// Addition System (XMAS) encription.
        /// </summary>
        public class Encription
        {
            /// <summary>
            /// Creates a new instance of the class.
            /// </summary>
            /// <param name="path">The path containing the numbers.</param>
            /// <param name="preambleCount">The length of the preamble.</param>
            public Encription(String path, int preambleCount)
            {
                numbers = System.IO.File.ReadAllLines(path)
                    .Select(x => long.Parse(x)).ToList();

                this.preambleCount = preambleCount;
            }

            /// <summary>
            /// Gets the first invalid number - where the previous numbers
            /// don't contain a pair that add up to it.
            /// </summary>
            /// <returns>The invalid number.</returns>
            public long GetInvalidNumber()
            {
                for (int i = preambleCount; i < numbers.Count; ++i)
                {
                    long number = numbers[i];
                    var preamble = numbers.GetRange(i - preambleCount, preambleCount);

                    bool isNumberValid = ListExtension.GetPairs<long>(preamble)
                        .Count(x => x.Item1 + x.Item2 == number) != 0;

                    if (!isNumberValid)
                    {
                        return number;
                    }
                }

                return 0;
            }

            /// <summary>
            /// Get the encryption weakness. This is the minimum and
            /// maximum of a range that add up to the invalid number.
            /// </summary>
            /// <returns>The encryption weakness.</returns>
            public long GetEncryptionWeakness()
            {
                long invalidNumber = GetInvalidNumber();

                var range = GetNumberSequences()
                    .First(x => x.Sum() == invalidNumber);

                return range.Min() + range.Max();
            }

            /// <summary>
            /// Gets all contiguous number sequences larger than two
            /// from the list of numbers.
            /// </summary>
            /// <returns>The number sequences.</returns>
            public IEnumerable<List<long>> GetNumberSequences()
            {
                for (int i = 2; i < numbers.Count; ++i)
                {
                    for (int j = 0; j < numbers.Count - i; ++j)
                    {
                        yield return numbers.GetRange(j, i);
                    }
                }
            }

            /// <summary>
            /// The numbers in the encrypted sequence.
            /// </summary>
            private readonly List<long> numbers;

            /// <summary>
            /// The size of the preamble.
            /// </summary>
            private readonly int preambleCount;
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1()
        {
            var encription = new Encription("Day09/Example.txt", 5);
            long result = encription.GetInvalidNumber();

            Assert.AreEqual(127, result);
        }

        [TestMethod]
        public void SolvePart1()
        {
            var encription = new Encription("Day09/Input.txt", 25);
            long result = encription.GetInvalidNumber();

            Assert.AreEqual(41682220, result);
        }

        [TestMethod]
        public void SolveExample2()
        {
            var encription = new Encription("Day09/Example.txt", 5);
            long result = encription.GetEncryptionWeakness();

            Assert.AreEqual(62, result);
        }

        [TestMethod]
        public void SolvePart2()
        {
            var encription = new Encription("Day09/Input.txt", 25);
            long result = encription.GetEncryptionWeakness();

            Assert.AreEqual(5388976, result);
        }

        #endregion
    }
}
