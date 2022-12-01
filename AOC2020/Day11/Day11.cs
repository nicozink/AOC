using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AOC2020
{
    /// <summary>
    /// Solution for day 11:
    /// https://adventofcode.com/2020/day/11
    /// </summary>
    [TestClass]
    public class Day11
    {
        /// <summary>
        /// There are two ways of finding nearby seats.
        /// Find the nearest neighbouring seats, or the
        /// first seat visible in all directions.
        /// </summary>
        private enum SeatFinder
        {
            Nearest,
            Visible
        }

        /// <summary>
        /// Stores a grid of seats. Empty seats are L, occupied seats are
        /// # and no seats are .
        /// </summary>
        private class SeatingChart
        {
            /// <summary>
            /// Creates a new instance by reading in the seat layout.
            /// </summary>
            /// <param name="path">A file containing the seats.</param>
            public SeatingChart(String path)
            {
                seats = System.IO.File.ReadLines(path).Select(x => x.ToList()).ToList();
            }

            /// <summary>
            /// Counts the number of occupied seats.
            /// </summary>
            /// <returns>The occupied seats.</returns>
            public int CountOccupiedSeats()
            {
                return seats.Sum(x => x.Count(y => y == '#'));
            }

            /// <summary>
            /// Gets the nearest seats for a particular location.
            /// This can be the neighbouring seats, or visible seats.
            /// </summary>
            /// <param name="type">The search type - e.g. nearest or visible.</param>
            /// <param name="row">The starting row.</param>
            /// <param name="column">The starting column.</param>
            /// <returns></returns>
            public IEnumerable<char> GetNearestSeats(SeatFinder type, int row, int column)
            {
                // This is a list containing all valid directions
                // e.e. up, down, left, right and diagonals.
                var directions = new List<Tuple<int, int>>()
                {
                    new Tuple<int, int>(-1, -1),
                    new Tuple<int, int>(-1, 0),
                    new Tuple<int, int>(-1, 1),
                    new Tuple<int, int>(0, -1),
                    new Tuple<int, int>(0, 1),
                    new Tuple<int, int>(1, -1),
                    new Tuple<int, int>(1, 0),
                    new Tuple<int, int>(1, 1)
                };

                foreach (var direction in directions)
                {
                    int posX = row + direction.Item1;
                    int posY = column + direction.Item2;

                    if (type == SeatFinder.Nearest)
                    {
                        // For a nearest search, we just return the seat in that
                        // direction.
                        if (posX >= 0 && posX < seats.Count &&
                            posY >= 0 && posY < seats[0].Count)
                        {
                            yield return seats[posX][posY];
                        }
                    }
                    else
                    {
                        // For a visible search, we start scanning the seats
                        // away from the current position, in the specified
                        // direction.
                        while (posX >= 0 && posX < seats.Count &&
                            posY >= 0 && posY < seats[0].Count)
                        {
                            var seat = seats[posX][posY];

                            if (seat == 'L' || seat == '#')
                            {
                                yield return seat;
                                break;
                            }

                            posX += direction.Item1;
                            posY += direction.Item2;
                        }
                    }
                }
            }

            /// <summary>
            /// Update the seating chart based on the rules provided.
            /// </summary>
            /// <param name="type">The type of search - e.g. nearest or visible.</param>
            /// <param name="threshold">The threshold for when to leave a seat unoccupied.</param>
            /// <returns>The number of seats that were changed.</returns>
            public int Update(SeatFinder type, int threshold)
            {
                int updated = 0;

                // We make a copy of the list to modify.
                var newList = seats.Select(x => x.ToList()).ToList();

                for (int i = 0; i < seats.Count; ++i)
                {
                    for (int j = 0; j < seats[0].Count; ++j)
                    {
                        // Look through the nearest seats, and count the number of occupied neighbours.

                        var seat = seats[i][j];
                        var nearestNeighbours = GetNearestSeats(type, i, j);
                        var occupiedNeigbours = nearestNeighbours.Count(x => x == '#');

                        if (seat == 'L')
                        {
                            // If the seat is empry, and all neighbours are unoccupied, we
                            // fill the seat.
                            if (occupiedNeigbours == 0)
                            {
                                newList[i][j] = '#';

                                ++updated;
                            }
                        }
                        else if (seat == '#')
                        {
                            // If the number of occupied seats is larger than the threshold,
                            // we empty the seat.
                            if (occupiedNeigbours >= threshold)
                            {
                                newList[i][j] = 'L';

                                ++updated;
                            }
                        }
                    }
                }

                seats = newList;

                return updated;
            }

            private List<List<char>> seats;
        }

        public int GetSolution1(String path)
        {
            var seatChart = new SeatingChart(path);

            while (seatChart.Update(SeatFinder.Nearest, 4) != 0) { }

            return seatChart.CountOccupiedSeats();
        }

		public int GetSolution2(String path)
        {
            var seatChart = new SeatingChart(path);

            while (seatChart.Update(SeatFinder.Visible, 5) != 0) { }

            return seatChart.CountOccupiedSeats();
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(37, GetSolution1("AOC2020/Day11/Example.txt"));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(2346, GetSolution1("AOC2020/Day11/Input.txt"));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(26, GetSolution2("AOC2020/Day11/Example.txt"));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(2111, GetSolution2("AOC2020/Day11/Input.txt"));

        #endregion
    }
}
