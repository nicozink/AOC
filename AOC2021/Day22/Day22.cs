using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Common;

namespace AOC2021
{
    /// <summary>
    /// Solution for day 22:
    /// https://adventofcode.com/2021/day/22
    /// </summary>
    [TestClass]
    public class Day22
    {
        /// <summary>
        /// Stores a range of values - this defines the axis of a cube.
        /// </summary>
        /// <param name="Min">The minimum value.</param>
        /// <param name="Max">The maximum value.</param>
        record Range(int Min, int Max)
        {
            /// <summary>
            /// Returns true if the range is valid.
            /// </summary>
            public bool IsValid => Min <= Max;

            /// <summary>
            /// Returns the length of the range. Bounds are inclusive, so we
            /// need to add 1.
            /// </summary>
            public long Length => Max - Min + 1L;

            /// <summary>
            /// Checks if the range completely contains the bounds for another range.
            /// </summary>
            /// <param name="other">The other range.</param>
            /// <returns>True if the range completely includes the other range.</returns>
            public bool Contains(Range other) => Min <= other.Min && Max >= other.Max;

            /// <summary>
            /// Intersects this range with another, and returns the new bounds. If there is
            /// no intersection, the new range will be invalid with min and max flipped.
            /// </summary>
            /// <param name="other">The other range.</param>
            /// <returns>The new range.</returns>
            public Range Intersect(Range other) =>
                new(
                    Math.Max(Min, other.Min),
                    Math.Min(Max, other.Max)
                );
        }

        /// <summary>
        /// Stores a cube defined by ranges along each axis.
        /// </summary>
        /// <param name="RangeX">The range along the x axis.</param>
        /// <param name="RangeY">The range along the x axis.</param>
        /// <param name="RangeZ">The range along the z axis.</param>
        /// <param name="IsOn">A flag determining whether tyhe cube is on (additive) or off.</param>
        record Cube(Range RangeX, Range RangeY, Range RangeZ, bool IsOn)
        {
            /// <summary>
            /// Calculates the area of the cube.
            /// </summary>
            public long Area => RangeX.Length * RangeY.Length * RangeZ.Length * Sign;

            /// <summary>
            /// Returns true if the cube is in the initialisation range.
            /// </summary>
            public bool IsInitialisation =>
                initialisationRange.Contains(RangeX) &&
                initialisationRange.Contains(RangeY) &&
                initialisationRange.Contains(RangeZ);

            /// <summary>
            /// This gives a range to check if the cube is within the initialisation bounds.
            /// </summary>
            private static readonly Range initialisationRange = new(-50, 50);

            /// <summary>
            /// Returns true if the cube is valid when performing an intersection.
            /// </summary>
            private bool IsValid => RangeX.IsValid && RangeY.IsValid && RangeZ.IsValid;

            /// <summary>
            /// Returns the sign of the cube, which is 1 if the cube is additive and
            /// -1 if the cube is subtractive.
            /// </summary>
            private long Sign => IsOn ? 1L : -1L;

            /// <summary>
            /// Intersects one cube with another. We flip the sign, since an on cube
            /// intersecting with another cube will remove the intersection area. On
            /// the other hand, an off cube intersecting with another cube will add
            /// them back. Returns null if there is no intersection.
            /// </summary>
            /// <param name="other">The other cube.</param>
            /// <returns>The intersection cube.</returns>
            public Cube? Intersect(Cube other)
            {
                Range rangeX = RangeX.Intersect(other.RangeX);
                Range rangeY = RangeY.Intersect(other.RangeY);
                Range rangeZ = RangeZ.Intersect(other.RangeZ);

                Cube newCube = new(rangeX, rangeY, rangeZ, !IsOn);

                return newCube.IsValid ? newCube : null;
            }
        }

        /// <summary>
        /// Gets a cube from a string.
        /// </summary>
        /// <param name="line">The string containing the cube.</param>
        /// <returns>The cube.</returns>
        static Cube GetCube(string line)
        {
            bool isOn = true;
            if (line.Substring(0, 3) == "off")
            {
                isOn = false;
            }

            var numbers = line.Replace("on x=", "")
                .Replace("off x=", "")
                .Replace(",y=", " ")
                .Replace(",z=", " ")
                .Replace("..", " ")
                .Split()
                .Select(int.Parse)
                .ToArray();

            var rangeX = new Range(numbers[0], numbers[1]);
            var rangeY = new Range(numbers[2], numbers[3]);
            var rangeZ = new Range(numbers[4], numbers[5]);

            return new(rangeX, rangeY, rangeZ, isOn);
        }

        /// <summary>
        /// Reads a list of cube descriptions from the input, and returns the
        /// number of spaces which are on after processing them.
        /// </summary>
        /// <param name="path">The file containing the instructions.</param>
        /// <param name="initialiseOnly">Whether to only consider initialisation cubes
        /// (in the -50, 50 range).</param>
        /// <returns></returns>
        static long CountCubes(string path, bool initialiseOnly)
        {
            var instructions = System.IO.File.ReadLines(path)
                .Select(GetCube)
                .Where(x => !initialiseOnly || x.IsInitialisation);

            List<Cube> cubes = new();
            foreach (var newCube in instructions)
            {
                List<Cube> newCubes = new();
                if (newCube.IsOn)
                {
                    newCubes.Add(newCube);
                }

                foreach (var existingCube in cubes)
                {
                    var intersection = existingCube.Intersect(newCube);

                    if (intersection != null)
                    {
                        newCubes.Add(intersection);
                    }
                }

                cubes.AddRange(newCubes);
            }
            
            return cubes.Sum(x => x.Area);
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(39, CountCubes("AOC2021/Day22/Example1.txt", initialiseOnly: true));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(590784, CountCubes("AOC2021/Day22/Example2.txt", initialiseOnly: true));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(602574, CountCubes("AOC2021/Day22/Input.txt", initialiseOnly: true));

        [TestMethod]
        public void SolveExample3() => Assert.AreEqual(474140, CountCubes("AOC2021/Day22/Example3.txt", initialiseOnly: true));

        [TestMethod]
        public void SolveExample4() => Assert.AreEqual(2758514936282235, CountCubes("AOC2021/Day22/Example3.txt", initialiseOnly: false));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(1288707160324706, CountCubes("AOC2021/Day22/Input.txt", initialiseOnly: false));

        #endregion
    }
}
