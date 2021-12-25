using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solutions;

namespace Tests
{
    [TestClass]
    public class Day24Test
    {
        [TestMethod]
        public void TestSolution1()
        {
            Assert.AreEqual("53999995829399", Day24Solution.SolvePart1());
        }

        [TestMethod]
        public void TestSolution2()
        {
            Assert.AreEqual("11721151118175", Day24Solution.SolvePart2());
        }
    }
}
