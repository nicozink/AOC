using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solutions;

namespace Tests
{
    [TestClass]
    public class Day04Test
    {
        private readonly Day04Solution solver = new ();

        [TestMethod]
        public void TestExample1()
        {
            Assert.AreEqual(4512, solver.SolveExample1());
        }

        [TestMethod]
        public void TestSolution1()
        {
            Assert.AreEqual(33462, solver.SolvePart1());
        }

        [TestMethod]
        public void TestExample2()
        {
            Assert.AreEqual(1924, solver.SolveExample2());
        }

        [TestMethod]
        public void TestSolution2()
        {
            Assert.AreEqual(30070, solver.SolvePart2());
        }
    }
}
