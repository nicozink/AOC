using Microsoft.VisualStudio.TestTools.UnitTesting;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AOC2015
{
    /// <summary>
    /// Solution for day 9:
    /// https://adventofcode.com/2015/day/9
    /// </summary>
    [TestClass]
    public class Day09
    {
        /// <summary>
        /// Stores the description of the cost between a pair
        /// of points.
        /// </summary>
        /// <param name="From">The starting point.</param>
        /// <param name="To">The end point.</param>
        /// <param name="Cost">The cost.</param>
        record CostDescription(int From, int To, int Cost);

        /// <summary>
        /// Reads the costs from the input file.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The costs.</returns>
        static IEnumerable<CostDescription> ReadInput(string path)
        {
            // We keep track of places so that we can convert them
            // to a unique zero-based int index. This makes looking up
            // costs a bit easier.
            var places = new List<string>();

            var lines = System.IO.File.ReadLines(path);
            foreach (var line in lines)
            {
                var split = line.Replace(" to ", " ")
                    .Replace(" = ", " ")
                    .Split();

                // We check if the from location exists, and insert it otherwise.
                // We use the index as the id for the location.

                if (!places.Contains(split[0]))
                {
                    places.Add(split[0]);
                }

                int from = places.IndexOf(split[0]);

                // We check if the to location exists, and insert it otherwise.
                // We use the index as the id for the location.

                if (!places.Contains(split[1]))
                {
                    places.Add(split[1]);
                }

                int to = places.IndexOf(split[1]);

                // We need to the cost going in both directions.
                yield return new(from, to, int.Parse(split[2]));
                yield return new(to, from, int.Parse(split[2]));
            }
        }

        /// <summary>
        /// A class used to solve all paths through all locations.
        /// </summary>
        class PathSolver
        {
            /// <summary>
            /// Creates a new solver.
            /// </summary>
            /// <param name="path">The input file describing all costs.</param>
            public PathSolver(string path)
            {
                costs = ReadInput(path).ToArray();

                var max = costs.Max(x => x.From);
                allVisited = (1 << (max + 1)) - 1;
            }

            /// <summary>
            /// Gets the costs of all combinations of paths through each location.
            /// </summary>
            /// <returns>The costs.</returns>
            public IEnumerable<int> GetAllCosts()
            {
                foreach (var start in costs.Select(x => x.From))
                {
                    var newCosts = GetAllCosts(0, start, 0);

                    foreach (var newCost in newCosts)
                    {
                        yield return newCost;
                    }
                }
            }

            /// <summary>
            /// Recursively gets all paths through the locations. If we reach the end,
            /// we return the cost.
            /// </summary>
            /// <param name="history">A bit mask containing the history of previous visits.</param>
            /// <param name="next">The next location to visit.</param>
            /// <param name="cost">The cost up to this point.</param>
            /// <returns>The costs.</returns>
            private IEnumerable<int> GetAllCosts(int history, int next, int cost)
            {
                history |= 1 << next;

                if (history == allVisited)
                {
                    yield return cost;
                }

                foreach (var nextCost in costs.Where(x => x.From == next))
                {
                    if ((history & 1 << nextCost.To) != 0)
                    {
                        continue;
                    }

                    var newCosts = GetAllCosts(history, nextCost.To, cost + nextCost.Cost);

                    foreach (var newCost in newCosts)
                    {
                        yield return newCost;
                    }
                }
            }

            /// <summary>
            /// The bit mask which corresponds to all locations having been visited.
            /// </summary>
            private readonly int allVisited;

            /// <summary>
            /// The costs of visiting each location from another.
            /// </summary>
            private readonly CostDescription[] costs;
        }

        /// <summary>
        /// Gets the shortest path through each location given the costs.
        /// </summary>
        /// <param name="path">The file containing the costs.</param>
        /// <returns>The shortest path.</returns>
        private int GetShortestPath(string path)
        {
            var solver = new PathSolver(path);
            return solver.GetAllCosts().Min();
        }

        /// <summary>
        /// Gets the longest path through each location given the costs.
        /// </summary>
        /// <param name="path">The file containing the costs.</param>
        /// <returns>The longest path.</returns>
        private int GetLongestPath(string path)
        {
            var solver = new PathSolver(path);
            return solver.GetAllCosts().Max();
        }

        #region Solve Problems

        [TestMethod]
        public void TestExample1() => Assert.AreEqual(605, GetShortestPath("Day09/Example.txt"));

        [TestMethod]
        public void TestSolution1() => Assert.AreEqual(141, GetShortestPath("Day09/Input.txt"));

        [TestMethod]
        public void TestExample2() => Assert.AreEqual(982, GetLongestPath("Day09/Example.txt"));

        [TestMethod]
        public void TestSolution2() => Assert.AreEqual(736, GetLongestPath("Day09/Input.txt"));

        #endregion
    }
}
