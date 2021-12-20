using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solutions;

namespace Tests
{
    [TestClass]
    public class Day20Test
    {
        [TestMethod]
        public void TestExample1()
        {
            Assert.AreEqual(35, Day20Solution.SolveExample1());
        }

        [TestMethod]
        public void TestSolution1()
        {
            Assert.AreEqual(5819, Day20Solution.SolvePart1());
        }

        [TestMethod]
        public void TestExample2()
        {
            Assert.AreEqual(3351, Day20Solution.SolveExample2());
        }

        [TestMethod]
        public void TestSolution2()
        {
            Assert.AreEqual(18516, Day20Solution.SolvePart2());
        }
    }
}
