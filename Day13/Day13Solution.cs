using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Solutions.Day11Utils;

namespace Solutions
{
    /// <summary>
    /// Solution for day 13:
    /// https://adventofcode.com/2021/day/13
    /// </summary>
    [SolutionClass(Day = 13)]
    public class Day13Solution
    {
        class PiecePaper
        {
            /// <summary>
            /// Stores the locations on the paper which have a mark on them.
            /// </summary>
            private HashSet<(int x, int y)> entries = new();

            /// <summary>
            /// Stores the instructions used to fold the paper.
            /// </summary>
            private readonly List<(char c, int value)> instructions = new();

            /// <summary>
            /// Creates a new piece of paper based on the input given in the file.
            /// </summary>
            /// <param name="path">The path to the input.</param>
            public PiecePaper(String path)
            {
                var lines = System.IO.File.ReadLines(path);
                var enumerator = lines.GetEnumerator();

                while (enumerator.MoveNext() && !String.IsNullOrEmpty(enumerator.Current))
                {
                    String entry = enumerator.Current;

                    var values = entry.Split(',')
                        .Select(int.Parse)
                        .ToArray();

                    entries.Add((values[0], values[1]));
                }

                while (enumerator.MoveNext())
                {
                    String entry = enumerator.Current;
                    entry = entry.Replace("fold along ", "");

                    var values = entry.Split('=')
                        .ToArray();

                    instructions.Add((values[0][0], int.Parse(values[1])));
                }
            }

            /// <summary>
            /// Counts the number of entries.
            /// </summary>
            /// <returns>The number of entries.</returns>
            internal int CountEntries()
            {
                return entries.Count;
            }

            /// <summary>
            /// Make the first fold according to the instructions given as input.
            /// </summary>
            internal void MakeFirstFold()
            {
                var instruction = instructions.First();

                PerformFold(instruction);
            }

            /// <summary>
            /// Make the folds for all instructions which were given as input.
            /// </summary>
            internal void MakeAllFolds()
            {
                foreach (var instruction in instructions)
                {
                    PerformFold(instruction);
                }
            }

            /// <summary>
            /// Perform a fold along the given axis as given by the instruction.
            /// </summary>
            /// <param name="instruction">The instruction.</param>
            private void PerformFold((char c, int value) instruction)
            {
                var newEntries = new HashSet<(int x, int y)>();

                foreach (var entry in entries)
                {
                    if (instruction.c == 'x' && entry.x > instruction.value)
                    {
                        newEntries.Add((2 * instruction.value - entry.x, entry.y));
                    }
                    else if (instruction.c == 'y' && entry.y > instruction.value)
                    {
                        newEntries.Add((entry.x, 2 * instruction.value - entry.y));
                    }
                    else
                    {
                        newEntries.Add(entry);
                    }
                }

                entries = newEntries;
            }

            /// <summary>
            /// Gets a string showing the entries on the paper.
            /// </summary>
            /// <returns>The string.</returns>
            internal String GetString()
            {
                var maxx = entries.Max(x => x.x);
                var maxy = entries.Max(x => x.y);
                
                var characters = Enumerable.Range(0, maxy + 1)
                    .Select(x => Enumerable.Range(0, maxx + 1)
                        .Select(x => ' ')
                        .ToArray())
                    .ToArray();

                foreach (var (x, y) in entries)
                {
                    characters[y][x] = '*';
                }

                var output = "";

                foreach (var line in characters)
                {
                    output += new String(line) + Environment.NewLine;
                }

                return output;
            }
        }

        /// <summary>
        /// Gets a piece of paper, performs the first fold and
        /// returns the number of entries.
        /// </summary>
        /// <param name="path">The path to the input.</param>
        /// <returns>The number of entries.</returns>
        private int CountAfterFirstFold(String path)
        {
            var piecePaper = new PiecePaper(path);
            piecePaper.MakeFirstFold();

            return piecePaper.CountEntries();
        }

        /// <summary>
        /// Gets a piece of paper and folds it several times,
        /// and returns the folded result.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The folded paper.</returns>
        private String GetFoldedPaper(string path)
        {
            var piecePaper = new PiecePaper(path);
            piecePaper.MakeAllFolds();

            return piecePaper.GetString();
        }

        #region Solve Problems

        public int SolveExample1() => CountAfterFirstFold("Day13/Example.txt");

        [SolutionMethod(Part = 1)]
        public int SolvePart1() => CountAfterFirstFold("Day13/Input.txt");

        [SolutionMethod(Part = 2)]
        public String SolvePart2() => GetFoldedPaper("Day13/Input.txt");

        #endregion
    }
}
