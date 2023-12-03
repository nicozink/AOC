using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AOC2023
{
    /// <summary>
    /// Solution for day 1:
    /// https://adventofcode.com/2023/day/2
    /// </summary>
    [TestClass]
    public class Day02
    {
        record Cubes(int Red, int Green, int Blue);

        record Game(int Id, Cubes[] Cubes);
        
        private static IEnumerable<Game> ReadInput(string path)
        {
            var lines = System.IO.File.ReadAllLines(path);

            foreach (var line in lines)
            {
                var processedLine = line
                    .Replace("Game ", "")
                    .Replace(": ", ":")
                    .Replace("; ", ";")
                    .Replace(", ", ",");

                var gameParts = processedLine.Split(":");

                int id = int.Parse(gameParts[0]);
                var game = gameParts[1];

                var cubes = new List<Cubes>();

                var picks = game.Split(";");
                foreach (var pick in picks)
                {
                    int redValue = 0;
                    int blueValue = 0;
                    int greenValue = 0;

                    var pickItems = pick.Split(",");
                    foreach (var pickItem in pickItems)
                    {
                        var splitItems = pickItem.Split();

                        var cubeCount = int.Parse(splitItems[0]);
                        var cubeType = splitItems[1];

                        if (cubeType == "red")
                        {
                            redValue = cubeCount;
                        }
                        else if (cubeType == "green")
                        {
                            greenValue = cubeCount;
                        }
                        else if (cubeType == "blue")
                        {
                            blueValue = cubeCount;
                        }
                    }

                    cubes.Add(new Cubes(redValue, greenValue, blueValue));
                }

                yield return new Game(id, cubes.ToArray());
            }
        }

        private int SumPossibleGames(string path)
        {
            var games = ReadInput(path);

            var redCubes = 12;
            var greenCubes = 13;
            var blueCubes = 14;

            int sum = 0;
            foreach (var game in games)
            {
                bool isPossible = true;

                foreach (var pick in game.Cubes)
                {
                    if (pick.Red > redCubes ||
                        pick.Green > greenCubes ||
                        pick.Blue > blueCubes)
                    {
                        isPossible = false;
                        break;
                    }
                }

                if (isPossible)
                {
                    sum += game.Id;
                }
            }

            return sum;
        }

        private int CalculateCubePower(string path)
        {
            var games = ReadInput(path);

            int sum = 0;
            foreach (var game in games)
            {
                var redCubes = 0;
                var greenCubes = 0;
                var blueCubes = 0;

                foreach (var pick in game.Cubes)
                {
                    if (pick.Red > redCubes)
                    {
                        redCubes = pick.Red;
                    }

                    if (pick.Green > greenCubes)
                    {
                        greenCubes = pick.Green;
                    }

                    if (pick.Blue > blueCubes)
                    {
                        blueCubes = pick.Blue;
                    }
                }

                sum += redCubes * greenCubes * blueCubes;
            }

            return sum;
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(8, SumPossibleGames("AOC2023/Day02/Example.txt"));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(2076, SumPossibleGames("AOC2023/Day02/Input.txt"));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(2286, CalculateCubePower("AOC2023/Day02/Example.txt"));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(0, CalculateCubePower("AOC2023/Day02/Input.txt"));

        #endregion
    }
}
