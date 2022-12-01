using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AOC2020
{
    /// <summary>
    /// Solution for day 19:
    /// https://adventofcode.com/2020/day/19
    /// </summary>
    [TestClass]
    public class Day19
    {
        /// <summary>
        /// Stores a rule. This is an abstract class, which is inherited
        /// for specific rule types.
        /// </summary>
        private abstract class Rule
        {

        }

        /// <summary>
        /// Stores a rule token, which is a character - either 'a' or 'b'
        /// in the sample input.
        /// </summary>
        private class RuleToken : Rule
        {
            public char Value;
        }

        /// <summary>
        /// Stores a sequence of rules. Rules can be expanded to either other
        /// sequences, or individual characters.
        /// </summary>
        private class RuleSequence : Rule
        {
            public List<int> Nodes = new();
        }

        /// <summary>
        /// Implements a message parser that recursively checks a message against a rule.
        /// </summary>
        private class MessageParser
        {
            #region Constructors

            /// <summary>
            /// Initialises a new message parser. Reads the messages and rules form a file.
            /// </summary>
            /// <param name="path">The input file.</param>
            internal MessageParser(String path)
            {
                var input = System.IO.File.ReadAllLines(path)
                    .ToList();

                rules = ReadRules(input);
                messages = ReadMessages(input).ToList();
            }

            #endregion

            #region Internal Methods

            /// <summary>
            /// Counts the number of valid methods according to the rules.
            /// </summary>
            /// <returns>The number of valid messages.</returns>
            internal int CountValidMessages()
            {
                return messages.Count(x => MatchesRule(x, 0));
            }

            /// <summary>
            /// Replaces two rules according to the instructions for part two of the
            /// solution.
            /// </summary>
            internal void ReplaceRules()
            {
                rules[8] = new List<Rule>
                {
                    new RuleSequence{ Nodes = new List<int>{ 42 } },
                    new RuleSequence{ Nodes = new List<int>{ 42, 8 } }
                };

                rules[11] = new List<Rule>
                {
                    new RuleSequence{ Nodes = new List<int>{ 42, 31 } },
                    new RuleSequence{ Nodes = new List<int>{ 42, 11, 31 } }
                };
            }

            #endregion

            #region Private Methods

            /// <summary>
            /// Consumes a single character, and returns the new position (as
            /// an enumerator to match other consume methods).
            /// </summary>
            /// <param name="message">The message.</param>
            /// <param name="rule">The rule.</param>
            /// <param name="position">The current position.</param>
            /// <returns>The next position.</returns>
            private IEnumerable<int> ConsumeRule(string message, RuleToken rule, int position)
            {
                if (position >= message.Length)
                {
                    yield break;
                }

                var expected = rule.Value;

                if (message[position] == expected)
                {
                    yield return position + 1;
                }
            }

            /// <summary>
            /// Consumes a sequence of rules. This is done recursively, so
            /// that we get all combinations of rules.
            /// </summary>
            /// <param name="message">The message.</param>
            /// <param name="rule">The rule.</param>
            /// <param name="ruleIndex">The index of the current rule to ckeck.</param>
            /// <param name="position">The current position in the message.</param>
            /// <returns>The new positions.</returns>
            private IEnumerable<int> ConsumeRule(string message, RuleSequence rule, int ruleIndex, int position)
            {
                // We exit if we've read each part of the rule.
                if (ruleIndex == rule.Nodes.Count)
                {
                    yield return position;
                    yield break;
                }

                // We read the first token, and get all valid positions once the first token has
                // been read.
                foreach (var firstPosition in ConsumeRule(message, rule.Nodes[ruleIndex], position))
                {
                    // We then call this function recursively, to read the next token.
                    foreach (var secondPosition in ConsumeRule(message, rule, ruleIndex + 1, firstPosition))
                    {
                        yield return secondPosition;
                    }
                }
            }

            /// <summary>
            /// Consumes a single rule. This can either be a character matching rule, or
            /// a sequence of sub-rules.
            /// </summary>
            /// <param name="message">The message.</param>
            /// <param name="rule">The rule.</param>
            /// <param name="position">The current position in the message.</param>
            /// <returns>The next positions.</returns>
            private IEnumerable<int> ConsumeRule(string message, Rule rule, int position)
            {
                if (rule is RuleToken)
                {
                    var token = (RuleToken)rule;

                    foreach (var newPosition in ConsumeRule(message, token, position))
                    {
                        yield return newPosition;
                    }
                }
                else
                {
                    var sequence = (RuleSequence)rule;

                    foreach (var newPosition in ConsumeRule(message, sequence, 0, position))
                    {
                        yield return newPosition;
                    }
                }
            }

            /// <summary>
            /// Consumes a rule based on the rule index. Each rule can have a number of
            /// alternative rules, so this function checks each one.
            /// </summary>
            /// <param name="message">The message.</param>
            /// <param name="ruleIndex">The rule index.</param>
            /// <param name="position">The current position.</param>
            /// <returns>The next positions.</returns>
            private IEnumerable<int> ConsumeRule(string message, int ruleIndex, int position)
            {
                foreach (var rule in rules[ruleIndex])
                {
                    foreach (var newPosition in ConsumeRule(message, rule, position))
                    {
                        yield return newPosition;
                    }
                }
            }

            /// <summary>
            /// Checks whether a certain message matches the provided rule.
            /// </summary>
            /// <param name="message">The message.</param>
            /// <param name="rule">The rule.</param>
            /// <returns>True if the message is valid.</returns>
            private bool MatchesRule(string message, int rule)
            {
                foreach (var position in ConsumeRule(message, 0, 0))
                {
                    if (position == message.Length)
                    {
                        return true;
                    }
                }

                return false;
            }

            /// <summary>
            /// Reads the messages from the input.
            /// </summary>
            /// <param name="input">The input.</param>
            /// <returns>The messages.</returns>
            private IEnumerable<string> ReadMessages(List<string> input)
            {
                var split = input.ToList().IndexOf("");

                return input.GetRange(split + 1, input.Count - split - 1);
            }

            /// <summary>
            /// Reads a specific rule from the input. The rule is given in the format:
            /// 80: 116 116 | 119 134
            /// Where the '|' character seperates alternative sub-rules.
            /// </summary>
            /// <param name="input">The input.</param>
            /// <returns>The list of sub-rules.</returns>
            private List<Rule> ReadRule(string input)
            {
               var subRules = input
                    .Split(new[] { " | " }, StringSplitOptions.None)
                    .Select(x => x.Split().ToList())
                    .ToList();

                var rules = new List<Rule>();

                foreach (var subRule in subRules)
                {
                    if (subRule.Count == 1 && subRule[0].Contains("\""))
                    {
                        var rule = new RuleToken()
                        {
                            Value = subRule[0][1]
                        };

                        rules.Add(rule);
                    }
                    else
                    {
                        var rule = new RuleSequence()
                        {
                            Nodes = subRule
                                .Select(x => int.Parse(x))
                                .ToList()
                        };

                        rules.Add(rule);
                    }
                }

                return rules;
            }

            /// <summary>
            /// Reads the rules from the input.
            /// </summary>
            /// <param name="input">The input.</param>
            /// <returns>The rules.</returns>
            private Dictionary<int, List<Rule>> ReadRules(List<string> input)
            {
                var split = input.ToList().IndexOf("");

                var rules = new Dictionary<int, List<Rule>>();

                foreach (var ruleString in input.GetRange(0, split))
                {
                    var ruleDefinitionString = ruleString.Split(new[] { ": " }, StringSplitOptions.None);

                    var ruleName = ruleDefinitionString[0];
                    var ruleDefinition = ruleDefinitionString[1];

                    rules.Add(int.Parse(ruleName), ReadRule(ruleDefinition));
                }

                return rules;
            }

            #endregion

            #region Private Variables

            /// <summary>
            /// Contains the messages which are then checked against the rules.
            /// </summary>
            List<string> messages;

            /// <summary>
            /// Contains the rules used to check the messages.
            /// </summary>
            Dictionary<int, List<Rule>> rules;

            #endregion
        }        

        public int GetSolution1(String path)
        {
            var parser = new MessageParser(path);

            return parser.CountValidMessages();
        }

        public int GetSolution2(String path)
        {
            var parser = new MessageParser(path);

            parser.ReplaceRules();

            return parser.CountValidMessages();
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(2, GetSolution1("AOC2020/Day19/Example1.txt"));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(134, GetSolution1("AOC2020/Day19/Input.txt"));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(12, GetSolution2("AOC2020/Day19/Example2.txt"));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(377, GetSolution2("AOC2020/Day19/Input.txt"));

        #endregion
    }
}
