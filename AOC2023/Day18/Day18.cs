using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace AOC2023
{
    /// <summary>
    /// Solution for day 18:
    /// https://adventofcode.com/2023/day/18
    /// </summary>
    [TestClass]
    public class Day18
    {
        /// <summary>
        /// A position on the grid.
        /// </summary>
        /// <param name="Row">The row.</param>
        /// <param name="Col">The column.</param>
        private record Position(long Row, long Col);

        /// <summary>
        /// Gets a position after moving in a direction for the number of steps.
        /// </summary>
        /// <param name="position">The starting position.</param>
        /// <param name="direction">The direction to move in.</param>
        /// <param name="steps">The number of steps to move.</param>
        /// <returns>The new position.</returns>
        private static Position GetPosition(Position position, char direction, int steps)
        {
            return (direction) switch
            {
                'U' => position with { Row = position.Row - steps },
                'D' => position with { Row = position.Row + steps },
                'L' => position with { Col = position.Col - steps },
                _ => position with { Col = position.Col + steps }
            };
        }

        /// <summary>
        /// Convert an int direction into a character direction.
        /// </summary>
        /// <param name="direction">The integer direction.</param>
        /// <returns>The character direction.</returns>
        private static char ConvertDirection(char direction)
        {
            return (direction) switch
            {
                '0' => 'R',
                '1' => 'D',
                '2' => 'L',
                _ => 'U'
            };
        }

        /// <summary>
        /// Reads the input, and constructs a list of positions defining the polygon.
        /// </summary>
        /// <param name="input">The path to the input file.</param>
        /// <param name="convertHex">Whether to convert the hexadecimal values.</param>
        /// <returns>The positions defining the polygon.</returns>
        private static Position[] ReadInput(string input, bool convertHex)
        {
            var position = new Position(0, 0);
            var positions = new List<Position>()
            {
                position
            };

            var lines = System.IO.File.ReadAllLines(input);
            foreach (var line in lines)
            {
                var split = line.Split();

                var direction = split[0][0];
                var numMoves = int.Parse(split[1]);

                if (convertHex)
                {
                    var hex = split[2]
                        .Replace("(#", "")
                        .Replace(")", "");

                    direction = ConvertDirection(hex[5]);

                    var movesString = hex[..^1];
                    numMoves = int.Parse(movesString, System.Globalization.NumberStyles.HexNumber);
                }

                position = GetPosition(position, direction, numMoves);
                positions.Add(position);
            }

            return positions.ToArray();
        }

        /// <summary>
        /// Calculate the area of the polygon defined by the movement instructions.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <param name="convertHex">Whether to convert hexadecimal values.</param>
        /// <returns>The area of the polygon.</returns>
        private static long CalculateArea(string path, bool convertHex)
        {
            var positions = ReadInput(path, convertHex);

            long area = 0;
            long perimeter = 0;
            for (int i = 0; i < positions.Length - 1; i++)
            {
                // Shoelace algorithm to give the interior of the polygon
                var val1 = positions[i];
                var val2 = positions[i + 1];

                area += val1.Row * val2.Col - val1.Col * val2.Row;

                // But we also need to consider the perimeter
                long dRow = Math.Abs(val1.Row - val2.Row);
                long dCol = Math.Abs(val1.Col - val2.Col);
                perimeter += dRow + dCol;
            }

            // The shoelace algorithm is multiplied by 1/2
            return (Math.Abs(area) + perimeter) / 2 + 1 ;
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(62, CalculateArea("AOC2023/Day18/Example.txt", convertHex: false));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(56923, CalculateArea("AOC2023/Day18/Input.txt", convertHex: false));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(952408144115, CalculateArea("AOC2023/Day18/Example.txt", convertHex: true));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(66296566363189, CalculateArea("AOC2023/Day18/Input.txt", convertHex: true));

        #endregion
    }
}
