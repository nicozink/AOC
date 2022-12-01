using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AOC2015
{
    /// <summary>
    /// Solution for day 3:
    /// https://adventofcode.com/2015/day/3
    /// </summary>
    [TestClass]
    public class Day03
    {
        /// <summary>
        /// Stores the (x,y) location of a house.
        /// </summary>
        /// <param name="X">The x coord.</param>
        /// <param name="Y">The y coord.</param>
        record House(int X, int Y);

        /// <summary>
        /// Follows the directions, and returns all houses
        /// along the way.
        /// </summary>
        /// <param name="directions">The directions.</param>
        /// <returns>The houses.</returns>
        IEnumerable<House> VisitHouses(String directions)
        {
            House position = new(0, 0);
            yield return position;

            foreach (var direction in directions)
            {
                position = direction switch
                {
                    '<' => position with { X = position.X - 1 },
                    '>' => position with { X = position.X + 1 },
                    '^' => position with { Y = position.Y - 1 },
                    'v' => position with { Y = position.Y + 1 },
                    _ => throw new()
                };

                yield return position;
            }
        }

        /// <summary>
        /// Follows the directions, and counts the number of
        /// unique houses visited.
        /// </summary>
        /// <param name="directions">The directions.</param>
        /// <returns>The number of unique houses visited.</returns>
        public int CountSantaHouses(string directions)
        {
            return VisitHouses(directions).Distinct().Count();
        }

        /// <summary>
        /// Reads the instructions from the file, and returns the number
        /// of unique houses visited.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The number of unique houses visited.</returns>
        public int SolveSantaHouses(String path)
        {
            var directions = System.IO.File.ReadAllText(path);

            return CountSantaHouses(directions);
        }

        /// <summary>
        /// Splits the directions into two sets of instructions and
        /// counts the number of houses visited.
        /// </summary>
        /// <param name="directions">The directions.</param>
        /// <returns>The number of houses visited.</returns>
        public int CountSantaAndRobotHouses(string directions)
        {
            var santaDirections = new string(directions.Where((value, index) => index % 2 == 0).ToArray());
            var robotDirections = new string(directions.Where((value, index) => index % 2 == 1).ToArray());

            var santaHouses = VisitHouses(santaDirections).Distinct();
            var robotHouses = VisitHouses(robotDirections).Distinct();

            return santaHouses.Union(robotHouses).Count();
        }

        /// <summary>
        /// Reads the directions from a file, and returns the number
        /// of unique houses visited.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The unique houses visited.</returns>
        public long SolveSantaAndRobotHouses(String path)
        {
            var directions = System.IO.File.ReadAllText(path);

            return CountSantaAndRobotHouses(directions);
        }

        #region Solve Problems

        [TestMethod]
        public void TestExample1()
        {
            Assert.AreEqual(2, CountSantaHouses(">"));
            Assert.AreEqual(4, CountSantaHouses("^>v<"));
            Assert.AreEqual(2, CountSantaHouses("^v^v^v^v^v"));
        }

        [TestMethod]
        public void TestSolution1() => Assert.AreEqual(2081, SolveSantaHouses("AOC2015/Day03/Input.txt"));

        [TestMethod]
        public void TestExample2()
        {
            Assert.AreEqual(3, CountSantaAndRobotHouses("^v"));
            Assert.AreEqual(3, CountSantaAndRobotHouses("^>v<"));
            Assert.AreEqual(11, CountSantaAndRobotHouses("^v^v^v^v^v"));
        }

        [TestMethod]
        public void TestSolution2() => Assert.AreEqual(2341, SolveSantaAndRobotHouses("AOC2015/Day03/Input.txt"));

        #endregion
    }
}
