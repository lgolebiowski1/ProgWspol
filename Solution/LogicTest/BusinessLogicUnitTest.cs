using DataAPI = TP.ConcurrentProgramming.Data.DataAbstractAPI;
using DataIVector = TP.ConcurrentProgramming.Data.IVector;
using DataIBall = TP.ConcurrentProgramming.Data.IBall;

namespace TP.ConcurrentProgramming.BusinessLogic.Test
{
    [TestClass]
    public class BusinessLogicImplementationUnitTest
    {
        [TestMethod]
        public void Constructor_NotDisposed()
        {
            using BusinessLogicImplementation instance = new(new ConstructorFixture());
            bool disposed = true;
            instance.CheckObjectDisposed(x => disposed = x);
            Assert.IsFalse(disposed);
        }

        [TestMethod]
        public void Dispose_SetsDisposedFlag()
        {
            DisposeFixture dataFixture = new();
            BusinessLogicImplementation instance = new(dataFixture);

            instance.Dispose();

            bool disposed = false;
            instance.CheckObjectDisposed(x => disposed = x);
            Assert.IsTrue(disposed);
            Assert.IsTrue(dataFixture.Disposed);
        }

        [TestMethod]
        public void Dispose_ThrowsOnSecondCall()
        {
            BusinessLogicImplementation instance = new(new ConstructorFixture());
            instance.Dispose();
            bool threw = false;
            try { instance.Dispose(); } catch (ObjectDisposedException) { threw = true; }
            Assert.IsTrue(threw);
        }

        [TestMethod]
        public void Start_ThrowsWhenDisposed()
        {
            BusinessLogicImplementation instance = new(new ConstructorFixture());
            instance.Dispose();
            bool threw = false;
            try { instance.Start(1, (p, b) => { }); } catch (ObjectDisposedException) { threw = true; }
            Assert.IsTrue(threw);
        }

        [TestMethod]
        public void Start_CreatesBalls()
        {
            StartFixture dataFixture = new();
            using BusinessLogicImplementation instance = new(dataFixture);

            int called = 0;
            instance.Start(3, (pos, ball) =>
            {
                called++;
                Assert.IsNotNull(pos);
                Assert.IsNotNull(ball);
            });

            Assert.AreEqual(3, called);
            instance.CheckBallCount(n => Assert.AreEqual(3, n));
        }

        [TestMethod]
        public void Stop_ClearsBalls()
        {
            StopFixture dataFixture = new();
            using BusinessLogicImplementation instance = new(dataFixture);
            instance.Start(2, (p, b) => { });
            instance.Stop();

            instance.CheckBallCount(n => Assert.AreEqual(0, n));
            Assert.IsTrue(dataFixture.StopCalled);
        }

        [TestMethod]
        public void Collision_TwoBallsResolvedCorrectly()
        {
            // Two balls moving toward each other should swap velocities (equal mass)
            CollisionFixture dataFixture = new();
            using BusinessLogicImplementation instance = new(dataFixture);

            int called = 0;
            instance.Start(2, (p, b) => called++);
            Assert.AreEqual(2, called);

            // Trigger movement on ball A moving right toward ball B
            dataFixture.RaiseBallA(new VecFixture(70, 100)); // close to ball B at 70,100

            // After collision, ball A should be moving left (vx < 0)
            Assert.IsTrue(dataFixture.BallAVelocity.x > 0,
                $"Ball A should bounce left after collision, vx={dataFixture.BallAVelocity.x}");
        }

        #region Fixtures

        private class ConstructorFixture : DataAPI
        {
            public override double TableWidth => 800;
            public override double TableHeight => 500;
            public override void Dispose() { }
            public override void Stop() { }
            public override void Start(int n, Action<DataIVector, DataIBall> h)
                => throw new NotImplementedException();
        }

        private class DisposeFixture : DataAPI
        {
            internal bool Disposed = false;
            public override double TableWidth => 800;
            public override double TableHeight => 500;
            public override void Dispose() => Disposed = true;
            public override void Stop() { }
            public override void Start(int n, Action<DataIVector, DataIBall> h)
                => throw new NotImplementedException();
        }

        private class StartFixture : DataAPI
        {
            public override double TableWidth => 800;
            public override double TableHeight => 500;
            public override void Dispose() { }
            public override void Stop() { }
            public override void Start(int n, Action<DataIVector, DataIBall> h)
            {
                for (int i = 0; i < n; i++)
                    h(new VecFixture(100 + i * 50, 100), new BallFixture(100 + i * 50, 100, 1, 0));
            }
        }

        private class StopFixture : DataAPI
        {
            internal bool StopCalled = false;
            public override double TableWidth => 800;
            public override double TableHeight => 500;
            public override void Dispose() { }
            public override void Stop() => StopCalled = true;
            public override void Start(int n, Action<DataIVector, DataIBall> h)
            {
                for (int i = 0; i < n; i++)
                    h(new VecFixture(100, 100), new BallFixture(100, 100, 1, 0));
            }
        }

        private class CollisionFixture : DataAPI
        {
            internal BallFixture? BallA;
            internal BallFixture? BallB;

            internal DataIVector BallAVelocity => BallA?.Velocity ?? new VecFixture(0, 0);

            public override double TableWidth => 800;
            public override double TableHeight => 500;
            public override void Dispose() { }
            public override void Stop() { }

            public override void Start(int n, Action<DataIVector, DataIBall> h)
            {
                // Ball A at x=30 moving right, Ball B at x=70 stationary
                BallA = new BallFixture(30, 100, 3, 0);
                BallB = new BallFixture(70, 100, 0, 0);
                h(new VecFixture(30, 100), BallA);
                h(new VecFixture(70, 100), BallB);
            }

            internal void RaiseBallA(DataIVector pos) => BallA?.RaisePosition(pos);
        }

        internal class BallFixture : DataIBall
        {
            private DataIVector _velocity;
            private DataIVector _position;

            public BallFixture(double x, double y, double vx, double vy)
            {
                _position = new VecFixture(x, y);
                _velocity = new VecFixture(vx, vy);
            }

            public double Radius => 15.0;
            public double Mass => 1.0;
            public DataIVector Position => _position;
            public DataIVector Velocity
            {
                get => _velocity;
                set => _velocity = value;
            }
            public event EventHandler<DataIVector>? NewPositionNotification;
            public void RaisePosition(DataIVector pos)
            {
                _position = pos;
                NewPositionNotification?.Invoke(this, pos);
            }
        }

        private record VecFixture(double x, double y) : DataIVector;

        #endregion
    }
}
