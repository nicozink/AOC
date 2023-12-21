using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;

namespace AOC2023
{
    /// <summary>
    /// Solution for day 19:
    /// https://adventofcode.com/2023/day/19
    /// </summary>
    [TestClass]
    public class Day19
    {
        /// <summary>
        /// Stores a comparison which is used to evaluate rules against ratings.
        /// </summary>
        private enum Comparison
        {
            LessThan,
            GreaterThan
        }

        /// <summary>
        /// A rule which is used to evaluate ratings for the parts.
        /// </summary>
        /// <param name="Index">The index of the rating, which is either x, m, a or s.</param>
        /// <param name="Comparison">The comparison method.</param>
        /// <param name="Value">The value to compare against.</param>
        /// <param name="Destination">The destination if the comparison was true.</param>
        record Rule(int Index, Comparison Comparison, int Value, string Destination);

        /// <summary>
        /// A range holding a min and max value.
        /// </summary>
        /// <param name="Min">The minimum value.</param>
        /// <param name="Max">The maximum value.</param>
        record Range(int Min, int Max)
        {
            /// <summary>
            /// Generate a new min/max pair based on evaluating a rule and the expected result.
            /// </summary>
            /// <param name="comparison">The comparison which needs to be performed.</param>
            /// <param name="value">The value to compare against.</param>
            /// <param name="isTrue">Whether the range includes true results.</param>
            /// <returns>The new range.</returns>
            public Range Evaluate(Comparison comparison, int value, bool isTrue)
            {
                if (comparison == Comparison.LessThan && isTrue)
                {
                    var newMax = Math.Min(value - 1, Max);
                    return this with { Max = newMax };
                }
                else if (comparison == Comparison.LessThan)
                {
                    var newMin = Math.Max(Min, value);
                    return this with { Min = newMin };
                }
                else if (comparison == Comparison.GreaterThan && isTrue)
                {
                    var newMin = Math.Max(value + 1, Min);
                    return this with { Min = newMin };
                }
                else
                {
                    var newMax = Math.Min(value, Max);
                    return this with { Max = newMax };
                }
            }
        }

        /// <summary>
        /// Reads the workflows from the input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="i">The index which is modified and points to the empty line in the input.</param>
        /// <returns>The workflows.</returns>
        private Dictionary<string, Rule[]> ReadWorkflows(string[] input, ref int i)
        {
            var workflows = new Dictionary<string, Rule[]>();

            while (!string.IsNullOrEmpty(input[i]))
            {
                var splitRules = input[i]
                    .Replace('{', ',')
                    .Trim('}')
                    .Split(',');

                var rules = new List<Rule>();
                for (int ruleIndex = 1; ruleIndex < splitRules.Length; ruleIndex++)
                {
                    var ruleString = splitRules[ruleIndex];

                    bool lessThan = ruleString.Contains('<');
                    bool greaterThan = ruleString.Contains('>');
                    if (lessThan || greaterThan)
                    {
                        var compareChar = lessThan ? '<' : '>';
                        var ruleSplit = ruleString.Split(compareChar, ':');

                        var index = (ruleSplit[0][0]) switch
                        {
                            'x' => 0,
                            'm' => 1,
                            'a' => 2,
                            _ => 3
                        };
                        var comparison = lessThan ? Comparison.LessThan : Comparison.GreaterThan;
                        var value = int.Parse(ruleSplit[1]);
                        var destination = ruleSplit[2];

                        var rule = new Rule(index, comparison, value, destination);
                        rules.Add(rule);
                    }
                    else
                    {
                        // For the last rule, we just add a custom rule that is always true - since
                        // values are always positive, we just add a check for greater than zero.
                        var rule = new Rule(0, Comparison.GreaterThan, 0, ruleString);
                        rules.Add(rule);
                    }
                }

                var ruleName = splitRules[0];
                workflows.Add(ruleName, rules.ToArray());

                i++;
            }

            return workflows;
        }

        /// <summary>
        /// Read the ratings from the input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="i">The start line of the ratings.</param>
        /// <returns>The ratings.</returns>
        private int[][] ReadRatings(string[] input, int i)
        {
            var ratings = new List<int[]>();

            for (; i < input.Length; i++)
            {
                var line = input[i];

                var formatted = line
                    .Replace("{x=", "")
                    .Replace(",m=", ",")
                    .Replace(",a=", ",")
                    .Replace(",s=", ",")
                    .Trim('}');

                var split = formatted
                    .Split(',')
                    .Select(int.Parse)
                    .ToArray();

                var rating = new int[] { split[0], split[1], split[2], split[3] };
                ratings.Add(rating);
            }

            return ratings.ToArray();
        }

        /// <summary>
        /// Counts the number of combinations that are present in the ranges.
        /// </summary>
        /// <param name="xmasBounds">The ranges of values.</param>
        /// <returns>The number of combinations.</returns>
        private long CountCombinations(Range[] xmasBounds)
        {
            long product = 1;
            foreach (var (min, max) in xmasBounds)
            {
                long count = max - min + 1;
                product *= count;
            }

            return product;
        }

        /// <summary>
        /// Count the number of combinations that are given by the workflows for a particular range.
        /// </summary>
        /// <param name="workflows">The workflows.</param>
        /// <param name="xmasBounds">The range.</param>
        /// <param name="ruleName">The name of the rule to start evaluating.</param>
        /// <returns>The number of combinations.</returns>
        private long CountCombinations(Dictionary<string, Rule[]> workflows, Range[] xmasBounds, string ruleName)
        {
            if (ruleName == "A")
            {
                return CountCombinations(xmasBounds);
            }

            if (ruleName == "R")
            {
                return 0;
            }

            var combinations = 0L;
            xmasBounds = xmasBounds.ToArray(); // Make a copy so we can safely modify it below when being called recursively.

            var rules = workflows[ruleName];
            foreach (var rule in rules)
            {
                var range = xmasBounds[rule.Index];

                Range trueRange = range.Evaluate(rule.Comparison, rule.Value, true);
                Range falseRange = range.Evaluate(rule.Comparison, rule.Value, false);

                if (trueRange.Min <= trueRange.Max)
                {
                    xmasBounds[rule.Index] = trueRange;
                    combinations += CountCombinations(workflows, xmasBounds, rule.Destination);
                    xmasBounds[rule.Index] = range;
                }

                if (falseRange.Min > falseRange.Max)
                {
                    break;
                }

                xmasBounds[rule.Index] = falseRange;
            }

            return combinations;
        }

        /// <summary>
        /// Solve the ratings from the input file by evaluating the workflow for each one.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The sum of the ratings that are accepted.</returns>
        private long SolveRatings(string path)
        {
            var input = System.IO.File.ReadAllLines(path);

            int i = 0;
            var workflows = ReadWorkflows(input, ref i);

            i++;
            var ratings = ReadRatings(input, i);

            long sum = 0;
            foreach (var rating in ratings)
            {
                var xmasBounds = new Range[]
                {
                    new(rating[0], rating[0]),
                    new(rating[1], rating[1]),
                    new(rating[2], rating[2]),
                    new(rating[3], rating[3])
                };

                var combinations = CountCombinations(workflows, xmasBounds, "in");
                if (combinations == 1)
                {
                    sum += rating.Sum();
                }
            }

            return sum;
        }

        /// <summary>
        /// Solve the combinations for the range of values by evaluating the workflows.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The number of combinations.</returns>
        private long SolveCombinations(string path)
        {
            var input = System.IO.File.ReadAllLines(path);

            int i = 0;
            var workflows = ReadWorkflows(input, ref i);

            var xmasBounds = new Range[]
            {
                new(1, 4000),
                new(1, 4000),
                new(1, 4000),
                new(1, 4000)
            };

            return CountCombinations(workflows, xmasBounds, "in");
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(19114, SolveRatings("AOC2023/Day19/Example.txt"));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(472630, SolveRatings("AOC2023/Day19/Input.txt"));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(167409079868000, SolveCombinations("AOC2023/Day19/Example.txt"));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(116738260946855, SolveCombinations("AOC2023/Day19/Input.txt"));

        #endregion
    }
}
