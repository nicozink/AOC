using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Solutions.Day11Utils;

namespace Solutions
{
    /// <summary>
    /// Solution for day 12:
    /// https://adventofcode.com/2021/day/12
    /// </summary>
    [SolutionClass(Day = 12)]
    public class Day12Solution
    {
        /// <summary>
        /// Read the input from the file. Input consists of
        /// connections between points, and returns a dictionary
        /// containing connections for each point.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <returns>The connections.</returns>
        Dictionary<String, List<String>> ReadInput(String path)
        {
            var output = new Dictionary<String, List<String>>();

            var lines = System.IO.File.ReadLines(path);
            foreach (var line in lines)
            {
                var split = line.Split('-');

                var from = split[0];
                var to = split[1];

                if (!output.ContainsKey(from))
                {
                    output.Add(from, new());
                }

                if (!output.ContainsKey(to))
                {
                    output.Add(to, new());
                }

                if (from != "end" && to != "start")
                {
                    output[from].Add(to);
                }

                if (from != "start" && to != "end")
                {
                    output[to].Add(from);
                }
            }

            // We sort them so that execution order matches
            // the samples given.
            foreach (var list in output.Values)
            {
                list.Sort();
            }

            return output;
        }

        /// <summary>
        /// Counts the possible paths. Uppercase points
        /// can be visited multiple times, and lowercase
        /// ones can be visited once, or twice as a special
        /// case.
        /// </summary>
        /// <param name="current">The currrent point.</param>
        /// <param name="visited">The other points which have been visited.</param>
        /// <param name="paths">The possible paths to follow.</param>
        /// <param name="allowMultiVisit">Whether to allow multiple visits.</param>
        /// <returns>The number of possible paths.</returns>
        private int CountPossiblePaths(string current, HashSet<string> visited, Dictionary<String, List<String>> paths, bool allowMultiVisit)
        {
            int totalPaths = 0;
            foreach (var next in paths[current])
            {
                if (next == "end")
                {
                    totalPaths++;
                    continue;
                }

                bool canAddFurtherDuplicates = allowMultiVisit;

                if (visited.Contains(next))
                {
                    if (allowMultiVisit)
                    {
                        canAddFurtherDuplicates = false;
                    }
                    else
                    {
                        continue;
                    }
                }

                bool addedToVisited = !char.IsUpper(next[0]) && visited.Add(next);

                totalPaths += CountPossiblePaths(next, visited, paths, canAddFurtherDuplicates);

                if (addedToVisited)
                {
                    visited.Remove(next);
                }
            }

            return totalPaths;
        }

        /// <summary>
        /// Count the number of possible paths based on the input.
        /// </summary>
        /// <param name="path">The input file.</param>
        /// <param name="allowMultiVisit">Whether to allow multipe visits.</param>
        /// <returns>The number of paths.</returns>
        private int CountPossiblePaths(String path, bool allowMultiVisit)
        {
            var paths = ReadInput(path);

            return CountPossiblePaths("start", new(), paths, allowMultiVisit);
        }

        #region Solve Problems

        public long SolveExample1() => CountPossiblePaths("Day12/Example1.txt", allowMultiVisit: false);

        public long SolveExample2() => CountPossiblePaths("Day12/Example2.txt", allowMultiVisit: false);

        public long SolveExample3() => CountPossiblePaths("Day12/Example3.txt", allowMultiVisit: false);

        [SolutionMethod(Part = 1)]
        public long SolvePart1() => CountPossiblePaths("Day12/Input.txt", allowMultiVisit: false);

        public long SolveExample4() => CountPossiblePaths("Day12/Example1.txt", allowMultiVisit: true);

        public long SolveExample5() => CountPossiblePaths("Day12/Example2.txt", allowMultiVisit: true);

        public long SolveExample6() => CountPossiblePaths("Day12/Example3.txt", allowMultiVisit: true);

        [SolutionMethod(Part = 2)]
        public long SolvePart2() => CountPossiblePaths("Day12/Input.txt", allowMultiVisit: true);

        #endregion
    }
}
