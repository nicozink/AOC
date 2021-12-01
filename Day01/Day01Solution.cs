using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace Solutions
{
    /// <summary>
    /// Solution for day 1:
    /// https://adventofcode.com/2021/day/1
    /// </summary>
    [SolutionClass(Day = 1)]
    public class Day01
    {
        public int GetSolution1(String path)
        {
            return 0;
        }

        public int GetSolution2(String path)
        {
            return 0;
        }

        #region Solve Problems

        public int SolveExample1()
        {
            return GetSolution1("Day01/Example.txt");
        }

        public int SolveExample2()
        {
            return GetSolution2("Day01/Example.txt");
        }

        [SolutionMethod(Part = 1)]
        public int SolvePart1()
        {
            return GetSolution1("Day01/Input.txt");
        }
        
        [SolutionMethod(Part = 2)]
        public int SolvePart2()
        {
            return GetSolution2("Day01/Input.txt");
        }

        #endregion
    }
}
