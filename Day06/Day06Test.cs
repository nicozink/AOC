using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solutions;

namespace Tests
{
    [TestClass]
    public class Day06Test
    {
        private readonly Day06Solution solver = new ();

        [TestMethod]
        public void TestExample1()
        {
            Assert.AreEqual(5934, solver.SolveExample1());
        }

        [TestMethod]
        public void TestSolution1()
        {
            Assert.AreEqual(390923, solver.SolvePart1());
        }

        [TestMethod]
        public void TestExample2()
        {
            Assert.AreEqual(26984457539, solver.SolveExample2());
        }

        [TestMethod]
        public void TestSolution2()
        {
            Assert.AreEqual(1749945484935, solver.SolvePart2());
        }
    }
}
