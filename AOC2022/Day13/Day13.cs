using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace AOC2022
{
    /// <summary>
    /// Solution for day 13:
    /// https://adventofcode.com/2022/day/13
    /// </summary>
    [TestClass]
    public class Day13
    {
        /// <summary>
        /// Base class for a node in a list - which can either be a value,
        /// or a list.
        /// </summary>
        class Node
        {

        }

        /// <summary>
        /// A value node that stores an integer.
        /// </summary>
        class NodeValue : Node
        {
            public int Value { get; }

            public NodeValue(int value)
            {
                Value = value;
            }
        }

        /// <summary>
        /// This node consists of a list of sub-nodes.
        /// </summary>
        class NodeList : Node
        {
            public List<Node> Items { get; }

            public NodeList(List<Node> items)
            {
                Items = items;
            }
        }

        /// <summary>
        /// Try to read the given character from the position. Advances the
        /// position if it is read.
        /// </summary>
        /// <param name="str">The string to parse.</param>
        /// <param name="expected">The expected character.</param>
        /// <param name="position">The position.</param>
        /// <returns>True if the character was read.</returns>
        private static bool TryReadChar(string str, char expected, ref int position)
        {
            var ch = str[position];
            if (ch != expected)
            {
                return false;
            }

            position++;
            return true;
        }

        /// <summary>
        /// Read a character from the position. Advances the position if it is read, and
        /// throws an exception otherwise.
        /// </summary>
        /// <param name="str">The string to parse.</param>
        /// <param name="expected">The expected character.</param>
        /// <param name="position">The position.</param>
        /// <exception cref="Exception">Thrown when the character is not read.</exception>
        private static void ReadChar(string str, char expected, ref int position)
        {
            var ch = str[position];
            if (ch != expected)
            {
                throw new Exception($"Expected {expected}, received {ch}");
            }

            position++;
        }

        /// <summary>
        /// Reads a digit from the input. Returns false if the digit can't be read.
        /// </summary>
        /// <param name="str">The input string.</param>
        /// <param name="position">The position.</param>
        /// <param name="value">The output value.</param>
        /// <returns>True if the digit was parsed.</returns>
        private static bool ReadDigit(string str, ref int position, out int value)
        {
            int digitLength = 0;
            while (char.IsDigit(str[position + digitLength]))
            {
                digitLength++;
            }

            if (digitLength != 0)
            {
                var subStr = str.Substring(position, digitLength);
                value = int.Parse(subStr);
                position += digitLength;

                return true;
            }
            else
            {
                value = 0;
                return false;
            }
        }

        /// <summary>
        /// Parse a node from the input at the position. Advances the position
        /// if it was read.
        /// </summary>
        /// <param name="str">The input text.</param>
        /// <param name="position">The position.</param>
        /// <param name="node">The output node.</param>
        /// <returns>True if parsed.</returns>
        private static bool ParseNode(string str, ref int position, out Node node)
        {
            if (ReadDigit(str, ref position, out int value))
            {
                node = new NodeValue(value);
                return true;
            }
            else if (TryReadChar(str, '[', ref position))
            {
                var nodes = new List<Node>();
                while (true)
                {
                    if (ParseNode(str, ref position, out Node childNode))
                    {
                        nodes.Add(childNode);
                    }

                    if (!TryReadChar(str, ',', ref position))
                    {
                        break;
                    }
                }

                ReadChar(str, ']', ref position);

                node = new NodeList(nodes);
                return true;
            }

            node = new NodeValue(0);
            return false;
        }

        /// <summary>
        /// Parse a node from the input.
        /// </summary>
        /// <param name="str">The input.</param>
        /// <returns>The node.</returns>
        /// <exception cref="Exception">Thrown if the input can't be parsed.</exception>
        private static Node ParseNode(string str)
        {
            int position = 0;
            if (ParseNode(str, ref position, out Node node))
            {
                return node;
            }
            throw new Exception("Could not parse node.");
        }

        /// <summary>
        /// Read the input from the file.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The parsed nodes.</returns>
        private static IEnumerable<Node> ReadInput(string path)
        {
            var lines = System.IO.File.ReadAllLines(path);

            for (int i = 0; i < lines.Length; i+= 3)
            {
                yield return ParseNode(lines[i]);
                yield return ParseNode(lines[i + 1]);
            }
        }

        /// <summary>
        /// Compare two nodes for equality according to the rules given.
        /// </summary>
        /// <param name="node1"></param>
        /// <param name="node2"></param>
        /// <returns></returns>
        private static int Compare(Node node1, Node node2)
        {
            if (node1 is NodeValue value1 && node2 is NodeValue value2)
            {
                var comparer = Comparer<int>.Default;
                return comparer.Compare(value1.Value, value2.Value);
            }
            else if (node1 is NodeList list1 && node2 is NodeList list2)
            {
                var max = Math.Min(list1.Items.Count, list2.Items.Count);
                for (int i = 0; i < max; i++)
                {
                    var childNode1 = list1.Items[i];
                    var childNode2 = list2.Items[i];

                    var compare = Compare(childNode1, childNode2);
                    if (compare != 0)
                    {
                        return compare;
                    }
                }

                var comparer = Comparer<int>.Default;
                return comparer.Compare(list1.Items.Count, list2.Items.Count);
            }
            else
            {
                if (node1 is not NodeList nodeList1)
                {
                    nodeList1 = new NodeList(new() { node1 });
                }

                if (node2 is not NodeList nodeList2)
                {
                    nodeList2 = new NodeList(new() { node2 });
                }

                return Compare(nodeList1, nodeList2);
            }
        }

        /// <summary>
        /// Solve the first problem - consider pairs of nodes,
        /// and decide which ones are in the correct order.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The sum of the indices.</returns>
        private static int SolvePart1(string path)
        {
            var pairs = ReadInput(path)
                .ToList();

            int sum = 0;
            int index = 0;
            for (int i = 0; i < pairs.Count; i+=2)
            {
                index++;
                int compare = Compare(pairs[i], pairs[i + 1]);
                if (compare < 0)
                {
                    sum += index;
                }
            }

            return sum;
        }

        /// <summary>
        /// Solve part 2. Consider the whole list of nodes. Insert some dividers, sort
        /// the list, and get the product of the divider locations.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The product of the dividers.</returns>
        private static int SolvePart2(string path)
        {
            var pairs = ReadInput(path)
                .ToList();

            var divider1 = ParseNode("[[2]]");
            pairs.Add(divider1);

            var divider2 = ParseNode("[[6]]");
            pairs.Add(divider2);

            pairs.Sort((x, y) => Compare(x, y));

            int index1 = pairs.IndexOf(divider1);
            int index2 = pairs.IndexOf(divider2);
            return (index1 + 1) * (index2 + 1);
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(13, SolvePart1("AOC2022/Day13/Example.txt"));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(6484, SolvePart1("AOC2022/Day13/Input.txt"));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(140, SolvePart2("AOC2022/Day13/Example.txt"));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(19305, SolvePart2("AOC2022/Day13/Input.txt"));

        #endregion
    }
}
