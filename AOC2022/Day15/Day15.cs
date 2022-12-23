using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace AOC2022
{
    /// <summary>
    /// Solution for day 15:
    /// https://adventofcode.com/2022/day/15
    /// </summary>
    [TestClass]
    public class Day15
    {
        /// <summary>
        /// Stores a senseor and the nearest beacon.
        /// </summary>
        /// <param name="SensorX">The sensor x position.</param>
        /// <param name="SensorY">The sensor y position.</param>
        /// <param name="BeaconX">The beacon x position.</param>
        /// <param name="BeaconY">The beacon y position.</param>
        record Sensor(int SensorX, int SensorY, int BeaconX, int BeaconY)
        {
            /// <summary>
            /// Returns the manhattan distance between the sensor and beacon.
            /// </summary>
            public int Distance => Math.Abs(SensorX - BeaconX) + Math.Abs(SensorY - BeaconY);
        };

        /// <summary>
        /// Stores a range with a start and end point.
        /// </summary>
        /// <param name="Start">The start point.</param>
        /// <param name="End">The end point.</param>
        record IntRange(int Start, int End);

        /// <summary>
        /// Stores a list of ranges which is constructed from individual start/end points.
        /// </summary>
        class SummedRanges
        {
            /// <summary>
            /// The list of ranges.
            /// </summary>
            private readonly List<IntRange> ranges = new();

            /// <summary>
            /// Gets the list of ranges.
            /// </summary>
            public List<IntRange> Ranges => ranges;

            /// <summary>
            /// Adds a start/end point to the list of ranges.
            /// </summary>
            /// <param name="range">The start/end point.</param>
            public void AddRange(IntRange range)
            {
                int index = 0;

                while (index < ranges.Count && ranges[index].Start < range.Start)
                {
                    index++;
                }

                if (index > 0 && ranges[index - 1].End >= range.Start)
                {
                    index--;

                    if (range.End > ranges[index].End)
                    {
                        ranges[index] = ranges[index] with { End = range.End };
                    }
                }
                else
                {
                    ranges.Insert(index, range);
                }

                while (index < ranges.Count - 1 && ranges[index + 1].Start <= range.End)
                {
                    if (ranges[index + 1].End > ranges[index].End)
                    {
                        ranges[index] = new IntRange(ranges[index].Start, ranges[index + 1].End);
                    }

                    ranges.RemoveAt(index + 1);
                }
            }
        }

        /// <summary>
        /// Reads the sensor data from the file.
        /// </summary>
        /// <param name="path">The inpout file path.</param>
        /// <returns>The sensor data.</returns>
        private static IEnumerable<Sensor> ReadInput(string path)
        {
            var lines = System.IO.File.ReadLines(path);
            foreach (var line in lines)
            {
                var formattedLine = line
                    .Replace("Sensor at x=", "")
                    .Replace(", y=", " ")
                    .Replace(": closest beacon is at x=", " ")
                    .Replace(", y=", " ");
                var splitLines = formattedLine
                    .Split()
                    .Select(int.Parse)
                    .ToArray();

                yield return new Sensor(
                    splitLines[0], splitLines[1], splitLines[2], splitLines[3]);
            }
        }

        /// <summary>
        /// Gets the positions which are known to not contain a beacon.
        /// </summary>
        /// <param name="sensors">The sensor data.</param>
        /// <param name="row">The row to investigate.</param>
        /// <param name="returnBeacons">Whether to include beacons.</param>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <returns></returns>
        private static SummedRanges GetKnownPositions(IEnumerable<Sensor> sensors, int row, bool returnBeacons, int min, int max)
        {
            var knownPositions = new SummedRanges();

            foreach (var sensor in sensors)
            {
                var rowDiff = Math.Abs(sensor.SensorY - row);
                var colDiff = sensor.Distance - rowDiff;

                if (colDiff > 0)
                {
                    var start = sensor.SensorX - colDiff;
                    var end = sensor.SensorX + colDiff;

                    if (end < min)
                    {
                        continue;
                    }

                    if (start > max)
                    {
                        continue;
                    }

                    if (!returnBeacons && sensor.BeaconY == row)
                    {
                        if (sensor.BeaconX == start)
                        {
                            start++;
                        }
                        else
                        {
                            end--;
                        }
                    }

                    knownPositions.AddRange(new IntRange(start, end));
                }
            }

            return knownPositions;
        }

        /// <summary>
        /// Gets the number of spaces which can't contain a beacon (this excludes spaces which
        /// have the beacon).
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <param name="row">The row to investigate.</param>
        /// <returns>The number of spaces which don't contain a beacon.</returns>
        private static int GetNumWithoutBeacon(string path, int row)
        {
            var sensors = ReadInput(path);
            var knownPositions = GetKnownPositions(sensors, row, false, int.MinValue, int.MaxValue);

            var numKnownPositions = knownPositions.Ranges.Sum(x => x.End - x.Start + 1);
            return numKnownPositions;
        }

        /// <summary>
        /// Gets the tuning frequency of the space which contains the distress signal.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <param name="limit">The limit.</param>
        /// <returns>The tuning frequency.</returns>
        /// <exception cref="Exception">Throws an exception if it can't find the signal.</exception>
        private static long GetTuningFrequency(string path, int limit)
        {
            var sensors = ReadInput(path).ToArray();

            for (int r = 0; r < limit; r++)
            {
                var knownPositions = GetKnownPositions(sensors, r, true, 0, limit);
                for (int i = 0; i < knownPositions.Ranges.Count; i++)
                {
                    int last = knownPositions.Ranges[i].End;
                    int next = last + 1;

                    if (next >= 0 && next <= limit)
                    {
                        return (long)next * 4000000 + r;
                    }
                }
            }

            throw new Exception("Shouldn't be here");
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(26, GetNumWithoutBeacon("AOC2022/Day15/Example.txt", 10));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(5525847, GetNumWithoutBeacon("AOC2022/Day15/Input.txt", 2000000));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(56000011, GetTuningFrequency("AOC2022/Day15/Example.txt", 20));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(13340867187704, GetTuningFrequency("AOC2022/Day15/Input.txt", 4000000));

        #endregion
    }
}
