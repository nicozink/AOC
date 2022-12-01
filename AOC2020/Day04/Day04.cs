using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AOC2020
{
    /// <summary>
    /// Solution for day 4:
    /// https://adventofcode.com/2020/day/4
    /// </summary>
    [TestClass]
    public class Day04
    {
        public int GetSolution1(String path)
        {
            var passports = ReadPassports(path);

            return passports.Count(x => IsPassportComplete(x));
        }

        public long GetSolution2(String path)
        {
            var passports = ReadPassports(path);

            return passports.Count(x => IsPassportValid(x));
        }

        IEnumerable<Dictionary<String, String>> ReadPassports(String path)
        {
            var input = System.IO.File.ReadLines(path);

            var currentPassport = new Dictionary<String, String>();
            foreach (var line in input)
            {
                if (line  == "")
                {
                    yield return currentPassport;
                    currentPassport.Clear();
                }
                else
                {
                    var keyValues = line.Split(' ').Select(x => x.Split(':'));

                    foreach (var keyValue in keyValues)
                    {
                        currentPassport.Add(keyValue[0], keyValue[1]);
                    }
                }
            }

            if (currentPassport.Count != 0)
            {
                yield return currentPassport;
            }    
        }

        bool IsPassportComplete(Dictionary<String, String> passport)
        {
            String[] requiredFields = {
                "byr",
                "iyr",
                "eyr",
                "hgt",
                "hcl",
                "ecl",
                "pid"
            };

            foreach (var requiredField in requiredFields)
            {
                if (!passport.ContainsKey(requiredField))
                {
                    return false;
                }
            }

            return true;
        }

        bool IsPassportValid(Dictionary<String, String> passport)
        {
            if (!IsPassportComplete(passport))
            {
                return false;
            }

            var byr = int.Parse(passport["byr"]);
            if (byr < 1920 || byr > 2002)
            {
                return false;
            }

            var iyr = int.Parse(passport["iyr"]);
            if (iyr < 2010 || iyr > 2020)
            {
                return false;
            }

            var eyr = int.Parse(passport["eyr"]);
            if (eyr < 2020 || eyr > 2030)
            {
                return false;
            }

            var hgt = passport["hgt"];

            if (hgt.Contains("cm"))
            {
                int height = int.Parse(hgt.Replace("cm", ""));

                if (height < 150 || height > 193)
                {
                    return false;
                }
            }
            else if (hgt.Contains("in"))
            {
                int height = int.Parse(hgt.Replace("in", ""));

                if (height < 59 || height > 76)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            var hcl = passport["hcl"];

            if (hcl[0] != '#')
            {
                return false;
            }

            var hairCode = hcl.Substring(1);
            if (hairCode.Length != 6 || hairCode.Count(x => !((x >= '0' && x <= '9') || (x >= 'a' && x <= 'f'))) > 0)
            {
                return false;
            }

            var ecl = passport["ecl"];
            String[] validEyeColour = { "amb", "blu", "brn", "gry", "grn", "hzl", "oth" };
            if (!validEyeColour.Contains(ecl))
            {
                return false;
            }

            var pid = passport["pid"];

            if (pid.Length != 9)
            {
                return false;
            }

            if (pid.Count(x => x < '0' || x > '9') > 0)
            {
                return false;
            }

            return true;
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(2, GetSolution1("AOC2020/Day04/Example1.txt"));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(237, GetSolution1("AOC2020/Day04/Input.txt"));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(4, GetSolution2("AOC2020/Day04/Example2.txt"));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(172, GetSolution2("AOC2020/Day04/Input.txt"));

        #endregion
    }
}
