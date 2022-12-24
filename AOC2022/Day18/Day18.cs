using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace AOC2022
{
    /// <summary>
    /// Solution for day 18:
    /// https://adventofcode.com/2022/day/18
    /// </summary>
    [TestClass]
    public class Day18
    {
        record Coord(int X, int Y, int Z);

        private static IEnumerable<Coord> GetAdjacent(Coord position)
        {
            yield return position with { X = position.X - 1 };
            yield return position with { X = position.X + 1 };
            yield return position with { Y = position.Y - 1 };
            yield return position with { Y = position.Y + 1 };
            yield return position with { Z = position.Z - 1 };
            yield return position with { Z = position.Z + 1 };
        }

        private static int GetSolution(string path, bool ignoreBubbles)
        {
            var lines = System.IO.File.ReadLines(path);

            var coords = lines
                .Select(x => x.Split(',').ToArray())
                .Select(x => new Coord(int.Parse(x[0]), int.Parse(x[1]), int.Parse(x[2])))
                .ToDictionary(x => x, y => 1);

            int minX = coords.Keys.Min(x => x.X) - 1;
            int maxX = coords.Keys.Max(x => x.X) + 1;
            int minY = coords.Keys.Min(x => x.Y) - 1;
            int maxY = coords.Keys.Max(x => x.Y) + 1;
            int minZ = coords.Keys.Min(x => x.Z) - 1;
            int maxZ = coords.Keys.Max(x => x.Z) + 1;

            if (ignoreBubbles)
            {
                var frontier = new Queue<Coord>();
                frontier.Enqueue(new Coord(minX, minY, minZ));

                while (frontier.Count > 0)
                {
                    var nextCoord = frontier.Dequeue();

                    if (nextCoord.X < minX || nextCoord.X > maxX ||
                        nextCoord.Y < minY || nextCoord.Y > maxY ||
                        nextCoord.Z < minZ || nextCoord.Z > maxZ)
                    {
                        continue;
                    }

                    if (coords.ContainsKey(nextCoord))
                    {
                        continue;
                    }

                    coords.Add(nextCoord, 0);

                    foreach (var neighbour in GetAdjacent(nextCoord))
                    {
                        frontier.Enqueue(neighbour);
                    }
                }
            }
            else
            {
                for (int x = minX; x <= maxX; x++)
                {
                    for (int y = minY; y <= maxY; y++)
                    {
                        for (int z = minZ; z <= maxZ; z++)
                        {
                            var position = new Coord(x, y, z);

                            if (coords.ContainsKey(position))
                            {
                                continue;
                            }

                            coords.Add(position, 0);
                        }
                    }
                }
            }

            int numSides = 0;
            foreach (var (position, value) in coords)
            {
                if (value != 1)
                {
                    continue;
                }

                var neightbours = GetAdjacent(position);
                var emptyNeighbours = neightbours
                    .Where(x => coords.ContainsKey(x) && coords[x] == 0);

                numSides += emptyNeighbours.Count();
            }

            return numSides;
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(64, GetSolution("AOC2022/Day18/Example.txt", false));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(4314, GetSolution("AOC2022/Day18/Input.txt", false));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(58, GetSolution("AOC2022/Day18/Example.txt", true));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(0, GetSolution("AOC2022/Day18/Input.txt", true));

        #endregion
    }
}
