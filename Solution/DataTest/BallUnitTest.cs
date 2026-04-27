namespace TP.ConcurrentProgramming.Data.Test
{
    [TestClass]
    public class BallUnitTest
    {
        [TestMethod]
        public void ConstructorTestMethod()
        {
            Vector pos = new(0.0, 0.0);
            Ball ball = new(pos, pos);
        }

        [TestMethod]
        public void MoveTestMethod()
        {
            Vector initialPosition = new(10.0, 10.0);
            Ball ball = new(initialPosition, new Vector(0.0, 0.0));
            IVector? reported = null;
            int calls = 0;
            ball.NewPositionNotification += (s, p) => { reported = p; calls++; };

            ball.Move(new Vector(0.0, 0.0));

            Assert.AreEqual<int>(1, calls);
            Assert.IsNotNull(reported);
            Assert.AreEqual(initialPosition.x, reported!.x);
            Assert.AreEqual(initialPosition.y, reported!.y);
        }

        [TestMethod]
        public void Move_UpdatesPositionByDelta()
        {
            Vector initial = new(10.0, 20.0);
            Ball ball = new(initial, new Vector(0.0, 0.0));
            IVector? reported = null;
            ball.NewPositionNotification += (s, p) => reported = p;

            ball.Move(new Vector(1.5, -2.5));

            Assert.IsNotNull(reported);
            Assert.IsTrue(Math.Abs(reported!.x - 11.5) < 0.001);
            Assert.IsTrue(Math.Abs(reported!.y - 17.5) < 0.001);
        }
    }
}
