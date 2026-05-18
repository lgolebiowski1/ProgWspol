using System.Diagnostics;
using UnderneathLayerAPI = TP.ConcurrentProgramming.Data.DataAbstractAPI;

namespace TP.ConcurrentProgramming.BusinessLogic
{
    internal class BusinessLogicImplementation : BusinessLogicAbstractAPI
    {
        private bool _disposed = false;
        private readonly UnderneathLayerAPI _layerBelow;
        private readonly List<Ball> _balls = [];

        // Single lock for all collision detection - critical section
        private readonly object _collisionLock = new();

        private const double _ballDiameter = 30.0;
        private double BallRadius => _ballDiameter / 2.0;

        #region ctor

        public BusinessLogicImplementation() : this(null) { }

        internal BusinessLogicImplementation(UnderneathLayerAPI? underneathLayer)
        {
            _layerBelow = underneathLayer ?? UnderneathLayerAPI.GetDataLayer();
        }

        #endregion

        #region BusinessLogicAbstractAPI

        public override double TableWidth => _layerBelow.TableWidth;
        public override double TableHeight => _layerBelow.TableHeight;
        public override double BallDiameter => _ballDiameter;

        public override void Start(int numberOfBalls, Action<IPosition, IBall> upperLayerHandler)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(BusinessLogicImplementation));
            if (upperLayerHandler == null)
                throw new ArgumentNullException(nameof(upperLayerHandler));

            _layerBelow.Start(numberOfBalls, (startingPosition, dataBall) =>
            {
                Ball logicBall = new Ball(dataBall, TableWidth, TableHeight);

                // Hook into position changes to detect collisions
                dataBall.NewPositionNotification += (s, pos) => OnBallMoved(logicBall);

                lock (_collisionLock)
                    _balls.Add(logicBall);

                upperLayerHandler(new Position(startingPosition.x, startingPosition.y), logicBall);
            });
        }

        public override void Stop()
        {
            lock (_collisionLock)
                _balls.Clear();
            _layerBelow.Stop();
        }

        public override void Dispose()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(BusinessLogicImplementation));
            _layerBelow.Dispose();
            lock (_collisionLock)
                _balls.Clear();
            _disposed = true;
        }

        #endregion

        #region Collision detection — critical section

        private void OnBallMoved(Ball movedBall)
        {
            // Lock ensures only one collision check at a time — prevents race conditions
            lock (_collisionLock)
            {
                foreach (Ball other in _balls)
                {
                    if (ReferenceEquals(movedBall, other)) continue;
                    ResolveCollision(movedBall, other);
                }
            }
        }

        /// <summary>
        /// Elastic collision between two balls with equal or different masses.
        /// Uses component decomposition along the collision axis.
        /// </summary>
        private static void ResolveCollision(Ball a, Ball b)
        {
            Data.IVector posA = a.Position;
            Data.IVector posB = b.Position;

            double dx = posB.x - posA.x;
            double dy = posB.y - posA.y;
            double distance = Math.Sqrt(dx * dx + dy * dy);
            double minDist = a.Radius + b.Radius;

            if (distance >= minDist || distance < 1e-10)
                return;

            double nx = dx / distance;
            double ny = dy / distance;

            Data.IVector velA = a.Velocity;
            Data.IVector velB = b.Velocity;

            double dvx = velA.x - velB.x;
            double dvy = velA.y - velB.y;
            double relativeVelocityAlongNormal = dvx * nx + dvy * ny;

            if (relativeVelocityAlongNormal <= 0)
                return;

            double mA = a.Mass;
            double mB = b.Mass;
            double impulse = (2.0 * relativeVelocityAlongNormal) / (mA + mB);

            a.Velocity = new VelocityVector(
                velA.x - impulse * mB * nx,
                velA.y - impulse * mB * ny);
            b.Velocity = new VelocityVector(
                velB.x + impulse * mA * nx,
                velB.y + impulse * mA * ny);
        }

        private record VelocityVector(double x, double y) : Data.IVector;

        #endregion

        #region TestingInfrastructure

        [Conditional("DEBUG")]
        internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
            => returnInstanceDisposed(_disposed);

        [Conditional("DEBUG")]
        internal void CheckBallCount(Action<int> returnCount)
        {
            lock (_collisionLock)
                returnCount(_balls.Count);
        }

        #endregion
    }
}
