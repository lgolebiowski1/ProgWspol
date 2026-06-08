namespace TP.ConcurrentProgramming.Data.Test
{
    [TestClass]
    public class DataImplementationUnitTest
    {
        [TestMethod]
        public void Constructor_NoBalls()
        {
            using DataImplementation instance = new(new NullDiagnosticLogger());
            int count = -1;
            instance.CheckNumberOfBalls(x => count = x);
            Assert.AreEqual(0, count);
        }

        [TestMethod]
        public void Constructor_UsesNullLogger_ForTests()
        {
            using DataImplementation instance = new(new NullDiagnosticLogger());
            IDiagnosticLogger? logger = null;
            instance.CheckLogger(l => logger = l);
            Assert.IsNotNull(logger);
            Assert.IsInstanceOfType(logger, typeof(NullDiagnosticLogger));
        }

        [TestMethod]
        public void Dispose_SetsDisposedFlag()
        {
            DataImplementation instance = new(new NullDiagnosticLogger());
            bool disposed = false;
            instance.CheckObjectDisposed(x => disposed = x);
            Assert.IsFalse(disposed);

            instance.Dispose();
            instance.CheckObjectDisposed(x => disposed = x);
            Assert.IsTrue(disposed);
        }

        [TestMethod]
        public void Dispose_ThrowsOnSecondCall()
        {
            DataImplementation instance = new(new NullDiagnosticLogger());
            instance.Dispose();
            bool threw = false;
            try { instance.Dispose(); } catch (ObjectDisposedException) { threw = true; }
            Assert.IsTrue(threw);
        }

        [TestMethod]
        public void Start_ThrowsWhenDisposed()
        {
            DataImplementation instance = new(new NullDiagnosticLogger());
            instance.Dispose();
            bool threw = false;
            try { instance.Start(1, (p, b) => { }); } catch (ObjectDisposedException) { threw = true; }
            Assert.IsTrue(threw);
        }

        [TestMethod]
        public void Start_CreatesBalls()
        {
            using DataImplementation instance = new(new NullDiagnosticLogger());
            int called = 0;
            instance.Start(5, (pos, ball) =>
            {
                called++;
                Assert.IsNotNull(ball);
                Assert.IsTrue(pos.x > 0);
                Assert.IsTrue(pos.y > 0);
            });
            Assert.AreEqual(5, called);
            instance.CheckNumberOfBalls(n => Assert.AreEqual(5, n));
        }

        [TestMethod]
        public void Stop_ClearsBalls()
        {
            using DataImplementation instance = new(new NullDiagnosticLogger());
            instance.Start(3, (p, b) => { });
            instance.Stop();
            instance.CheckNumberOfBalls(n => Assert.AreEqual(0, n));
        }

        [TestMethod]
        public void Logger_ReceivesEntriesAfterStart()
        {
            CountingLogger logger = new();
            using DataImplementation instance = new(logger);
            instance.Start(2, (p, b) => { });
            Thread.Sleep(200);
            instance.Stop();

            Assert.IsTrue(logger.Count > 0, "Logger should have received entries");
        }
    }
}
