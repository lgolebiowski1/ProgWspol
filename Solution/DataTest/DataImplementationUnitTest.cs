namespace TP.ConcurrentProgramming.Data.Test
{
    [TestClass]
    public class DataImplementationUnitTest
    {
        [TestMethod]
        public void ConstructorTestMethod()
        {
            using DataImplementation instance = new();
            int count = -1;
            instance.CheckNumberOfBalls(x => count = x);
            Assert.AreEqual<int>(0, count);
        }

        [TestMethod]
        public void DisposeTestMethod()
        {
            DataImplementation instance = new();
            bool disposed = false;
            instance.CheckObjectDisposed(x => disposed = x);
            Assert.IsFalse(disposed);

            instance.Dispose();
            instance.CheckObjectDisposed(x => disposed = x);
            Assert.IsTrue(disposed);

            bool threw = false;
            try { instance.Dispose(); } catch (ObjectDisposedException) { threw = true; }
            Assert.IsTrue(threw, "Second Dispose should throw ObjectDisposedException");

            threw = false;
            try { instance.Start(1, (p, b) => { }); } catch (ObjectDisposedException) { threw = true; }
            Assert.IsTrue(threw, "Start after Dispose should throw ObjectDisposedException");
        }

        [TestMethod]
        public void StartTestMethod()
        {
            using DataImplementation instance = new();
            int callbackCount = 0;
            const int ballsToCreate = 10;

            instance.Start(ballsToCreate, (pos, ball) =>
            {
                callbackCount++;
                Assert.IsTrue(pos.x >= 0);
                Assert.IsTrue(pos.y >= 0);
                Assert.IsNotNull(ball);
            });

            Assert.AreEqual<int>(ballsToCreate, callbackCount);
            instance.CheckNumberOfBalls(x => Assert.AreEqual<int>(ballsToCreate, x));
        }
    }
}
