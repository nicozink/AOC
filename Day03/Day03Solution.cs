using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace Solutions
{
    /// <summary>
    /// Solution for day 3:
    /// https://adventofcode.com/2021/day/3
    /// </summary>
    [SolutionClass(Day = 3)]
    public class Day03Solution
    {
        /// <summary>
        /// Searches for either the most or least common character in
        /// a given list of strings at a specific index.
        /// </summary>
        /// <param name="lines">The list of strings.</param>
        /// <param name="index">The index to search.</param>
        /// <param name="mostCommon">Whether to seach for the most common character.</param>
        /// <returns>The most/least common character.</returns>
        char GetCommonChar(string[] lines, int index, bool mostCommon)
        {
            var lineSearch = from ch in lines.Select(x => x[index])
                             group ch by ch into grp
                             orderby grp.Count(), grp.Key
                             select grp.Key;

            if (mostCommon)
            {
                return lineSearch.First();
            }
            else
            {
                return lineSearch.Last();
            }
        }

        /// <summary>
        /// Constructs a string containing either the most or least
        /// common character in every column.
        /// </summary>
        /// <param name="lines">The list of strings.</param>
        /// <param name="mostCommon">Whether to look for the most or least common.</param>
        /// <returns>A string consisting of the most/least common characters.</returns>
        String BuildRateString(string[] lines, bool mostCommon)
        {
            var enumerateColumns = Enumerable.Range(0, lines[0].Length);
            var gamma = enumerateColumns.Select(x => GetCommonChar(lines, x, mostCommon));

            return new String(gamma.ToArray());
        }

        /// <summary>
        /// Calculates the power consumption for a list of strings.
        /// </summary>
        /// <param name="path">The file containing the list of strings.</param>
        /// <returns>The power consumption.</returns>
        private int GetPowerConsumption(String path)
        {
            var lines = System.IO.File.ReadAllLines(path).ToArray();

            var gamma = BuildRateString(lines, true);
            var epsilon = BuildRateString(lines, false);

            return Convert.ToInt32(gamma, 2) * Convert.ToInt32(epsilon, 2);
        }

        /// <summary>
        /// Find the best matching string by eliminating non-matching ones based on
        /// the most/least common character in each column.
        /// </summary>
        /// <param name="lines">The file containing the list of strings.</param>
        /// <param name="mostCommon">Whether to look for the most or least common characters.</param>
        /// <returns>The best match string.</returns>
        /// <exception cref="InvalidOperationException">Thrown when not able to find a single match.</exception>
        private String FindRatingValue(string[] lines, bool mostCommon)
        {
            var remainingLines = lines.ToArray();
            for (int i = 0; i < lines[0].Length; ++i)
            {
                var commonChar = GetCommonChar(remainingLines, i, mostCommon);

                remainingLines = remainingLines
                    .Where(x => x[i] == commonChar)
                    .ToArray();

                if (remainingLines.Length == 1)
                {
                    return remainingLines[0];
                }
            }

            throw new InvalidOperationException("Cannot find single value");
        }

        /// <summary>
        /// Calculates the CO2 scrubbing rating for a list of strings.
        /// </summary>
        /// <param name="path">A file containing the list of strings.</param>
        /// <returns>The CO2 scrubbing value.</returns>
        private int GetCO2Scrubbing(String path)
        {
            var lines = System.IO.File.ReadAllLines(path).ToArray();

            var gamma = FindRatingValue(lines, true);
            var epsilon = FindRatingValue(lines, false);

            return Convert.ToInt32(gamma, 2) * Convert.ToInt32(epsilon, 2);
        }

        #region Solve Problems

        public int SolveExample1() => GetPowerConsumption("Day03/Example.txt");

        [SolutionMethod(Part = 1)]
        public int SolvePart1() => GetPowerConsumption("Day03/Input.txt");

        public int SolveExample2() => GetCO2Scrubbing("Day03/Example.txt");

        [SolutionMethod(Part = 2)]
        public int SolvePart2() => GetCO2Scrubbing("Day03/Input.txt");

        #endregion
    }
}
