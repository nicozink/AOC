using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solutions;

namespace Tests
{
    [TestClass]
    public class Day14Test
    {
        private readonly Day14Solution solver = new ();

        [TestMethod]
        public void TestExample1()
        {
            Assert.AreEqual(1588, solver.SolveExample1());
        }

        [TestMethod]
        public void TestSolution1()
        {
            Assert.AreEqual(3906, solver.SolvePart1());
        }

        [TestMethod]
        public void TestExample2()
        {
            Assert.AreEqual(2188189693529, solver.SolveExample2());
        }

        [TestMethod]
        public void TestSolution2()
        {
            Assert.AreEqual(4441317262452, solver.SolvePart2());
        }
    }
}
