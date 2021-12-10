using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace Solutions
{
    /// <summary>
    /// Solution for day 10:
    /// https://adventofcode.com/2021/day/10
    /// </summary>
    [SolutionClass(Day = 10)]
    public class Day10Solution
    {
        /// <summary>
        /// Stores the opposite of each character.
        /// </summary>
        readonly Dictionary<char, char> opposite = new()
        {
            { '(', ')' },
            { '[', ']' },
            { '{', '}' },
            { '<', '>' }
        };

        /// <summary>
        /// Stores the syntax store for each character.
        /// </summary>
        readonly Dictionary<char, int> syntaxScores = new()
        {
            { ')', 3},
            { ']', 57},
            { '}', 1197},
            { '>', 25137}
        };

        /// <summary>
        /// Stores the middle score for each character.
        /// </summary>
        readonly Dictionary<char, int> middleScores = new()
        {
            { ')', 1},
            { ']', 2},
            { '}', 3},
            { '>', 4}
        };

        /// <summary>
        /// Parse the line, and return the error code for corrupted
        /// or incomplete lines.
        /// </summary>
        /// <param name="line">The lines</param>
        /// <returns>The error codes.</returns>
        (int syntaxError, long middleScore) ParseLine(String line)
        {
            var parens = new Stack<char>();

            foreach (var nextChar in line)
            {
                if ("<([{".Contains(nextChar))
                {
                    parens.Push(nextChar);
                }
                else
                {
                    var last = parens.Pop();
                    var expected = opposite[last];

                    if (nextChar != expected)
                    {
                        return (syntaxScores[nextChar], 0);
                    }
                }
            }

            if (parens.Count != 0)
            {
                long middleScore = 0;

                foreach (var c in parens)
                {
                    middleScore = middleScore * 5 + middleScores[opposite[c]];
                }

                return (0, middleScore);
            }

            return (0, 0);
        }

        /// <summary>
        /// Parses each line and returns the sum of syntax error scores.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The sum of error codes.</returns>
        int GetSyntaxErrorScore(String path)
        {
            var lines = System.IO.File.ReadAllLines(path);
            return lines.Sum(l => ParseLine(l).syntaxError);
        }

        /// <summary>
        /// Gets the middle value for incomplete lines.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The middle score.</returns>
        long GetMiddleScore(String path)
        {
            var lines = System.IO.File.ReadAllLines(path);

            var middleScores = lines.Select(l => ParseLine(l).middleScore)
                .Where(x => x != 0)
                .OrderBy(x => x)
                .ToList();

            return middleScores.Skip(middleScores.Count / 2)
                .First();
        }

        #region Solve Problems

        public long SolveExample1() => GetSyntaxErrorScore("Day10/Example.txt");

        [SolutionMethod(Part = 1)]
        public long SolvePart1() => GetSyntaxErrorScore("Day10/Input.txt");

        public long SolveExample2() => GetMiddleScore("Day10/Example.txt");

        [SolutionMethod(Part = 2)]
        public long SolvePart2() => GetMiddleScore("Day10/Input.txt");

        #endregion
    }
}
