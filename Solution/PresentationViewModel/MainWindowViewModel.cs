using System.Windows.Input;
using System.Collections.ObjectModel;
using TP.ConcurrentProgramming.Presentation.Model;
using TP.ConcurrentProgramming.Presentation.ViewModel.MVVMLight;
using ModelIBall = TP.ConcurrentProgramming.Presentation.Model.IBall;

namespace TP.ConcurrentProgramming.Presentation.ViewModel
{
    public class MainWindowViewModel : ViewModelBase, IDisposable
    {
        private bool _disposed = false;
        private readonly ModelAbstractApi _modelLayer;
        private IDisposable? _observer;
        private int _numberOfBalls = 5;

        #region ctor

        public MainWindowViewModel() : this(null) { }

        internal MainWindowViewModel(ModelAbstractApi? modelLayerAPI)
        {
            _modelLayer = modelLayerAPI ?? ModelAbstractApi.CreateModel();
            _observer = _modelLayer.Subscribe(new BallObserver(Balls));

            StartCommand = new RelayCommand(
                () => Start(NumberOfBalls),
                () => !_disposed);

            StopCommand = new RelayCommand(
                () => { Balls.Clear(); },
                () => true);
        }

        #endregion

        #region public API

        public ObservableCollection<ModelIBall> Balls { get; } = new();

        public int NumberOfBalls
        {
            get => _numberOfBalls;
            set { _numberOfBalls = value; RaisePropertyChanged(); }
        }

        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }

        public void Start(int numberOfBalls)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(MainWindowViewModel));
            Balls.Clear();
            _observer = _modelLayer.Subscribe(new BallObserver(Balls));
            _modelLayer.Start(numberOfBalls);
            _observer.Dispose();
        }

        #endregion

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Balls.Clear();
                    _observer?.Dispose();
                    _modelLayer.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(MainWindowViewModel));
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region private

        private class BallObserver : IObserver<ModelIBall>
        {
            private readonly ObservableCollection<ModelIBall> _balls;
            public BallObserver(ObservableCollection<ModelIBall> balls) => _balls = balls;
            public void OnNext(ModelIBall value) => _balls.Add(value);
            public void OnError(Exception error) { }
            public void OnCompleted() { }
        }

        #endregion
    }
}
