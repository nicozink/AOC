using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace AOC2022
{
    /// <summary>
    /// Solution for day 21:
    /// https://adventofcode.com/2022/day/21
    /// </summary>
    [TestClass]
    public class Day21
    {
        private static Dictionary<string, string> ReadInput(string path)
        {
            var lines = System.IO.File.ReadLines(path);

            var splitLines = lines
                .Select(x => x.Split(": "));

            return splitLines.ToDictionary(x => x[0], y => y[1]);
        }

        private static void ReadEquation(string monkeyString, out string num1, out string op, out string num2)
        {
            var splitString = monkeyString.Split(" ");
            num1 = splitString[0];
            op = splitString[1];
            num2 = splitString[2];
        }

        private static bool TryGetValue(Dictionary<string, string> monkeys, string name, out long value)
        {
            if (!monkeys.TryGetValue(name, out var monkeyString))
            {
                value = 0;
                return false;
            }

            if (long.TryParse(monkeyString, out value))
            {
                return true;
            }

            ReadEquation(monkeyString, out string name1, out var operation, out string name2);
            if (TryGetValue(monkeys, name1, out long num1)
                && TryGetValue(monkeys, name2, out long num2))
            {
                value = operation switch
                {
                    "+" => num1 + num2,
                    "-" => num1 - num2,
                    "*" => num1 * num2,
                    "/" => num1 / num2,
                    _ => throw new Exception("Invalid operation")
                };

                return true;
            }

            return false;
        }

        private static long GetRootValue(string path)
        {
            var input = ReadInput(path);
            TryGetValue(input, "root", out long value);
            return value;
        }

        private static long GetBalancedInput(Dictionary<string, string> monkeys, string name, long expected)
        {
            if (!monkeys.TryGetValue(name, out var monkeyString))
            {
                return expected;
            }

            long newExpected = 0;
            string nextChild = "";

            ReadEquation(monkeyString, out string name1, out var operation, out string name2);
            if (TryGetValue(monkeys, name1, out long value))
            {
                newExpected = operation switch
                {
                    "+" => expected - value,
                    "-" => value - expected,
                    "*" => expected / value,
                    "/" => value / expected,
                    _ => throw new Exception("Invalid operation")
                };

                nextChild = name2;
            }
            else if (TryGetValue(monkeys, name2, out value))
            {
                newExpected = operation switch
                {
                    "+" => expected - value,
                    "-" => expected + value,
                    "*" => expected / value,
                    "/" => expected * value,
                    _ => throw new Exception("Invalid operation")
                };

                nextChild = name1;
            }

            return GetBalancedInput(monkeys, nextChild, newExpected);
        }

        private static long GetHumanValue(string path)
        {
            var input = ReadInput(path);
            input.Remove("humn");

            var monkeyString = input["root"];
            ReadEquation(monkeyString, out string name1, out _, out string name2);

            if (TryGetValue(input, name1, out long value))
            {
                return GetBalancedInput(input, name2, value);
            }
            else if (TryGetValue(input, name2, out value))
            {
                return GetBalancedInput(input, name1, value);
            }

            throw new Exception("Cannot find value.");
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(152, GetRootValue("AOC2022/Day21/Example.txt"));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(84244467642604, GetRootValue("AOC2022/Day21/Input.txt"));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(301, GetHumanValue("AOC2022/Day21/Example.txt"));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(3759569926192, GetHumanValue("AOC2022/Day21/Input.txt"));

        #endregion
    }
}
