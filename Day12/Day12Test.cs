using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solutions;

namespace Tests
{
    [TestClass]
    public class Day12Test
    {
        private readonly Day12Solution solver = new ();

        [TestMethod]
        public void TestExample1()
        {
            Assert.AreEqual(10, solver.SolveExample1());
        }

        [TestMethod]
        public void TestExample2()
        {
            Assert.AreEqual(19, solver.SolveExample2());
        }

        [TestMethod]
        public void TestExample3()
        {
            Assert.AreEqual(226, solver.SolveExample3());
        }

        [TestMethod]
        public void TestSolution1()
        {
            Assert.AreEqual(3463, solver.SolvePart1());
        }

        [TestMethod]
        public void TestExample4()
        {
            Assert.AreEqual(36, solver.SolveExample4());
        }

        [TestMethod]
        public void TestExample5()
        {
            Assert.AreEqual(103, solver.SolveExample5());
        }

        [TestMethod]
        public void TestExample6()
        {
            Assert.AreEqual(3509, solver.SolveExample6());
        }

        [TestMethod]
        public void TestSolution2()
        {
            Assert.AreEqual(91533, solver.SolvePart2());
        }
    }
}
