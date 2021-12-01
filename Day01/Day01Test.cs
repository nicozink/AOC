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
            Assert.AreEqual(7, day01.SolveExample1());
        }

        [TestMethod]
        public void TestSolution1()
        {
            Assert.AreEqual(1583, day01.SolvePart1());
        }

        [TestMethod]
        public void TestExample2()
        {
            Assert.AreEqual(5, day01.SolveExample2());
        }

        [TestMethod]
        public void TestSolution2()
        {
            Assert.AreEqual(1627, day01.SolvePart2());
        }
    }
}
