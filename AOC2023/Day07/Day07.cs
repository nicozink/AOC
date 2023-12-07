using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AOC2023
{
    /// <summary>
    /// Solution for day 7:
    /// https://adventofcode.com/2023/day/7
    /// </summary>
    [TestClass]
    public class Day07
    {
        /// <summary>
        /// Stores a hand which is a set of cards, and a numerical bid.
        /// </summary>
        /// <param name="Cards">The set of cards.</param>
        /// <param name="Bid">The bid value.</param>
        private record Hand(string Cards, int Bid);

        private enum HandType
        {
            FiveOfAKind, // where all five cards have the same label: AAAAA
            FourOfAKind, // where four cards have the same label and one card has a different label: AA8AA
            FullHouse, // where three cards have the same label, and the remaining two cards share a different label: 23332
            ThreeOfAKind, // where three cards have the same label, and the remaining two cards are each different from any other card in the hand: TTT98
            TwoPair, // where two cards share one label, two other cards share a second label, and the remaining card has a third label: 23432
            OnePair, // where two cards share one label, and the other three cards have a different label from the pair and each other: A23A4
            HighCard // where all cards' labels are distinct: 23456
        }

        /// <summary>
        /// Evaluate a hand to find the hand type.
        /// </summary>
        /// <param name="hand">The hand containing the cards.</param>
        /// <returns>The type of the hand.</returns>
        private static HandType EvaluateHand(string hand)
        {
            // Count the number of jokers, and remove them from the hand.
            int jokers = hand.Count(x => x == '*');
            hand = hand.Replace("*", "");

            // Group the counts by type, and order them so we have the most common cards first.
            var cardCount = hand.GroupBy(x => x)
                .Select(x => x.Count())
                .OrderByDescending(x => x)
                .ToList();

            // Add the jokers back as the most common card, but consider that we may have a hand full
            // of jokers - and therefore an empty hand.
            if (cardCount.Count > 0)
            {
                cardCount[0] += jokers;
            }
            else
            {
                cardCount.Add(jokers);
            }

            // Find the hand type.
            if (cardCount.Count == 1)
            {
                return HandType.FiveOfAKind;
            }
            else if (cardCount[0] == 4)
            {
                return HandType.FourOfAKind;
            }
            else if (cardCount[0] == 3 && cardCount[1] == 2)
            {
                return HandType.FullHouse;
            }
            else if (cardCount[0] == 3)
            {
                return HandType.ThreeOfAKind;
            }
            else if (cardCount[0] == 2 && cardCount[1] == 2)
            {
                return HandType.TwoPair;
            }
            else if (cardCount[0] == 2)
            {
                return HandType.OnePair;
            }
            else
            {
                return HandType.HighCard;
            }
        }

        /// <summary>
        /// Reads the hands from the input file.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <param name="replaceJokerWildcard">Whether to interpret jokers as wildcards.</param>
        /// <returns>The hands.</returns>
        private static IEnumerable<Hand> ReadInput(string path, bool replaceJokerWildcard)
        {
            var lines = System.IO.File.ReadAllLines(path);
            foreach (var line in lines)
            {
                var hand = line.Split();

                var cards = hand[0];
                if (replaceJokerWildcard)
                {
                    cards = cards.Replace("J", "*");
                }

                var bid = int.Parse(hand[1]);
                
                yield return new(cards, bid);
            }
        }

        /// <summary>
        /// Compare two hands to determine which one is stronger.
        /// </summary>
        /// <param name="left">The left side of the comparison.</param>
        /// <param name="right">The right side of the comparison.</param>
        /// <returns>An integer which determines which side is stronger.</returns>
        private static int CompareEntries(Hand left, Hand right)
        {
            var leftType = EvaluateHand(left.Cards);
            var rightType = EvaluateHand(right.Cards);

            if (leftType < rightType)
            {
                return 1;
            }
            else if (leftType > rightType)
            {
                return -1;
            }

            var order = "AKQJT98765432*";
            for (int i = 0; i < left.Cards.Length; i++)
            {
                var leftChar = left.Cards[i];
                var rightChar = right.Cards[i];

                var leftStrength = order.IndexOf(leftChar);
                var rightStrength = order.IndexOf(rightChar);

                if (leftStrength < rightStrength)
                {
                    return 1;
                }
                else if (leftStrength > rightStrength)
                {
                    return -1;
                }
            }

            return 0;
        }

        /// <summary>
        /// Orders the hands by strength, and then uses the bids to calculate the winnings.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <param name="useJokerRule">Whether to interpret jokes as wildcards.</param>
        /// <returns>The total winnings.</returns>
        private static long CalculateTotalWinnings(string path, bool useJokerRule)
        {
            var hands = ReadInput(path, useJokerRule).ToList();
            hands.Sort(CompareEntries);

            long winnings = 0;
            for (int i = 0; i < hands.Count; i++)
            {
                winnings += (i + 1) * hands[i].Bid;
            }

            return winnings;
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(6440, CalculateTotalWinnings("AOC2023/Day07/Example.txt", useJokerRule: false));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(250898830, CalculateTotalWinnings("AOC2023/Day07/Input.txt", useJokerRule: false));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(5905, CalculateTotalWinnings("AOC2023/Day07/Example.txt", useJokerRule: true));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(252127335, CalculateTotalWinnings("AOC2023/Day07/Input.txt", useJokerRule: true));

        #endregion
    }
}
