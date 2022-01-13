using Microsoft.VisualStudio.TestTools.UnitTesting;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AOC2015
{
    /// <summary>
    /// Solution for day 7:
    /// https://adventofcode.com/2015/day/7
    /// </summary>
    [TestClass]
    public class Day07
    {
        /// <summary>
        /// Stores a binary operation with one or two values, and an operand.
        /// </summary>
        /// <param name="LHS">The left-hand side.</param>
        /// <param name="Op">The operand.</param>
        /// <param name="RHS">The right-hand side.</param>
        record Operation(string LHS, string? Op = null, string? RHS = null);

        /// <summary>
        /// Reads the list of operands from the file.
        /// </summary>
        /// <param name="path">The file containing the operands.</param>
        /// <returns>The operands.</returns>
        Dictionary<String, Operation> ReadInput(String path)
        {
            Dictionary<String, Operation> result = new();

            var lines = System.IO.File.ReadLines(path);
            foreach (var line in lines)
            {
                var split1 = line.Replace(" -> ", ",")
                    .Split(',');

                var symbol = split1[1];
                var rule = split1[0];

                var split2 = rule.Split();

                if (split2.Length == 1)
                {
                    result.Add(symbol, new(split2[0]));
                }
                else if (split2.Length == 2)
                {
                    result.Add(symbol, new(split2[1], split2[0]));
                }
                else
                {
                    result.Add(symbol, new(split2[0], split2[1], split2[2]));
                }
            }

            return result;
        }

        /// <summary>
        /// Solve the operands. Variables are replaced with values. Each operand that
        /// contains values is evaluated. 
        /// </summary>
        /// <param name="values">The operations.</param>
        void Solve(Dictionary<string, Operation> values)
        {
            bool hasReduced = true;
            while (hasReduced)
            {
                hasReduced = false;

                foreach (var (symbol, rule) in values)
                {
                    // Check if the LHS has a variable that can be replaced with a value.

                    if (char.IsLetter(rule.LHS[0]) && values[rule.LHS].Op == null)
                    {
                        values[symbol] = rule with
                        {
                            LHS = values[rule.LHS].LHS
                        };

                        hasReduced = true;

                        continue;
                    }

                    // Check if the RHS has a variable that can be replaced with a value.

                    if (rule.RHS != null && char.IsLetter(rule.RHS[0]) && values[rule.RHS].Op == null)
                    {
                        values[symbol] = rule with
                        {
                            RHS = values[rule.RHS].LHS
                        };

                        hasReduced = true;

                        continue;
                    }

                    // If we find a operation with values, evaluate it.

                    bool hasLeftNumber = char.IsNumber(rule.LHS[0]);
                    bool hasRightNumber = rule.RHS != null && char.IsNumber(rule.RHS[0]);

                    if (hasLeftNumber && hasRightNumber)
                    {
                        UInt16 leftNumber = UInt16.Parse(rule.LHS);
                        UInt16 rightNumber = UInt16.Parse(rule.RHS!);

                        int result = rule.Op switch
                        {
                            "AND" => leftNumber & rightNumber,
                            "LSHIFT" => leftNumber << rightNumber,
                            "OR" => leftNumber | rightNumber,
                            "RSHIFT" => leftNumber >> rightNumber,
                            _ => throw new()
                        };

                        values[symbol] = new("" + result);

                        hasReduced = true;
                    }
                    else if (hasLeftNumber && rule.Op == "NOT")
                    {
                        UInt16 leftNumber = UInt16.Parse(rule.LHS);
                        int result = (UInt16)~leftNumber;

                        values[symbol] = new("" + result);

                        hasReduced = true;
                    }
                }
            }
        }

        int SolveValueA(string path)
        {
            var values = ReadInput(path);
            Solve(values);

            return int.Parse(values["a"].LHS);
        }

        int SolveLoopedValueA(string path)
        {
            int valueA = SolveValueA(path);

            var values = ReadInput(path);
            values["b"] = new("" + valueA);
            Solve(values);

            return int.Parse(values["a"].LHS);
        }

        #region Solve Problems

        [TestMethod]
        public void TestExample()
        {
            var values = ReadInput("Day07/Example.txt");

            Solve(values);

            Assert.AreEqual("72", values["d"].LHS);
            Assert.AreEqual("507", values["e"].LHS);
            Assert.AreEqual("492", values["f"].LHS);
            Assert.AreEqual("114", values["g"].LHS);
            Assert.AreEqual("65412", values["h"].LHS);
            Assert.AreEqual("65079", values["i"].LHS);
            Assert.AreEqual("123", values["x"].LHS);
            Assert.AreEqual("456", values["y"].LHS);
        }

        [TestMethod]
        public void TestSolution1() => Assert.AreEqual(956, SolveValueA("Day07/Input.txt"));

        [TestMethod]
        public void TestSolution2() => Assert.AreEqual(40149, SolveLoopedValueA("Day07/Input.txt"));

        #endregion
    }
}
