using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AOC2020
{
    /// <summary>
    /// Solution for day 18:
    /// https://adventofcode.com/2020/day/18
    /// </summary>
    [TestClass]
    public class Day18
    {
        /// <summary>
        /// A class that solves an aritmetic expression including numbers,
        /// multiplication, addition and parentheses.
        /// </summary>
        public abstract class Solver
        {
            #region Public Methods

            /// <summary>
            /// Takes an input string and solves it. An example is:
            /// 1 + (2 * 3) + (4 * (5 + 6))
            /// </summary>
            /// <param name="problemString">The input string</param>
            /// <returns>The solution.</returns>
            public long Solve(string problemString)
            {
                // We pad the parentheses with spaces to make
                // Them easier to parse. That means we just
                // need to use split to get the parentheses
                // as tokens.
                var problem = problemString
                    .Replace("(", "( ")
                    .Replace(")", " )")
                    .Split()
                    .ToList();

                return Solve(problem);
            }

            #endregion

            #region Protected Abstract Methods

            /// <summary>
            /// This function takes a list of numbers and
            /// operators, and evaluates it based on an
            /// implementation-specicic set of rules.
            /// </summary>
            /// <param name="problem">The input tokens</param>
            /// <returns>The result</returns>
            protected abstract long Evaluate(List<string> problem);

            #endregion

            #region Protected Methods

            /// <summary>
            /// This function takes a list of tokens, and
            /// solves them.
            /// </summary>
            /// <param name="problem">The list of tokens</param>
            /// <returns>The result</returns>
            protected long Solve(List<string> problem)
            {
                // First, we replace the parentheses by solving them
                // individually.
                ReplaceParens(ref problem);

                // Now we evaluate the raw additions and multiplications.
                return Evaluate(problem);
            }

            /// <summary>
            /// This takes an operator at the specified index, solves
            /// it, and replaces the sub-expression wiht the result.
            /// </summary>
            /// <param name="problem">The tokens.</param>
            /// <param name="index">The operator index.</param>
            protected void ReplaceOperatorAt(ref List<string> problem, int index)
            {
                var firstIndex = index - 1;
                var secondIndex = index + 1;

                var opertor = problem[index];
                var first = long.Parse(problem[firstIndex]);
                var second = long.Parse(problem[secondIndex]);

                // Remove the sub-expression. This will be two operator, and
                // the two numbers on either side.
                problem.RemoveRange(firstIndex, 3);

                // Replace this with the result of the operation.
                if (opertor == "+")
                {
                    problem.Insert(firstIndex, Convert.ToString(first + second));
                }
                else
                {
                    problem.Insert(firstIndex, Convert.ToString(first * second));
                }
            }

            /// <summary>
            /// Finds matching pairs of parentheses, and solves the sub-expression,
            /// and replaces the sub-expression with the result.
            /// </summary>
            /// <param name="problem">The input problem.</param>
            protected void ReplaceParens(ref List<string> problem)
            {
                while (problem.Contains("("))
                {
                    // Find the start and end position of matching parentheses.
                    // These will need to be balanced, so we find the correct
                    // matching pair.
                    
                    var startPosition = problem.IndexOf("(");
                    var endPosition = startPosition;

                    int parenCount = 1;
                    while (parenCount != 0)
                    {
                        ++endPosition;

                        if (problem[endPosition] == "(")
                        {
                            ++parenCount;
                        }
                        else if (problem[endPosition] == ")")
                        {
                            --parenCount;
                        }
                    }

                    // Extract the sub-problem and solve it.

                    var subProblem = problem.GetRange(startPosition + 1, endPosition - startPosition - 1);
                    long result = Solve(subProblem);

                    // Now replace the sub-problem with the result.

                    problem.RemoveRange(startPosition, endPosition - startPosition + 1);
                    problem.Insert(startPosition, Convert.ToString(result));
                }
            }

            #endregion
        }

        /// <summary>
        /// This solves an arithmetic expression by ignoring operator
        /// precedence.
        /// </summary>
        public class LinearSolver : Solver
        {
            #region Protected Override Methods

            protected override long Evaluate(List<string> problem)
            {
                // This always takes the first operator, until none
                // are left.

                while (problem.Count != 1)
                {
                    ReplaceOperatorAt(ref problem, 1);
                }

                return long.Parse(problem[0]);
            }

            #endregion
        }

        /// <summary>
        /// Solves the arithmetic problem by applying operator
        /// precedence. In this case, all '+' operators take
        /// precedence over '*' operators.
        /// </summary>
        public class PrecedenceSolver : Solver
        {
            #region Protected Override Methods

            protected override long Evaluate(List<string> problem)
            {
                // Find each '+' operator, solve it and replace it
                while (problem.Contains("+"))
                {
                    var index = problem.IndexOf("+");
                    ReplaceOperatorAt(ref problem, index);
                }

                // Find each '*' operator, solve it and replace it
                while (problem.Contains("*"))
                {
                    var index = problem.IndexOf("*");
                    ReplaceOperatorAt(ref problem, index);
                }

                return long.Parse(problem[0]);
            }

            #endregion
        }

        public long GetSolution1(String path)
        {
            var input = System.IO.File.ReadLines(path);

            var solver = new LinearSolver();
            return input.Sum(x => (long)solver.Solve(x));
        }

        public long GetSolution2(String path)
        {
            var input = System.IO.File.ReadLines(path);

            var solver = new PrecedenceSolver();
            return input.Sum(x => (long)solver.Solve(x));
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1()
        {
            var solver = new LinearSolver();

            Assert.AreEqual(71, solver.Solve("1 + 2 * 3 + 4 * 5 + 6"));
            Assert.AreEqual(51, solver.Solve("1 + (2 * 3) + (4 * (5 + 6))"));
            Assert.AreEqual(26, solver.Solve("2 * 3 + (4 * 5)"));
            Assert.AreEqual(437, solver.Solve("5 + (8 * 3 + 9 + 3 * 4 * 3)"));
            Assert.AreEqual(12240, solver.Solve("5 * 9 * (7 * 3 * 3 + 9 * 3 + (8 + 6 * 4))"));
            Assert.AreEqual(13632, solver.Solve("((2 + 4 * 9) * (6 + 9 * 8 + 6) + 6) + 2 + 4 * 2"));
        }

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(1890866893020, GetSolution1("AOC2020/Day18/Input.txt"));

        [TestMethod]
        public void SolveExample2()
        {
            var solver = new PrecedenceSolver();

            Assert.AreEqual(231, solver.Solve("1 + 2 * 3 + 4 * 5 + 6"));
            Assert.AreEqual(51, solver.Solve("1 + (2 * 3) + (4 * (5 + 6))"));
            Assert.AreEqual(46, solver.Solve("2 * 3 + (4 * 5)"));
            Assert.AreEqual(1445, solver.Solve("5 + (8 * 3 + 9 + 3 * 4 * 3)"));
            Assert.AreEqual(669060, solver.Solve("5 * 9 * (7 * 3 * 3 + 9 * 3 + (8 + 6 * 4))"));
            Assert.AreEqual(23340, solver.Solve("((2 + 4 * 9) * (6 + 9 * 8 + 6) + 6) + 2 + 4 * 2"));
        }

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(34646237037193, GetSolution2("AOC2020/Day18/Input.txt"));

        #endregion
    }
}
