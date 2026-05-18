using DataAPI = TP.ConcurrentProgramming.Data.DataAbstractAPI;
using DataIVector = TP.ConcurrentProgramming.Data.IVector;
using DataIBall = TP.ConcurrentProgramming.Data.IBall;

namespace TP.ConcurrentProgramming.BusinessLogic.Test
{
    [TestClass]
    public class BusinessBallUnitTest
    {
        [TestMethod]
        public void Ball_ForwardsPositionEvent()
        {
            DataBallFixture dataBall = new(100, 100, 0, 0);
            Ball logicBall = new(dataBall, 800, 500);
            IPosition? reported = null;
            logicBall.NewPositionNotification += (s, p) => reported = p;

            dataBall.RaisePosition(new VecFixture(120, 130));

            Assert.IsNotNull(reported);
            Assert.AreEqual(120.0, reported!.x);
            Assert.AreEqual(130.0, reported!.y);
        }

        [TestMethod]
        public void Ball_BouncesOffLeftWall()
        {
            DataBallFixture dataBall = new(5, 100, -2, 0);
            Ball logicBall = new(dataBall, 800, 500);

            dataBall.RaisePosition(new VecFixture(5, 100));

            Assert.IsTrue(dataBall.Velocity.x > 0, "Should reflect off left wall");
        }

        [TestMethod]
        public void Ball_BouncesOffRightWall()
        {
            DataBallFixture dataBall = new(795, 100, 2, 0);
            Ball logicBall = new(dataBall, 800, 500);

            dataBall.RaisePosition(new VecFixture(795, 100));

            Assert.IsTrue(dataBall.Velocity.x < 0, "Should reflect off right wall");
        }

        [TestMethod]
        public void Ball_BouncesOffTopWall()
        {
            DataBallFixture dataBall = new(100, 5, 0, -2);
            Ball logicBall = new(dataBall, 800, 500);

            dataBall.RaisePosition(new VecFixture(100, 5));

            Assert.IsTrue(dataBall.Velocity.y > 0, "Should reflect off top wall");
        }

        [TestMethod]
        public void Ball_BouncesOffBottomWall()
        {
            DataBallFixture dataBall = new(100, 495, 0, 2);
            Ball logicBall = new(dataBall, 800, 500);

            dataBall.RaisePosition(new VecFixture(100, 495));

            Assert.IsTrue(dataBall.Velocity.y < 0, "Should reflect off bottom wall");
        }

        #region Fixtures

        internal class DataBallFixture : DataIBall
        {
            private DataIVector _velocity;
            private DataIVector _position;

            public DataBallFixture(double x, double y, double vx, double vy)
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
