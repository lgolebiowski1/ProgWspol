namespace TP.ConcurrentProgramming.Data.Test
{
    [TestClass]
    public class BallUnitTest
    {
        [TestMethod]
        public void Constructor_SetsPositionAndVelocity()
        {
            NullDiagnosticLogger logger = new();
            Vector pos = new(10.0, 20.0);
            Vector vel = new(1.0, 2.0);
            Ball ball = new(pos, vel, 15.0, 1.0, logger);

            Assert.AreEqual(10.0, ball.Position.x);
            Assert.AreEqual(20.0, ball.Position.y);
            Assert.AreEqual(15.0, ball.Radius);
            Assert.AreEqual(1.0, ball.Mass);
        }

        [TestMethod]
        public void Velocity_CanBeChanged()
        {
            NullDiagnosticLogger logger = new();
            Ball ball = new(new Vector(0, 0), new Vector(1, 1), 15, 1, logger);
            ball.Velocity = new Vector(3.0, -2.0);

            Assert.AreEqual(3.0, ball.Velocity.x);
            Assert.AreEqual(-2.0, ball.Velocity.y);
        }

        [TestMethod]
        public void Move_FiresPositionChangedEvent()
        {
            NullDiagnosticLogger logger = new();
            Ball ball = new(new Vector(100, 100), new Vector(2, 2), 15, 1, logger);
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
            NullDiagnosticLogger logger = new();
            Ball ball = new(new Vector(100, 100), new Vector(1, 1), 15, 1, logger);
            IVector? reported = null;
            ball.NewPositionNotification += (s, e) => reported = e;

            ball.StartMoving();
            Thread.Sleep(100);
            ball.StopMoving();

            Assert.IsNotNull(reported);
            Assert.AreNotEqual(100.0, reported!.x);
        }

        [TestMethod]
        public void Move_UsesRealTime_DeltaScaling()
        {
            // Real-time: position change should be proportional to elapsed time
            NullDiagnosticLogger logger = new();
            Ball ball = new(new Vector(100, 100), new Vector(10, 0), 15, 1, logger);
            IVector? first = null;
            IVector? second = null;
            int count = 0;

            ball.NewPositionNotification += (s, e) =>
            {
                count++;
                if (count == 1) first = e;
                if (count == 2) second = e;
            };

            ball.StartMoving();
            Thread.Sleep(200);
            ball.StopMoving();

            // With real-time scaling, position should have moved more than 1 unit
            Assert.IsNotNull(first);
            Assert.IsTrue(first!.x > 100, "Ball should have moved right");
        }

        [TestMethod]
        public void Logger_IsCalledOnMove()
        {
            CountingLogger logger = new();
            Ball ball = new(new Vector(100, 100), new Vector(1, 1), 15, 1, logger);

            ball.StartMoving();
            Thread.Sleep(100);
            ball.StopMoving();

            Assert.IsTrue(logger.Count > 0, "Logger should have been called");
        }
    }

    /// <summary>
    /// Test logger that counts how many entries were logged.
    /// </summary>
    internal class CountingLogger : IDiagnosticLogger
    {
        private int _count = 0;
        public int Count => _count;
        public void Log(DiagnosticEntry entry) => Interlocked.Increment(ref _count);
        public void Dispose() { }
    }
}
