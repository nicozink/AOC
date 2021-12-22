using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solutions;

namespace Tests
{
    [TestClass]
    public class Day22Test
    {
        [TestMethod]
        public void TestExample1()
        {
            Assert.AreEqual(39, Day22Solution.SolveExample1());
        }

        [TestMethod]
        public void TestExample2()
        {
            Assert.AreEqual(590784, Day22Solution.SolveExample2());
        }

        [TestMethod]
        public void TestSolution1()
        {
            Assert.AreEqual(602574, Day22Solution.SolvePart1());
        }

        [TestMethod]
        public void TestExample3()
        {
            Assert.AreEqual(474140, Day22Solution.SolveExample3());
        }

        [TestMethod]
        public void TestExample4()
        {
            Assert.AreEqual(2758514936282235, Day22Solution.SolveExample4());
        }

        [TestMethod]
        public void TestSolution2()
        {
            Assert.AreEqual(1288707160324706, Day22Solution.SolvePart2());
        }
    }
}
