using DataAPI = TP.ConcurrentProgramming.Data.DataAbstractAPI;
using DataIVector = TP.ConcurrentProgramming.Data.IVector;
using DataIBall = TP.ConcurrentProgramming.Data.IBall;

namespace TP.ConcurrentProgramming.BusinessLogic.Test
{
    [TestClass]
    public class BusinessLogicImplementationUnitTest
    {
        [TestMethod]
        public void ConstructorTestMethod()
        {
            using BusinessLogicImplementation instance = new(new DataLayerConstructorFixture());
            bool disposed = true;
            instance.CheckObjectDisposed(x => disposed = x);
            Assert.IsFalse(disposed);
        }

        [TestMethod]
        public void DisposeTestMethod()
        {
            DataLayerDisposeFixture dataFixture = new();
            BusinessLogicImplementation instance = new(dataFixture);
            Assert.IsFalse(dataFixture.Disposed);

            instance.Dispose();

            bool disposed = false;
            instance.CheckObjectDisposed(x => disposed = x);
            Assert.IsTrue(disposed);
            Assert.IsTrue(dataFixture.Disposed);

            bool threw = false;
            try { instance.Dispose(); } catch (ObjectDisposedException) { threw = true; }
            Assert.IsTrue(threw, "Second Dispose should throw ObjectDisposedException");

            threw = false;
            try { instance.Start(0, (p, b) => { }); } catch (ObjectDisposedException) { threw = true; }
            Assert.IsTrue(threw, "Start after Dispose should throw");
        }

        [TestMethod]
        public void StartTestMethod()
        {
            DataLayerStartFixture dataFixture = new();
            using BusinessLogicImplementation instance = new(dataFixture);

            int called = 0;
            instance.Start(10, (pos, ball) =>
            {
                called++;
                Assert.IsNotNull(pos);
                Assert.IsNotNull(ball);
            });

            Assert.AreEqual<int>(1, called);
            Assert.IsTrue(dataFixture.StartCalled);
            Assert.AreEqual<int>(10, dataFixture.NumberOfBallsCreated);
        }

        #region Fixtures — own implementations of DataAbstractAPI (no external mocks)

        private class DataLayerConstructorFixture : DataAPI
        {
            public override double TableWidth => 800;
            public override double TableHeight => 500;
            public override void Dispose() { }
            public override void Start(int n, Action<DataIVector, DataIBall> h)
                => throw new NotImplementedException();
        }

        private class DataLayerDisposeFixture : DataAPI
        {
            internal bool Disposed = false;
            public override double TableWidth => 800;
            public override double TableHeight => 500;
            public override void Dispose() => Disposed = true;
            public override void Start(int n, Action<DataIVector, DataIBall> h)
                => throw new NotImplementedException();
        }

        private class DataLayerStartFixture : DataAPI
        {
            internal bool StartCalled = false;
            internal int NumberOfBallsCreated = -1;
            public override double TableWidth => 800;
            public override double TableHeight => 500;
            public override void Dispose() { }
            public override void Start(int n, Action<DataIVector, DataIBall> h)
            {
                StartCalled = true;
                NumberOfBallsCreated = n;
                h(new VecFixture(50, 50), new DataBallFixture());
            }
        }

        private record VecFixture(double x, double y) : DataIVector;

        private class DataBallFixture : DataIBall
        {
            public DataIVector Velocity { get => new VecFixture(1, 1); set { } }
            public event EventHandler<DataIVector>? NewPositionNotification;
        }

        #endregion
    }
}
