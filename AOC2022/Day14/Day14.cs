using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace AOC2022
{
    /// <summary>
    /// Solution for day 14:
    /// https://adventofcode.com/2022/day/14
    /// </summary>
    [TestClass]
    public class Day14
    {
        /// <summary>
        /// Gets the next position according to the rules. Sand first falls down.
        /// If that is blocked, then it falls diagonally left. And then diagonally
        /// right.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns>The next positions.</returns>
        private static IEnumerable<(int, int)> GetNextPosition((int, int) position)
        {
            yield return (position.Item1, position.Item2 + 1);
            yield return (position.Item1 - 1, position.Item2 + 1);
            yield return (position.Item1 + 1, position.Item2 + 1);
        }

        /// <summary>
        /// Gets the positions between two points in the input.
        /// </summary>
        /// <param name="start">The string giving the start location.</param>
        /// <param name="end">The string giving the end location.</param>
        /// <returns>The positions.</returns>
        private static IEnumerable<(int, int)> GetPositions(string start, string end)
        {
            var startValues = start
                .Split(",")
                .Select(int.Parse)
                .ToArray();

            var endValues = end
                .Split(",")
                .Select(int.Parse)
                .ToArray();

            int startX = Math.Min(startValues[0], endValues[0]);
            int endX = Math.Max(startValues[0], endValues[0]);
            int startY = Math.Min(startValues[1], endValues[1]);
            int endY = Math.Max(startValues[1], endValues[1]);

            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    yield return (x, y);
                }
            }
        }

        /// <summary>
        /// Read the input positions from the file.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The positions of obstacles.</returns>
        private static IEnumerable<(int, int)> ReadInput(string path)
        {
            var lines = System.IO.File.ReadLines(path);

            foreach (var line in lines)
            {
                var directionSplit = line.Split(" -> ")
                    .ToArray();

                for (int i = 0; i < directionSplit.Length - 1; i++)
                {
                    var prev = directionSplit[i];
                    var next = directionSplit[i + 1];

                    foreach (var position in GetPositions(prev, next))
                    {
                        yield return position;
                    }
                }
            }
        }

        /// <summary>
        /// Drops sand from the opening, fills the area and counts the
        /// amount of sand that falls until the conditions are met.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <param name="hasBottom">Whether there is a bottom.</param>
        /// <returns>The amount of sand.</returns>
        private static int CountSand(string path, bool hasBottom)
        {
            var obstacles = ReadInput(path).ToHashSet();
            var bottom = obstacles.Max(x => x.Item2) + 2;

            int numAdded = 0;
            while (true)
            {
                var newPosition = (500, 0);

                while (true)
                {
                    bool moved = false;
                    foreach (var next in GetNextPosition(newPosition))
                    {
                        if (!obstacles.Contains(next) && next.Item2 < bottom)
                        {
                            moved = true;
                            newPosition = next;
                            break;
                        }
                    }

                    if (!moved)
                    {
                        break;
                    }
                }

                if (!hasBottom && newPosition.Item2 >= bottom - 1)
                {
                    break;
                }

                numAdded++;
                obstacles.Add(newPosition);

                if (newPosition == (500, 0))
                {
                    break;
                }
            }

            return numAdded;
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(24, CountSand("AOC2022/Day14/Example.txt", false));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(862, CountSand("AOC2022/Day14/Input.txt", false));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(93, CountSand("AOC2022/Day14/Example.txt", true));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(28744, CountSand("AOC2022/Day14/Input.txt", true));

        #endregion
    }
}
