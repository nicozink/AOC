using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AOC2023
{
    /// <summary>
    /// Solution for day 3:
    /// https://adventofcode.com/2023/day/3
    /// </summary>
    [TestClass]
    public class Day03
    {
        record Part(char Symbol, int Row, int Col);

        record PartNumber(Part Part, int Number);

        /// <summary>
        /// Reads a number from the string and returns the string
        /// version.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="index">The start position of the number.</param>
        /// <returns>The number.</returns>
        private static string ReadNumber(string line, int index)
        {
            string number = "";
            do
            {
                number += line[index];
                index++;
            }
            while (index < line.Length &&
                char.IsDigit(line[index]));

            return number;
        }

        /// <summary>
        /// Gets the associated part info from a number on the grid.
        /// The part is a surrounding symbol that is within one or
        /// or column of the number, so we scan the surrounding area.
        /// </summary>
        /// <param name="lines">The grid of numbers.</param>
        /// <param name="number">The number.</param>
        /// <param name="row">The row of the number.</param>
        /// <param name="col">The column containing the first digit of the number.</param>
        /// <returns>The info for the part.</returns>
        private static Part GetPartInfo(string[] lines, string number, int row, int col)
        {
            int startRow = row - 1;
            int endRow = row + 1;

            int startCol = col - 1;
            int endCol = col + number.Length;

            for (int curRow = startRow; curRow <= endRow; curRow++)
            {
                if (curRow < 0 || curRow >= lines.Length)
                {
                    continue;
                }

                for (int curCol = startCol; curCol <= endCol; curCol++)
                {
                    if (curCol < 0 || curCol >= lines[curRow].Length)
                    {
                        continue;
                    }

                    char curChar = lines[curRow][curCol];
                    if (curChar == '.' || char.IsDigit(curChar))
                    {
                        continue;
                    }

                    return new Part(curChar, curRow, curCol);
                }
            }

            return new Part('.', row, col);
        }

        /// <summary>
        /// Reads the serial number and associated part info from the input.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static IEnumerable<PartNumber> ReadInput(string path)
        {
            var lines = System.IO.File.ReadAllLines(path);

            for (int i = 0; i < lines.Length; i++)
            {
                for (int j = 0; j < lines[i].Length; j++)
                {
                    var ch = lines[i][j];

                    if (char.IsDigit(ch))
                    {
                        string number = ReadNumber(lines[i], j);
                        Part part = GetPartInfo(lines, number, i, j);
                        yield return new PartNumber(part, int.Parse(number));

                        j += number.Length;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the sum of part numbers with valid parts from the input.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The sum of part numbers.</returns>
        private int SumPartNumbers(string path)
        {
            var partNumbers = ReadInput(path);
            return partNumbers
                .Where(x => x.Part.Symbol != '.')
                .Sum(x => x.Number);
        }

        /// <summary>
        /// Returns the sum of gear ratios (where two numbers connect to the same '*'
        /// part).
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The sum of gear ratios.</returns>
        private int SumGearRatios(string path)
        {
            int sum = 0;
            var partNumbers = ReadInput(path);

            var groupedParts = partNumbers
                .Where(x => x.Part.Symbol == '*')
                .GroupBy(x => x.Part);

            foreach (var part in groupedParts)
            {
                var numbers = part.ToArray();
                if (numbers.Length != 2)
                {
                    continue;
                }

                sum += numbers[0].Number * numbers[1].Number;
            }

            return sum;
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(4361, SumPartNumbers("AOC2023/Day03/Example.txt"));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(536576, SumPartNumbers("AOC2023/Day03/Input.txt"));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(467835, SumGearRatios("AOC2023/Day03/Example.txt"));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(75741499, SumGearRatios("AOC2023/Day03/Input.txt"));

        #endregion
    }
}
