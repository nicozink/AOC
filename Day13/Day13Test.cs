using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solutions;

namespace Tests
{
    [TestClass]
    public class Day13Test
    {
        private readonly Day13Solution solver = new ();

        [TestMethod]
        public void TestExample1()
        {
            Assert.AreEqual(17, solver.SolveExample1());
        }

        [TestMethod]
        public void TestSolution1()
        {
            Assert.AreEqual(731, solver.SolvePart1());
        }
    }
}
