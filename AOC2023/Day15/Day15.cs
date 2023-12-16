using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Emit;

namespace AOC2023
{
    /// <summary>
    /// Solution for day 15:
    /// https://adventofcode.com/2023/day/15
    /// </summary>
    [TestClass]
    public class Day15
    {
        /// <summary>
        /// Concerts a string into a hash according the the custom hashing algorithm.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>The output hash.</returns>
        private static int CalculateHash(string input)
        {
            int value = 0;

            foreach (var ch in input)
            {
                value += ch;
                value *= 17;
                value %= 256;
            }

            return value;
        }

        /// <summary>
        /// Evaluate a sequence by adding the sum of the hashed parts.
        /// </summary>
        /// <param name="input">The path to the input file.</param>
        /// <returns>The sum of the hashes.</returns>
        private static int EvaluateSequence(string input)
        {
            int sum = 0;

            var lines = System.IO.File.ReadAllText(input);
            var steps = lines.Split(',');
            foreach (var step in steps)
            {
                sum += CalculateHash(step);
            }

            return sum;
        }

        /// <summary>
        /// Stores a lens which is a lable and trhe focal length.
        /// </summary>
        /// <param name="Name">The name of the lens.</param>
        /// <param name="FocalLength">The focal length of the lens.</param>
        record Lens(string Name, int FocalLength);

        /// <summary>
        /// Gets the instruction describing the step.
        /// </summary>
        /// <param name="step">The input string.</param>
        /// <returns>The label and focal length (or -1 for removal).</returns>
        private static Lens GetStep(string step)
        {            
            if (step.Contains('-'))
            {
                var label = step.TrimEnd('-');
                return new(label, -1);
            }
            else
            {
                var delimited = step.Split('=');
                var label = delimited[0];
                var focalLength = int.Parse(delimited[1]);
                return new(label, focalLength);
            }
        }

        /// <summary>
        /// Calculate the focus power of the sequence by arranging the lenses into boxes.
        /// </summary>
        /// <param name="input">The path to the input file.</param>
        /// <returns>The focus power.</returns>
        private static int CalculateFocusPower(string input)
        {
            var boxes = Enumerable.Range(0, 256)
                .Select(x => new List<Lens>())
                .ToArray();

            var lines = System.IO.File.ReadAllText(input);
            var steps = lines.Split(',')
                .Select(x => GetStep(x));
            foreach (var newLens in steps)
            {
                var box = CalculateHash(newLens.Name);
                
                var lenses = boxes[box];
                var lens = lenses.FirstOrDefault(x => x.Name == newLens.Name);

                if (newLens.FocalLength != -1 && lens == null)
                {
                    lenses.Add(newLens);
                }
                else if (newLens.FocalLength != -1 && lens != null)
                {
                    var lensIndex = lenses.IndexOf(lens);
                    lenses[lensIndex] = newLens;
                }
                else if (lens != null)
                {
                    lenses.Remove(lens);
                }
            }

            int sum = 0;
            for (int boxIndex = 0; boxIndex < boxes.Length; boxIndex++)
            {
                for (int lensIndex = 0; lensIndex < boxes[boxIndex].Count; lensIndex++)
                {
                    var power = (boxIndex + 1) * (lensIndex + 1) * boxes[boxIndex][lensIndex].FocalLength;
                    sum += power;
                }
            }

            return sum;
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(52, CalculateHash("HASH"));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(1320, EvaluateSequence("AOC2023/Day15/Example.txt"));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(517315, EvaluateSequence("AOC2023/Day15/Input.txt"));

        [TestMethod]
        public void SolveExample3() => Assert.AreEqual(145, CalculateFocusPower("AOC2023/Day15/Example.txt"));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(247763, CalculateFocusPower("AOC2023/Day15/Input.txt"));


        #endregion
    }
}
