using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AOC2020
{
    /// <summary>
    /// Solution for day 20:
    /// https://adventofcode.com/2020/day/20
    /// </summary>
    [TestClass]
    public class Day20
    {
        #region Direction

        /// <summary>
        /// Stores a direction. This is used to find the position of a tile
        /// relative to anohter tile, or used to determine the rotation of a
        /// tile. E.g. a tile that is facing left will be rotated by 90 degrees.
        /// </summary>
        enum Direction
        {
            Top = 1,
            Right = 2,
            Bottom = 3,
            Left = 4
        }

        /// <summary>
        /// Gets an enumarable collection of all possible directions.
        /// </summary>
        /// <returns>The collection of directions.</returns>
        private static IEnumerable<Direction> GetDirections()
        {
            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
            {
                yield return direction;
            }
        }

        /// <summary>
        /// Gets the opposite of the specified direction.
        /// </summary>
        /// <param name="direction">The direction.</param>
        /// <returns>The opposite direction.</returns>
        private static Direction GetOpposite(Direction direction)
        {
            switch (direction)
            {
                case Direction.Top:
                    {
                        return Direction.Bottom;
                    }

                case Direction.Bottom:
                    {
                        return Direction.Top;
                    }

                case Direction.Left:
                    {
                        return Direction.Right;
                    }

                case Direction.Right:
                    {
                        return Direction.Left;
                    }

                default:
                    {
                        throw new Exception("Invalid direction.");
                    }
            }
        }

        #endregion

        #region Orientation

        /// <summary>
        /// Gives the orientation of a tile. A tile can be rotated to a
        /// specific direction, or flipped.
        /// </summary>
        private class Orientation
        {
            internal Direction Direction;

            internal bool Flipped;
        }

        /// <summary>
        /// Gets a collection of all orientations - which includes all
        /// combinations of being rotated and flipped.
        /// </summary>
        /// <returns>The orientations.</returns>
        private static IEnumerable<Orientation> GetOrientations()
        {
            foreach (var direction in GetDirections())
            {
                yield return new Orientation()
                {
                    Direction = direction,
                    Flipped = false
                };

                yield return new Orientation()
                {
                    Direction = direction,
                    Flipped = true
                };
            }
        }

        #endregion

        /// <summary>
        /// A tile that represents a fragment of an image. Tiles are
        /// received out of order, and can be re-arranged, rotated and
        /// flipped to produce the image.
        /// </summary>
        private class Tile
        {
            #region Constructors

            /// <summary>
            /// Creates a new tile.
            /// </summary>
            /// <param name="tileId">The tile Id</param>
            /// <param name="grid">The contents of the tile</param>
            internal Tile(int tileId, string[] grid)
            {
                TileId = tileId;
                this.grid = grid;

                Orientation = new Orientation()
                {
                    Direction = Direction.Top,
                    Flipped = false
                };
            }

            #endregion

            #region Internal Properties

            /// <summary>
            /// Stores the current orientation of the tile. The
            /// orientation can be changed to find a fit with
            /// other tiles.
            /// </summary>
            internal Orientation Orientation
            {
                get;
                set;
            }

            /// <summary>
            /// Returns the size of the tile.
            /// </summary>
            internal int Size
            {
                get
                {
                    return grid.Length;
                }
            }

            /// <summary>
            /// Returns the tile id.
            /// </summary>
            internal int TileId
            {
                get;
            }

            #endregion

            #region Internal Methods

            /// <summary>
            /// Gets a value from the tile at the specified position.
            /// This method accounts for the rotation of the tile, and
            /// whether it is flipped.
            /// </summary>
            /// <param name="row">The row.</param>
            /// <param name="col">The column.</param>
            /// <returns>The value.</returns>
            internal char GetValue(int row, int col)
            {
                Direction direction = Orientation.Direction;

                // Adjusts the rotation in 90 degree intervals based
                // on the direction of the tile.
                while (direction != Direction.Top)
                {
                    int temp_row = row;

                    row = col;
                    col = Size - temp_row - 1;

                    direction = (Direction)((int)direction - 1);
                }

                // If the card is marked as flipped, invert the row.
                if (Orientation.Flipped)
                {
                    row = Size - row - 1;
                }

                return grid[row][col];
            }

            /// <summary>
            /// Gets a sequence of values from the tile. E.g. with a
            /// direction of top, this returns all entries in the top
            /// row after accounting for rotation.
            /// </summary>
            /// <param name="direction">The direction of the values</param>
            /// <returns>The Values</returns>
            internal IEnumerable<char> GetValues(Direction direction)
            {
                switch (direction)
                {
                    case Direction.Top:
                        {
                            return GetRow(0);
                        }

                    case Direction.Bottom:
                        {
                            return GetRow(Size - 1);
                        }

                    case Direction.Left:
                        {
                            return GetCol(0);
                        }

                    case Direction.Right:
                        {
                            return GetCol(Size - 1);
                        }

                    default:
                        {
                            throw new Exception("Invalid direction.");
                        }
                }
            }

            #endregion

            #region Private Methods

            /// <summary>
            /// Gets a row from the tile.
            /// </summary>
            /// <param name="row">The row index.</param>
            /// <returns>The values in the row.</returns>
            private IEnumerable<char> GetRow(int row)
            {
                for (int col = 0; col < Size; ++col)
                {
                    yield return GetValue(row, col);
                }
            }

            /// <summary>
            /// Gets a column from the tile.
            /// </summary>
            /// <param name="col">The column index.</param>
            /// <returns>The values in the column.</returns>
            private IEnumerable<char> GetCol(int col)
            {
                for (int row = 0; row < Size; ++row)
                {
                    yield return GetValue(row, col);
                }
            }

            #endregion

            #region Private Variables

            /// <summary>
            /// Stores the values in the tile. These are in the order
            /// they were originally provided, but access function to
            /// this data account for rotation of the tile.
            /// </summary>
            private readonly string[] grid;

            #endregion
        }

        /// <summary>
        /// Stores the information for a match between two tiles.
        /// Holds the index of the two tiles, and in which direction
        /// the matching edge is located (relative to the first tile).
        /// </summary>
        private class MatchInfo
        {
            internal int Index1;
            internal int Index2;

            internal Direction Direction;
        }

        /// <summary>
        /// Stores an image, which is given as an unsorted bunch of tiles.
        /// Has functionality that sorts and combines the tiles, and finds
        /// sea monsters in the final image.
        /// </summary>
        private class Image
        {
            /// <summary>
            /// Creates a new instance of the image.
            /// </summary>
            /// <param name="path">The file path.</param>
            internal Image(String path)
            {
                tiles = ReadTiles(path).ToArray();
                matches = GetMatches().ToArray();
                solution = Solve();
            }

            /// <summary>
            /// After solving the layout of the image, this function returns
            /// the product of the four corner tile ids.
            /// </summary>
            /// <returns>The product of the tile ids.</returns>
            internal long GetCornerProduct()
            {
                var corners = GetCorners();

                long product = 1;
                foreach (var corner in corners)
                {
                    product *= tiles[corner].TileId;
                }

                return product;
            }

            /// <summary>
            /// After solving the layout of the image, this function identifies
            /// sea monsters, and then counts the surrounding noise as the water
            /// roughness.
            /// </summary>
            /// <returns>The water roughness.</returns>
            internal int GetWaterRoughness()
            {
                var size = solution.GetLength(0);
                var tileSize = tiles[0].Size;

                // After solving the layout of the image, we combine the image and
                // smaller tiles into a larger tile that holds the complete image.

                var combinedStrings = new List<string>();
                for (int row = 0; row < size; ++row)
                {
                    for (int tileRow = 1; tileRow < tileSize - 1; ++tileRow)
                    {
                        string result = "";

                        for (int col = 0; col < size; ++col)
                        {
                            var tile = solution[row, col];

                            for (int tileCol = 1; tileCol < tileSize - 1; ++tileCol)
                            {
                                result += tile.GetValue(tileRow, tileCol);
                            }
                        }

                        combinedStrings.Add(result);
                    }
                }

                var combinedTile = new Tile(0, combinedStrings.ToArray());

                // This is the pattern of the sea monster we are looking for. We also
                // store the number of '#' characters in the pattern, since we will
                // subtract these from the image to count the background noise.
                var seaMonster = new string[]
                {
                    "                  # ",
                    "#    ##    ##    ###",
                    " #  #  #  #  #  #   "
                };
                int seaMonsterCount = seaMonster.Sum(x => x.Count(c => c == '#'));

                // This is the total number of markings appearing in the image. We
                // look for sea monsters in them, which will be subtracted from this
                // grand total.
                var totalSeaRoughness = 0;
                for (int row = 0; row < combinedTile.Size; ++row)
                {
                    for (int col = 0; col < combinedTile.Size; ++col)
                    {
                        var tileChar = combinedTile.GetValue(row, col);

                        if (tileChar == '#')
                        {
                            ++totalSeaRoughness;
                        }
                    }
                }

                // This finds the patterns for the sea monster in the image. Sea monsters can
                // appear in any orientation, so we need to adjust the image to find them. Any
                // sea monsters found will be subtracted from the total marks in the image.
                foreach (var orientation in GetOrientations())
                {
                    combinedTile.Orientation = orientation;
                    
                    for (int row = 0; row < combinedTile.Size - 2; ++row)
                    {
                        for (int col = 0; col < combinedTile.Size - seaMonster[0].Length; ++col)
                        {
                            var isMatch = true;

                            for (int irow = 0; irow < seaMonster.Length && isMatch; ++irow)
                            {
                                for (int icol = 0; icol < seaMonster[0].Length && isMatch; ++icol)
                                {
                                    var tileRow = row + irow;
                                    var tileCol = col + icol;

                                    var tileChar = combinedTile.GetValue(tileRow, tileCol);

                                    if (seaMonster[irow][icol] == '#' && tileChar != '#')
                                    {
                                        isMatch = false;
                                    }
                                }
                            }

                            if (isMatch)
                            {
                                totalSeaRoughness -= seaMonsterCount;
                            }
                        }
                    }
                }

                return totalSeaRoughness;
            }

            #region Private Methods

            /// <summary>
            /// This gets all corners in the reconstructed image, based on
            /// the matches that have been calculated.
            /// </summary>
            /// <returns>The corners.</returns>
            private IEnumerable<int> GetCorners()
            {
                var tiles = matches
                    .Select(match => match.Index1)
                    .Distinct();

                foreach (var tile in tiles)
                {
                    int numMatches = matches.Count(match => match.Index1 == tile);

                    if (numMatches == 2)
                    {
                        yield return tile;
                    }
                }
            }

            /// <summary>
            /// This goes through the tiles that have been received, and constructs
            /// a sequence of matches. The orientation of the first tile is taken as
            /// fixed, and the rest of the tiles are rotated and flipped, so that they
            /// all end up having matches between one another.
            /// </summary>
            /// <returns>The collection of matches.</returns>
            private IEnumerable<MatchInfo> GetMatches()
            {
                var finished = new HashSet<int>();

                // We start with the first tile, which is assumed to be fixed. We then
                // find and rotate any neighbours, and continue finding matches for those,
                // until we have found matches for all tiles.

                var frontier = new Stack<int>();
                frontier.Push(0);

                while (frontier.Count != 0)
                {
                    // Get the next tile to process from the frontier. If it
                    // has already been processed, then we can ignore it.

                    var nextTile = frontier.Pop();

                    if (finished.Contains(nextTile))
                    {
                        continue;
                    }

                    // We get all neighbours for the current tile. The function to do
                    // this will rotate the tiles if necessary.

                    var neightbours = GetNeighbours(nextTile);
                    foreach (var neighbour in neightbours)
                    {
                        yield return neighbour;

                        // Add the next tile to the frontier. We may have changed it's
                        // orientation in order to find a match, but from now on the
                        // orientation will stay fixed.
                        frontier.Push(neighbour.Index2);
                    }

                    finished.Add(nextTile);
                }
            }

            /// <summary>
            /// This checks the tile with the current index, and finds all neighbours
            /// the match along one of the sides. The orientation of the provided
            /// tile will be fixed, but all neighbours may be rotated.
            /// </summary>
            /// <param name="index">The index of the tile to check.</param>
            /// <returns>Any matching neighbours.</returns>
            private IEnumerable<MatchInfo> GetNeighbours(int index)
            {
                // We use i to search through all tiles, and match them to the
                // provided tile.
                for (int i = 0; i < tiles.Length; ++i)
                {
                    if (i == index)
                    {
                        continue;
                    }

                    // We inspect each neighbouring tile. To do this, we change the
                    // orientation to detect a match with the current tile. We later
                    // need to revert the orientation, if we can't find a match. So
                    // we save the current orientation.
                    var neighbourTile = tiles[i];
                    var orientation = neighbourTile.Orientation;

                    // We search though all orientations to match a neighbouring tile.
                    foreach (var nextOrientation in GetOrientations())
                    {
                        // We apply the orientation.
                        neighbourTile.Orientation = nextOrientation;

                        // We need to check each direction.
                        foreach (var direction in GetDirections())
                        {
                            var match = new MatchInfo()
                            {
                                Index1 = index,
                                Index2 = i,
                                Direction = direction
                            };

                            // If there is a match, then we return it as a match, and
                            // permanently apply the new orientation.
                            if (IsValidMatch(match))
                            {
                                orientation = nextOrientation;

                                yield return match;
                            }
                        }
                    }

                    // Restore the previous orientation, unless it was replaced after
                    // finding a match.
                    neighbourTile.Orientation = orientation;
                }
            }

            /// <summary>
            /// Gets the top left corner based on the list of matches that have been found.
            /// </summary>
            /// <returns>The index of the top left corner.</returns>
            private int GetTopLeftCorner()
            {
                var corners = GetCorners();

                foreach (var corner in corners)
                {
                    var topLeft = from match in matches
                                  where match.Index1 == corner &&
                                    (match.Direction == Direction.Bottom ||
                                    match.Direction == Direction.Right)
                                  select match;

                    if (topLeft.Count() == 2)
                    {
                        return corner;
                    }
                }

                throw new Exception("Could not find the top left corner");
            }

            /// <summary>
            /// Checks whether a match is valid for two tiles. This takes
            /// the two tiles and direction as input, and considers the
            /// current orientation when getting the values from the
            /// tile.
            /// </summary>
            /// <param name="matchInfo">The information to query a match.</param>
            /// <returns>True if the match is valid.</returns>
            private bool IsValidMatch(MatchInfo matchInfo)
            {
                var direction = matchInfo.Direction;

                var tile1 = tiles[matchInfo.Index1];
                var tile2 = tiles[matchInfo.Index2];

                var edge1 = tile1.GetValues(direction);
                var edge2 = tile2.GetValues(GetOpposite(direction));

                if (Enumerable.SequenceEqual(edge1, edge2))
                {
                    return true;
                }

                return false;
            }

            /// <summary>
            /// Reads all tiles from the input file.
            /// </summary>
            /// <param name="path">The input file.</param>
            /// <returns>The tiles.</returns>
            private IEnumerable<Tile> ReadTiles(string path)
            {
                var input = System.IO.File.ReadLines(path);
                var enumerator = input.GetEnumerator();

                while (enumerator.MoveNext())
                {
                    var tileNumber = enumerator.Current
                        .Replace("Tile ", "")
                        .Replace(":", "");

                    var grid = new List<string>();
                    while (enumerator.MoveNext() && enumerator.Current != "")
                    {
                        grid.Add(enumerator.Current);
                    }

                    yield return new Tile(int.Parse(tileNumber), grid.ToArray());
                }
            }

            /// <summary>
            /// Works out the order of the tiles to produce the final image based
            /// on the matches that were calculated.
            /// </summary>
            /// <returns>The tiles arranged into the final image.</returns>
            private Tile[,] Solve()
            {
                int size = (int)Math.Sqrt(tiles.Length);
                var result = new Tile[size, size];

                // We start at the top left corner, and travers the rows and columns
                // to reconstruct the image.

                int topLeftCorner = GetTopLeftCorner();

                int row = 0;
                foreach (var rowTile in TraverseMatches(topLeftCorner, Direction.Bottom))
                {
                    int col = 0;
                    foreach (var colTile in TraverseMatches(rowTile, Direction.Right))
                    {
                        result[row, col] = tiles[colTile];

                        ++col;
                    }

                    ++row;
                }

                return result;
            }

            /// <summary>
            /// Starts at a given tile, and traverses the matched tiles in the direction
            /// provided.
            /// </summary>
            /// <param name="tile">The starting tile</param>
            /// <param name="direction">The direction</param>
            /// <returns>The matching tiles in the direction</returns>
            private IEnumerable<int> TraverseMatches(int tile, Direction direction)
            {
                while (true)
                {
                    yield return tile;

                    var nextMatch = matches
                        .Where(match => match.Index1 == tile && match.Direction == direction)
                        .SingleOrDefault();

                    if (nextMatch != null)
                    {
                        tile = nextMatch.Index2;
                    }
                    else
                    {
                        yield break;
                    }
                }
            }

            #endregion

            #region Private Variables

            /// <summary>
            /// Stores the matches that were found between tiles.
            /// </summary>
            private readonly MatchInfo[] matches;

            /// <summary>
            /// Stores the ordered sequence of tiles, which store the reconstructed image.
            /// </summary>
            private readonly Tile[,] solution;

            /// <summary>
            /// Stores the tiles that were received. These are unordered, in the sequence
            /// they were given.
            /// </summary>
            private readonly Tile[] tiles;

            #endregion
        }

        public long GetSolution1(String path)
        {
            var image = new Image(path);
            
            return image.GetCornerProduct();
        }

        public long GetSolution2(String path)
        {
            var image = new Image(path);

            return image.GetWaterRoughness();
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(20899048083289, GetSolution1("Day20/Example.txt"));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(18482479935793, GetSolution1("Day20/Input.txt"));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(273, GetSolution2("Day20/Example.txt"));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(2118, GetSolution2("Day20/Input.txt"));

        #endregion
    }
}
