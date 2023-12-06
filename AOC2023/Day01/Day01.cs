using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AOC2023
{
    /// <summary>
    /// Solution for day 1:
    /// https://adventofcode.com/2023/day/1
    /// </summary>
    [TestClass]
    public class Day01
    {
        readonly List<(string number, int value)> numbers = new() {
            ("one", 1),
            ("two", 2),
            ("three", 3),
            ("four", 4),
            ("five", 5),
            ("six", 6),
            ("seven", 7),
            ("eight", 8),
            ("nine", 9)
        };

        private IEnumerable<int> ConvertToDigits(string input, bool convertCharacters)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (char.IsDigit(input[i]))
                {
                    yield return input[i] - '0';
                }
                else if (convertCharacters)
                {
                    foreach (var (number, value) in numbers)
                    {
                        if (i + number.Length <= input.Length &&
                            input.Substring(i, number.Length).Equals(number))
                        {
                            yield return value;

                            break;
                        }
                    }
                }
            }
        }

        private int SumCalibrationValues(string path, bool convertCharacters)
        {
            var lines = System.IO.File.ReadAllLines(path);

            var sum = 0;
            foreach (var line in lines)
            {
                var digits = ConvertToDigits(line, convertCharacters);

                var firstDigit = digits.First();
                var lastDigit = digits.Last();

                var calibrationValue = $"{firstDigit}{lastDigit}";
                sum += int.Parse(calibrationValue);
            }

            return sum;
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(142, SumCalibrationValues("AOC2023/Day01/Example1.txt", convertCharacters: false));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(54390, SumCalibrationValues("AOC2023/Day01/Input.txt", convertCharacters: false));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(281, SumCalibrationValues("AOC2023/Day01/Example2.txt", convertCharacters: true));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(54277, SumCalibrationValues("AOC2023/Day01/Input.txt", convertCharacters: true));

        #endregion
    }
}
