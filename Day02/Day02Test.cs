using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solutions;

namespace Tests
{
    [TestClass]
    public class Day02Test
    {
        private readonly Day02Solution solver = new ();

        [TestMethod]
        public void TestExample1()
        {
            Assert.AreEqual(150, solver.SolveExample1());
        }

        [TestMethod]
        public void TestSolution1()
        {
            Assert.AreEqual(1507611, solver.SolvePart1());
        }

        [TestMethod]
        public void TestExample2()
        {
            Assert.AreEqual(900, solver.SolveExample2());
        }

        [TestMethod]
        public void TestSolution2()
        {
            Assert.AreEqual(1880593125, solver.SolvePart2());
        }
    }
}
