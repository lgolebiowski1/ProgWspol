namespace TP.ConcurrentProgramming.Data
{
    internal class Ball : IBall
    {
        private Vector _position;

        internal Ball(Vector initialPosition, Vector initialVelocity)
        {
            _position = initialPosition;
            Velocity = initialVelocity;
        }

        #region IBall

        public event EventHandler<IVector>? NewPositionNotification;

        public IVector Velocity { get; set; }

        #endregion

        #region internal

        internal void Move(Vector delta)
        {
            _position = new Vector(_position.x + delta.x, _position.y + delta.y);
            NewPositionNotification?.Invoke(this, _position);
        }

        #endregion
    }
}
