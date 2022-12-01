using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AOC2020
{
    /// <summary>
    /// Solution for day 24:
    /// https://adventofcode.com/2020/day/24
    /// </summary>
    [TestClass]
    public class Day24
    {
        /// <summary>
        /// Gives the directions for each
        /// neighbour in a hexagonal grid.
        /// </summary>
        private enum Direction
        {
            E,
            SE,
            SW,
            W,
            NW,
            NE
        }

        /// <summary>
        /// Contains a lookup from a direction to the delta position
        /// in the grid. Rows of hexagonal cells are offset diagonally.
        /// </summary>
        private Dictionary<Direction, Tuple<int, int>> DirectionLookup =
            new Dictionary<Direction, Tuple<int, int>>()
        {
            { Direction.E, new Tuple<int, int>(0, 1) },
            { Direction.SE, new Tuple<int, int>(1, 0) },
            { Direction.SW, new Tuple<int, int>(1, -1) },
            { Direction.W, new Tuple<int, int>(0, -1) },
            { Direction.NW, new Tuple<int, int>(-1, 0) },
            { Direction.NE, new Tuple<int, int>(-1, 1) }
        };

        /// <summary>
        /// Gets the directions from an input string.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>The direction.</returns>
        private IEnumerable<Direction> GetDirections(string input)
        {
            int position = 0;

            while (position != input.Length)
            {
                if (input[position] == 'e')
                {
                    yield return Direction.E;
                    ++position;
                }
                else if (input[position] == 's' && input[position + 1] == 'e')
                {
                    yield return Direction.SE;
                    position += 2;
                }
                else if (input[position] == 's' && input[position + 1] == 'w')
                {
                    yield return Direction.SW;
                    position += 2;
                }
                else if (input[position] == 'w')
                {
                    yield return Direction.W;
                    ++position;
                }
                else if (input[position] == 'n' && input[position + 1] == 'w')
                {
                    yield return Direction.NW;
                    position += 2;
                }
                else if (input[position] == 'n' && input[position + 1] == 'e')
                {
                    yield return Direction.NE;
                    position += 2;
                }
            }
        }

        /// <summary>
        /// For a starting tile and a direction, returns the position
        /// of the next tile in that direction.
        /// </summary>
        /// <param name="tile">The starting tile.</param>
        /// <param name="direction">The direction.</param>
        /// <returns>The next tile.</returns>
        private Tuple<int, int> MoveNext(Tuple<int, int> tile, Direction direction)
        {
            var nextDirection = DirectionLookup[direction];

            return new Tuple<int, int>(
                tile.Item1 + nextDirection.Item1,
                tile.Item2 + nextDirection.Item2);
        }

        /// <summary>
        /// Gets the starting tile by reading the tiles in the file
        /// and following the directions.
        /// </summary>
        /// <param name="path">The input file.</param>
        /// <returns>The set of black tiles.</returns>
        private HashSet<Tuple<int, int>> GetInitialTiles(string path)
        {
            var input = System.IO.File.ReadLines(path);

            var blackTiles = new HashSet<Tuple<int, int>>();
            foreach (var line in input)
            {
                var tile = new Tuple<int, int>(0, 0);

                foreach (var direction in GetDirections(line))
                {
                    tile = MoveNext(tile, direction);
                }

                var hadTile = blackTiles.Remove(tile);
                if (!hadTile)
                {
                    blackTiles.Add(tile);
                }
            }

            return blackTiles;
        }

        /// <summary>
        /// Enumerate the neighbours for a given tile.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns>The neighbours.</returns>
        private IEnumerable<Tuple<int, int>> EnumerateNeighbours(Tuple<int, int> tile)
        {
            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
            {
                var neighbour = MoveNext(tile, direction);
                yield return neighbour;
            }
        }

        /// <summary>
        /// For a given set of black tiles, this enumerates all tiles and
        /// their neighbours.
        /// </summary>
        /// <param name="tiles">The black tiles.</param>
        /// <returns>The black tiles and all neighbours.</returns>
        private IEnumerable<Tuple<int, int>> EnumerateTiles(HashSet<Tuple<int, int>> tiles)
        {
            var allTiles = new HashSet<Tuple<int, int>>();

            foreach (var tile in tiles)
            {
                allTiles.Add(tile);

                foreach (var neighbour in EnumerateNeighbours(tile))
                {
                    allTiles.Add(neighbour);
                }
            }

            return allTiles;
        }

        /// <summary>
        /// Goes through the rules provided, and flips the matching tiles.
        /// </summary>
        /// <param name="tiles">The starting tiles.</param>
        /// <returns>The flipped tiles.</returns>
        private HashSet<Tuple<int, int>> FlipTiles(HashSet<Tuple<int, int>> tiles)
        {
            var newTiles = new HashSet<Tuple<int, int>>();

            foreach (var tile in EnumerateTiles(tiles))
            {
                var neighbours = EnumerateNeighbours(tile)
                    .Count(x => tiles.Contains(x));

                if (tiles.Contains(tile))
                {
                    if (neighbours == 1 || neighbours == 2)
                    {
                        newTiles.Add(tile);
                    }
                }
                else
                {
                    if (neighbours == 2)
                    {
                        newTiles.Add(tile);
                    }
                }
            }

            return newTiles;
        }

        public long GetSolution1(String path)
        {
            var blackTiles = GetInitialTiles(path);

            return blackTiles.Count();
        }

        public long GetSolution2(String path)
        {
            var blackTiles = GetInitialTiles(path);

            for (int i = 0; i < 100; ++i)
            {
                blackTiles = FlipTiles(blackTiles);
            }

            return blackTiles.Count();
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(10, GetSolution1("AOC2020/Day24/Example.txt"));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(538, GetSolution1("AOC2020/Day24/Input.txt"));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(2208, GetSolution2("AOC2020/Day24/Example.txt"));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(4259, GetSolution2("AOC2020/Day24/Input.txt"));

        #endregion
    }
}
