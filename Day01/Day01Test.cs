using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solutions;

namespace Tests
{
    [TestClass]
    public class Day01Test
    {
        private Day01 day01 = new Day01();

        [TestMethod]
        public void TestExample1()
        {
            Assert.AreEqual(day01.SolveExample1(), 0);
        }

        [TestMethod]
        public void TestSolution1()
        {
            Assert.AreEqual(day01.SolvePart1(), 0);
        }

        [TestMethod]
        public void TestExample2()
        {
            Assert.AreEqual(day01.SolveExample2(), 0);
        }

        [TestMethod]
        public void TestSolution2()
        {
            Assert.AreEqual(day01.SolvePart2(), 0);
        }
    }
}
