using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solutions;

namespace Tests
{
    [TestClass]
    public class Day10Test
    {
        private readonly Day10Solution solver = new ();

        [TestMethod]
        public void TestExample1()
        {
            Assert.AreEqual(26397, solver.SolveExample1());
        }

        [TestMethod]
        public void TestSolution1()
        {
            Assert.AreEqual(265527, solver.SolvePart1());
        }

        [TestMethod]
        public void TestExample2()
        {
            Assert.AreEqual(288957, solver.SolveExample2());
        }

        [TestMethod]
        public void TestSolution2()
        {
            Assert.AreEqual(3969823589, solver.SolvePart2());
        }
    }
}
