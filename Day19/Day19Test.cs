using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solutions;

namespace Tests
{
    [TestClass]
    public class Day19Test
    {
        [TestMethod]
        public void TestExample1()
        {
            Assert.AreEqual(0, Day19Solution.SolveExample1());
        }

        [TestMethod]
        public void TestSolution1()
        {
            Assert.AreEqual(0, Day19Solution.SolvePart1());
        }

        [TestMethod]
        public void TestExample2()
        {
            Assert.AreEqual(0, Day19Solution.SolveExample2());
        }

        [TestMethod]
        public void TestSolution2()
        {
            Assert.AreEqual(0, Day19Solution.SolvePart2());
        }
    }
}
