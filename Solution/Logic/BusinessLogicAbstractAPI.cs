namespace TP.ConcurrentProgramming.BusinessLogic
{
    public abstract class BusinessLogicAbstractAPI : IDisposable
    {
        #region Layer Factory

        public static BusinessLogicAbstractAPI GetBusinessLogicLayer()
            => new BusinessLogicImplementation();

        public static BusinessLogicAbstractAPI GetBusinessLogicLayer(Data.DataAbstractAPI? dataLayer)
            => new BusinessLogicImplementation(dataLayer);

        #endregion

        #region Layer API

        public abstract double TableWidth { get; }
        public abstract double TableHeight { get; }
        public abstract double BallDiameter { get; }

        public abstract void Start(int numberOfBalls, Action<IPosition, IBall> upperLayerHandler);
        public abstract void Stop();

        #endregion

        #region IDisposable

        public abstract void Dispose();

        #endregion
    }

    public interface IPosition
    {
        double x { get; init; }
        double y { get; init; }
    }

    public interface IBall
    {
        event EventHandler<IPosition> NewPositionNotification;
    }
}
