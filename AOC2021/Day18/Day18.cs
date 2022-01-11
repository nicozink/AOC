using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Common;

namespace AOC2021
{
    /// <summary>
    /// Solution for day 18:
    /// https://adventofcode.com/2021/day/18
    /// </summary>
    [TestClass]
    public class Day18
    {
        /// <summary>
        /// Stores a binary tree which represents numbers stored in
        /// snailfish format. Each node is a pair of child nodes.
        /// Leaf nodes contain numbers.
        /// </summary>
        class BinaryTree
        {
            #region Nested Types

            /// <summary>
            /// The tree can have two children, one to the left
            /// and one to the right.
            /// </summary>
            enum ChildType
            {
                Left,
                Right
            }

            #endregion

            #region Variables

            /// <summary>
            /// Stores a value. This is valid for leaf nodes, and
            /// represents the number stored in the tree.
            /// </summary>
            private int Value;

            /// <summary>
            /// This is the left child node for the binary subtree.
            /// This is null for leaf nodes.
            /// </summary>
            private BinaryTree? Left;

            /// <summary>
            /// This is the right child node for the binary subtree.
            /// This is null for leaf nodes.
            /// </summary>
            private BinaryTree? Right;

            /// <summary>
            /// This is the parent node for the binary tree. This
            /// is null for the root node.
            /// </summary>
            private BinaryTree? Parent;

            #endregion

            #region Constructors

            /// <summary>
            /// This creates a lead node with a given value.
            /// </summary>
            /// <param name="value">The value.</param>
            public BinaryTree(int value)
            {
                this.Value = value;
            }

            /// <summary>
            /// This creates a binary tree node with two child nodes.
            /// </summary>
            /// <param name="left">The left child.</param>
            /// <param name="right">The right child.</param>
            public BinaryTree(BinaryTree left, BinaryTree right)
            {
                this.Left = left;
                this.Right = right;

                left.Parent = this;
                right.Parent = this;
            }

            #endregion

            #region Public Methods

            /// <summary>
            /// This receives another node, and adds them together.
            /// The new tree is a new root, by attaching the existing
            /// trees as left and right children. Performs a reduce
            /// on the new tree.
            /// </summary>
            /// <param name="otherTree">The other tree.</param>
            /// <returns>The combined tree.</returns>
            public BinaryTree Add(BinaryTree otherTree)
            {
                BinaryTree newTree = new(this, otherTree);
                newTree.Reduce();

                return newTree;
            }

            /// <summary>
            /// Gets the magnitude of the subtree. This is either the node
            /// value for leaf nodes, or a product of the left and right child
            /// nodes.
            /// </summary>
            /// <returns>The magnitude.</returns>
            public int GetMagnitude()
            {
                if (IsLeafNode())
                {
                    return Value;
                }
                else
                {
                    return 3 * Left!.GetMagnitude() + 2 * Right!.GetMagnitude();
                }
            }

            /// <summary>
            /// Converts the subtree back to an easily readable format.
            /// </summary>
            /// <returns>The string representation.</returns>
            public override string ToString()
            {
                if (IsLeafNode())
                {
                    return Value.ToString();
                }

                string result = "[";
                result += Left!.ToString();
                result += ",";
                result += Right!.ToString();
                result += "]";

                return result;
            }

            #endregion

            #region Private Methods

            /// <summary>
            /// Finds any values beyond a certain depth, and "explodes" them.
            /// This removes them from the tree, and adds the values to the
            /// neighbouring nodes in tree traversal order.
            /// </summary>
            /// <param name="depth">The current depth.</param>
            /// <returns>True if nodes were changed.</returns>
            private bool Explode(int depth = 0)
            {
                if (depth >= 4 && IsLeafPair())
                {
                    var leftValue = Left!.Value;
                    var previousNode = GetNextNode(ChildType.Left);
                    if (previousNode != null)
                    {
                        previousNode.Value += leftValue;
                    }

                    var rightValue = Right!.Value;
                    var nextNode = GetNextNode(ChildType.Right);
                    if (nextNode != null)
                    {
                        nextNode.Value += rightValue;
                    }

                    Left = null;
                    Right = null;
                    Value = 0;

                    return true;
                }

                if (Left != null && Left.Explode(depth + 1))
                {
                    return true;
                }

                if (Right != null && Right.Explode(depth + 1))
                {
                    return true;
                }

                return false;
            }

            /// <summary>
            /// Finds either the previous or next node in order of tree traversal. Can
            /// be null if we are at the left or rightmost leaf nodes.
            /// </summary>
            /// <param name="direction">The direction (e.g. left or right) to traverse.</param>
            /// <returns>The next node.</returns>
            private BinaryTree? GetNextNode(ChildType direction)
            {
                var next = this;

                bool foundHighestParent = false;
                while (next != null && !foundHighestParent)
                {
                    var current = next;
                    next = next.Parent;

                    if (next != null)
                    {
                        var parentSibling = direction == ChildType.Right ? next.Right : next.Left;
                        if (next != null && parentSibling != current)
                        {
                            foundHighestParent = true;
                        }
                    }
                }

                if (next == null)
                {
                    return null;
                }

                if (direction == ChildType.Left)
                {
                    next = next.Left;
                }
                else
                {
                    next = next.Right;
                }

                while (next != null && !next!.IsLeafNode())
                {
                    if (direction == ChildType.Left)
                    {
                        next = next.Right;
                    }
                    else
                    {
                        next = next.Left;
                    }
                }

                return next;
            }

            /// <summary>
            /// Checks if a node is a leaf node.
            /// </summary>
            /// <returns>True if this is a leaf node.</returns>
            private bool IsLeafNode() => Left == null && Right == null;

            /// <summary>
            /// Checks if this is a leaf pair - i.e. any node where both
            /// children are leaf nodes.
            /// </summary>
            /// <returns></returns>
            private bool IsLeafPair() => !IsLeafNode() && Left!.IsLeafNode() && Right!.IsLeafNode();

            /// <summary>
            /// Reduce the tree. This applies explide and split operations until
            /// the node can't be reduced further.
            /// </summary>
            private void Reduce()
            {
                while (true)
                {
                    if (Explode())
                    {
                        continue;
                    }

                    if (!Split())
                    {
                        break;
                    }
                }
            }

            /// <summary>
            /// This finds the first value that is larger than 9, and
            /// splits it into two values that are half the sum.
            /// </summary>
            /// <returns>True if a split was performed.</returns>
            private bool Split()
            {
                if (Left != null && Left.Split())
                {
                    return true;
                }

                if (Right != null && Right.Split())
                {
                    return true;
                }

                if (Left == null && Right == null && Value > 9)
                {
                    int leftValue = Value / 2;
                    Left = new(leftValue)
                    {
                        Parent = this
                    };

                    int rightValue = (int)(Value / 2.0f + 0.5f);
                    Right = new(rightValue)
                    {
                        Parent = this
                    };

                    Value = 0;

                    return true;
                }

                return false;
            }

            #endregion
        }

        /// <summary>
        /// Reads a number from input, and converts it into a binary tree.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="position">The current read position.</param>
        /// <returns>The root node of the binary tree.</returns>
        private static BinaryTree ReadNumber(string input, ref int position)
        {
            if (char.IsDigit(input[position]))
            {
                char value = input[position];
                position++;

                return new(value - '0');
            }

            Debug.Assert(input[position] == '[');
            position++;

            var left = ReadNumber(input, ref position);

            Debug.Assert(input[position] == ',');
            position++;

            var right = ReadNumber(input, ref position);

            Debug.Assert(input[position] == ']');
            position++;

            return new(left, right);
        }

        /// <summary>
        /// Reads a number from input, and converts it into a binary tree.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="position">The current read position.</param>
        /// <returns>The root node of the binary tree.</returns>
        private static BinaryTree ReadNumber(string input)
        {
            int position = 0;
            return ReadNumber(input, ref position);
        }

        /// <summary>
        /// Reads a list of numbers from input, adds them together
        /// one after the other, and returns the magnitude of the result.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The magnitude of the result.</returns>
        private static int GetMagnitudeSum(String path)
        {
            var lines = System.IO.File.ReadLines(path)
                .Select(ReadNumber);

            var first = lines.First();
            var tree = lines.Skip(1)
                .Aggregate(first, (a, b) => a.Add(b));

            return tree.GetMagnitude();
        }

        /// <summary>
        /// Reads a list of numbers from the input, and adds each combination
        /// together. Returns the magnitude of the largest combination.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The largest magnitude.</returns>
        private static int GetMaxMagnitudeCombo(String path)
        {
            List<string> lines = System.IO.File.ReadLines(path).ToList();

            var trees = ListExtension.GetPairs(lines)
                .Select((x) => ReadNumber(x.Item1).Add(ReadNumber(x.Item2)));

            return trees.Max(x => x.GetMagnitude());
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(4140, GetMagnitudeSum("Day18/Example.txt"));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(4132, GetMagnitudeSum("Day18/Input.txt"));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(3993, GetMaxMagnitudeCombo("Day18/Example.txt"));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(4685, GetMaxMagnitudeCombo("Day18/Input.txt"));

        #endregion
    }
}
