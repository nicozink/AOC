using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace AOC2021
{
    /// <summary>
    /// Solution for day 4:
    /// https://adventofcode.com/2021/day/4
    /// </summary>
    [TestClass]
    public class Day04
    {
        /// <summary>
        /// Stores a board containing numbers used in a
        /// bingo game. Numbers are removed as they are marked,
        /// and a score is given as the sum of all unmarked
        /// numbers.
        /// </summary>
        private class BingoBoard
        {
            /// <summary>
            /// Stores each row of the board.
            /// </summary>
            private readonly List<HashSet<int>> rows = new();

            /// <summary>
            /// Stores each column of the board.
            /// </summary>
            private readonly List<HashSet<int>> columns = new();

            /// <summary>
            /// Creates a new board based on the grid of numbers
            /// given as input.
            /// </summary>
            /// <param name="numbers">The grid of numnbers.</param>
            public BingoBoard(List<List<int>> numbers)
            {
                for (int i = 0; i < numbers.Count; i++)
                {
                    rows.Add(new());
                    columns.Add(new());
                }

                for (int i = 0; i < numbers.Count; i++)
                {
                    for (int j = 0; j < numbers.Count; j++)
                    {
                        rows[i].Add(numbers[i][j]);
                        columns[j].Add(numbers[i][j]);
                    }
                }
            }

            /// <summary>
            /// Mark a number as called. If the number exists,
            /// it is removed from the grid.
            /// </summary>
            /// <param name="value">The number which was called.</param>
            public void MarkNumber(int value)
            {
                foreach (var row in rows)
                {
                    row.Remove(value);
                }

                foreach (var col in columns)
                {
                    col.Remove(value);
                }
            }

            /// <summary>
            /// Gets the score which evalueates to the sum of
            /// numbers which haven't been called.
            /// </summary>
            /// <returns>The sum of uncalled numbers.</returns>
            public int GetScore()
            {
                return rows.Sum(x => x.Sum());
            }

            /// <summary>
            /// Checks whether we have bingo if an entire row or
            /// column has been removed.
            /// </summary>
            /// <returns>True if we have bingo.</returns>
            public bool HasBingo()
            {
                if (rows.Exists(x => x.Count == 0))
                {
                    return true;
                }

                if (columns.Exists(x => x.Count == 0))
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Reads the board from the enumerator. This contains a block of numbers,
        /// followed by an empty line (or the end of the enumerator).
        /// </summary>
        /// <param name="enumerator">The enumerators containing the input.</param>
        /// <returns>The board.</returns>
        BingoBoard ReadBoard(IEnumerator<string> enumerator)
        {
            List<List<int>> boardValues = new();

            while (!String.IsNullOrEmpty(enumerator.Current))
            {
                var boardNumbers = enumerator.Current.Split()
                        .Where(x => !String.IsNullOrWhiteSpace(x))
                        .Select(int.Parse)
                        .ToList();

                boardValues.Add(boardNumbers);

                if (!enumerator.MoveNext())
                {
                    break;
                }
            }

            return new BingoBoard(boardValues);
        }

        /// <summary>
        /// Reads the input from a file. Input consists of a list of numbers
        /// which are called in order, followed by several grids cobntainig
        /// the bingo boards.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The numbers and the bingo boards.</returns>
        (List<BingoBoard> boards, List<int> numbers) ReadInput(String path)
        {
            var lines = System.IO.File.ReadAllLines(path).ToList();

            var numbers = lines.First()
                .Split(',')
                .Select(int.Parse)
                .ToList();

            List<BingoBoard> boards = new();

            List<List<int>> boardValues = new();

            var enumerator = lines.Skip(2).GetEnumerator();

            while (enumerator.MoveNext())
            {
                boards.Add(ReadBoard(enumerator));
            }
            
            return (boards, numbers);
        }

        /// <summary>
        /// Calls out the numbers in sequence, and gets the score of
        /// the first board to reach bingo.
        /// </summary>
        /// <param name="path">Thye input file.</param>
        /// <returns>The first winning score.</returns>
        /// <exception cref="InvalidOperationException">Thrown if we can't find a winner.</exception>
        int GetWinningScore(string path)
        {
            (var boards, var numbers) = ReadInput(path);

            foreach (var number in numbers)
            {
                foreach (var board in boards)
                {
                    board.MarkNumber(number);

                    if (board.HasBingo())
                    {
                        return board.GetScore() * number;
                    }
                }
            }

            throw new InvalidOperationException("Could not find a winning board");
        }

        /// <summary>
        /// Calls out the numbers in sequence, and gets the score of
        /// the last board to reach bingo.
        /// </summary>
        /// <param name="path">The input file.</param>
        /// <returns>The last winning score.</returns>
        /// <exception cref="InvalidOperationException">Thrown if we can't find a winner.</exception>
        int GetLastWinningScore(string path)
        {
            (var boards, var numbers) = ReadInput(path);

            var remainingBoards = boards.ToList();

            foreach (var number in numbers)
            {
                foreach (var board in remainingBoards)
                {
                    board.MarkNumber(number);
                }

                if (remainingBoards.Count != 1)
                {
                    remainingBoards = remainingBoards.Where(x => !x.HasBingo()).ToList();
                }

                if (remainingBoards.Count == 1 && remainingBoards[0].HasBingo())
                {
                    return remainingBoards[0].GetScore() * number;
                }
            }

            throw new InvalidOperationException("Could not find the last winning board");
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(4512, GetWinningScore("AOC2021/Day04/Example.txt"));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(33462, GetWinningScore("AOC2021/Day04/Input.txt"));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(1924, GetLastWinningScore("AOC2021/Day04/Example.txt"));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(30070, GetLastWinningScore("AOC2021/Day04/Input.txt"));

        #endregion
    }
}
