using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solutions;

namespace Tests
{
    [TestClass]
    public class Day18Test
    {
        [TestMethod]
        public void TestExample1()
        {
            Assert.AreEqual(4140, Day18Solution.SolveExample1());
        }

        [TestMethod]
        public void TestSolution1()
        {
            Assert.AreEqual(4132, Day18Solution.SolvePart1());
        }

        [TestMethod]
        public void TestExample2()
        {
            Assert.AreEqual(3993, Day18Solution.SolveExample2());
        }

        [TestMethod]
        public void TestSolution2()
        {
            Assert.AreEqual(4685, Day18Solution.SolvePart2());
        }
    }
}
