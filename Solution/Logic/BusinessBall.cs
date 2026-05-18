namespace TP.ConcurrentProgramming.BusinessLogic
{
    /// <summary>
    /// Wraps a Data.IBall, handles wall bouncing and exposes position events to upper layers.
    /// Ball-to-ball collision is handled by BusinessLogicImplementation.
    /// </summary>
    internal class Ball : IBall
    {
        internal readonly Data.IBall DataBall;
        private readonly double _tableWidth;
        private readonly double _tableHeight;

        public Ball(Data.IBall dataBall, double tableWidth, double tableHeight)
        {
            DataBall = dataBall;
            _tableWidth = tableWidth;
            _tableHeight = tableHeight;
            dataBall.NewPositionNotification += OnDataPositionChanged;
        }

        #region IBall

        public event EventHandler<IPosition>? NewPositionNotification;

        #endregion

        #region internal helpers

        internal double Radius => DataBall.Radius;
        internal double Mass => DataBall.Mass;

        internal Data.IVector Position => DataBall.Position;
        internal Data.IVector Velocity
        {
            get => DataBall.Velocity;
            set => DataBall.Velocity = value;
        }

        #endregion

        #region private

        private void OnDataPositionChanged(object? sender, Data.IVector position)
        {
            // Wall bounce
            Data.IVector vel = DataBall.Velocity;
            double vx = vel.x;
            double vy = vel.y;
            bool bounced = false;

            if (position.x - Radius < 0)         { vx =  Math.Abs(vx); bounced = true; }
            else if (position.x + Radius > _tableWidth)  { vx = -Math.Abs(vx); bounced = true; }

            if (position.y - Radius < 0)         { vy =  Math.Abs(vy); bounced = true; }
            else if (position.y + Radius > _tableHeight) { vy = -Math.Abs(vy); bounced = true; }

            if (bounced)
                DataBall.Velocity = new VelocityVector(vx, vy);

            NewPositionNotification?.Invoke(this, new Position(position.x, position.y));
        }

        private record VelocityVector(double x, double y) : Data.IVector;

        #endregion
    }
}
