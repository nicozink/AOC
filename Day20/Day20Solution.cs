using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Common;

namespace Solutions
{
    /// <summary>
    /// Solution for day 20:
    /// https://adventofcode.com/2021/day/20
    /// </summary>
    [SolutionClass(Day = 20)]
    public class Day20Solution
    {
        /// <summary>
        /// Stores the location of a cell.
        /// </summary>
        /// <param name="X">The x position.</param>
        /// <param name="Y">The y position.</param>
        record Index(int X, int Y);

        /// <summary>
        /// Reads the input. The input stores and image enhancement
        /// algorithm, and an image.
        /// </summary>
        /// <param name="input">The path to the input file.</param>
        /// <returns>The algorithm and image.</returns>
        private static (bool[] enhancement, HashSet<Index> image) ReadInput(String input)
        {
            var lines = System.IO.File.ReadLines(input);

            var imageEnhancement = lines.First()
                .Select(x => x == '#')
                .ToArray();

            HashSet<Index> image = new();

            var grid = lines.Skip(2).ToArray();
            for (int i = 0; i < grid.Length; i++)
            {
                for (int j = 0; j < grid[i].Length; j++)
                {
                    if (grid[i][j] == '#')
                    {
                        image.Add(new(i, j));
                    }
                }
            }

            return (imageEnhancement, image);
        }

        /// <summary>
        /// Stores a sensor image along with the image enhancement algortihm.
        /// Can apply the algorithm to the image to enhance it.
        /// </summary>
        class SensorImage
        {
            /// <summary>
            /// Stores the current bounds of the image. All points within
            /// the bounds are known and stored in the image. Points outside
            /// the image ren't known, and will have a value stored in
            /// outsideChar.
            /// </summary>
            private (Index min, Index max) bounds;

            /// <summary>
            /// Stores the image data. Pixels can either be on or off.
            /// </summary>
            private HashSet<Index> image;

            /// <summary>
            /// Stores the image enhancement algorithm.
            /// </summary>
            private readonly bool[] imageEnhancement;

            /// <summary>
            /// Stores the outside char, which is the current default value of
            /// any pixels outside the burrent bounds of the known image region.
            /// </summary>
            private char outsideChar;

            /// <summary>
            /// Creates a new sensor image. Takes a file and reads out the algorithm
            /// and the image data.
            /// </summary>
            /// <param name="input">The path to the input file.</param>
            public SensorImage(String input)
            {
                var (enhancement, image) = ReadInput(input);

                this.imageEnhancement = enhancement;
                this.image = image;

                int minx = image.Min(x => x.X) - 1;
                int maxx = image.Max(x => x.X) + 1;

                int miny = image.Min(x => x.Y) - 1;
                int maxy = image.Max(x => x.Y) + 1;

                bounds = new(new(minx, miny), new(maxx, maxy));

                outsideChar = '.';
            }

            /// <summary>
            /// Applies enhancements to the image using the image enhancement algorithm.
            /// </summary>
            public void ApplyEnhancement()
            {
                int minx = bounds.min.X - 1;
                int maxx = bounds.max.X + 1;

                int miny = bounds.min.Y - 1;
                int maxy = bounds.max.Y + 1;

                HashSet<Index> newImage = new();
                for (int x = minx; x <= maxx; x++)
                {
                    for (int y = miny; y <= maxy; y++)
                    {
                        var idx = new Index(x, y);

                        int lookup = GetBinaryLookup(idx);
                        if (imageEnhancement[lookup])
                        {
                            newImage.Add(idx);
                        }
                    }
                }

                image = newImage;

                // Set the new outside char if we need to flip it.
                if (imageEnhancement[0])
                {
                    if (outsideChar == '#')
                    {
                        outsideChar = '.';
                    }
                    else
                    {
                        outsideChar = '#';
                    }
                }

                // If the outside char is unlit, then we can trim the bounds down
                // to the known image bounds.
                if (outsideChar == '.')
                {
                    minx = image.Min(x => x.X);
                    maxx = image.Max(x => x.X);

                    miny = image.Min(x => x.Y);
                    maxy = image.Max(x => x.Y);
                }

                bounds = new(new(minx, miny), new(maxx, maxy));
            }

            /// <summary>
            /// Returns the number of 'lit' pixels in the image. This
            /// only includes the known image region.
            /// </summary>
            public int Count
            {
                get => image.Count;
            }

            /// <summary>
            /// Converts the grid to a string.
            /// </summary>
            /// <returns>The string.</returns>
            public override string ToString()
            {
                int minx = bounds.min.X;
                int maxx = bounds.max.X;

                int miny = bounds.min.Y;
                int maxy = bounds.max.Y;

                string imageString = "";
                for (int x = minx; x <= maxx; x++)
                {
                    for (int y = miny; y <= maxy; y++)
                    {
                        var idx = new Index(x, y);

                        imageString += GetCharAt(idx);
                    }

                    imageString += Environment.NewLine;
                }

                return imageString;
            }

            /// <summary>
            /// For a given index, this returns the binary-enced neighbours to use
            /// as a lookup in the image enhancement algorithm.
            /// </summary>
            /// <param name="idx">The index.</param>
            /// <returns>The lookup.</returns>
            private int GetBinaryLookup(Index idx)
            {
                var neighbours = GetNeighbours(idx);
                var chars = neighbours.Select(x => GetCharAt(x) == '#' ? '1' : '0');

                var binaryString = string.Concat(chars);

                return Convert.ToInt32(binaryString, 2);
            }

            /// <summary>
            /// Gets the character at a specific index. For regions outside the
            /// image, return the default outside value.
            /// </summary>
            /// <param name="idx">The index.</param>
            /// <returns>The character.</returns>
            private char GetCharAt(Index idx)
            {
                if (idx.X < bounds.min.X || idx.X > bounds.max.X ||
                    idx.Y < bounds.min.Y || idx.Y > bounds.max.Y)
                {
                    return outsideChar;
                }

                if (image.Contains(idx))
                {
                    return '#';
                }
                else
                {
                    return '.';
                }
            }

            /// <summary>
            /// Gets the neighbours for a point.
            /// </summary>
            /// <param name="idx">The point.</param>
            /// <returns>The neighbours.</returns>
            private IEnumerable<Index> GetNeighbours(Index idx)
            {
                for (int x = idx.X - 1; x <= idx.X + 1; x++)
                {
                    for (int y = idx.Y - 1; y <= idx.Y + 1; y++)
                    {
                        yield return new(x, y);
                    }
                }
            }
        }

        /// <summary>
        /// Reads the image from the file, and applies the enhancement
        /// algorithm the provided number of times.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <param name="numEnhancements">The number of enhancements.</param>
        /// <returns>The number of lit pixels.</returns>
        private static int CountLitPixels(String path, int numEnhancements)
        {
            SensorImage sensorImage = new(path);

            for (int i = 0; i < numEnhancements; i++)
            {
                sensorImage.ApplyEnhancement();
            }

            return sensorImage.Count;
        }

        #region Solve Problems

        public static int SolveExample1() => CountLitPixels("Day20/Example.txt", 2);

        [SolutionMethod(Part = 1)]
        public static int SolvePart1() => CountLitPixels("Day20/Input.txt", 2);

        public static int SolveExample2() => CountLitPixels("Day20/Example.txt", 50);

        [SolutionMethod(Part = 2)]
        public static int SolvePart2() => CountLitPixels("Day20/Input.txt", 50);

        #endregion
    }
}
