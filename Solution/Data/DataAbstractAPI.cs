namespace TP.ConcurrentProgramming.Data
{
    public abstract class DataAbstractAPI : IDisposable
    {
        #region Layer Factory

        public static DataAbstractAPI GetDataLayer()
        {
            return new DataImplementation();
        }

        // DI constructor for testing - allows injecting custom implementation
        public static DataAbstractAPI GetDataLayer(DataAbstractAPI? customInstance)
        {
            return customInstance ?? GetDataLayer();
        }

        #endregion

        #region public API

        /// <summary>
        /// Starts creating balls and invokes the callback for each one with its initial position and ball reference.
        /// </summary>
        public abstract void Start(int numberOfBalls, Action<IVector, IBall> upperLayerHandler);

        public abstract double TableWidth { get; }
        public abstract double TableHeight { get; }

        #endregion

        #region IDisposable

        public abstract void Dispose();

        #endregion
    }

    public interface IVector
    {
        double x { get; init; }
        double y { get; init; }
    }

    public interface IBall
    {
        event EventHandler<IVector> NewPositionNotification;
        IVector Velocity { get; set; }
    }
}
