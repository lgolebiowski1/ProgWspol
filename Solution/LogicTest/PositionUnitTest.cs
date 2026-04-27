using TP.ConcurrentProgramming.BusinessLogic;

namespace TP.ConcurrentProgramming.BusinessLogic.Test
{
    [TestClass]
    public class PositionUnitTest
    {
        [TestMethod]
        public void ConstructorTestMethod()
        {
            Random rng = new();
            double x = rng.NextDouble();
            double y = rng.NextDouble();
            IPosition pos = new Position(x, y);
            Assert.AreEqual<double>(x, pos.x);
            Assert.AreEqual<double>(y, pos.y);
        }
    }
}
