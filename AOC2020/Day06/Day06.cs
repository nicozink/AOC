using Microsoft.VisualStudio.TestTools.UnitTesting;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AOC2020
{
    /// <summary>
    /// Solution for day 6:
    /// https://adventofcode.com/2020/day/6
    /// </summary>
    [TestClass]
    public class Day06
    {
        /// <summary>
        /// Reads each group of answers from the input.
        /// Each group of answers is seperated by a blank
        /// newline.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <returns>The grouped answers.</returns>
        IEnumerable<List<String>> ReadGroupAnswers(String path)
        {
            List<String> groupAnswers = new List<string>();

            var input = System.IO.File.ReadLines(path);
            foreach (var line in input)
            {
                if (line == "")
                {
                    yield return groupAnswers;

                    groupAnswers.Clear();
                }
                else
                {
                    groupAnswers.Add(line);
                }
            }

            yield return groupAnswers;
        }

        /// <summary>
        /// Reads the distint answers for each group. These are
        /// all of the answers that anyone in the group answered.
        /// </summary>
        /// <param name="path">The data for each group.</param>
        /// <returns>The distinct answers.</returns>
        IEnumerable<String> ReadDistinctGroupAnswers(String path)
        {
            var groupedAnswers = ReadGroupAnswers(path);

            foreach (var groupedAnswer in groupedAnswers)
            {
                var allAnswers = "";

                groupedAnswer.ForEach(answer => {
                    allAnswers += answer;
                });

                var distinctCharacters = allAnswers.Distinct().OrderBy(x => x);
                yield return new String(distinctCharacters.ToArray());
            }
        }

        /// <summary>
        /// Reads the intersected answers for each group. These are
        /// all of the answers that everyone in the group answered.
        /// </summary>
        /// <param name="path">The data for each group.</param>
        /// <returns>The distinct answers.</returns>
        IEnumerable<String> ReadIntersectedGroupAnswers(String path)
        {
            var groupedAnswers = ReadGroupAnswers(path);

            foreach (var groupedAnswer in groupedAnswers)
            {
                var allAnswers = groupedAnswer.First();

                groupedAnswer.ForEach(answer => {
                    var commonCharacters = allAnswers.Intersect(answer).OrderBy(x => x);
                    allAnswers = new String(commonCharacters.ToArray());
                });

                yield return allAnswers;
            }
        }

        public int GetSolution1(String path)
        {
            var answers = ReadDistinctGroupAnswers(path);
            return answers.Sum(x => x.Length);
        }

		public int GetSolution2(String path)
        {
            var answers = ReadIntersectedGroupAnswers(path);
            return answers.Sum(x => x.Length);
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(11, GetSolution1("Day06/Example.txt"));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(6633, GetSolution1("Day06/Input.txt"));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(6, GetSolution2("Day06/Example.txt"));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(3202, GetSolution2("Day06/Input.txt"));

        #endregion
    }
}
