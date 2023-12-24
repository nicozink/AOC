using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using static System.Reflection.Metadata.BlobBuilder;

namespace AOC2023
{
    /// <summary>
    /// Solution for day 22:
    /// https://adventofcode.com/2023/day/22
    /// </summary>
    [TestClass]
    public class Day22
    {
        // Stores a coordinate in 3D space to position blocks.
        record Position(int X, int Y, int Z);

        /// <summary>
        /// A block which is positioned in 3D space with an adjustable height.
        /// </summary>
        struct Block
        {
            /// <summary>
            /// The bottom bounds of the block.
            /// </summary>
            private readonly Position bottom;

            /// <summary>
            /// The top bounds of the block.
            /// </summary>
            private readonly Position top;

            /// <summary>
            /// Create a new block.
            /// </summary>
            /// <param name="bottom">The bottom bounds of the block.</param>
            /// <param name="top">The top bounds of the block.</param>
            /// <param name="height">The initial height of the block.</param>
            public Block(Position bottom, Position top, int height)
            {
                this.bottom = bottom;
                this.top = top;
                Height = height;
            }

            /// <summary>
            /// Gets the bottom bound of the block.
            /// </summary>
            public readonly Position Bottom => bottom with { Z = bottom.Z + Height };

            /// <summary>
            /// Gets the top bounds of the block.
            /// </summary>
            public readonly Position Top => top with { Z = top.Z + Height };
            
            /// <summary>
            /// Gets or sets the height of the block.
            /// </summary>
            public int Height
            {
                get;
                set;
            }
        }

        /// <summary>
        /// Convert a string into a position.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>The position.</returns>
        private static Position GetPosition(string input)
        {
            var positions = input.Split(',')
                .Select(int.Parse)
                .ToArray();

            var position = new Position(positions[0], positions[1], positions[2]);
            return position;
        }

        /// <summary>
        /// Read the blocks from the input.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The blocks in the structure.</returns>
        private static IEnumerable<Block> ReadInput(string path)
        {
            var lines = System.IO.File.ReadAllLines(path);

            foreach (var line in lines)
            {
                var blockSplit = line.Split('~');

                var position1 = GetPosition(blockSplit[0]);
                var position2 = GetPosition(blockSplit[1]);

                var blockHeight = Math.Min(position1.Z, position2.Z);
                position1 = position1 with { Z = position1.Z - blockHeight };
                position2 = position2 with { Z = position2.Z - blockHeight };

                yield return new Block(position1, position2, blockHeight);
            }
        }
        
        /// <summary>
        /// Check if two blocks intersect with their XY coordinates. 
        /// </summary>
        /// <param name="block1">The first block.</param>
        /// <param name="block2">The second block.</param>
        /// <returns>True if they intersect.</returns>
        private static bool Intersect(Block block1, Block block2)
        {
            return block1.Bottom.X <= block2.Top.X && block2.Bottom.X <= block1.Top.X &&
                block1.Bottom.Y <= block2.Top.Y && block2.Bottom.Y <= block1.Top.Y;
        }

        /// <summary>
        /// Solve for gravity by moving the blocks into their resting positions.
        /// </summary>
        /// <param name="blocks">The blocks.</param>
        /// <returns>The number of blocks which were moved.</returns>
        private static int UpdateGravity(List<Block> blocks)
        {
            int numUpdated = 0;
            for (int i = 0; i < blocks.Count; i++)
            {
                var block = blocks[i];

                int newHeight = 1;
                for (int j = i - 1; j >= 0; j--)
                {
                    var otherBlock = blocks[j];

                    if (Intersect(block, otherBlock))
                    {
                        newHeight = otherBlock.Top.Z + 1;
                        break;
                    }
                }

                if (block.Height != newHeight)
                {
                    blocks[i] = block with { Height = newHeight };

                    // Keep the list sorted
                    int previousIndex = i - 1;
                    while (previousIndex > 0 &&
                        blocks[previousIndex].Top.Z > blocks[previousIndex + 1].Top.Z)
                    {
                        (blocks[previousIndex], blocks[previousIndex + 1]) = (blocks[previousIndex + 1], blocks[previousIndex]);
                        previousIndex--;
                    }

                    ++numUpdated;
                }
            }

            return numUpdated;
        }

        /// <summary>
        /// Count the number of blocks which fall for each block which is disintegrated.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The number of falling blocks for each disintegrated block.</returns>
        private static IEnumerable<int> CountDisintegrated(string path)
        {
            var blocks = ReadInput(path)
                .ToList();

            blocks = blocks.OrderBy(x => x.Top.Z)
                .ToList();

            UpdateGravity(blocks);

            for (int i = 0; i < blocks.Count; i++)
            {
                var copy = blocks.ToList();
                copy.RemoveAt(i);

                var blocksMoved = UpdateGravity(copy);
                yield return blocksMoved;
            }
        }

        /// <summary>
        /// Count the number of blocks which are safe to disintegrate without causing other
        /// blocks to fall.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The number of blocks which are safe to disintegrate.</returns>
        private static int CountSafeDisintegrate(string path)
        {
            return CountDisintegrated(path)
                .Count(x => x == 0);
        }

        /// <summary>
        /// Count the number of blocks which fall in total when removing each block
        /// in turn.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The sum of chain reactions.</returns>
        private static int SumChainReactions(string path)
        {
            return CountDisintegrated(path)
                .Sum();
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(5, CountSafeDisintegrate("AOC2023/Day22/Example.txt"));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(530, CountSafeDisintegrate("AOC2023/Day22/Input.txt"));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(7, SumChainReactions("AOC2023/Day22/Example.txt"));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(93292, SumChainReactions("AOC2023/Day22/Input.txt"));

        #endregion
    }
}
