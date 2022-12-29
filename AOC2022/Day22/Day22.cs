using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using System.Text.RegularExpressions;

namespace AOC2022
{
    /// <summary>
    /// Solution for day 22:
    /// https://adventofcode.com/2022/day/22
    /// </summary>
    [TestClass]
    public class Day22
    {
        /// <summary>
        /// Stores the direction in which we can move.
        /// </summary>
        enum Direction
        {
            Right = 0,
            Down = 1,
            Left = 2,
            Up = 3
        };

        /// <summary>
        /// Stores a position on the grid.
        /// </summary>
        /// <param name="X">The X position.</param>
        /// <param name="Y">The Y position.</param>
        record Position(int X, int Y)
        {
            /// <summary>
            /// Adds two positions.
            /// </summary>
            /// <param name="self">The first position.</param>
            /// <param name="other">The other position.</param>
            /// <returns>The sum.</returns>
            public static Position operator+(Position self, Position other)
            {
                return new Position(self.X + other.X, self.Y + other.Y);
            }

            /// <summary>
            /// Subtracts one position from the other.
            /// </summary>
            /// <param name="self">The first position.</param>
            /// <param name="other">The other position.</param>
            /// <returns>The subtracted position.</returns>
            public static Position operator-(Position self, Position other)
            {
                return new Position(self.X - other.X, self.Y - other.Y);
            }

            /// <summary>
            /// Takes the modulus from one position with another.
            /// </summary>
            /// <param name="self">The first position.</param>
            /// <param name="other">The other position.</param>
            /// <returns>The modulus.</returns>
            public static Position operator%(Position self, Position other)
            {
                return new Position(self.X % other.X, self.Y % other.Y);
            }

            /// <summary>
            /// Scales a position with a scale factor.
            /// </summary>
            /// <param name="self">The position.</param>
            /// <param name="scale">The scale factor.</param>
            /// <returns>The scaled position.</returns>
            public static Position operator*(Position self, int scale)
            {
                return new Position(self.X * scale, self.Y * scale);
            }

            /// <summary>
            /// Dicides a position with a scale factor.
            /// </summary>
            /// <param name="self">The position.</param>
            /// <param name="scale">The scale factor.</param>
            /// <returns>The scaled position.</returns>
            public static Position operator/(Position self, int scale)
            {
                return new Position(self.X / scale, self.Y / scale);
            }
        }

        /// <summary>
        /// The base solver class which is then inherited to
        /// provide the specific border handling.
        /// </summary>
        abstract class SolverBase
        {
            /// <summary>
            /// Stores the grid of walls (#) and floors (.) as
            /// well as empty spaces.
            /// </summary>
            protected readonly string[] grid;
            
            /// <summary>
            /// Stores the list of moves which are processed.
            /// </summary>
            private readonly string[] moves;

            /// <summary>
            /// Creates a new instance of the solver class.
            /// </summary>
            /// <param name="path">The path to the input file.</param>
            public SolverBase(string path)
            {
                (grid, moves) = GetInput(path);
            }

            /// <summary>
            /// Makes a move following the specific rules for handling borders.
            /// </summary>
            /// <param name="position">The position.</param>
            /// <param name="direction">The direction.</param>
            /// <returns>The new position and direction.</returns>
            protected abstract (Position, Direction) MakeMove(Position position, Direction direction);

            /// <summary>
            /// Follows the moves provided and returns the sum of the end position.
            /// </summary>
            /// <returns>The sum of the end position.</returns>
            public int GetSolution()
            {
                var position = new Position(grid[0].IndexOf('.'), 0);
                var direction = Direction.Right;
                foreach (var move in moves)
                {
                    if (move == "R" || move == "L")
                    {
                        direction = ChangeDirection(direction, move);
                        continue;
                    }

                    int steps = int.Parse(move);
                    for (int i = 0; i < steps; i++)
                    {
                        var (newPosition, newDirection) = MakeMove(position, direction);

                        if (grid[newPosition.Y][newPosition.X] != '#')
                        {
                            position = newPosition;
                            direction = newDirection;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                return 1000 * (position.Y + 1) + 4 * (position.X + 1) + (int)direction;
            }

            /// <summary>
            /// Gets the delta position for moving in a direction.
            /// </summary>
            /// <param name="direction">The direction.</param>
            /// <returns>The delta position.</returns>
            /// <exception cref="InvalidOperationException">Exception when given an incorrect direction.</exception>
            protected static Position GetDelta(Direction direction)
            {
                return direction switch
                {
                    Direction.Right => new(1, 0),
                    Direction.Down => new(0, 1),
                    Direction.Left => new(-1, 0),
                    Direction.Up => new(0, -1),
                    _ => throw new InvalidOperationException()
                };
            }

            /// <summary>
            /// Changes direction using the move provided.
            /// </summary>
            /// <param name="direction">The current direction.</param>
            /// <param name="move">The move command.</param>
            /// <returns>The new direction.</returns>
            /// <exception cref="InvalidOperationException">Given an incorrect direction.</exception>
            protected static Direction ChangeDirection(Direction direction, string move)
            {
                if (move == "R")
                {
                    return direction switch
                    {
                        Direction.Right => Direction.Down,
                        Direction.Down => Direction.Left,
                        Direction.Left => Direction.Up,
                        Direction.Up => Direction.Right,
                        _ => throw new InvalidOperationException()
                    };
                }
                else
                {
                    return direction switch
                    {
                        Direction.Right => Direction.Up,
                        Direction.Up => Direction.Left,
                        Direction.Left => Direction.Down,
                        Direction.Down => Direction.Right,
                        _ => throw new InvalidOperationException()
                    };
                }
            }

            /// <summary>
            /// Gets the input from the path.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns>The input.</returns>
            private static (string[], string[]) GetInput(string path)
            {
                var lines = System.IO.File.ReadAllLines(path);

                var grid = lines.Take(lines.Length - 2).ToArray();

                var moves = lines[^1]
                    .Replace("L", " L ")
                    .Replace("R", " R ")
                    .Split();

                return new(grid, moves);
            }
        }

        /// <summary>
        /// Implemen ts a solver for a flat grid where the position wraps around.
        /// </summary>
        class FlatSolver : SolverBase
        {
            /// <summary>
            /// Creates a new solver for a flat grid.
            /// </summary>
            /// <param name="path">The input path.</param>
            public FlatSolver(string path)
                : base(path)
            {

            }

            /// <summary>
            /// Makes a move by wrapping the grid.
            /// </summary>
            /// <param name="position">The current position.</param>
            /// <param name="direction">The current direction.</param>
            /// <returns>The new position and direction.</returns>
            protected override (Position, Direction) MakeMove(Position position, Direction direction)
            {
                var delta = GetDelta(direction);
                
                int maxX = grid.Max(x => x.Length);
                int maxY = grid.Length;
                var max = new Position(maxX, maxY);

                var newPosition = (max + position + delta) % max;
                while (grid[newPosition.Y].Length <= newPosition.X
                    || grid[newPosition.Y][newPosition.X] == ' ')
                {
                    newPosition = (max + newPosition + delta) % max;
                }

                return new(newPosition, direction);
            }
        }

        /// <summary>
        /// Implements a solver where the grid is a cube.
        /// </summary>
        class CubeSolver : SolverBase
        {
            /// <summary>
            /// The faces of the cube.
            /// </summary>
            enum Face
            {
                Top,
                Bottom,
                Left,
                Right,
                Front,
                Back
            }

            /// <summary>
            /// Stores the adjacent faces for a cube.
            /// </summary>
            static readonly Dictionary<Face, Face[]> adjacentFaces = new()
            {
                { Face.Top, new[] { Face.Right, Face.Front, Face.Left, Face.Back } },
                { Face.Bottom, new[] { Face.Right, Face.Back, Face.Left, Face.Front } },
                { Face.Left, new[] { Face.Front, Face.Bottom, Face.Back, Face.Top } },
                { Face.Right, new[] { Face.Back, Face.Bottom, Face.Front, Face.Top } },
                { Face.Front, new[] { Face.Right, Face.Bottom, Face.Left, Face.Top } },
                { Face.Back, new[] { Face.Left, Face.Bottom, Face.Right, Face.Top } }                
            };

            /// <summary>
            /// The size of each face of the cube.
            /// </summary>
            private readonly int cubeSize;

            /// <summary>
            /// Stores the grid position and rotation for each face of the cube in the input grid.
            /// </summary>
            private readonly Dictionary<Face, (Position Position, int Rotation)> cubeFaces = new();

            /// <summary>
            /// Creates a new cube solver, and unwraps the 2d representation of the cube.
            /// </summary>
            /// <param name="path">The input path.</param>
            public CubeSolver(string path)
                : base(path)
            {
                cubeSize = grid.Min(x => x.Count(y => !char.IsWhiteSpace(y)));

                var unknownFaces = new HashSet<Position>();
                for (var r = 0; r < grid.Length; r+=cubeSize)
                {
                    for (var c = 0; c < grid[r].Length; c+=cubeSize)
                    {
                        if (grid[r][c] == ' ')
                        {
                            continue;
                        }

                        var cubePos = new Position(c, r) / cubeSize;
                        unknownFaces.Add(cubePos);
                    }
                }

                var frontier = new Queue<(Position Segment, Face Face, int FromDirection, Face FromFace)>();
                
                var startPosition = new Position(grid[0].IndexOf('.'), 0);
                var startCube = startPosition / cubeSize;
                unknownFaces.Remove(startCube);

                frontier.Enqueue((startCube, Face.Front, 1, Face.Top));
                while (frontier.Any())
                {
                    var current = frontier.Dequeue();

                    var relativeFrom = current.FromDirection + 2 % 4;
                    var offset = (4 + relativeFrom - Array.IndexOf(adjacentFaces[current.Face], current.FromFace)) % 4;
                    cubeFaces[current.Face] = (current.Segment, offset);

                    for (var i = 0; i < 4; i++)
                    {
                        var direction = GetDelta((Direction)i);
                        var segment = current.Segment + direction;
                        if (unknownFaces.Contains(segment))
                        {
                            frontier.Enqueue((segment, adjacentFaces[current.Face][(4 + i - offset) % 4], i, current.Face));
                            unknownFaces.Remove(segment);
                        }
                    }
                }
            }

            /// <summary>
            /// Makes a move on a cube, and modifies the position and direction when m oving onto
            /// a new cube face.
            /// </summary>
            /// <param name="position">The position.</param>
            /// <param name="direction">The direction.</param>
            /// <returns>The new position and direction.</returns>
            protected override (Position, Direction) MakeMove(Position position, Direction direction)
            {
                var delta = GetDelta(direction);
                var newPosition = position + delta;
                if (IsSameCube(position, newPosition))
                {
                    return (newPosition, direction);
                }

                var cubePos = position / cubeSize;
                var currentFace = cubeFaces.First(x => x.Value.Position == cubePos).Key;

                var faceRotations = cubeFaces[currentFace].Rotation;
                var newFace = adjacentFaces[currentFace][(4 + (int)direction - faceRotations) % 4];
                
                var relativeFrom = ((int)direction + 2) % 4;
                var positionOffset = (4 + Array.IndexOf(adjacentFaces[newFace], currentFace) - relativeFrom) % 4;
                var offset = cubeFaces[newFace].Rotation;
                var rotations = (positionOffset + offset) % 4;

                var localPosition = position - cubePos * cubeSize;
                for (var j = 0; j < rotations; j++)
                {
                    direction = RotateDirection(direction);
                    localPosition = RotatePosition(localPosition);
                }
                localPosition = GetNewCubePosition(localPosition, direction);

                cubePos = cubeFaces[newFace].Position;
                newPosition = localPosition + cubePos * cubeSize;

                return (newPosition, direction);
            }

            /// <summary>
            /// Gets the position of the cube face containing a point on the grid.
            /// </summary>
            /// <param name="position">The grid position.</param>
            /// <returns>The position of the cube face.</returns>
            private Position GetCubePosition(Position position) => position / cubeSize;

            private Position GetNewCubePosition(Position position, Direction direction)
            {
                return direction switch
                {
                    Direction.Right => new Position(0, position.Y),
                    Direction.Down => new Position(position.X, 0),
                    Direction.Left => new Position(cubeSize - 1, position.Y),
                    Direction.Up => new Position(position.X, cubeSize - 1),
                    _ => throw new Exception()
                };
            }

            /// <summary>
            /// Checks wheter both positions are on the same face of the cube.
            /// </summary>
            /// <param name="oldPosition">The old position.</param>
            /// <param name="newPosition">The new position.</param>
            /// <returns>True if the positions are on the same face.</returns>
            private bool IsSameCube(Position oldPosition, Position newPosition)
            {
                if (newPosition.X < 0 || newPosition.Y < 0)
                {
                    return false;
                }

                var oldCubePosition = GetCubePosition(oldPosition);
                var newCubePosition = GetCubePosition(newPosition);
                if (oldCubePosition != newCubePosition)
                {
                    return false;
                }

                return true;
            }

            /// <summary>
            /// Rotate a direction along 90 degrees.
            /// </summary>
            /// <param name="direction">The direction.</param>
            /// <returns>The new direction.</returns>
            private static Direction RotateDirection(Direction direction)
            {
                return ChangeDirection(direction, "R");
            }

            /// <summary>
            /// Rotate the position by 90 degrees.
            /// </summary>
            /// <param name="position">The position.</param>
            /// <returns>The new position.</returns>
            private Position RotatePosition(Position position)
            {
                var newX = cubeSize - 1 - position.Y;
                var newY = position.X;
                return new Position(newX, newY);
            }
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(6032, new FlatSolver("AOC2022/Day22/Example.txt").GetSolution());

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(97356, new FlatSolver("AOC2022/Day22/Input.txt").GetSolution());

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(5031, new CubeSolver("AOC2022/Day22/Example.txt").GetSolution());

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(120175, new CubeSolver("AOC2022/Day22/Input.txt").GetSolution());

        #endregion
    }
}
