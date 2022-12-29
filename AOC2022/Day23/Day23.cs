using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using System.Text.RegularExpressions;

namespace AOC2022
{
    /// <summary>
    /// Solution for day 23:
    /// https://adventofcode.com/2022/day/23
    /// </summary>
    [TestClass]
    public class Day23
    {
        /// <summary>
        /// The directions in which elves can move.
        /// </summary>
        enum Direction
        {
            North,
            South,
            West,
            East
        }

        /// <summary>
        /// Stores the position of elves.
        /// </summary>
        /// <param name="X">The x position.</param>
        /// <param name="Y">The y podsition.</param>
        record Position(int X, int Y);

        /// <summary>
        /// Stores an elf.
        /// </summary>
        /// <param name="Position">The position.</param>
        /// <param name="Directions">The directions.</param>
        record Elf(Position Position, List<Direction> Directions);

        /// <summary>
        /// Reads the elves from the input.
        /// </summary>
        /// <param name="path">The path to the input.</param>
        /// <returns>The elves.</returns>
        private static IEnumerable<Elf> ReadInput(string path)
        {
            var lines = System.IO.File.ReadAllLines(path);

            for (int r = 0; r < lines.Length; r++)
            {
                for (int c = 0; c < lines[r].Length; c++)
                {
                    if (lines[r][c] == '#')
                    {
                        var defaultDirections = new List<Direction>()
                        {
                            Direction.North,
                            Direction.South,
                            Direction.West,
                            Direction.East
                        };

                        var position = new Position(c, r);
                        yield return new Elf(position, defaultDirections);
                    }
                }
            }
        }

        /// <summary>
        /// Checks whether an elf has any neighbours.
        /// </summary>
        /// <param name="positions">The elf positions.</param>
        /// <param name="position">The elf position.</param>
        /// <returns>True if the elf has neighbours.</returns>
        private static bool HasNeighbour(HashSet<Position> positions, Position position)
        {
            if (positions.Contains(new(position.X - 1, position.Y - 1))
                || positions.Contains(new(position.X, position.Y - 1))
                || positions.Contains(new(position.X + 1, position.Y - 1))
                || positions.Contains(new(position.X - 1, position.Y))
                || positions.Contains(new(position.X + 1, position.Y))
                || positions.Contains(new(position.X - 1, position.Y + 1))
                || positions.Contains(new(position.X, position.Y + 1))
                || positions.Contains(new(position.X + 1, position.Y + 1)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if the direction is empty from other elves.
        /// </summary>
        /// <param name="positions">The position of the elves.</param>
        /// <param name="position">The position of this elf.</param>
        /// <param name="direction">The facing direction.</param>
        /// <returns>True if the direction is empty.</returns>
        private static bool IsEmpty(HashSet<Position> positions, Position position, Direction direction)
        {
            if (direction == Direction.North
                && !positions.Contains(new(position.X - 1, position.Y - 1))
                && !positions.Contains(new(position.X, position.Y - 1))
                && !positions.Contains(new(position.X + 1, position.Y - 1)))
            {
                return true;
            }
            else if (direction == Direction.South
                && !positions.Contains(new(position.X - 1, position.Y + 1))
                && !positions.Contains(new(position.X, position.Y + 1))
                && !positions.Contains(new(position.X + 1, position.Y + 1)))
            {
                return true;
            }
            else if (direction == Direction.West
                && !positions.Contains(new(position.X - 1, position.Y - 1))
                && !positions.Contains(new(position.X - 1, position.Y))
                && !positions.Contains(new(position.X - 1, position.Y + 1)))
            {
                return true;
            }
            else if (direction == Direction.East
                && !positions.Contains(new(position.X + 1, position.Y - 1))
                && !positions.Contains(new(position.X + 1, position.Y))
                && !positions.Contains(new(position.X + 1, position.Y + 1)))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Get the position of the point in the direction.
        /// </summary>
        /// <param name="position">The position of the elf.</param>
        /// <param name="direction">The direction.</param>
        /// <returns>The position in the direction.</returns>
        private static Position GetPosition(Position position, Direction direction)
        {
            if (direction == Direction.North)
            {
                return new(position.X, position.Y - 1);
            }
            else if (direction == Direction.South)
            {
                return new(position.X, position.Y + 1);
            }
            else if (direction == Direction.West)
            {
                return new(position.X - 1, position.Y);
            }
            else
            {
                return new(position.X + 1, position.Y);
            }
        }

        /// <summary>
        /// Get the suggested move from the elf.
        /// </summary>
        /// <param name="positions">The positions of the elves.</param>
        /// <param name="elf">The elf.</param>
        /// <returns>The suggested move position.</returns>
        private static Position GetSuggestedMove(HashSet<Position> positions, Elf elf)
        {
            if (HasNeighbour(positions, elf.Position))
            {
                foreach (var rule in elf.Directions)
                {
                    if (IsEmpty(positions, elf.Position, rule))
                    {
                        return GetPosition(elf.Position, rule);
                    }
                }
            }

            return elf.Position;
        }

        /// <summary>
        /// Calculates the new positions and applies them to the elves.
        /// </summary>
        /// <param name="elves">The elves.</param>
        /// <returns>The number of elves that moved.</returns>
        private static int MoveElves(Elf[] elves)
        {
            var positions = elves.Select(x => x.Position).ToHashSet();

            var suggestedMoves = new Position[elves.Length];
            for (int i = 0; i < elves.Length; i++)
            {
                suggestedMoves[i] = GetSuggestedMove(positions, elves[i]);
            }

            var groupedMoves = suggestedMoves
                .GroupBy(x => x)
                .Where(x => x.Count() != 1)
                .Select(x => x.Key)
                .ToHashSet();

            if (groupedMoves.Count > 0)
            {
                for (int i = 0; i < suggestedMoves.Length; i++)
                {
                    if (groupedMoves.Contains(suggestedMoves[i]))
                    {
                        suggestedMoves[i] = elves[i].Position;
                    }
                }
            }

            int numMoved = suggestedMoves
                .Select((x, i) => x != elves[i].Position)
                .Count(x => x);

            for (int j = 0; j < suggestedMoves.Length; j++)
            {
                elves[j] = elves[j] with { Position = suggestedMoves[j] };
            }

            foreach (var elf in elves)
            {
                var lastDirection = elf.Directions[0];
                elf.Directions.RemoveAt(0);
                elf.Directions.Add(lastDirection);
            }

            return numMoved;
        }

        /// <summary>
        /// Solves part 1 by running the solver for 10 rounds.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The number of empty spaces.</returns>
        private static int GetPart1(string path)
        {
            var elves = ReadInput(path).ToArray();
            
            for (int i = 0; i < 10; i++)
            {
                MoveElves(elves);
            }

            int minX = elves.Min(x => x.Position.X);
            int maxX = elves.Max(x => x.Position.X);

            int minY = elves.Min(x => x.Position.Y);
            int maxY = elves.Max(x => x.Position.Y);

            return (maxX - minX + 1) * (maxY - minY + 1) - elves.Length;
        }

        /// <summary>
        /// Solve part 2 by running the solver until the elves are done
        /// moving.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The number of rounds.</returns>
        private static int GetPart2(string path)
        {
            var elves = ReadInput(path).ToArray();
            
            int numRounds = 1;
            while (MoveElves(elves) != 0)
            {
                numRounds++;
            }

            return numRounds;
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(110, GetPart1("AOC2022/Day23/Example.txt"));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(3762, GetPart1("AOC2022/Day23/Input.txt"));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(20, GetPart2("AOC2022/Day23/Example.txt"));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(997, GetPart2("AOC2022/Day23/Input.txt"));

        #endregion
    }
}
