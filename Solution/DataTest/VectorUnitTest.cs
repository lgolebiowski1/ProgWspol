namespace TP.ConcurrentProgramming.Data.Test
{
    [TestClass]
    public class VectorUnitTest
    {
        [TestMethod]
        public void ConstructorTestMethod()
        {
            Random rng = new();
            double x = rng.NextDouble();
            double y = rng.NextDouble();
            Vector v = new(x, y);
            Assert.AreEqual<double>(x, v.x);
            Assert.AreEqual<double>(y, v.y);
        }
    }
}
