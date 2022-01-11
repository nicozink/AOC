using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace AOC2021
{
    internal static class Day08Helper
    {
        /// <summary>
        /// Takes a string and sorts it.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>The sorted string.</returns>
        public static String GetSorted(this String input)
        {
            return String.Concat(input.OrderBy(x => x));
        }

        /// <summary>
        /// Converts a hash set to a sorted string.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>The sorted string.</returns>
        public static String GetSorted(this HashSet<char> input)
        {
            return String.Concat(input.OrderBy(x => x));
        }
    }

    /// <summary>
    /// Solution for day 8:
    /// https://adventofcode.com/2021/day/8
    /// </summary>
    [TestClass]
    public class Day08
    {
        /// <summary>
        /// Receives a list of strings representing digits from 0 to 9. Figures out what
        /// strings represent what digits. Retruns a dictionary of the mappings.
        /// </summary>
        /// <param name="numbers">The list of strings.</param>
        /// <param name="simpleOnly">Whether to only calculate simple mappings based on lengths.</param>
        /// <returns>The mapping from string to digits.</returns>
        private Dictionary<String, int> GetLookup(IEnumerable<String> numbers, bool simpleOnly)
        {
            var hashSets = numbers.Select(x => x.ToHashSet());

            // These are the original mappings with their lengths:
            // 0: abc_efg (6)
            // 1: __c__f_ (2)
            // 2: a_cde_g (5)
            // 3: a_cd_fg (5)
            // 4: _bcd_f_ (4)
            // 5: ab_d_fg (5)
            // 6: ab_defg (6)
            // 7: a_c__f_ (3)
            // 8: abcdefg (7)
            // 9: abcd_fg (6)
            // We need to work out the pattern for the shuffled ones.

            var output = new Dictionary<String, int>();

            // We know 1, 4, 7 and 8 based on their length.

            HashSet<char> one = hashSets.Single(x => x.Count == 2);
            HashSet<char> four = hashSets.Single(x => x.Count == 4);
            HashSet<char> seven = hashSets.Single(x => x.Count == 3);
            HashSet<char> eight = hashSets.Single(x => x.Count == 7);

            output.Add(one.GetSorted(), 1);
            output.Add(four.GetSorted(), 4);
            output.Add(seven.GetSorted(), 7);
            output.Add(eight.GetSorted(), 8);

            if (simpleOnly)
            {
                return output;
            }

            // We can figure out 3, as it contains the characters from 1
            var three = hashSets.Single(x => x.Count == 5 && one.IsSubsetOf(x));
            output.Add(three.GetSorted(), 3);

            // We can find 9 by combining 3 and 4
            var nine = three.Union(four).ToHashSet();
            output.Add(nine.GetSorted(), 9);

            // We can find 2 since it is missing two letters from 9
            var two = hashSets.Single(x => x.Count == 5 && nine.Except(x).Count() == 2);
            output.Add(two.GetSorted(), 2);

            // Find 5 by elimination
            var five = hashSets.Single(x => x.Count == 5 && !x.SetEquals(two) && !x.SetEquals(three));
            output.Add(five.GetSorted(), 5);

            // We can find 0 since it includes 7
            var zero = hashSets.Single(x => x.Count == 6 && !x.SetEquals(nine) && seven.IsSubsetOf(x));
            output.Add(zero.GetSorted(), 0);

            // Find 6 by elimination
            var six = hashSets.Single(x => x.Count == 6 && !x.SetEquals(nine) && !x.SetEquals(zero));
            output.Add(six.GetSorted(), 6);

            return output;
        }

        /// <summary>
        /// Reads the input. Each line consists of ten digits which give the pattern,
        /// and then four values which give a code we need to translate.
        /// </summary>
        /// <param name="path">The input file.</param>
        /// <returns>The pattern and code.</returns>
        IEnumerable<(String[] pattern, String[] code)> ReadInput(String path)
        {
            var lines = System.IO.File.ReadAllLines(path);

            foreach (var line in lines)
            {
                var split = line.Split('|');

                var pattern = split[0].Split()
                    .Where(x => !String.IsNullOrEmpty(x))
                    .Select(x => x.GetSorted());

                var code = split[1].Split()
                    .Where(x => !String.IsNullOrEmpty(x))
                    .Select(x => x.GetSorted());

                yield return (pattern.ToArray(), code.ToArray());
            }
        }

        /// <summary>
        /// For each line in the input, this calculates the
        /// number of times each line contains simple numbers,
        /// which can be identified based on the length.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The number of simple matches.</returns>
        int CountEasyNumbers(String path)
        {
            var input = ReadInput(path);

            int count = 0;
            foreach (var (pattern, code) in input)
            {
                var lookup = GetLookup(pattern, simpleOnly: true);
                count += code.Count(x => lookup.ContainsKey(x));
            }

            return count;
        }

        /// <summary>
        /// Gets the sum of numbers from the code on each line by
        /// translating the pattern.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The sum of the numbers.</returns>
        private int AddNumbers(String path)
        {
            var input = ReadInput(path);

            int sum = 0;
            foreach (var (pattern, code) in input)
            {
                var lookup = GetLookup(pattern, simpleOnly: false);

                for (int i = 0; i < code.Length; i++)
                {
                    int value = lookup[code[i]];
                    int tmp = (int)Math.Pow(10, (code.Length - i - 1 ));
                    sum += value * tmp;
                }
                
            }

            return sum;
        }

        #region Solve Problems
        
        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(26, CountEasyNumbers("Day08/Example.txt"));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(504, CountEasyNumbers("Day08/Input.txt"));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(61229, AddNumbers("Day08/Example.txt"));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(1073431, AddNumbers("Day08/Input.txt"));

        #endregion
    }
}
