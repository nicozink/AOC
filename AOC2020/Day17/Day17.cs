using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AOC2020
{
    /// <summary>
    /// Solution for day 17:
    /// https://adventofcode.com/2020/day/17
    /// </summary>
    [TestClass]
    public class Day17
    {
        /// <summary>
        /// Stores a 3D grid of energy sources.
        /// </summary>
        private class EnergyPocket
        {
            /// <summary>
            /// Creates a new instance by reading in the starting energy grid.
            /// </summary>
            /// <param name="path">A file containing the energy grid.</param>
            /// <param name="dimensions">The grid dimensions.</param>
            public EnergyPocket(String path, int dimensions)
            {
                this.dimensions = dimensions;
                energySources = new HashSet<int[]>(new Common.ArrayComparer<int>());

                var input = System.IO.File.ReadLines(path).ToList();

                for (int i = 0; i < input.Count; ++i)
                {
                    var line = input[i];

                    for (int j = 0; j < line.Length; ++j)
                    {
                        var value = line[j];

                        if (value == '#')
                        {
                            var index = Enumerable.Repeat(0, dimensions).ToArray();
                            index[0] = i;
                            index[1] = j;

                            energySources.Add(index);
                        }
                    }
                }
            }

            /// <summary>
            /// Counts the number of power sources.
            /// </summary>
            /// <returns>The number of power sources.</returns>
            public int CountPowerSources()
            {
                return energySources.Count;
            }

            /// <summary>
            /// Gets the maximum bounds of the grid.
            /// </summary>
            /// <returns>The maximum bounds.</returns>
            public int[] GetMaximumBounds()
            {
                var index = new List<int>();

                for (int i = 0; i < dimensions; ++i)
                {
                    int value = energySources.Max(location => location[i]) + 1;
                    index.Add(value);
                }
                
                return index.ToArray();
            }

            /// <summary>
            /// Gets the minimum bounds of the grid.
            /// </summary>
            /// <returns>The minimum bounds.</returns>
            public int[] GetMinimumBounds()
            {
                var index = new List<int>();

                for (int i = 0; i < dimensions; ++i)
                {
                    int value = energySources.Min(location => location[i]) - 1;
                    index.Add(value);
                }

                return index.ToArray();
            }

            /// <summary>
            /// Gets all locations that are between two bounds in the given dimension.
            /// </summary>
            /// <param name="minBounds">The minimum bounds.</param>
            /// <param name="maxBounds">The maximum bounds.</param>
            /// <returns>The locations.</returns>
            public IEnumerable<int[]> GetLocationsBetween(int[] minBounds, int[] maxBounds)
            {
                // We start with an initial list that is empty.
                // We then take that list, and expand it for
                // each value in each dimension.

                var locations = new List<List<int>>()
                {
                    new List<int>()
                };

                for (int i = 0; i < dimensions; ++i)
                {
                    var newLocations = new List<List<int>>();

                    for (int j = minBounds[i]; j <= maxBounds[i]; ++j)
                    {
                        foreach (var elements in locations.Select(x => x.ToList()))
                        {
                            elements.Add(j);
                            newLocations.Add(elements);
                        }
                    }

                    locations = newLocations;
                }

                return locations.Select(x => x.ToArray());
            }

            /// <summary>
            /// Gets the nearest locations for a particular location.
            /// </summary>
            /// <param name="location">The starting location.</param>
            /// <returns>The neighbouring locations.</returns>
            public IEnumerable<int[]> GetNearestLocations(int[] value)
            {
                var arrayComparer = new Common.ArrayComparer<int>();

                var minBounds = value.Select(x => x - 1).ToArray();
                var maxBounds = value.Select(x => x + 1).ToArray();

                foreach (var location in GetLocationsBetween(minBounds, maxBounds))
                {
                    if (!arrayComparer.Equals(location, value))
                    {
                        yield return location;
                    }
                }
            }

            /// <summary>
            /// Update the power grid based the state of the neighbours.
            /// </summary>
            public void Update()
            {
                for (int update = 0; update < 6; ++update)
                {
                    // We make a copy, since we need to update while looking
                    // at the state of the previous one.
                    var gridCopy = new HashSet<int[]>(new Common.ArrayComparer<int>());

                    // We iterate through each point in the grid defined by
                    // the minimum and maximum bounds surrounding active nodes.

                    var minBounds = GetMinimumBounds();
                    var maxBounds = GetMaximumBounds();

                    foreach (var location in GetLocationsBetween(minBounds, maxBounds))
                    {
                        // For each point, we now look at the immediate neighbours to
                        // count the active energy sources.

                        var neighbours = GetNearestLocations(location);
                        var numActive = neighbours.Count(neighbour => energySources.Contains(neighbour));

                        if (energySources.Contains(location))
                        {
                            if (numActive == 2 || numActive == 3)
                            {
                                gridCopy.Add(location);
                            }
                        }
                        else
                        {
                            if (numActive == 3)
                            {
                                gridCopy.Add(location);
                            }
                        }
                    }

                    energySources = gridCopy;
                }
            }

            /// <summary>
            /// The dimensions of the grid.
            /// </summary>
            private readonly int dimensions;

            /// <summary>
            /// The grid of energy sources.
            /// </summary>
            private HashSet<int[]> energySources;
        }

        public int GetSolution1(String path)
        {
            var grid = new EnergyPocket(path, 3);
            grid.Update();

            return grid.CountPowerSources();
        }

		public int GetSolution2(String path)
        {
            var grid = new EnergyPocket(path, 4);
            grid.Update();

            return grid.CountPowerSources();
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(112, GetSolution1("Day17/Example.txt"));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(348, GetSolution1("Day17/Input.txt"));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(848, GetSolution2("Day17/Example.txt"));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(2236, GetSolution2("Day17/Input.txt"));

        #endregion
    }
}
