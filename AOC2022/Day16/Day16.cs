using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace AOC2022
{
    /// <summary>
    /// Solution for day 15:
    /// https://adventofcode.com/2022/day/16
    /// </summary>
    [TestClass]
    public class Day16
    {
        /// <summary>
        /// Stores the description for a valve.
        /// </summary>
        struct Valve
        {
            /// <summary>
            /// The bitmask value for the valve.
            /// </summary>
            public long BitValue;

            /// <summary>
            /// The flow rate for the valve.
            /// </summary>
            public int FlowRate;

            /// <summary>
            /// The connections to other valves.
            /// </summary>
            public int[] Valves;
        }

        /// <summary>
        /// The key we use to cache previous calculations.
        /// </summary>
        /// <param name="Time">The current time.</param>
        /// <param name="Current">The current position.</param>
        /// <param name="WithElephant">Whether we have a companion elephant.</param>
        /// <param name="Open">The bitmask for open valves.</param>
        /// <param name="Score">The score.</param>
        record CacheValue(int Time, int Current, bool WithElephant, long Open, int Score);

        /// <summary>
        /// The solver which finds the highest pressure achievable in the system.
        /// </summary>
        class ValveSolver
        {
            /// <summary>
            /// The distances between valves.
            /// </summary>
            private readonly int[,] distances;

            /// <summary>
            /// The valves.
            /// </summary>
            private readonly Valve[] valves;

            /// <summary>
            /// Create a new valve solver.
            /// </summary>
            /// <param name="path">The path to the input file.</param>
            public ValveSolver(string path)
            {
                valves = ReadInput(path).ToArray();
                distances = CalculateDistances(valves);
            }

            /// <summary>
            /// Get the highest pressure achievable in the system.
            /// </summary>
            /// <param name="withElephant">Whether we have a companion elephant.</param>
            /// <param name="timeLimit">The time limit.</param>
            /// <returns>The highest pressure.</returns>
            public int GetHighestPressure(bool withElephant, int timeLimit)
            {
                return GetHighestPressure(0, withElephant, 0, 0, timeLimit, 0, new());
            }

            /// <summary>
            /// Get the highest pressure achievable in the system.
            /// </summary>
            /// <param name="current">The current position.</param>
            /// <param name="withElephant">Whether we have a companion elephant.</param>
            /// <param name="open">The bitmask for open valves.</param>
            /// <param name="time">The current time.</param>
            /// <param name="timeLimit">The time limit.</param>
            /// <param name="score">The score.</param>
            /// <param name="lookupCache">The lookup cache to store and retrieve previous results.</param>
            /// <returns>The highest pressure achievable.</returns>
            private int GetHighestPressure(int current, bool withElephant, long open, int time, int timeLimit, int score, Dictionary<CacheValue, int> lookupCache)
            {
                // First try to retrieve a previous result from the cache
                var cacheIndex = new CacheValue(time, current, withElephant, open, score);
                if (lookupCache.TryGetValue(cacheIndex, out var cacheValue))
                {
                    return cacheValue;
                }

                int highestPressure = score;
                for (int i = 0; i < valves.Length; i++)
                {
                    // Don't go there if the flow rate is zero
                    var nextValve = valves[i];
                    if (nextValve.FlowRate == 0)
                    {
                        continue;
                    }

                    // Don't go there if the valve is already open
                    if ((open & nextValve.BitValue) != 0)
                    {
                        continue;
                    }

                    // This checks if there is enough time.
                    var timeToNext = distances[current, i];
                    var newTime = time + timeToNext + 1;
                    if (newTime > timeLimit)
                    {
                        continue;
                    }

                    // We have a valid move we can make, so try it to find the result.
                    var newScore = score + nextValve.FlowRate * (timeLimit - newTime);
                    var nextHighhestPressure = GetHighestPressure(i, withElephant, open | nextValve.BitValue, newTime, timeLimit, newScore, lookupCache);

                    highestPressure = Math.Max(highestPressure, nextHighhestPressure);
                }

                // If we have an elephant, then try running again starting with a 0 timer, but with the same starting position.
                if (withElephant)
                {
                    var elephantPressure = score + GetHighestPressure(0, false, open, 0, timeLimit, 0, lookupCache);
                    highestPressure = Math.Max(elephantPressure, highestPressure);
                }

                lookupCache.Add(cacheIndex, highestPressure);

                return highestPressure;
            }

            /// <summary>
            /// Calculate all distances between the valves.
            /// </summary>
            /// <param name="valves">The valves.</param>
            /// <returns>The distances between each pair of valves.</returns>
            private static int[,] CalculateDistances(Valve[] valves)
            {
                var result = new int[valves.Length, valves.Length];
                for (int i = 0; i < valves.Length; i++)
                {
                    for (int j = 0; j < valves.Length; j++)
                    {
                        result[i, j] = int.MaxValue;
                    }
                    result[i, i] = 0;

                    var frontier = new Queue<int>();
                    frontier.Enqueue(i);

                    while (frontier.Count > 0)
                    {
                        int index = frontier.Dequeue();
                        var valve = valves[index];

                        int currentDistance = result[i, index];
                        int nextDistance = currentDistance + 1;

                        foreach (var nextIndex in valve.Valves)
                        {
                            if (result[i, nextIndex] > nextDistance)
                            {
                                result[i, nextIndex] = nextDistance;
                                frontier.Enqueue(nextIndex);
                            }
                        }
                    }
                }

                return result;
            }

            /// <summary>
            /// Read the input from the file.
            /// </summary>
            /// <param name="path">The path to the input file.</param>
            /// <returns>The valves.</returns>
            private static Valve[] ReadInput(string path)
            {
                var formatLine = (string line) => line
                        .Replace("Valve ", "")
                        .Replace(" has flow rate=", " ")
                        .Replace("; tunnels lead to valves ", " ")
                        .Replace("; tunnel leads to valve ", " ")
                        .Replace(", ", ",")
                        .Split();

                var lines = System.IO.File.ReadLines(path)
                    .OrderBy(x => x)
                    .Select(formatLine)
                    .ToArray();
                var result = new Valve[lines.Length];


                var pipeLookup = lines
                    .Select((x, i) => (x[0], i))
                    .ToDictionary(x => x.Item1, y => y.i);

                int index = 0;
                long bitValue = 1;
                foreach (var splitLines in lines)
                {
                    result[index] = new Valve
                    {
                        BitValue = bitValue,
                        FlowRate = int.Parse(splitLines[1]),
                        Valves = splitLines[2]
                            .Split(",")
                            .Select(x => pipeLookup[x])
                            .ToArray()
                    };

                    index++;
                    bitValue *= 2;
                }

                return result;
            }
        }

        /// <summary>
        /// Find the solution where we have 30 secs and no elephant.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The highest pressure.</returns>
        private static int GetSolutionWithoutElephant(string path)
        {
            var valves = new ValveSolver(path);
            return valves.GetHighestPressure(false, 30);
        }

        /// <summary>
        /// Find the solution where we have 26 secs and an elephant.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The highest pressure.</returns>
        private static int GetSolutionWithElephant(string path)
        {
            var valves = new ValveSolver(path);
            return valves.GetHighestPressure(true, 26);
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(1651, GetSolutionWithoutElephant("AOC2022/Day16/Example.txt"));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(2056, GetSolutionWithoutElephant("AOC2022/Day16/Input.txt"));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(1707, GetSolutionWithElephant("AOC2022/Day16/Example.txt"));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(2513, GetSolutionWithElephant("AOC2022/Day16/Input.txt"));

        #endregion
    }
}
