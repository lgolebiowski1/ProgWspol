using TP.ConcurrentProgramming.BusinessLogic;

namespace TP.ConcurrentProgramming.BusinessLogic.Test
{
    [TestClass]
    public class BusinessBallUnitTest
    {
        [TestMethod]
        public void MoveTestMethod()
        {
            DataBallFixture dataBall = new();
            Ball logicBall = new(dataBall, 800, 500, 15);
            int calls = 0;
            logicBall.NewPositionNotification += (s, p) => { Assert.IsNotNull(p); calls++; };

            dataBall.Raise(new VectorFixture(100.0, 100.0));

            Assert.AreEqual<int>(1, calls);
        }

        [TestMethod]
        public void BusinessBall_ForwardsPosition_Correctly()
        {
            DataBallFixture dataBall = new();
            Ball logicBall = new(dataBall, 800, 500, 15);
            IPosition? reported = null;
            logicBall.NewPositionNotification += (s, p) => reported = p;

            dataBall.Raise(new VectorFixture(2.5, 7.5));

            Assert.IsNotNull(reported);
            Assert.AreEqual<double>(2.5, reported!.x);
            Assert.AreEqual<double>(7.5, reported!.y);
        }

        [TestMethod]
        public void BusinessBall_BouncesOffLeftWall()
        {
            DataBallFixture dataBall = new() { CurrentVelocity = new VectorFixture(-2.0, 0.0) };
            Ball logicBall = new(dataBall, 800, 500, 15);

            // Ball hits left wall (x < radius)
            dataBall.Raise(new VectorFixture(5.0, 100.0));

            // Velocity x should be positive (reflected)
            Assert.IsTrue(dataBall.CurrentVelocity.x > 0);
        }

        #region Fixtures

        private class DataBallFixture : Data.IBall
        {
            public Data.IVector Velocity
            {
                get => CurrentVelocity;
                set => CurrentVelocity = value;
            }

            internal Data.IVector CurrentVelocity = new VectorFixture(1.0, 1.0);

            public event EventHandler<Data.IVector>? NewPositionNotification;

            internal void Raise(Data.IVector v) => NewPositionNotification?.Invoke(this, v);
        }

        private record VectorFixture(double x, double y) : Data.IVector;

        #endregion
    }
}
