using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace AOC2022
{
    /// <summary>
    /// Solution for day 17:
    /// https://adventofcode.com/2022/day/17
    /// </summary>
    [TestClass]
    public class Day17
    {
        /// <summary>
        /// Stores a position that can be empty or occupied by a rock.
        /// </summary>
        /// <param name="X">The x position.</param>
        /// <param name="Y">The y position.</param>
        record Position(int X, int Y)
        {
            /// <summary>
            /// Adds two positions together.
            /// </summary>
            /// <param name="self">The first instance.</param>
            /// <param name="other">The other instance.</param>
            /// <returns>The rpoduct of both positions.</returns>
            public static Position operator+(Position self, Position other)
            {
                return new Position(self.X + other.X, self.Y + other.Y);
            }
        };

        /// <summary>
        /// Stores the different blocks, and the positions of the areas are
        /// occupied.
        /// </summary>
        static readonly Position[][] blocks = {
            // ####
            new Position[] {
                new (0, 0),
                new (1, 0),
                new (2, 0),
                new (3, 0)
            },

            // .#.
            // ###
            // .#.
            new Position[] {
                new (1, 0),
                new (0, 1),
                new (1, 1),
                new (2, 1),
                new (1, 2)
            },

            // ..#
            // ..#
            // ###
            new Position[] {
                new (0, 0),
                new (1, 0),
                new (2, 0),
                new (2, 1),
                new (2, 2)
            },

            // #
            // #
            // #
            // #
            new Position[] {
                new (0, 0),
                new (0, 1),
                new (0, 2),
                new (0, 3)
            },

            // ##
            // ##
            new Position[] {
                new (0, 0),
                new (1, 0),
                new (0, 1),
                new (1, 1)
            }
        };

        /// <summary>
        /// Checks whether the current position is valid for a block. The
        /// position can be invalid because it is outside the bounds left
        /// and right, or because it hits the ground, or it can intersect
        /// with another rock.
        /// </summary>
        /// <param name="stack">The stack containing other rocks.</param>
        /// <param name="block">The block.</param>
        /// <param name="position">The position of the block.</param>
        /// <returns>True if the position is valid.</returns>
        private static bool IsValidPosition(HashSet<Position> stack, Position[] block, Position position)
        {
            var newPositions = block.Select(x => x + position);
            if (newPositions.Any(x => x.Y <= 0 || x.X < 0 || x.X >= 7 || stack.Contains(x)))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// We cache previous positions to detect a repeating pattern. These
        /// are the parameters we track.
        /// </summary>
        /// <param name="BlockIndex">The last block.</param>
        /// <param name="WindIndex">The last wind index.</param>
        /// <param name="Latest">The latest string representation of the top fo the stack.</param>
        record Cache(int BlockIndex, int WindIndex, string Latest);

        /// <summary>
        /// The result used to track the game including the height of the stack and the number of rocks.
        /// </summary>
        /// <param name="StackHeight">The height of the stack.</param>
        /// <param name="NumRocks">The number of rocks.</param>
        record Result(int StackHeight, int NumRocks);

        /// <summary>
        /// Play tetris for a bit and work out the tower height. We cache the state,
        /// and skip what we can if we find any cycles.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <param name="totalRocks">The total number of rocks.</param>
        /// <returns>The height of the tower.</returns>
        private static long GetTowerHeight(string path, long totalRocks)
        {
            var lookupCache = new Dictionary<Cache, Result>();
            var windDirections = System.IO.File.ReadAllText(path)
                .Select(x => x == '<' ? -1 : 1)
                .ToArray();

            int blockIndex = 0;
            int windIndex = 0;
            int numRocks = 0;

            long addedSkippedHeight = 0;

            var stack = new HashSet<Position>();
            int stackHeight = 0;
            while (numRocks != totalRocks)
            {
                var block = blocks[blockIndex];

                var position = new Position(2, stackHeight + 4);

                bool hasLanded = false;
                while (!hasLanded)
                {
                    var windDirection = windDirections[windIndex];

                    var newWindPosition = position with { X = position.X + windDirection };
                    if (IsValidPosition(stack, block, newWindPosition))
                    {
                        position = newWindPosition;
                    }

                    var newGravityPosition = position with { Y = position.Y - 1 };
                    if (IsValidPosition(stack, block, newGravityPosition))
                    {
                        position = newGravityPosition;
                    }
                    else
                    {
                        hasLanded = true;
                        numRocks++;

                        foreach (var rockPart in block)
                        {
                            stack.Add(rockPart + position);
                        }

                        stackHeight = stack.Max(x => x.Y);

                        int linesStart = stackHeight - 20;
                        linesStart = Math.Max(linesStart, 0);

                        var cacheString = "";
                        for (int i = linesStart; i <= stackHeight; i++)
                        {
                            var line = Enumerable.Range(0, 7)
                                .Select(x => stack.Contains(new Position(x, i)) ? '#' : '.')
                                .ToArray();

                            cacheString += new String(line);
                        }

                        var cache = new Cache(blockIndex, windIndex, cacheString);
                        if (!lookupCache.ContainsKey(cache))
                        {
                            lookupCache[cache] = new Result(stackHeight, numRocks);
                        }
                        else if (addedSkippedHeight == 0)
                        {
                            var oldResult = lookupCache[cache];
                            var heightDiff = stackHeight - oldResult.StackHeight;
                            var rockDiff = numRocks - oldResult.NumRocks;

                            var numLeft = totalRocks - numRocks;
                            var skippableCycles = numLeft / rockDiff;

                            var skippedRocks = rockDiff * skippableCycles;
                            totalRocks -= skippedRocks;

                            addedSkippedHeight = heightDiff * skippableCycles;
                        }
                    }

                    windIndex++;
                    if (windIndex >= windDirections.Length)
                    {
                        windIndex = 0;
                    }
                }

                blockIndex++;
                if (blockIndex >= blocks.Length)
                {
                    blockIndex = 0;
                }
            }

            return stackHeight + addedSkippedHeight;
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(3068, GetTowerHeight("AOC2022/Day17/Example.txt", 2022));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(3191, GetTowerHeight("AOC2022/Day17/Input.txt", 2022));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(1514285714288, GetTowerHeight("AOC2022/Day17/Example.txt", 1000000000000));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(1572093023267, GetTowerHeight("AOC2022/Day17/Input.txt", 1000000000000));

        #endregion
    }
}
