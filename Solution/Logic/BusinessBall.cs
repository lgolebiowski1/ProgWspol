namespace TP.ConcurrentProgramming.BusinessLogic
{
    internal class Ball : IBall
    {
        private readonly Data.IBall _dataBall;
        private readonly double _tableWidth;
        private readonly double _tableHeight;
        private readonly double _radius;

        public Ball(Data.IBall dataBall, double tableWidth, double tableHeight, double radius)
        {
            _dataBall = dataBall;
            _tableWidth = tableWidth;
            _tableHeight = tableHeight;
            _radius = radius;
            dataBall.NewPositionNotification += OnDataPositionChanged;
        }

        #region IBall

        public event EventHandler<IPosition>? NewPositionNotification;

        #endregion

        #region private

        private void OnDataPositionChanged(object? sender, Data.IVector position)
        {
            // Bounce: reflect velocity when hitting walls
            Data.IVector vel = _dataBall.Velocity;
            double vx = vel.x;
            double vy = vel.y;
            bool bounced = false;

            if (position.x - _radius < 0) { vx = Math.Abs(vx); bounced = true; }
            else if (position.x + _radius > _tableWidth) { vx = -Math.Abs(vx); bounced = true; }

            if (position.y - _radius < 0) { vy = Math.Abs(vy); bounced = true; }
            else if (position.y + _radius > _tableHeight) { vy = -Math.Abs(vy); bounced = true; }

            if (bounced)
                _dataBall.Velocity = new VelocityVector(vx, vy);

            NewPositionNotification?.Invoke(this, new Position(position.x, position.y));
        }

        private record VelocityVector(double x, double y) : Data.IVector;

        #endregion
    }
}
