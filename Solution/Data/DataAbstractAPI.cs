namespace TP.ConcurrentProgramming.Data
{
    public abstract class DataAbstractAPI : IDisposable
    {
        #region Layer Factory

        public static DataAbstractAPI GetDataLayer()
            => new DataImplementation();

        // DI overload — allows injecting custom implementation and logger for tests
        public static DataAbstractAPI GetDataLayer(DataAbstractAPI? customInstance)
            => customInstance ?? GetDataLayer();

        #endregion

        #region public API

        public abstract void Start(int numberOfBalls, Action<IVector, IBall> upperLayerHandler);
        public abstract void Stop();
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
        IVector Position { get; }
        IVector Velocity { get; set; }
        double Radius { get; }
        double Mass { get; }
    }
}
