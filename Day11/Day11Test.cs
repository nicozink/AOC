using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solutions;

namespace Tests
{
    [TestClass]
    public class Day11Test
    {
        private readonly Day11Solution solver = new ();

        [TestMethod]
        public void TestExample1()
        {
            Assert.AreEqual(1656, solver.SolveExample1());
        }

        [TestMethod]
        public void TestSolution1()
        {
            Assert.AreEqual(1743, solver.SolvePart1());
        }

        [TestMethod]
        public void TestExample2()
        {
            Assert.AreEqual(195, solver.SolveExample2());
        }

        [TestMethod]
        public void TestSolution2()
        {
            Assert.AreEqual(364, solver.SolvePart2());
        }
    }
}
