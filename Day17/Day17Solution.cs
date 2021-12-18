using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Solutions.Day11Utils;

namespace Solutions
{
    /// <summary>
    /// Solution for day 17:
    /// https://adventofcode.com/2021/day/17
    /// </summary>
    [SolutionClass(Day = 17)]
    public class Day17Solution
    {
        /// <summary>
        /// Stores the position of a probe after it was fired.
        /// </summary>
        /// <param name="X">The x position (distance).</param>
        /// <param name="Y">The y position (depth).</param>
        record Position(int X, int Y);

        /// <summary>
        /// Stores the velocity of a probe after it was fired.
        /// </summary>
        /// <param name="X">The x velocity (distance).</param>
        /// <param name="Y">The y velocity (depth).</param>
        record Velocity(int X, int Y);

        /// <summary>
        /// Stores the size and location of the target.
        /// </summary>
        /// <param name="Min">The minimum bounds.</param>
        /// <param name="Max">The maximum bounds.</param>
        record Target(Position Min, Position Max);

        /// <summary>
        /// Reads a target area fro a file. Input is in the form
        /// "target area: x=20..30, y=-10..-5".
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The target area.</returns>
        static Target ReadInput(string path)
        {
            var input = System.IO.File.ReadAllText(path);

            var numbers = input
                .Replace("target area: x=", "")
                .Replace("..", " ")
                .Replace(", y=", " ")
                .Split()
                .Select(int.Parse)
                .ToArray();

            Position min = new(numbers[0], numbers[2]);
            Position max = new(numbers[1], numbers[3]);

            return new(min, max);
        }

        /// <summary>
        /// Integrate the position and velocity of a probe.
        /// Gravity makes the y velocity faster, and drag
        /// makes the x velocity smaller.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="velocity">The velocity.</param>
        static void Integrate(ref Position position, ref Velocity velocity)
        {
            int newPositionX = position.X + velocity.X;
            int newPositionY = position.Y + velocity.Y;

            position = new(newPositionX, newPositionY);

            int newVelocityX = velocity.X - Math.Sign(velocity.X);
            int newVelocityY = velocity.Y - 1;

            velocity = new(newVelocityX, newVelocityY);
        }

        /// <summary>
        /// For a given target area, this finds the highest trajectory (highest)
        /// as well as the number of trajectories (count) which reach the target.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The highest trajectory, and count of possible trajectories.</returns>
        static (int highest, int count) FindHighestTrajectory(string path)
        {
            // This function makes some assumptions - the starting trajectory is
            // always in positive x direction, and the target is lower than the
            // starting position.

            var target = ReadInput(path);

            int maxHeight = 0;
            int count = 0;

            for (int x = 0; x <= target.Max.X; x++)
            {
                // Another assumption here - we sue the absolute value of the
                // y coordinate. Not sure how correct this is - this assumes
                // that trajectories which start too high will overshoot the
                // target. The absolute value seems to bound this, but there
                // might be a better way.
                for (int y = target.Min.Y; y < Math.Abs(target.Min.Y); y++)
                {
                    Position position = new(0, 0);
                    Velocity velocity = new(x, y);

                    int curMaxHeight = 0;

                    while (position.X <= target.Max.X && position.Y >= target.Min.Y)
                    {
                        Integrate(ref position, ref velocity);

                        if (curMaxHeight < position.Y)
                        {
                            curMaxHeight = position.Y;
                        }

                        if (position.X >= target.Min.X && position.X <= target.Max.X &&
                            position.Y >= target.Min.Y && position.Y <= target.Max.Y)
                        {
                            count++;

                            if (maxHeight < curMaxHeight)
                            {
                                maxHeight = curMaxHeight;
                            }

                            break;
                        }
                    }
                }
            }

            return (maxHeight, count);
        }

        #region Solve Problems

        public long SolveExample1() => FindHighestTrajectory("Day17/Example.txt").highest;

        [SolutionMethod(Part = 1)]
        public long SolvePart1() => FindHighestTrajectory("Day17/Input.txt").highest;

        public long SolveExample2() => FindHighestTrajectory("Day17/Example.txt").count;

        [SolutionMethod(Part = 2)]
        public long SolvePart2() => FindHighestTrajectory("Day17/Input.txt").count;

        #endregion
    }
}
