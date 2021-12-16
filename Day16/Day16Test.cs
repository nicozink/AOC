using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solutions;

namespace Tests
{
    [TestClass]
    public class Day16Test
    {
        private readonly Day16Solution solver = new ();

        [TestMethod]
        public void TestExample01()
        {
            Assert.AreEqual(6, solver.CountVersionNumbers("D2FE28"));
        }

        [TestMethod]
        public void TestExample02()
        {
            Assert.AreEqual(9, solver.CountVersionNumbers("38006F45291200"));
        }

        [TestMethod]
        public void TestExample03()
        {
            Assert.AreEqual(14, solver.CountVersionNumbers("EE00D40C823060"));
        }

        [TestMethod]
        public void TestExample04()
        {
            Assert.AreEqual(16, solver.CountVersionNumbers("8A004A801A8002F478"));
        }

        [TestMethod]
        public void TestExample05()
        {
            Assert.AreEqual(12, solver.CountVersionNumbers("620080001611562C8802118E34"));
        }

        [TestMethod]
        public void TestExample06()
        {
            Assert.AreEqual(23, solver.CountVersionNumbers("C0015000016115A2E0802F182340"));
        }

        [TestMethod]
        public void TestExample07()
        {
            Assert.AreEqual(31, solver.CountVersionNumbers("A0016C880162017C3686B18A3D4780"));
        }

        [TestMethod]
        public void TestSolution1()
        {
            Assert.AreEqual(949, solver.SolvePart1());
        }

        [TestMethod]
        public void TestExample08()
        {
            Assert.AreEqual(3, solver.DecodeBitPattern("C200B40A82"));
        }

        [TestMethod]
        public void TestExample09()
        {
            Assert.AreEqual(54, solver.DecodeBitPattern("04005AC33890"));
        }

        [TestMethod]
        public void TestExample10()
        {
            Assert.AreEqual(7, solver.DecodeBitPattern("880086C3E88112"));
        }

        [TestMethod]
        public void TestExample11()
        {
            Assert.AreEqual(9, solver.DecodeBitPattern("CE00C43D881120"));
        }

        [TestMethod]
        public void TestExample12()
        {
            Assert.AreEqual(1, solver.DecodeBitPattern("D8005AC2A8F0"));
        }

        [TestMethod]
        public void TestExample13()
        {
            Assert.AreEqual(0, solver.DecodeBitPattern("F600BC2D8F"));
        }

        [TestMethod]
        public void TestExample14()
        {
            Assert.AreEqual(0, solver.DecodeBitPattern("9C005AC2F8F0"));
        }

        [TestMethod]
        public void TestExample15()
        {
            Assert.AreEqual(1, solver.DecodeBitPattern("9C0141080250320F1802104A08"));
        }

        [TestMethod]
        public void TestSolution2()
        {
            Assert.AreEqual(1114600142730, solver.SolvePart2());
        }
    }
}
