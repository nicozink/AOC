using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace AOC2022
{
    /// <summary>
    /// Solution for day 2:
    /// https://adventofcode.com/2022/day/2
    /// </summary>
    [TestClass]
    public class Day02
    {
        /// <summary>
        /// Stores the hand shapes that can be made for a game
        /// or rock paper scissors.
        /// </summary>
        enum HandShape
        {
            Rock = 1,
            Paper = 2,
            Scissors = 3
        }

        /// <summary>
        /// Stores the results of a game of rock-paper-scissors,
        /// and the possible scores for each round.
        /// </summary>
        enum Result
        {
            Loss = 0,
            Draw = 3,
            Win = 6
        }

        /// <summary>
        /// Stores a round consisting of the opponent's move, and
        /// a hint for what your move should be.
        /// </summary>
        /// <param name="Opponent">The opponent's move</param>
        /// <param name="Hint">A hint for your move.</param>
        record Round(HandShape Opponent, char Hint);

        /// <summary>
        /// A lookup for the winning move, for each opponent move.
        /// </summary>
        static readonly Dictionary<HandShape, HandShape> winningMoves = new()
        {
            { HandShape.Rock, HandShape.Paper },
            { HandShape.Paper, HandShape.Scissors },
            { HandShape.Scissors, HandShape.Rock }
        };

        /// <summary>
        /// A lookup for the losing move, for each opponent move. We
        /// take the winning dictionary in this case and reverse the
        /// keys/pairs.
        /// </summary>
        static readonly Dictionary<HandShape, HandShape> losingMoves = winningMoves.ToDictionary(x => x.Value, y => y.Key);

        /// <summary>
        /// Gets the rounds from the file.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <returns>The information for each round.</returns>
        private static IEnumerable<Round> GetRounds(string path)
        {
            var lines = System.IO.File.ReadLines(path);

            foreach (var line in lines)
            {
                var moves = line.Split(' ')
                    .Select(x => x[0])
                    .ToArray();

                var opponentMove = (HandShape)(moves[0] - 'A' + 1);
                yield return new Round(opponentMove, moves[1]);
            }
        }

        /// <summary>
        /// Gets the score for two moves by the opponent and you.
        /// </summary>
        /// <param name="opponent">The opponent's move.</param>
        /// <param name="you">Your move.</param>
        /// <returns>The score.</returns>
        private static int GetScore(HandShape opponent, HandShape you)
        {
            if (opponent == you)
            {
                return (int)Result.Draw + (int)you;
            }

            if (winningMoves[opponent] == you)
            {
                return (int)Result.Win + (int)you;
            }

            return (int)Result.Loss + (int)you;
        }

        /// <summary>
        /// According to the strategy for part 1, the hint is
        /// the type of move to make where 'X' is rock, 'Y' is
        /// paper, and 'Z' is scissors. So we make that move,
        /// and return the scoire.
        /// </summary>
        /// <param name="round">The round.</param>
        /// <returns>The score for that round.</returns>
        private static int GetScoreStrategy1(Round round)
        {
            var yourMove = (HandShape)(round.Hint - 'X' + 1);
            return GetScore(round.Opponent, yourMove);
        }

        /// <summary>
        /// According to the strategy for part 2, the hint is
        /// to make a move such that 'X' is a loss, 'Y' is a draw,
        /// and 'Z' is a win. We make the move accordingly, and return
        /// the score.
        /// </summary>
        /// <param name="round">The round.</param>
        /// <returns>The score for that round.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        private static int GetScoreStrategy2(Round round)
        {
            if (round.Hint == 'X') // You need to lose
            {
                var yourMove = losingMoves[round.Opponent];
                return GetScore(round.Opponent, yourMove);
            }
            else if (round.Hint == 'Y') // The game should draw
            {
                var yourMove = round.Opponent;
                return GetScore(round.Opponent, yourMove);
            }
            else if (round.Hint == 'Z') // You need to win
            {
                var yourMove = winningMoves[round.Opponent];
                return GetScore(round.Opponent, yourMove);
            }

            throw new InvalidOperationException("Received invalid hint");
        }

        /// <summary>
        /// Play the games using the rules for part 1, and return the
        /// sum of the scores.
        /// </summary>
        /// <param name="path">The path to the file describing the rounds.</param>
        /// <returns>The sum of the scores.</returns>
        private static int CalculateScorePart1(string path) => GetRounds(path).Sum(x => GetScoreStrategy1(x));

        /// <summary>
        /// Play the games using the rules for part 2, and return the
        /// sum of the scores.
        /// </summary>
        /// <param name="path">The path to the file describing the rounds.</param>
        /// <returns>The sum of the scores.</returns>
        private static int CalculateScorePart2(string path) => GetRounds(path).Sum(x => GetScoreStrategy2(x));

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(15, CalculateScorePart1("AOC2022/Day02/Example.txt"));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(14163, CalculateScorePart1("AOC2022/Day02/Input.txt"));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(12, CalculateScorePart2("AOC2022/Day02/Example.txt"));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(12091, CalculateScorePart2("AOC2022/Day02/Input.txt"));

        #endregion
    }
}
