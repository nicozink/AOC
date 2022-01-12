using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AOC2020
{
    /// <summary>
    /// Some extension methods for a linked list to support
    /// circular lookup.
    /// </summary>
    static class CircularLinkedList
    {
        /// <summary>
        /// Gets the next value in the list, or the first value
        /// when at the end of the list.
        /// </summary>
        /// <typeparam name="T">The type stored in the list.</typeparam>
        /// <param name="current">The current node.</param>
        /// <returns>The next node.</returns>
        public static LinkedListNode<T> CircularNext<T>(this LinkedListNode<T> current)
        {
            return current.Next ?? current.List!.First!;
        }

        /// <summary>
        /// Iterates through the nodes in the linked list.
        /// </summary>
        /// <typeparam name="T">The type stored in the list.</typeparam>
        /// <param name="current">The linked list.</param>
        /// <returns>The nodes.</returns>
        public static IEnumerable<LinkedListNode<T>> GetNodes<T>(this LinkedList<T> list)
        {
            var nextNode = list.First;

            while (nextNode != null)
            {
                yield return nextNode;

                nextNode = nextNode.Next;
            }
        }
    }

    /// <summary>
    /// Solution for day 23:
    /// https://adventofcode.com/2020/day/23
    /// </summary>
    [TestClass]
    public class Day23
    {
        /// <summary>
        /// Gets the cups from the string. If more cups
        /// are requested than given in the input, these
        /// are generated from subsequent numbers starting
        /// from the maximum.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="numCups">The number of cups.</param>
        /// <returns>The cups.</returns>
        private LinkedList<int> GetCups(string input, int numCups)
        {
            // Reads a list of the first nodes by converting from char to int.

            var cups = input
                .Select(x => (int)(x - '0'))
                .ToList();

            // Then fill the remaining positions starting after the maximum
            // value.

            int nextValue = cups.Max() + 1;

            for (int i = cups.Count; i < numCups; ++i)
            {
                cups.Add(nextValue);
                ++nextValue;
            }

            return new LinkedList<int>(cups);
        }

        /// <summary>
        /// Takes the cups, and applies the number of moves to them.
        /// </summary>
        /// <param name="cups">The cups.</param>
        /// <param name="moves">The number of moves.</param>
        private void MakeMoves(LinkedList<int> cups, int moves)
        {
            // We create a lookup so that we can easily find the
            // destination node.
            var nodeLookup = cups
                .GetNodes()
                .ToDictionary(x => x.Value, x => x);

            // We need the min and max values to find the destination
            // node - as that starts from the current node, decreasing,
            // and wraps around.
            int minValue = cups.Min();
            int maxValue = cups.Max();

            var current = cups.First!;
            for (int i = 0; i < moves; ++i)
            {
                // We pick the next three nodes, which
                // will be moved to the destination.

                var pickUp = new int[3];

                var next = current.CircularNext();
                for (int j = 0; j < 3; ++j)
                {
                    pickUp[j] = next.Value;
                    next = next.CircularNext();
                }

                // Now we pick the destination by decreasing
                // until we find one that we're not picking.

                var destinationValue = current.Value;
                
                do
                {
                    --destinationValue;

                    if (destinationValue < minValue)
                    {
                        destinationValue = maxValue;
                    }
                }
                while (pickUp.Contains(destinationValue));

                var destination = nodeLookup[destinationValue];

                // Now we move the next three nodes from the current
                // location to the destination.
                for (int j = 0; j < 3; ++j)
                {
                    var currentNext = current.CircularNext();
                    cups.Remove(currentNext);

                    cups.AddAfter(destination, currentNext);

                    destination = currentNext;
                }

                current = current.CircularNext();
            }
        }

        /// <summary>
        /// Gets the full sequence of cups. We start at the position
        /// of the cup labelled '1', and get all subsequent ones.
        /// </summary>
        /// <param name="cups">The cups.</param>
        /// <returns>The sequence of cups.</returns>
        public IEnumerable<int> GetFullSequence(LinkedList<int> cups)
        {
            var firstResult = cups.Find(1)!;
            var next = firstResult.CircularNext();

            while (firstResult != next)
            {
                yield return next.Value;
                next = next.CircularNext();
            }
        }

        /// <summary>
        /// Takes an input string, applies the number of moves,
        /// and returns the result sequence.
        /// </summary>
        /// <param name="input">The inoput.</param>
        /// <param name="moves">The number of moves.</param>
        /// <returns>The output sequence.</returns>
        public string GetSolution1(String input, int moves)
        {
            var cups = GetCups(input, input.Length);
            MakeMoves(cups, moves);

            var result = GetFullSequence(cups)
                .Select(x => (char)('0' + x));

            return new string(result.ToArray());
        }

        /// <summary>
        /// Gets the input, and pads it up to a million
        /// cups. Applies ten million moves, and returns
        /// the product of the first two values.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>The product of the first two cups.</returns>
        public long GetSolution2(String input)
        {
            var cups = GetCups(input, 1000000);
            MakeMoves(cups, 10000000);

            long product = 1;
            foreach (var cup in GetFullSequence(cups).Take(2))
            {
                product *= cup;
            }

            return product;
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1()
        {
            var result10 = GetSolution1("389125467", 10);
            Assert.AreEqual("92658374", result10);

            var result100 = GetSolution1("389125467", 100);
            Assert.AreEqual("67384529", result100);
        }

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual("26354798", GetSolution1("284573961", 100));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(149245887792, GetSolution2("389125467"));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(166298218695, GetSolution2("284573961"));

        #endregion
    }
}
