using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solutions;

namespace Tests
{
    [TestClass]
    public class Day09Test
    {
        private readonly Day09Solution solver = new ();

        [TestMethod]
        public void TestExample1()
        {
            Assert.AreEqual(15, solver.SolveExample1());
        }

        [TestMethod]
        public void TestSolution1()
        {
            Assert.AreEqual(436, solver.SolvePart1());
        }

        [TestMethod]
        public void TestExample2()
        {
            Assert.AreEqual(1134, solver.SolveExample2());
        }

        [TestMethod]
        public void TestSolution2()
        {
            Assert.AreEqual(1317792, solver.SolvePart2());
        }
    }
}
