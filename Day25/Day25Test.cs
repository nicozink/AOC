using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solutions;

namespace Tests
{
    [TestClass]
    public class Day25Test
    {
        [TestMethod]
        public void TestExample()
        {
            Assert.AreEqual(58, Day25Solution.SolveExample());
        }

        [TestMethod]
        public void TestSolution()
        {
            Assert.AreEqual(482, Day25Solution.Solve());
        }
    }
}
