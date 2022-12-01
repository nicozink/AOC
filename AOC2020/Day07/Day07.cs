using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AOC2020
{
    using BagRules = List<Tuple<String, int>>;

    /// <summary>
    /// Solution for day 7:
    /// https://adventofcode.com/2020/day/7
    /// </summary>
    [TestClass]
    public class Day07
    {
        /// <summary>
        /// Reads the bag rules from a text file. Example rule would be:
        /// vibrant gray bags contain 3 plaid orange bags, 2 dotted teal bags.
        /// </summary>
        /// <param name="path">The input file.</param>
        /// <returns>The bag rules.</returns>
        Dictionary<String, BagRules> ReadBagRules(String path)
        {
            var rules = new Dictionary<String, BagRules>();

            foreach (var line in System.IO.File.ReadAllLines(path))
            {
                var processedLine = line.Replace(" bags contain ", ":")
                    .Replace(" bag, ", ",").Replace(" bags, ", ",")
                    .Replace(" bag.", "").Replace(" bags.", "")
                    .Replace("no other", "1 no other");

                var ruleDefinition = processedLine.Split(':');

                var ruleName = ruleDefinition[0];
                var ruleContents = new List<Tuple<String, int>>();

                foreach (var singleRule in ruleDefinition[1].Split(','))
                {
                    var singleRuleName = singleRule.Substring(2);
                    var singleRuleValue = Int32.Parse(singleRule.Substring(0, 1));

                    ruleContents.Add(new Tuple<string, int>(singleRuleName, singleRuleValue));
                }
                
                rules.Add(ruleName, ruleContents);
            }

            return rules;
        }

        /// <summary>
        /// For a given bag, return all bags that contain it.
        /// This includes indirectly, so this function searches
        /// recursively.
        /// </summary>
        /// <param name="rules">The bag rules.</param>
        /// <param name="bagName">The bag name.</param>
        /// <returns>The bag names.</returns>
        private HashSet<String> GetValidBagsFor(Dictionary<string, BagRules> rules, string bagName)
        {
            HashSet<String> validStrings = new HashSet<string>();

            foreach (var keyValuePair in rules)
            {
                if (keyValuePair.Value.Count(x => x.Item1 == bagName) != 0)
                {
                    validStrings.Add(keyValuePair.Key);
                }
            }

            foreach (var validString in validStrings.ToList())
            {
                validStrings.UnionWith(GetValidBagsFor(rules, validString));
            }

            return validStrings;
        }

        /// <summary>
        /// Gets the total number of bags contained in a given bag.
        /// This number includes the given bag.
        /// </summary>
        /// <param name="rules">The bag rules.</param>
        /// <param name="bagName">The bag name.</param>
        /// <returns>The number of bags.</returns>
        private int CountBagsRequiredFor(Dictionary<string, BagRules> rules, string bagName)
        {
            if (bagName == "no other")
            {
                return 0;
            }

            var rule = rules[bagName];

            int bagCount = 1;

            foreach (var ruleItem in rule)
            {
                bagCount += CountBagsRequiredFor(rules, ruleItem.Item1) * ruleItem.Item2;
            }

            return bagCount;
        }

        public int GetSolution1(String path)
        {
            var rules = ReadBagRules(path);

            var validBags = GetValidBagsFor(rules, "shiny gold");

            return validBags.Count;
        }

        public int GetSolution2(String path)
        {
            var rules = ReadBagRules(path);

            var bagsRequired = CountBagsRequiredFor(rules, "shiny gold");

            return bagsRequired - 1;
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(4, GetSolution1("AOC2020/Day07/Example1.txt"));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(185, GetSolution1("AOC2020/Day07/Input.txt"));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(126, GetSolution2("AOC2020/Day07/Example2.txt"));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(89084, GetSolution2("AOC2020/Day07/Input.txt"));

        #endregion
    }
}
