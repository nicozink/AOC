using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Z3 = Microsoft.Z3;

namespace AOC2023
{
    /// <summary>
    /// Solution for day 24:
    /// https://adventofcode.com/2023/day/24
    /// </summary>
    [TestClass]
    public class Day24
    {
        /// <summary>
        /// Stores the integer-based coordinate of a hailstone.
        /// </summary>
        /// <param name="X">The x coordinate.</param>
        /// <param name="Y">The y coordinate.</param>
        /// <param name="Z">The z coordinate.</param>
        private record V3(long X, long Y, long Z);

        /// <summary>
        /// Stores the start position and velocity of a hailstone.
        /// </summary>
        /// <param name="Position">The position.</param>
        /// <param name="Velocity">The volcity.</param>
        private record HailStone(V3 Position, V3 Velocity);

        /// <summary>
        /// Read the position and velocity of the hailstones form the input file.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The information for each hailstone.</returns>
        private static IEnumerable<HailStone> ReadInput(string path)
        {
            var lines = System.IO.File.ReadLines(path);
            foreach (var line in lines)
            {
                var numbers = line
                    .Replace('@', ',')
                    .Replace(" ", "")
                    .Split(',')
                    .Select(long.Parse)
                    .ToArray();

                var position = new V3(numbers[0], numbers[1], numbers[2]);
                var velocity = new V3(numbers[3], numbers[4], numbers[5]);

                yield return new HailStone(position, velocity);
            }
        }

        /// <summary>
        /// Calculates whether the paths for two hailstones intersect. This is not necessarily a collision,
        /// but a check that the paths intersect, that the intersection is in the forward direction for each
        /// hailstone, and that the intersection is inside the bounds given.
        /// </summary>
        /// <param name="hailStone1">The first hailstone.</param>
        /// <param name="hailStone2">The second hailstone.</param>
        /// <param name="minRange">The minimum range bounds.</param>
        /// <param name="maxRange">The maximum range bounds.</param>
        /// <returns>True if the paths intersect.</returns>
        private static bool PathsIntersectXY(HailStone hailStone1, HailStone hailStone2, long minRange, long maxRange)
        {
            double px1 = hailStone1.Position.X;
            double py1 = hailStone1.Position.Y;
            double px2 = hailStone2.Position.X;
            double py2 = hailStone2.Position.Y;

            double vx1 = hailStone1.Velocity.X;
            double vy1 = hailStone1.Velocity.Y;
            double vx2 = hailStone2.Velocity.X;
            double vy2 = hailStone2.Velocity.Y;

            var det = vx2 * vy1 - vy2 * vx1;
            if (det == 0)
            {
                return false;
            }

            var dx = px2 - px1;
            var dy = py2 - py1;
            var u = (dy * vx2 - dx * vy2) / det;
            var v = (dy * vx1 - dx * vy1) / det;
            
            if (u < 0 || v < 0)
            {
                return false;
            }
            
            var px = px1 + vx1 * u;
            var py = py1 + vy1 * u;

            if (px < minRange ||
                px > maxRange ||
                py < minRange ||
                py > maxRange)
            {
                return false;
            }
            
            return true;
        }

        /// <summary>
        /// Count the number of intersecting paths between all pairs of hailstones.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <param name="minRange">The minimum of the range for valid inspections.</param>
        /// <param name="maxRange">The maximum of the range for valid inspections.</param>
        /// <returns>The number of intersections.</returns>
        private static int CountIntersections(string path, long minRange, long maxRange)
        {
            var hailStones = ReadInput(path)
                .ToArray();

            var intersections = 0;
            for (int i = 0; i < hailStones.Length - 1; i++)
            {
                for (int j = i + 1; j < hailStones.Length; j++)
                {
                    var hailStone1 = hailStones[i];
                    var hailStone2 = hailStones[j];

                    if (PathsIntersectXY(hailStone1, hailStone2, minRange, maxRange))
                    {
                        intersections++;
                    }
                }
            }

            return intersections;
        }

        /// <summary>
        /// Create an expression that calculates the new position given starting position,
        /// velocity and time.
        /// </summary>
        /// <param name="context">The context used to create the expression.</param>
        /// <param name="position">The starting position.</param>
        /// <param name="velocity">The velocity.</param>
        /// <param name="time">The time.</param>
        /// <returns>The expression.</returns>
        private static Z3.ArithExpr CreatePositionExpr(Z3.Context context, Z3.ArithExpr position, Z3.ArithExpr velocity, Z3.IntExpr time)
        {
            var delta = context.MkMul(velocity, time);
            var newPosition = context.MkAdd(position, delta);

            return newPosition;
        }

        /// <summary>
        /// Create an expression which evaluates the path of two objects for intersection given two
        /// setating points, two velocities and a time. The objects are given as a constant which
        /// is known, and a variable which is unknown and we want to solve for.
        /// </summary>
        /// <param name="context">The context used to create the expression.</param>
        /// <param name="posConst">The constant-value starting position.</param>
        /// <param name="velConst">The constant-value velocity.</param>
        /// <param name="posVar">The variable starting position.</param>
        /// <param name="velVar">The variable velocity.</param>
        /// <param name="time">The time.</param>
        /// <returns>The expression.</returns>
        private static Z3.BoolExpr CreateEqualityExpr(Z3.Context context, long posConst, long velConst, Z3.IntExpr posVar, Z3.IntExpr velVar, Z3.IntExpr time)
        {
            var lhs = CreatePositionExpr(context, context.MkInt(posConst), context.MkInt(velConst), time);
            var rhs = CreatePositionExpr(context, posVar, velVar, time);

            var equals = context.MkEq(lhs, rhs);
            return equals;
        }

        /// <summary>
        /// Gets the integer result from an expression variable.
        /// </summary>
        /// <param name="model">The solved model.</param>
        /// <param name="expr">The expression to evaluate.</param>
        /// <returns>The result.</returns>
        private static long GetResult(Z3.Model model, Z3.IntExpr expr)
        {
            var assignment = model.ConstInterp(expr) as Z3.IntNum;
            Assert.IsNotNull(assignment);
            return assignment.Int64;
        }

        /// <summary>
        /// Gets the ideal throw position to launch a stone which intersects with all
        /// hail stones.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The ideal throw position.</returns>
        private static double GetThrowPosition(string path)
        {
            var hailStones = ReadInput(path)
                .ToArray();

            using var context = new Z3.Context();
            var solver = context.MkSolver();

            var px = context.MkIntConst("px");
            var py = context.MkIntConst("py");
            var pz = context.MkIntConst("pz");

            var vx = context.MkIntConst("vx");
            var vy = context.MkIntConst("vy");
            var vz = context.MkIntConst("vz");

            // We only need to consider three hail stones in the entire collection - as
            // that gives us nine equations with nine unknowns (3 pos, 3 vel and 3 time)
            // so this system is already solvable with 3 and has a unique solution.
            for (int index = 0; index < 3; index++)
            {
                var (position, velocity) = hailStones[index];

                var time = context.MkIntConst($"t{index}");

                var xconstraint = CreateEqualityExpr(context, position.X, velocity.X, px, vx, time);
                var yconstraint = CreateEqualityExpr(context, position.Y, velocity.Y, py, vy, time);
                var zconstraint = CreateEqualityExpr(context, position.Z, velocity.Z, pz, vz, time);
                solver.Add(xconstraint);
                solver.Add(yconstraint);
                solver.Add(zconstraint);
            }

            if (solver.Check() != Z3.Status.SATISFIABLE)
            {
                throw new InvalidOperationException("Unable to solve the system given the input.");
            }

            var model = solver.Model;
            return GetResult(model, px) +
                GetResult(model, py) +
                GetResult(model, pz);
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(2, CountIntersections("AOC2023/Day24/Example.txt", 7, 27));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(17906, CountIntersections("AOC2023/Day24/Input.txt", 200000000000000, 400000000000000));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(47, GetThrowPosition("AOC2023/Day24/Example.txt"));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(571093786416929, GetThrowPosition("AOC2023/Day24/Input.txt"));

        #endregion
    }
}
