using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace AOC2023
{
    /// <summary>
    /// Solution for day 13:
    /// https://adventofcode.com/2023/day/13
    /// </summary>
    [TestClass]
    public class Day13
    {
        /// <summary>
        /// Reads the grids from the input file.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The grids in the input.</returns>
        private static IEnumerable<string[]> ReadInput(string path)
        {
            var input = System.IO.File.ReadAllLines(path);

            var lines = new List<string>();
            foreach (var line in input)
            {
                if (string.IsNullOrEmpty(line))
                {
                    yield return lines.ToArray();

                    lines = new List<string>();
                }
                else
                {
                    lines.Add(line);
                }
               
            }

            yield return lines.ToArray();
        }

        /// <summary>
        /// Gets the length of the collection when considering a direction.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="horizontal">The horizontal or vertical flag.</param>
        /// <returns>The length for the direction of the flag.</returns>
        private static int GetLength(string[] grid, bool horizontal)
        {
            if (horizontal)
            {
                return grid.Length;
            }
            else
            {
                return grid[0].Length;
            }
        }

        /// <summary>
        /// Gets the characters from a grid in a direction.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="index">The index.</param>
        /// <param name="horizontal">The horizontal or vertical flag.</param>
        /// <returns>The characters for the direction and the index.</returns>
        private static IEnumerable<char> GetCharacters(string[] grid, int index, bool horizontal)
        {
            if (horizontal)
            {
                return grid[index];
            }
            else
            {
                return grid.Select(x => x[index]);
            }
        }

        /// <summary>
        /// Gets the index in a specific direction when considering the number of smudges.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="numSmudges">The number of smudges that are considered.</param>
        /// <param name="horizontal">The horizontal or vertical flag.</param>
        /// <returns>The index of the mirror.</returns>
        private static int GetMirrorIndex(string[] grid, int numSmudges, bool horizontal)
        {
            var length = GetLength(grid, horizontal);
            for (int i = 1; i < length; i++)
            {
                int actualSmudges = 0;
                for (int j = 0; j < i; j++)
                {
                    var left = i - j - 1;
                    if (left < 0)
                    {
                        continue;
                    }

                    var right = i + j;
                    if (right >= length)
                    {
                        continue;
                    }

                    var leftChars = GetCharacters(grid, left, horizontal);
                    var rightChars = GetCharacters(grid, right, horizontal);

                    actualSmudges += leftChars.Zip(rightChars).Count(x => x.First != x.Second);
                }

                if (actualSmudges == numSmudges)
                {
                    return i;
                }
            }

            return 0;
        }

        /// <summary>
        /// Gets the sums of the notes designated by the mirror locations.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <param name="numSmudges">The number of smudges to consider.</param>
        /// <returns>The sum of the notes.</returns>
        private static int SumNotes(string path, int numSmudges)
        {
            var input = ReadInput(path);

            int sum = 0;
            foreach (var grid in input)
            {
                sum += GetMirrorIndex(grid, numSmudges, horizontal: false);
                sum += GetMirrorIndex(grid, numSmudges, horizontal: true) * 100;
            }

            return sum;
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(405, SumNotes("AOC2023/Day13/Example.txt", numSmudges: 0));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(37718, SumNotes("AOC2023/Day13/Input.txt", numSmudges: 0));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(400, SumNotes("AOC2023/Day13/Example.txt", numSmudges: 1));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(40995, SumNotes("AOC2023/Day13/Input.txt", numSmudges: 1));


        #endregion
    }
}
