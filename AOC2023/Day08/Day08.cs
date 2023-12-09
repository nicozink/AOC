using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace AOC2023
{
    /// <summary>
    /// Solution for day 8:
    /// https://adventofcode.com/2023/day/8
    /// </summary>
    [TestClass]
    public class Day08
    {
        /// <summary>
        /// A node in the tree giving directions.
        /// </summary>
        /// <param name="Left">The left hand node.</param>
        /// <param name="Right">The fright hand node.</param>
        private record TreeNode(string Left, string Right);

        /// <summary>
        /// The tree containing some traversal commands, and the nodes in the tree.
        /// </summary>
        /// <param name="Commands">The traversal commands.</param>
        /// <param name="TreeNodes">The nodes in the tree.</param>
        private record Tree(string Commands, Dictionary<string, TreeNode> TreeNodes);

        /// <summary>
        /// Reads the tree from the input file.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The tree.</returns>
        private static Tree ReadInput(string path)
        {
            var lines = System.IO.File.ReadAllLines(path);

            var commands = lines[0];
            var treeNodes = new Dictionary<string, TreeNode>();

            for (int i = 2; i < lines.Length; i++)
            {
                var line = lines[i];
                var splitLine = line.Split(" = ");

                var key = splitLine[0];
                var values = splitLine[1]
                    .Replace("(", "")
                    .Replace(")", "")
                    .Split(", ");

                treeNodes.Add(key, new(values[0], values[1]));
            }

            return new(commands, treeNodes);
        }

        /// <summary>
        /// Traverse the key based on the commands given, starting from the
        /// initial location.
        /// </summary>
        /// <param name="tree">The tree.</param>
        /// <param name="startKey">The initial location.</param>
        /// <returns>The number of steps to reach the end.</returns>
        /// <exception cref="InvalidOperationException">When invalid input is received.</exception>
        private static int CountSteps(Tree tree, string startKey)
        {
            var currentKey = startKey;

            int steps = 0;
            while (currentKey[2] != 'Z')
            {
                var nextCommand = steps % tree.Commands.Length;
                var command = tree.Commands[nextCommand];

                var treeNode = tree.TreeNodes[currentKey];
                currentKey = command switch
                {
                    'L' => treeNode.Left,
                    'R' => treeNode.Right,
                    _ => throw new InvalidOperationException()
                };

                steps++;
            }

            return steps;
        }

        /// <summary>
        /// Counts the steps required to solve the puzzle.
        /// </summary>
        /// <param name="input">The path to the input file.</param>
        /// <param name="isGhost">Whether the traversal is done by a ghost.</param>
        /// <returns>The number of steps.</returns>
        private static long CountSteps(string input, bool isGhost)
        {
            var tree = ReadInput(input);

            if (!isGhost)
            {
                return CountSteps(tree, "AAA");
            }
            
            var startKeys = tree.TreeNodes.Keys
                .Where(x => x[2] == 'A')
                .ToList();

            long product = 1;
            foreach (var key in startKeys)
            {
                var steps = CountSteps(tree, key);

                product = Common.MathUtil.LeastCommonMultiple(product, steps);
            }

            return product;
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(2, CountSteps("AOC2023/Day08/Example1.txt", isGhost: false));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(6, CountSteps("AOC2023/Day08/Example2.txt", isGhost: false));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(23147, CountSteps("AOC2023/Day08/Input.txt", isGhost: false));

        [TestMethod]
        public void SolveExample3() => Assert.AreEqual(6, CountSteps("AOC2023/Day08/Example3.txt", isGhost: true));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(22289513667691, CountSteps("AOC2023/Day08/Input.txt", isGhost: true));

        #endregion
    }
}
