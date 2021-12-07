using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solutions;

namespace Tests
{
    [TestClass]
    public class Day07Test
    {
        private readonly Day07Solution solver = new ();

        [TestMethod]
        public void TestExample1()
        {
            Assert.AreEqual(37, solver.SolveExample1());
        }

        [TestMethod]
        public void TestSolution1()
        {
            Assert.AreEqual(337833, solver.SolvePart1());
        }

        [TestMethod]
        public void TestExample2()
        {
            Assert.AreEqual(168, solver.SolveExample2());
        }

        [TestMethod]
        public void TestSolution2()
        {
            Assert.AreEqual(96678050, solver.SolvePart2());
        }
    }
}
