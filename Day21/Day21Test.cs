using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solutions;

namespace Tests
{
    [TestClass]
    public class Day21Test
    {
        [TestMethod]
        public void TestExample1()
        {
            Assert.AreEqual(739785, Day21Solution.SolveExample1());
        }

        [TestMethod]
        public void TestSolution1()
        {
            Assert.AreEqual(864900, Day21Solution.SolvePart1());
        }

        [TestMethod]
        public void TestExample2()
        {
            Assert.AreEqual(444356092776315, Day21Solution.SolveExample2());
        }

        [TestMethod]
        public void TestSolution2()
        {
            Assert.AreEqual(575111835924670, Day21Solution.SolvePart2());
        }
    }
}
