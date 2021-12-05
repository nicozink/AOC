using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace Solutions
{
    /// <summary>
    /// Solution for day 5:
    /// https://adventofcode.com/2021/day/5
    /// </summary>
    [SolutionClass(Day = 5)]
    public class Day05Solution
    {
        /// <summary>
        /// Stores a cloud consisting of start and end positions.
        /// </summary>
        struct VentCloud
        {
            public (int x, int y) Start { get; set; }

            public (int x, int y) End { get; set; }
        }

        /// <summary>
        /// Reads the inut from a file. Each line consists
        /// of numbers in the for 'x1,y1 -> x2,y2'.
        /// </summary>
        /// <param name="path">The input file.</param>
        /// <returns>The clouds.</returns>
        IEnumerable<VentCloud> ReadInput(String path)
        {
            var lines = System.IO.File.ReadAllLines(path);

            foreach (var line in lines)
            {
                var splitLine = line.Replace(" -> ", " ")
                    .Split();

                var start = splitLine[0].Split(',')
                    .Select(int.Parse)
                    .ToArray();
                
                var end = splitLine[1].Split(',')
                    .Select(int.Parse)
                    .ToArray();

                yield return new()
                {
                    Start = (start[0], start[1]),
                    End = (end[0], end[1])
                };
            }
        }

        /// <summary>
        /// Gets the coordinates for a cloud based on the
        /// start and end points. This function will return
        /// the points in between (including start + end).
        /// </summary>
        /// <param name="cloud">The cloud.</param>
        /// <param name="expandDiagonals">Whether to expand diagonals.</param>
        /// <returns>The expanded cloud coordinates.</returns>
        IEnumerable<(int x, int y)> GetCoordinates(VentCloud cloud, bool expandDiagonals)
        {
            int deltax = Math.Sign(cloud.End.x - cloud.Start.x);
            int deltay = Math.Sign(cloud.End.y - cloud.Start.y);

            if (!expandDiagonals && deltax != 0 && deltay != 0)
            {
                yield break;
            }

            int currentx = cloud.Start.x;
            int currenty = cloud.Start.y;

            while (currentx != cloud.End.x || currenty != cloud.End.y)
            {
                yield return (currentx, currenty);

                currentx += deltax;
                currenty += deltay;
            }

            // We only loop until the destination, so need to send that as well.
            yield return (currentx, currenty);
        }

        /// <summary>
        /// Gets the coordinates for each cloud based on their
        /// start and end points. This function will return
        /// all points in between (including each start + end).
        /// </summary>
        /// <param name="clouds">The clouds.</param>
        /// <param name="expandDiagonals">Whether to expand diagonals.</param>
        /// <returns>The expanded cloud coordinates.</returns>
        IEnumerable<(int x, int y)> GetCoordinates(IEnumerable<VentCloud> clouds, bool expandDiagonals)
        {
            foreach (var cloud in clouds)
            {
                foreach (var coord in GetCoordinates(cloud, expandDiagonals))
                {
                    yield return coord;
                }
            }
        }
        
        /// <summary>
        /// Counts the danger zones (places with two or more overlapping
        /// clouds).
        /// </summary>
        /// <param name="path">The input file.</param>
        /// <param name="expandDiagonals">Whether to include diagonals.</param>
        /// <returns>Thenumber of danger zones.</returns>
        int CountDangerZones(String path, bool expandDiagonals)
        {
            var clouds = ReadInput(path);

            var coords = GetCoordinates(clouds, expandDiagonals);

            return coords.GroupBy(x => x)
                .Select(x => x.Count())
                .Count(x => x >= 2);
        }

        #region Solve Problems

        public int SolveExample1() => CountDangerZones("Day05/Example.txt", false);

        [SolutionMethod(Part = 1)]
        public int SolvePart1() => CountDangerZones("Day05/input.txt", false);

        public int SolveExample2() => CountDangerZones("Day05/Example.txt", true);

        [SolutionMethod(Part = 2)]
        public int SolvePart2() => CountDangerZones("Day05/input.txt", true);

        #endregion
    }
}
