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
            Assert.AreEqual(79, Day19Solution.SolveExample1());
        }

        [TestMethod]
        public void TestSolution1()
        {
            Assert.AreEqual(353, Day19Solution.SolvePart1());
        }

        [TestMethod]
        public void TestExample2()
        {
            Assert.AreEqual(3621, Day19Solution.SolveExample2());
        }

        [TestMethod]
        public void TestSolution2()
        {
            Assert.AreEqual(10832, Day19Solution.SolvePart2());
        }
    }
}
