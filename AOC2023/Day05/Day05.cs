using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AOC2023
{
    /// <summary>
    /// Solution for day 5:
    /// https://adventofcode.com/2023/day/5
    /// </summary>
    [TestClass]
    public class Day05
    {
        /// <summary>
        /// Stores a mapping between a range of source and destination numbers,
        /// e.g. seeds and locations.
        /// </summary>
        /// <param name="DestinationIndex">The starting destination index.</param>
        /// <param name="SourceIndex">The starting source index.</param>
        /// <param name="Length">The length.</param>
        private record SeedRange(long DestinationIndex, long SourceIndex, long Length);

        /// <summary>
        /// Stores the starting seeds as well as a mapping from seeds to locations,
        /// as well as the steps in between.
        /// </summary>
        /// <param name="Seeds">The starting seeds.</param>
        /// <param name="SeedMaps">The mapping between seeds and locations.</param>
        private record SeedGuide(long[] Seeds, SeedRange[][] SeedMaps);

        /// <summary>
        /// Reads the seed guide data from the input file.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The seed guide.</returns>
        private static SeedGuide ReadInput(string path)
        {
            var lines = System.IO.File.ReadAllLines(path);

            var seeds = lines[0]
                .Replace("seeds: ", "")
                .Split()
                .Select(long.Parse)
                .ToArray();

            List<SeedRange[]> seedMaps = new();
            for (int i = 3; i < lines.Length; i++)
            {
                List<SeedRange> ranges = new();

                while (i < lines.Length && !string.IsNullOrEmpty(lines[i]))
                {
                    var map = lines[i]
                        .Split()
                        .Select(long.Parse)
                        .ToArray();

                    ranges.Add(new(map[0], map[1], map[2]));

                    i++;
                }

                seedMaps.Add(ranges.ToArray());

                i++;
            }

            return new(seeds, seedMaps.ToArray());
        }

        /// <summary>
        /// Gets the location from a given seed by following the ranged maps.
        /// </summary>
        /// <param name="seedGuide">The seed guide giving the seeds.</param>
        /// <param name="item">The starting seed item.</param>
        /// <returns>The location for the corresponding seed.</returns>
        private static long GetSeedLocation(SeedGuide seedGuide, long item)
        {
            foreach (var map in seedGuide.SeedMaps)
            {
                foreach (var range in map)
                {
                    long sourceStart = range.SourceIndex;
                    long sourceEnd = sourceStart + range.Length;

                    if (item >= sourceStart && item <= sourceEnd)
                    {
                        long offset = item - sourceStart;
                        item = range.DestinationIndex + offset;

                        break;
                    }
                }
            }

            return item;
        }

        /// <summary>
        /// Find the lowest seed location for the given input.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The lowest seed location.</returns>
        private static long FindLowestSeedLocation(string path)
        {
            var seedMaps = ReadInput(path);

            long lowestLocation = int.MaxValue;
            foreach (var seed in seedMaps.Seeds)
            {
                var location = GetSeedLocation(seedMaps, seed);
                if (location < lowestLocation)
                {
                    lowestLocation = location;
                }
            }

            return lowestLocation;
        }

        /// <summary>
        /// Gets the seed for a specific location by reverse-searching the range
        /// mappings.
        /// </summary>
        /// <param name="reversedSeedMap">The reverse seed map.</param>
        /// <param name="item">The starting location.</param>
        /// <returns>The initial seed for the location.</returns>
        private static long GetLocationSeed(SeedRange[][] reversedSeedMap, long item)
        {
            foreach (var map in reversedSeedMap)
            {
                foreach (var range in map)
                {
                    long destinationStart = range.DestinationIndex;
                    long destinationEnd = destinationStart + range.Length;

                    if (item >= destinationStart && item <= destinationEnd)
                    {
                        long offset = item - destinationStart;
                        item = range.SourceIndex + offset;

                        break;
                    }
                }
            }

            return item;
        }

        /// <summary>
        /// Find the lowest location for the ranges of seeds in the input.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <param name="locationSeed">A seed giving a lower bound to speed up computation.</param>
        /// <returns>The lowest location based on the range of seeds.</returns>
        private static long FindLowestRangeLocation(string path, int locationSeed = 0)
        {
            var seedMaps = ReadInput(path);
            var seeds = seedMaps.Seeds;

            var reversedSeedMap = seedMaps.SeedMaps
                .Reverse()
                .ToArray();

            long location = locationSeed;
            while (true)
            {
                var seed = GetLocationSeed(reversedSeedMap, location);

                for (int seedLocation = 0; seedLocation < seeds.Length; seedLocation += 2)
                {
                    var seedStart = seeds[seedLocation];
                    var seedEnd = seedStart + seeds[seedLocation + 1];

                    if (seed >= seedStart && seed <= seedEnd)
                    {
                        return location;
                    }
                }

                location++;
            }
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(35, FindLowestSeedLocation("AOC2023/Day05/Example.txt"));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(579439039, FindLowestSeedLocation("AOC2023/Day05/Input.txt"));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(46, FindLowestRangeLocation("AOC2023/Day05/Example.txt"));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(7873084, FindLowestRangeLocation("AOC2023/Day05/Input.txt", 7800000));

        #endregion
    }
}
