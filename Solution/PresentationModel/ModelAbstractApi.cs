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
        {
            return new ModelImplementation();
        }

        // DI overload for testing
        public static ModelAbstractApi CreateModel(BusinessLogic.BusinessLogicAbstractAPI? logicLayer)
        {
            return new ModelImplementation(logicLayer);
        }

        #endregion

        public abstract void Start(int numberOfBalls);

        #region IObservable

        public abstract IDisposable Subscribe(IObserver<IBall> observer);

        #endregion

        #region IDisposable

        public abstract void Dispose();

        #endregion
    }


}
