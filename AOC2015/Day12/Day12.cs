using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Text.Json;

namespace AOC2015
{
    /// <summary>
    /// Solution for day 12:
    /// https://adventofcode.com/2015/day/12
    /// </summary>
    [TestClass]
    public class Day12
    {
        #region Solve Problems

        private static int SumAllNumbers(JsonElement currentElement, string? ignoreProperty)
        {
            if (currentElement.ValueKind == JsonValueKind.Array)
            {
                int sum = 0;
                foreach (var nextElement in currentElement.EnumerateArray())
                {
                    sum += SumAllNumbers(nextElement, ignoreProperty);
                }

                return sum;
            }
            else if (currentElement.ValueKind == JsonValueKind.Object)
            {
                if (!string.IsNullOrEmpty(ignoreProperty))
                {
                    foreach (var nextElement in currentElement.EnumerateObject())
                    {
                        if (nextElement.Value.ToString().Equals(ignoreProperty))
                        {
                            return 0;
                        }
                    }
                }

                int sum = 0;
                foreach (var nextElement in currentElement.EnumerateObject())
                {
                    sum += SumAllNumbers(nextElement.Value, ignoreProperty);
                }

                return sum;
            }
            else if (currentElement.ValueKind == JsonValueKind.Number)
            {
                return (int)currentElement.GetDecimal();
            }

            return 0;
        }

        private static double SumAllNumbers(string path, string? ignoreProperty = null)
        {
            var input = System.IO.File.ReadAllText(path);
            var document = JsonDocument.Parse(input);
            var root = document.RootElement;

            return SumAllNumbers(root, ignoreProperty);
        }

        [TestMethod]
        public void TestSolution1() => Assert.AreEqual(119433, SumAllNumbers("AOC2015/Day12/Input.txt"));

        [TestMethod]
        public void TestSolution2() => Assert.AreEqual(68466, SumAllNumbers("AOC2015/Day12/Input.txt", "red"));

        #endregion
    }
}
