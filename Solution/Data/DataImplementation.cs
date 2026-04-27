using System.Diagnostics;

namespace TP.ConcurrentProgramming.Data
{
    internal class DataImplementation : DataAbstractAPI
    {
        private bool _disposed = false;
        private readonly Timer _moveTimer;
        private readonly Random _random = new();
        private readonly List<Ball> _balls = [];
        private readonly object _lock = new();

        private const double TableW = 800;
        private const double TableH = 500;
        private const double BallRadius = 15.0;

        public DataImplementation()
        {
            _moveTimer = new Timer(Move, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(16));
        }

        #region DataAbstractAPI

        public override double TableWidth => TableW;
        public override double TableHeight => TableH;

        public override void Start(int numberOfBalls, Action<IVector, IBall> upperLayerHandler)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(DataImplementation));
            if (upperLayerHandler == null)
                throw new ArgumentNullException(nameof(upperLayerHandler));

            for (int i = 0; i < numberOfBalls; i++)
            {
                double x = BallRadius + _random.NextDouble() * (TableW - 2 * BallRadius);
                double y = BallRadius + _random.NextDouble() * (TableH - 2 * BallRadius);
                Vector startPos = new(x, y);

                double angle = _random.NextDouble() * 2 * Math.PI;
                double speed = 2.0 + _random.NextDouble() * 3.0;
                Vector velocity = new(Math.Cos(angle) * speed, Math.Sin(angle) * speed);

                Ball newBall = new(startPos, velocity);
                lock (_lock)
                    _balls.Add(newBall);
                upperLayerHandler(startPos, newBall);
            }
        }

        #endregion

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _moveTimer.Dispose();
                    lock (_lock)
                        _balls.Clear();
                }
                _disposed = true;
            }
            else
                throw new ObjectDisposedException(nameof(DataImplementation));
        }

        public override void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region private

        private void Move(object? state)
        {
            List<Ball> snapshot;
            lock (_lock)
                snapshot = new List<Ball>(_balls);

            foreach (Ball ball in snapshot)
            {
                IVector vel = ball.Velocity;
                ball.Move(new Vector(vel.x, vel.y));
            }
        }

        #endregion

        #region TestingInfrastructure

        [Conditional("DEBUG")]
        internal void CheckBallsList(Action<IEnumerable<IBall>> returnBallsList)
        {
            lock (_lock)
                returnBallsList(_balls);
        }

        [Conditional("DEBUG")]
        internal void CheckNumberOfBalls(Action<int> returnNumberOfBalls)
        {
            lock (_lock)
                returnNumberOfBalls(_balls.Count);
        }

        [Conditional("DEBUG")]
        internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
        {
            returnInstanceDisposed(_disposed);
        }

        #endregion
    }
}
