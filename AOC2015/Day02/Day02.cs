using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AOC2015
{
    /// <summary>
    /// Solution for day 2:
    /// https://adventofcode.com/2015/day/2
    /// </summary>
    [TestClass]
    public class Day02
    {
        /// <summary>
        /// Stores the dimensions of a present.
        /// </summary>
        /// <param name="L">The length.</param>
        /// <param name="W">The width.</param>
        /// <param name="H">The height.</param>
        record Dimension(int L, int W, int H);

        /// <summary>
        /// Reads a collection of dimensions from a file. Each
        /// line consosts of dimensions in the form wxhxd.
        /// </summary>
        /// <param name="path">The input file.</param>
        /// <returns>The dimensions.</returns>
        IEnumerable<Dimension> GetDimensions(String path)
        {
            var input = System.IO.File.ReadAllLines(path);

            foreach (var line in input)
            {
                var dimensions = line.Split('x')
                    .Select(int.Parse)
                    .ToArray();

                yield return new(dimensions[0], dimensions[1], dimensions[2]);
            }
        }

        /// <summary>
        /// Calculates the amount of wrapping paper needed
        /// for a present with the dimensions.
        /// </summary>
        /// <param name="dim">The dimensions.</param>
        /// <returns>The amount of paper.</returns>
        int CalculatePaper(Dimension dim)
        {
            int lxw = dim.L * dim.W;
            int wxh = dim.W * dim.H;
            int hxl = dim.H * dim.L;

            int wrappingPaper = 2 * lxw + 2 * wxh + 2 * hxl;

            int extra = Math.Min(lxw, Math.Min(wxh, hxl));

            return wrappingPaper + extra;
        }

        /// <summary>
        /// Reads a list of dimensions from the file, and calculates
        /// the amount of paper needed.
        /// </summary>
        /// <param name="path">The path to the inout file.</param>
        /// <returns>The amount of paper.</returns>
        int SolvePaper(String path) => GetDimensions(path).Sum(CalculatePaper);

        /// <summary>
        /// Calculates the amount of ribbon needed for a present with
        /// the dimensions.
        /// </summary>
        /// <param name="dim">The dimensions.</param>
        /// <returns>The amount of ribbon.</returns>
        int CalculateRibbon(Dimension dim)
        {
            int lxw = (dim.L + dim.W) * 2;
            int wxh = (dim.W + dim.H) * 2;
            int hxl = (dim.H + dim.L) * 2;

            int ribbon = Math.Min(lxw, Math.Min(wxh, hxl));

            int extra = dim.L * dim.W * dim.H;

            return ribbon + extra;
        }

        /// <summary>
        /// Reads a list of dimensions from the file, and calculates the
        /// total ribbon needed.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The amount of ribbon.</returns>
        public int SolveRibbon(String path) => GetDimensions(path).Sum(CalculateRibbon);

        #region Solve Problems

        [TestMethod]
        public void TestExample1() => Assert.AreEqual(58, CalculatePaper(new(2, 3, 4)));

        [TestMethod]
        public void TestExample2() => Assert.AreEqual(43, CalculatePaper(new(1, 1, 10)));

        [TestMethod]
        public void TestSolution1() => Assert.AreEqual(1588178, SolvePaper("AOC2015/Day02/Input.txt"));

        [TestMethod]
        public void TestSolution2() => Assert.AreEqual(3783758, SolveRibbon("AOC2015/Day02/Input.txt"));

        #endregion
    }
}
