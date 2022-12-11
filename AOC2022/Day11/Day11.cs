using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace AOC2022
{
    /// <summary>
    /// Solution for day 11:
    /// https://adventofcode.com/2022/day/11
    /// </summary>
    [TestClass]
    public class Day11
    {
        /// <summary>
        /// Switch for the two solution parts.
        /// </summary>
        enum Solution
        {
            /// <summary>
            /// Execute the solution for part 1.
            /// </summary>
            Part1,

            /// <summary>
            /// Execute the solution for part 2.
            /// </summary>
            Part2
        };

        /// <summary>
        /// Stores the parameters for a monkey, and the conditions
        /// under which it throws items to different monkeys.
        /// </summary>
        class Monkey
        {
            /// <summary>
            /// The items which the monkey is currently holding.
            /// </summary>
            public Queue<long> Items = new();

            /// <summary>
            /// The operation which is applied to the worry score
            /// for each item. Can be a product or sum.
            /// </summary>
            public char Operation;

            /// <summary>
            /// The valuw which is used to modify the worry score for
            /// each item. This can be a number, or "old" to re-use the
            /// current value.
            /// </summary>
            public string OperationValue = "";

            /// <summary>
            /// The value used to test the divisibility of the worry score.
            /// </summary>
            public int TestValue;

            /// <summary>
            /// The monkey which receives the item if the divisibility score
            /// passes.
            /// </summary>
            public int TestTrue;

            /// <summary>
            /// The monkey which receives the item if the divisibility score
            /// fails.
            /// </summary>
            public int TestFalse;
        }

        /// <summary>
        /// Reads the list of monkeys from the input.
        /// </summary>
        /// <param name="path">The path containing the monkey data.</param>
        /// <returns>The monkey data.</returns>
        private static IEnumerable<Monkey> ReadInput(string path)
        {
            // Example input:
            // Monkey 0:
            //   Starting items: 79, 98
            //   Operation: new = old * 19
            //   Test: divisible by 23
            //     If true: throw to monkey 2
            //     If false: throw to monkey 3

            var lines = System.IO.File.ReadAllLines(path);

            for (int i = 0; i < lines.Length; i += 7)
            {
                var startingItems = lines[i + 1]
                    .Replace("  Starting items: ", "")
                    .Split(", ")
                    .Select(x => long.Parse(x));

                var operationString = lines[i + 2]
                    .Replace("  Operation: new = old ", "")
                    .Split()
                    .ToArray();

                var operation = operationString[0][0];
                var operationValue = operationString[1];

                var testValueString = lines[i + 3]
                    .Replace("  Test: divisible by ", "");
                var testValue = int.Parse(testValueString);

                var testTrueString = lines[i + 4]
                    .Replace("    If true: throw to monkey ", "");
                var testTrue = int.Parse(testTrueString);

                var testFalseString = lines[i + 5]
                    .Replace("    If false: throw to monkey ", "");
                var testFalse = int.Parse(testFalseString);

                yield return new Monkey()
                {
                    Items = new(startingItems),
                    Operation = operation,
                    OperationValue = operationValue,
                    TestValue = testValue,
                    TestTrue = testTrue,
                    TestFalse = testFalse
                };
            }
        }

        /// <summary>
        /// Calculate the monkey business from the monkey input based
        /// on the solution type.
        /// </summary>
        /// <param name="path">The file containing the monkey data.</param>
        /// <param name="solution">The solution type.</param>
        /// <returns>The monkey business value.</returns>
        private static long GetLevelMonkeyBusiness(string path, Solution solution)
        {
            var monkeys = ReadInput(path)
                .ToArray();

            // We calculate the product of test values to limit the worry value for part 2.
            // See the caluclation further down.
            var monkeyProduct = monkeys.Aggregate(1, (prod, next) => prod * next.TestValue);

            // Keep track of the number of inspeactions for each monkey to calculate the result.
            var inspectionCount = new long[monkeys.Length];

            // For part 1, we calculate 20 steps, and 10k for part 2.
            int numSteps = (solution == Solution.Part1) ? 20 : 10000;
            for (int i = 0; i < numSteps; i++)
            {
                for (int monkeyIndex = 0; monkeyIndex < monkeys.Length; monkeyIndex++)
                {
                    var monkey = monkeys[monkeyIndex];
                    inspectionCount[monkeyIndex] += monkey.Items.Count;

                    while (monkey.Items.Count > 0)
                    {
                        long worryLevel = monkey.Items.Dequeue();

                        // Work out the modifier. If the value specifies "old", we use the current
                        // worry level. Otherwise, we parse the operation value.
                        long worryModifier = worryLevel;
                        if (monkey.OperationValue != "old")
                        {
                            worryModifier = long.Parse(monkey.OperationValue);
                        }

                        // Apply the modifier based on whether we perform a product or a sum.
                        if (monkey.Operation == '+')
                        {
                            worryLevel += worryModifier;
                        }
                        else
                        {
                            worryLevel *= worryModifier;
                        }

                        if (solution == Solution.Part1)
                        {
                            worryLevel /= 3; // Part 1 divides by three to limit the worry level.
                        }
                        else
                        {
                            worryLevel %= monkeyProduct; // Part 2 doesn't perform the division. Since we only care
                                                         // about the divisibility, we limit the level by performing
                                                         // a modulo with the product of all divisibility checks.
                                                         // This guarantees that we still get the correct result.
                        }

                        int newMonkeyIndex = monkey.TestFalse;
                        if (worryLevel % monkey.TestValue == 0)
                        {
                            newMonkeyIndex = monkey.TestTrue;

                        }

                        var newMonkey = monkeys[newMonkeyIndex];
                        newMonkey.Items.Enqueue(worryLevel);
                    }
                }
            }

            // Return the top two inspection counts.
            var topInspections = inspectionCount
                .OrderByDescending(x => x)
                .Take(2)
                .ToArray();
            return topInspections[0] * topInspections[1];
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(10605, GetLevelMonkeyBusiness("AOC2022/Day11/Example.txt", Solution.Part1));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(57838, GetLevelMonkeyBusiness("AOC2022/Day11/Input.txt", Solution.Part1));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(2713310158, GetLevelMonkeyBusiness("AOC2022/Day11/Example.txt", Solution.Part2));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(15050382231, GetLevelMonkeyBusiness("AOC2022/Day11/Input.txt", Solution.Part2));

        #endregion
    }
}
