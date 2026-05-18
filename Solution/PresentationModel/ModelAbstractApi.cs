using System.ComponentModel;

namespace TP.ConcurrentProgramming.Presentation.Model
{
    public interface IBall : INotifyPropertyChanged
    {
        double Top { get; }
        double Left { get; }
        double Diameter { get; }
    }

    public abstract class ModelAbstractApi : IObservable<IBall>, IDisposable
    {
        #region Factory

        public static ModelAbstractApi CreateModel()
            => new ModelImplementation();

        public static ModelAbstractApi CreateModel(BusinessLogic.BusinessLogicAbstractAPI? logicLayer)
            => new ModelImplementation(logicLayer);

        #endregion

        public abstract void Start(int numberOfBalls, double canvasWidth, double canvasHeight);
        public abstract void Stop();

        public abstract IDisposable Subscribe(IObserver<IBall> observer);
        public abstract void Dispose();
    }
}
