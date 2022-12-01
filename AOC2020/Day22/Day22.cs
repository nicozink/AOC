using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AOC2020
{
    /// <summary>
    /// Solution for day 22:
    /// https://adventofcode.com/2020/day/22
    /// </summary>
    [TestClass]
    public class Day22
    {
        /// <summary>
        /// Stores the player cards. We need to keep track of previous
        /// games, so the need to compare and clone them.
        /// </summary>
        private class PlayerCards : ICloneable
        {
            #region ICloneable Functions

            /// <summary>
            /// Constructs a copy of the player cards.
            /// </summary>
            /// <returns>The copy.</returns>
            public object Clone()
            {
                return new PlayerCards()
                {
                    Player1Cards = new List<int>(Player1Cards),
                    Player2Cards = new List<int>(Player2Cards)
                };
            }

            #endregion

            #region Object Methods

            /// <summary>
            /// Checks two sets of player cards to see if they are equal.
            /// </summary>
            /// <param name="obj">The player cards.</param>
            /// <returns>The copy.</returns>
            public override bool Equals(object? obj)
            {
                PlayerCards other = (PlayerCards)obj!;

                return Enumerable.SequenceEqual(Player1Cards, other.Player1Cards) &&
                    Enumerable.SequenceEqual(Player2Cards, other.Player2Cards);
            }

            /// <summary>
            /// Gets the hash code of the player cards.
            /// </summary>
            /// <returns>The hash code.</returns>
            public override int GetHashCode()
            {
                int hash = 17;

                foreach (var card in Player1Cards)
                {
                    hash = hash * 31 + card;
                }

                foreach (var card in Player2Cards)
                {
                    hash = hash * 31 + card;
                }

                return hash;
            }

            #endregion

            #region Internal Variables

            /// <summary>
            /// The playing cards held by the first player.
            /// </summary>
            internal List<int> Player1Cards = new List<int>();

            /// <summary>
            /// The playing cards held by the second player.
            /// </summary>
            internal List<int> Player2Cards = new List<int>();

            #endregion
        }

        /// <summary>
        /// Calculates the score of the winning player.
        /// </summary>
        /// <param name="playerCards">The playing cards.</param>
        /// <returns>The score.</returns>
        private int CalculateScore(PlayerCards playerCards)
        {
            var winningStack = playerCards.Player1Cards;

            if (winningStack.Count == 0)
            {
                winningStack = playerCards.Player2Cards;
            }

            return winningStack
                .Select((value, index) => (winningStack.Count - index) * value)
                .Sum();
        }

        /// <summary>
        /// Reads the cards from the input.
        /// </summary>
        /// <param name="input">The input file.</param>
        /// <returns>The playing cards.</returns>
        private PlayerCards ReadCards(String input)
        {
            var playerCards = new PlayerCards();

            var cardInput = System.IO.File.ReadLines(input).GetEnumerator();

            // Skip past the Player 1 line.
            cardInput.MoveNext();

            while (cardInput.MoveNext() && cardInput.Current != "")
            {
                playerCards.Player1Cards.Add(int.Parse(cardInput.Current));
            }

            // Skip past the Player 2 line.
            cardInput.MoveNext();

            while (cardInput.MoveNext() && cardInput.Current != "")
            {
                playerCards.Player2Cards.Add(int.Parse(cardInput.Current));
            }

            return playerCards;
        }

        /// <summary>
        /// Play a single game of combat.
        /// </summary>
        /// <param name="playerCards">The playing cards at the start of the game.</param>
        /// <returns>The playing cards at the end of the game.</returns>
        private PlayerCards PlayCombat(PlayerCards playerCards)
        {
            // We keep track of the stacks containing the smaller and larger
            // cards on top. This is initalised arbitraarily for now.
            var minStack = playerCards.Player1Cards;
            var maxStack = playerCards.Player2Cards;

            while (minStack.Count != 0 && maxStack.Count != 0)
            {
                // If the minimum and maximum are incorrectly assigned,
                // we correct it now based on the top card.
                if (minStack[0] > maxStack[0])
                {
                    (minStack, maxStack) = (maxStack, minStack);
                }

                // Remove the top cards from both players' stacks.

                var minValue = minStack[0];
                minStack.RemoveAt(0);

                var maxValue = maxStack[0];
                maxStack.RemoveAt(0);

                // Add both to the winner's stack.
                maxStack.Add(maxValue);
                maxStack.Add(minValue);
            }

            return playerCards;
        }

        /// <summary>
        /// Play a game of recursive combat.
        /// </summary>
        /// <param name="playerCards">The starting player cards.</param>
        /// <returns>The players' cards at the end of the game.</returns>
        private PlayerCards PlayRecursiveCombat(PlayerCards playerCards)
        {
            // We keep track of any previous games to avoid recursion.
            HashSet<PlayerCards> history = new HashSet<PlayerCards>()
            {
                (PlayerCards)playerCards.Clone()
            };

            var player1Cards = playerCards.Player1Cards;
            var player2Cards = playerCards.Player2Cards;

            while (player1Cards.Count != 0 && player2Cards.Count != 0)
            {
                // Remove the top cards from each player's hand.

                var player1Value = player1Cards[0];
                player1Cards.RemoveAt(0);

                var player2Value = player2Cards[0];
                player2Cards.RemoveAt(0);

                int winner = 1;
                if (!history.Contains(playerCards))
                {
                    history.Add((PlayerCards)playerCards.Clone());

                    if (player1Cards.Count >= player1Value &&
                        player2Cards.Count >= player2Value)
                    {
                        // If both players have enough cards to play a sub-game recursively,
                        // we do that.

                        // We build a sub-deck containing copies of the cards from each hand
                        // based on the number of the drawn card.
                        var subDeck = new PlayerCards()
                        {
                            Player1Cards = player1Cards.GetRange(0, player1Value),
                            Player2Cards = player2Cards.GetRange(0, player2Value),
                        };

                        // Play recusive combat, and decide the winner of this round.

                        PlayerCards result = PlayRecursiveCombat(subDeck);

                        if (result.Player2Cards.Count != 0)
                        {
                            winner = 2;
                        }
                    }
                    else
                    {
                        // Otherwise, the winner is the player with the higher card.
                        if (player2Value > player1Value)
                        {
                            winner = 2;
                        }
                    }
                }
                
                // The winner gets the drawn cards.
                if (winner == 1)
                {
                    player1Cards.Add(player1Value);
                    player1Cards.Add(player2Value);
                }
                else
                {
                    player2Cards.Add(player2Value);
                    player2Cards.Add(player1Value);
                }
            }

            return playerCards;
        }

        public long GetSolution1(String path)
        {
            var playerCards = ReadCards(path);
            var result = PlayCombat(playerCards);
            var score = CalculateScore(result);

            return score;
        }

        public long GetSolution2(String path)
        {
            var playerCards = ReadCards(path);

            var result = PlayRecursiveCombat(playerCards);
            var score = CalculateScore(result);
            
            return score;
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(306, GetSolution1("AOC2020/Day22/Example.txt"));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(33559, GetSolution1("AOC2020/Day22/Input.txt"));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(291, GetSolution2("AOC2020/Day22/Example.txt"));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(32789, GetSolution2("AOC2020/Day22/Input.txt"));

        #endregion
    }
}
