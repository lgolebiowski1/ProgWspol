using System.Diagnostics;
using UnderneathLayerAPI = TP.ConcurrentProgramming.BusinessLogic.BusinessLogicAbstractAPI;

namespace TP.ConcurrentProgramming.Presentation.Model
{
    internal class ModelImplementation : ModelAbstractApi
    {
        private bool _disposed = false;
        private readonly UnderneathLayerAPI _layerBelow;
        private readonly List<IObserver<IBall>> _observers = new();
        private readonly object _observerLock = new();

        internal ModelImplementation() : this(null) { }

        internal ModelImplementation(UnderneathLayerAPI? underneathLayer)
        {
            _layerBelow = underneathLayer ?? UnderneathLayerAPI.GetBusinessLogicLayer();
        }

        #region ModelAbstractApi

        public override void Start(int numberOfBalls)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(ModelImplementation));

            _layerBelow.Start(numberOfBalls, (position, ball) =>
            {
                ModelBall newBall = new ModelBall(position.y, position.x, ball)
                {
                    Diameter = _layerBelow.BallDiameter
                };
                NotifyObservers(newBall);
            });
        }

        public override IDisposable Subscribe(IObserver<IBall> observer)
        {
            lock (_observerLock)
                _observers.Add(observer);
            return new Unsubscriber(_observers, observer, _observerLock);
        }

        public override void Dispose()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(ModelImplementation));
            _layerBelow.Dispose();
            lock (_observerLock)
            {
                foreach (var o in _observers)
                    o.OnCompleted();
                _observers.Clear();
            }
            _disposed = true;
        }

        #endregion

        #region private

        private void NotifyObservers(IBall ball)
        {
            List<IObserver<IBall>> snapshot;
            lock (_observerLock)
                snapshot = new List<IObserver<IBall>>(_observers);
            foreach (var observer in snapshot)
                observer.OnNext(ball);
        }

        private class Unsubscriber : IDisposable
        {
            private readonly List<IObserver<IBall>> _list;
            private readonly IObserver<IBall> _observer;
            private readonly object _lock;

            public Unsubscriber(List<IObserver<IBall>> list, IObserver<IBall> observer, object lockObj)
            {
                _list = list;
                _observer = observer;
                _lock = lockObj;
            }

            public void Dispose()
            {
                lock (_lock)
                    _list.Remove(_observer);
            }
        }

        #endregion

        #region TestingInfrastructure

        [Conditional("DEBUG")]
        internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
            => returnInstanceDisposed(_disposed);

        [Conditional("DEBUG")]
        internal void CheckUnderneathLayerAPI(Action<UnderneathLayerAPI> returnLayer)
            => returnLayer(_layerBelow);

        [Conditional("DEBUG")]
        internal void CheckObserverCount(Action<int> returnCount)
        {
            lock (_observerLock)
                returnCount(_observers.Count);
        }

        #endregion

    }

    public class BallChaneEventArgs : EventArgs
    {
        public IBall Ball { get; init; } = null!;
    }


}
