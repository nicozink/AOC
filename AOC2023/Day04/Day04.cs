using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AOC2023
{
    /// <summary>
    /// Solution for day 4:
    /// https://adventofcode.com/2023/day/4
    /// </summary>
    [TestClass]
    public class Day04
    {
        record ScratchCard(int Game, int[] Number1, int[] Number2);

        /// <summary>
        /// Reads the scratch card info from the input.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The scratch card info.</returns>
        private static IEnumerable<ScratchCard> ReadInput(string path)
        {
            var lines = System.IO.File.ReadAllLines(path);

            foreach (var line in lines)
            {
                var processedLine = line
                    .Replace("  ", " ")
                    .Replace("Card ", "")
                    .Replace(": ", ":")
                    .Replace(" | ", "|");

                var gameDescription = processedLine.Split(':');
                int gameNumber = int.Parse(gameDescription[0]);

                var numbers = gameDescription[1].Split('|');

                var numbers1 = numbers[0]
                    .Split()
                    .Select(int.Parse)
                    .ToArray();

                var numbers2 = numbers[1]
                    .Split()
                    .Select(int.Parse)
                    .ToArray();

                yield return new ScratchCard(gameNumber, numbers1, numbers2);
            }
        }

        /// <summary>
        /// Calculate the number of winning numbers on a scratch card.
        /// </summary>
        /// <param name="scratchCard">The scratch card.</param>
        /// <returns>The number of winning numbers.</returns>
        private int CalculateWinningNumbers(ScratchCard scratchCard)
        {
            var numbers1 = scratchCard.Number1.ToHashSet();
            var numbers2 = scratchCard.Number2.ToHashSet();

            var winningNumbers = numbers1.Intersect(numbers2);

            return winningNumbers.Count();
        }

        /// <summary>
        /// Calculate the points based on the winning numbers on each scratch card.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The sum of points.</returns>
        private int CalculatePoints(string path)
        {
            var scratchCards = ReadInput(path);

            int sum = 0;
            foreach (var scratchCard in scratchCards)
            {
                var winningNumbers = CalculateWinningNumbers(scratchCard);
                if (winningNumbers != 0)
                {
                    sum += (int)Math.Pow(2, winningNumbers - 1);
                }
            }

            return sum;
        }

        /// <summary>
        /// Calculate the number of cards spawned by a particular scratch cards, based on
        /// recursively evaluating the cards based on winning numbers.
        /// </summary>
        /// <param name="scratchCards">The scratch cards.</param>
        /// <param name="index">The index of the card to evaluate.</param>
        /// <param name="lookup">The lookup for winning combinations.</param>
        /// <returns></returns>
        private int CalculateNumberCards(Dictionary<int, ScratchCard> scratchCards, int index, ref Dictionary<int, int> lookup)
        {
            if (lookup.TryGetValue(index, out var value))
            {
                return value;
            }

            var scratchCard = scratchCards[index];
            var winningNumbers = CalculateWinningNumbers(scratchCard);

            int sum = winningNumbers;
            for (int i = 1; i <= winningNumbers; i++)
            {
                var newNumber = index + i;
                sum += CalculateNumberCards(scratchCards, newNumber, ref lookup);
            }

            lookup[index] = sum;
            return sum;
        }

        /// <summary>
        /// Calculate the number of cards after evaluating all winning scratch cards.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The total number of cards.</returns>
        private int CalculateCards(string path)
        {
            var scratchCards = ReadInput(path)
                .ToDictionary(x => x.Game, y => y);
            Dictionary<int, int> lookup = new();

            int sum = scratchCards.Count;
            foreach (var scratchCard in scratchCards.Values)
            {
                sum += CalculateNumberCards(scratchCards, scratchCard.Game, ref lookup);
            }

            return sum;
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(13, CalculatePoints("AOC2023/Day04/Example.txt"));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(24160, CalculatePoints("AOC2023/Day04/Input.txt"));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(30, CalculateCards("AOC2023/Day04/Example.txt"));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(5659035, CalculateCards("AOC2023/Day04/Input.txt"));

        #endregion
    }
}
