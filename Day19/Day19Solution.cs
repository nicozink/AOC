using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Common;

namespace Solutions
{
    /// <summary>
    /// Solution for day 19:
    /// https://adventofcode.com/2021/day/19
    /// </summary>
    [SolutionClass(Day = 19)]
    public class Day19Solution
    {
        /// <summary>
        /// Stores a coordinate, which represents the 3D location of a
        /// beacon.
        /// </summary>
        /// <param name="X">The x coordinate.</param>
        /// <param name="Y">The y coordinate.</param>
        /// <param name="Z">The z coordingate.</param>
        record Coord(int X, int Y, int Z)
        {
            /// <summary>
            /// Subtracts the given amount from the coordinate.
            /// </summary>
            /// <param name="left">The left hand side.</param>
            /// <param name="right">The right hand side.</param>
            /// <returns>The result.</returns>
            public static Coord operator-(Coord left, Coord right)
            {
                return new Coord(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
            }

            /// <summary>
            /// Adds the given amount to the 3D location of a
            /// beacon.
            /// </summary>
            /// <param name="left">The left hand side.</param>
            /// <param name="right">The right hand side.</param>
            /// <returns>The result.</returns>
            public static Coord operator+(Coord left, Coord right)
            {
                return new Coord(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
            }
        }

        /// <summary>
        /// A function which takes a coordinate and orientates it towards
        /// a new location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <returns>The new location.</returns>
        private delegate Coord Transform(Coord location);

        /// <summary>
        /// Stores all orientation and rotation changes that are possible for
        /// a beacon.
        /// </summary>
        private static readonly Transform[] Transforms = new Transform[]
        {
            // Sensors can be poitioned anywhere along each axis direction. We
            // look at every combination of x and y coordinate. The new z
            // coordinate is given from the other two, and I've pre-calculated
            // the sign using cross products of the unit vectors.

            (pos) => { return new(+pos.X, +pos.Y, +pos.Z); }, // +X x +Y = +Z
            (pos) => { return new(+pos.X, -pos.Y, -pos.Z); }, // +X x -Y = -Z
            (pos) => { return new(+pos.X, +pos.Z, -pos.Y); }, // +X x +Z = -Y
            (pos) => { return new(+pos.X, -pos.Z, +pos.Y); }, // +X x -Z = +Y

            (pos) => { return new(-pos.X, +pos.Y, -pos.Z); }, // -X x +Y = -Z
            (pos) => { return new(-pos.X, -pos.Y, +pos.Z); }, // -X x -Y = +Z
            (pos) => { return new(-pos.X, +pos.Z, +pos.Y); }, // -X x +Z = +Y
            (pos) => { return new(-pos.X, -pos.Z, -pos.Y); }, // -X x -Z = -Y

            (pos) => { return new(+pos.Y, +pos.X, -pos.Z); }, // +Y x +X = -Z
            (pos) => { return new(+pos.Y, -pos.X, +pos.Z); }, // +Y x -X = +Z
            (pos) => { return new(+pos.Y, +pos.Z, +pos.X); }, // +Y x +Z = +X
            (pos) => { return new(+pos.Y, -pos.Z, -pos.X); }, // +Y x -Z = -X

            (pos) => { return new(-pos.Y, +pos.X, +pos.Z); }, // -Y x +X = +Z
            (pos) => { return new(-pos.Y, -pos.X, -pos.Z); }, // -Y x -X = -Z
            (pos) => { return new(-pos.Y, +pos.Z, -pos.X); }, // -Y x +Z = -X
            (pos) => { return new(-pos.Y, -pos.Z, +pos.X); }, // -Y x -Z = +X

            (pos) => { return new(+pos.Z, +pos.X, +pos.Y); }, // +Z x +X = +Y
            (pos) => { return new(+pos.Z, -pos.X, -pos.Y); }, // +Z x -X = -Y
            (pos) => { return new(+pos.Z, +pos.Y, -pos.X); }, // +Z x +Y = -X
            (pos) => { return new(+pos.Z, -pos.Y, +pos.X); }, // +Z x -Y = +X

            (pos) => { return new(-pos.Z, +pos.X, -pos.Y); }, // -Z x +X = -Y
            (pos) => { return new(-pos.Z, -pos.X, +pos.Y); }, // -Z x -X = +Y
            (pos) => { return new(-pos.Z, +pos.Y, +pos.X); }, // -Z x +Y = +X
            (pos) => { return new(-pos.Z, -pos.Y, -pos.X); }  // -Z x -Y = -X
        };

        /// <summary>
        /// Calculates the manhasttan distance between two points.
        /// </summary>
        /// <param name="coord1">The first point.</param>
        /// <param name="coord2">The second point.</param>
        /// <returns>The manhattan distance.</returns>
        static int GetManhattanDistance(Coord coord1, Coord coord2)
        {
            return Math.Abs(coord1.X - coord2.X) +
                Math.Abs(coord1.Y - coord2.Y) +
                Math.Abs(coord1.Z - coord2.Z);
        }

        /// <summary>
        /// Receives a list of beacons for each sensor, and finds the orientation and position
        /// of each. Returns the modified beacon positions, and the sensor position.
        /// </summary>
        /// <param name="sensorBeacons">The beacons for each sensor.</param>
        /// <returns>The sensor and beacon positions.</returns>
        private static IEnumerable<(Coord[] beacons, Coord sensor)> SolveLocations(IEnumerable<Coord[]> sensorBeacons)
        {
            var sensorArray = sensorBeacons.ToArray();

            HashSet<int> remainingIndices = Enumerable
                .Range(1, sensorArray.Length - 1)
                .ToHashSet();

            yield return (sensorArray[0], new(0, 0, 0));

            HashSet<Coord> completed = sensorArray[0].ToHashSet();

            while (remainingIndices.Count != 0)
            {
                foreach (int index in remainingIndices)
                {
                    var potentialMatch = sensorArray[index];

                    foreach (var transform in Transforms)
                    {
                        var transformed = potentialMatch
                            .Select(x => transform(x))
                            .ToList();

                        var differences = transformed
                            .SelectMany(item1 =>
                                completed.Select(item2 => item1 - item2)
                            );

                        var grouped = differences.GroupBy(x => x)
                            .Select(x => (x.Key, x.Count()))
                            .OrderByDescending(x => x.Item2);
                        var mostCommon = grouped
                            .First();

                        if (mostCommon.Item2 >= 12)
                        {
                            var translated = transformed
                                .Select(x => x - mostCommon.Key)
                                .ToArray();

                            completed.UnionWith(translated);
                            remainingIndices.Remove(index);

                            yield return (translated, mostCommon.Key);

                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Reads the input from the file. The input contains the beacon positions
        /// per sensor.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The beacon positions per sensor.</returns>
        static IEnumerable<Coord[]> ReadInput(String path)
        {
            var lines = System.IO.File.ReadLines(path).GetEnumerator();

            lines.MoveNext();

            do
            {
                List<Coord> grid = new();

                while (lines.MoveNext() && !string.IsNullOrEmpty(lines.Current))
                {
                    var elements = lines.Current
                        .Split(',')
                        .Select(int.Parse)
                        .ToArray();

                    grid.Add(new(elements[0], elements[1], elements[2]));
                }

                yield return grid.ToArray();
            }
            while (lines.MoveNext());
        }

        /// <summary>
        /// Reads the beacon positions from the input, and finds the number
        /// of unique beacons once duplicates have been removed.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The number of beacons.</returns>
        static int CountNumberOfProbes(String path)
        {
            var grids = ReadInput(path);

            var sensors = SolveLocations(grids);

            var completeGrid = sensors.Aggregate(new HashSet<Coord>(), (x, y) =>
            {
                x.UnionWith(y.beacons);
                return x;
            });

            return completeGrid.Count;
        }

        /// <summary>
        /// Counts the manhattan distance with the farthest distance between
        /// sensors.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The manhattan distance.</returns>
        static int CountMaxManhattanDistance(String path)
        {
            var grids = ReadInput(path);

            var sensors = SolveLocations(grids)
                .Select(x => x.sensor)
                .ToList();

            return ListExtension.GetPairs(sensors)
                .Max(x => GetManhattanDistance(x.Item1, x.Item2));
        }

        #region Solve Problems

        public static int SolveExample1() => CountNumberOfProbes("Day19/Example.txt");

        [SolutionMethod(Part = 1)]
        public static int SolvePart1() => CountNumberOfProbes("Day19/Input.txt");

        public static int SolveExample2() => CountMaxManhattanDistance("Day19/Example.txt");

        [SolutionMethod(Part = 2)]
        public static int SolvePart2() => CountMaxManhattanDistance("Day19/Input.txt");

        #endregion
    }
}
