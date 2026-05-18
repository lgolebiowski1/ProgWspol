namespace TP.ConcurrentProgramming.Data.Test
{
    [TestClass]
    public class BallUnitTest
    {
        [TestMethod]
        public void Constructor_SetsPositionAndVelocity()
        {
            Vector pos = new(10.0, 20.0);
            Vector vel = new(1.0, 2.0);
            Ball ball = new(pos, vel, 15.0, 1.0);

            Assert.AreEqual(10.0, ball.Position.x);
            Assert.AreEqual(20.0, ball.Position.y);
            Assert.AreEqual(15.0, ball.Radius);
            Assert.AreEqual(1.0, ball.Mass);
        }

        [TestMethod]
        public void Velocity_CanBeChanged()
        {
            Ball ball = new(new Vector(0, 0), new Vector(1, 1), 15, 1);
            ball.Velocity = new Vector(3.0, -2.0);

            Assert.AreEqual(3.0, ball.Velocity.x);
            Assert.AreEqual(-2.0, ball.Velocity.y);
        }

        [TestMethod]
        public void Move_FiresPositionChangedEvent()
        {
            Ball ball = new(new Vector(100, 100), new Vector(2, 2), 15, 1);
            bool fired = false;
            ball.NewPositionNotification += (s, e) => fired = true;

            ball.StartMoving();
            Thread.Sleep(100);
            ball.StopMoving();

            Assert.IsTrue(fired);
        }

        [TestMethod]
        public void Move_UpdatesPosition()
        {
            Ball ball = new(new Vector(100, 100), new Vector(1, 1), 15, 1);
            Data.IVector? reported = null;
            ball.NewPositionNotification += (s, e) => reported = e;

            ball.StartMoving();
            Thread.Sleep(100);
            ball.StopMoving();

            Assert.IsNotNull(reported);
            Assert.AreNotEqual(100.0, reported!.x);
        }
    }
}
