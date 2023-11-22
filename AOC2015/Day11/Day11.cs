using Microsoft.VisualStudio.TestTools.UnitTesting;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AOC2015
{
    /// <summary>
    /// Solution for day 11:
    /// https://adventofcode.com/2015/day/11
    /// </summary>
    [TestClass]
    public class Day11
    {
        private bool IsCondition1Valid(StringBuilder password)
        {
            for (int x = 0; x < password.Length - 3; x++)
            {
                char first = password[x];
                char second = password[x + 1];
                char third = password[x + 2];

                if (second == first + 1 && third == first + 2)
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsCondition2Valid(StringBuilder password)
        {
            for (int x = 0; x < password.Length; x++)
            {
                char currentChar = password[x];
                
                if (currentChar == 'i' ||
                    currentChar == 'o' ||
                    currentChar == 'l')
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsCondition3Valid(StringBuilder password)
        {
            int numPairs = 0;
            for (int x = 0; x < password.Length - 1; x++)
            {
                char first = password[x];
                char second = password[x + 1];
                
                if (first == second)
                {
                    if (++numPairs == 2)
                    {
                        return true;
                    }

                    x++;
                }
            }

            return false;
        }

        private void Increment(StringBuilder password, int position = 7)
        {
            if (password[position] == 'z')
            {
                Increment(password, position - 1);
            }
            else
            {
                char currentChar = password[position];
                char nextChar = (char)(currentChar + 1);

                password[position] = nextChar;

                for (int x = position + 1; x < password.Length; x++)
                {
                    password[x] = 'a';
                }
            }
        }

        private string GetNextPassword(string password)
        {
            StringBuilder stringBuilder = new(password);

            bool isPasswordValid = false;
            while (!isPasswordValid)
            {
                Increment(stringBuilder);

                isPasswordValid = IsCondition1Valid(stringBuilder) &&
                    IsCondition2Valid(stringBuilder) &&
                    IsCondition3Valid(stringBuilder);
            }
            
            return stringBuilder.ToString();
        }

        #region Solve Problems

        [TestMethod]
        public void TestExample1()
        {
            Assert.IsTrue(IsCondition1Valid(new("hijklmmn")));
            Assert.IsFalse(IsCondition2Valid(new("hijklmmn")));

            Assert.IsFalse(IsCondition1Valid(new("abbceffg")));
            Assert.IsTrue(IsCondition3Valid(new("abbceffg")));

            Assert.IsFalse(IsCondition3Valid(new("abbcegjk")));

            Assert.AreEqual("abcdffaa", GetNextPassword("abcdefgh"));
            Assert.AreEqual("ghjaabcc", GetNextPassword("ghijklmn"));
        }

        [TestMethod]
        public void TestSolution1() => Assert.AreEqual("vzbxxyzz", GetNextPassword("vzbxkghb"));

        [TestMethod]
        public void TestSolution2() => Assert.AreEqual("vzbxxyzz", GetNextPassword("vzbxxyzz"));

        #endregion
    }
}
