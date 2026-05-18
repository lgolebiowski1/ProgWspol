namespace TP.ConcurrentProgramming.Data
{
    internal class Ball : IBall, IDisposable
    {
        private Vector _position;
        private Vector _velocity;
        private readonly object _lock = new();
        private Timer? _timer;
        private bool _disposed = false;

        private const int TickMs = 16;

        public event EventHandler<IVector>? NewPositionNotification;

        internal Ball(Vector initialPosition, Vector initialVelocity, double radius, double mass)
        {
            _position = initialPosition;
            _velocity = initialVelocity;
            Radius = radius;
            Mass = mass;
        }

        #region IBall

        public double Radius { get; }
        public double Mass { get; }

        public IVector Position
        {
            get { lock (_lock) return _position; }
        }

        public IVector Velocity
        {
            get { lock (_lock) return _velocity; }
            set { lock (_lock) _velocity = new Vector(value.x, value.y); }
        }

        #endregion

        #region internal

        internal void StartMoving()
        {
            _timer = new Timer(Tick, null, 0, TickMs);
        }

        internal void StopMoving()
        {
            _timer?.Change(Timeout.Infinite, Timeout.Infinite);
            _timer?.Dispose();
            _timer = null;
        }

        private void Tick(object? state)
        {
            Vector newPos;
            lock (_lock)
            {
                newPos = new Vector(_position.x + _velocity.x, _position.y + _velocity.y);
                _position = newPos;
            }
            NewPositionNotification?.Invoke(this, newPos);
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            StopMoving();
        }

        #endregion
    }
}
