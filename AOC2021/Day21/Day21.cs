using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Common;

namespace AOC2021
{
    /// <summary>
    /// Solution for day 21:
    /// https://adventofcode.com/2021/day/21
    /// </summary>
    [TestClass]
    public class Day21
    {
        /// <summary>
        /// Makes a player's move by updating the position and score based
        /// on the dice roll.
        /// </summary>
        /// <param name="position">The player position.</param>
        /// <param name="score">The player score.</param>
        /// <param name="diceValue">The values of the dice.</param>
        static void MovePlayer(ref int position, ref int score, int diceValue)
        {
            position += diceValue;
            position = (position - 1) % 10 + 1;

            score += position;
        }

        /// <summary>
        /// Play a game of deterministic dice. We are given the start positions, and alternate turns
        /// until a player wins.
        /// </summary>
        /// <param name="player1Position">The first player's position.</param>
        /// <param name="player2Position">The second player's position.</param>
        /// <returns>The final score.</returns>
        private static int PlayDeterministicDice(int player1Position, int player2Position)
        {
            var playerPosition = new int[]
            {
                    player1Position,
                    player2Position
            };

            int[] playerScore = new int[] { 0, 0 };

            int currentTurn = 0;
            int diceCount = 0;

            while (playerScore[0] < 1000 && playerScore[1] < 1000)
            {
                int diceValue = 0;
                for (int i = 0; i < 3; i++)
                {
                    diceCount++;
                    diceValue += (diceCount - 1) % 100 + 1;
                }

                MovePlayer(ref playerPosition[currentTurn], ref playerScore[currentTurn], diceValue);

                currentTurn = currentTurn == 0 ? 1 : 0;
            }

            return playerScore.Min() * diceCount;
        }

        /// <summary>
        /// Gets all combinations of dice rolls for three three-sided dice.
        /// </summary>
        /// <returns>The dice rolls.</returns>
        private static IEnumerable<int> GetDiceRolls()
        {
            var diceRolls = Enumerable.Range(1, 3);

            foreach (var i in diceRolls)
            {
                foreach (var j in diceRolls)
                {
                    foreach (var k in diceRolls)
                    {
                        yield return i + j + k;
                    }
                }
            }
        }

        /// <summary>
        /// Stores a class which can play dirac dice, where each die roll creates a
        /// new universe. Follows all paths, and returns the number of wins and losses
        /// for the players.
        /// </summary>
        class DiracDice
        {
            /// <summary>
            /// Stores the game state when starting each turn.
            /// </summary>
            /// <param name="Score1">The first player's score.</param>
            /// <param name="Score2">The second player's score.</param>
            /// <param name="Position1">The first player's position.</param>
            /// <param name="Position2">The second player's position.</param>
            /// <param name="CurrentPlayer">The player whose turn it is.</param>
            internal record GameState(int Score1, int Score2, int Position1, int Position2, int CurrentPlayer);

            /// <summary>
            /// Stores the number of wins and losses for both players after a turn.
            /// </summary>
            /// <param name="Player1Wins">The number of times the first player won.</param>
            /// <param name="Player2Wins">The number of times the second player won.</param>
            internal record PlayerWins(long Player1Wins, long Player2Wins);

            private readonly Dictionary<GameState, PlayerWins> universeLookup = new();

            internal PlayerWins CountWins(GameState state)
            {
                if (universeLookup.ContainsKey(state))
                {
                    return universeLookup[state];
                }

                long player1WinTotal = 0;
                long player2WinTotal = 0;

                foreach (var diceRoll in GetDiceRolls())
                {
                    var (player1Wins, player2Wins) = MakeMove(state, diceRoll);
                    player1WinTotal += player1Wins;
                    player2WinTotal += player2Wins;
                }

                var numUniverses = new PlayerWins(player1WinTotal, player2WinTotal);

                universeLookup.Add(state, numUniverses);

                return numUniverses;
            }

            /// <summary>
            /// Makes a move given the input state and the dice roll in the current turn.
            /// </summary>
            /// <param name="state">The game state.</param>
            /// <param name="diceRoll">The next dice roll.</param>
            /// <returns>The number of wins and losses.</returns>
            private PlayerWins MakeMove(GameState state, int diceRoll)
            {
                if (state.CurrentPlayer == 0)
                {
                    int position1 = state.Position1;
                    int score1 = state.Score1;
                    
                    MovePlayer(ref position1, ref score1, diceRoll);

                    if (score1 >= 21)
                    {
                        return new(1L, 0L);
                    }

                    state = state with
                    {
                        Position1 = position1,
                        Score1 = score1,
                        CurrentPlayer = 1
                    };
                }
                else
                {
                    int position2 = state.Position2;
                    int score2 = state.Score2;

                    MovePlayer(ref position2, ref score2, diceRoll);

                    if (score2 >= 21)
                    {
                        return new(0L, 1L);
                    }

                    state = state with
                    {
                        Position2 = position2,
                        Score2 = score2,
                        CurrentPlayer = 0
                    };
                }

                return CountWins(state);
            }
        }

        /// <summary>
        /// Plays a game of dirac dice based on the players' start positions.
        /// </summary>
        /// <param name="player1Position">The first player's start position.</param>
        /// <param name="player2Position">The second player's start position.</param>
        /// <returns>The number of wins for the best player.</returns>
        private static long PlayDiracDice(int player1Position, int player2Position)
        {
            var diracDice = new DiracDice();
            var gameState = new DiracDice.GameState(0, 0, player1Position, player2Position, 0);

            var (player1Wins, player2Wins) = diracDice.CountWins(gameState);

            return Math.Max(player1Wins, player2Wins);
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(739785, PlayDeterministicDice(4, 8));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(864900, PlayDeterministicDice(4, 5));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(444356092776315, PlayDiracDice(4, 8));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(575111835924670, PlayDiracDice(4, 5));

        #endregion
    }
}
