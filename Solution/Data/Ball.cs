using System.Diagnostics;

namespace TP.ConcurrentProgramming.Data
{
    internal class Ball : IBall, IDisposable
    {
        private Vector _position;
        private Vector _velocity;
        private readonly object _lock = new();
        private Timer? _timer;
        private bool _disposed = false;

        // Real-time: Stopwatch measures actual elapsed time between ticks
        private readonly Stopwatch _stopwatch = new();
        private const double TargetTickMs = 16.0;

        private readonly IDiagnosticLogger _logger;

        public event EventHandler<IVector>? NewPositionNotification;

        internal Ball(Vector initialPosition, Vector initialVelocity, double radius, double mass,
                      IDiagnosticLogger logger)
        {
            _position = initialPosition;
            _velocity = initialVelocity;
            Radius = radius;
            Mass = mass;
            _logger = logger;
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
            _stopwatch.Start();
            _timer = new Timer(Tick, null, 0, Timeout.Infinite);
        }

        internal void StopMoving()
        {
            _timer?.Change(Timeout.Infinite, Timeout.Infinite);
            _timer?.Dispose();
            _timer = null;
            _stopwatch.Stop();
        }

        private void Tick(object? state)
        {
            // Real-time: use actual elapsed time instead of fixed step
            double elapsed = _stopwatch.Elapsed.TotalMilliseconds;
            _stopwatch.Restart();

            // Scale factor: how many "target ticks" have passed
            double dt = elapsed / TargetTickMs;

            Vector newPos;
            lock (_lock)
            {
                newPos = new Vector(
                    _position.x + _velocity.x * dt,
                    _position.y + _velocity.y * dt);
                _position = newPos;
            }

            // Log diagnostic data asynchronously
            _logger.Log(new DiagnosticEntry(
                DateTime.UtcNow,
                Radius,
                newPos.x,
                newPos.y,
                _velocity.x,
                _velocity.y,
                elapsed));

            NewPositionNotification?.Invoke(this, newPos);

            // Schedule next tick — one-shot timer for real-time accuracy
            _timer?.Change((int)TargetTickMs, Timeout.Infinite);
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
