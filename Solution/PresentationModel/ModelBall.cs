using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using LogicIBall = TP.ConcurrentProgramming.BusinessLogic.IBall;

namespace TP.ConcurrentProgramming.Presentation.Model
{
    internal class ModelBall : IBall
    {
        private double _top;
        private double _left;

        public ModelBall(double top, double left, LogicIBall underneathBall)
        {
            _top = top;
            _left = left;
            underneathBall.NewPositionNotification += OnNewPosition;
        }

        #region IBall

        public double Top
        {
            get => _top - Diameter / 2.0;
            private set { if (_top == value) return; _top = value; RaisePropertyChanged(); }
        }

        public double Left
        {
            get => _left - Diameter / 2.0;
            private set { if (_left == value) return; _left = value; RaisePropertyChanged(); }
        }

        public double Diameter { get; init; } = 0;

        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion

        #region private

        private void OnNewPosition(object? sender, BusinessLogic.IPosition e)
        {
            Top = e.y;
            Left = e.x;
        }

        private void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region TestingInfrastructure

        [Conditional("DEBUG")]
        internal void SetLeft(double x) { Left = x; }

        [Conditional("DEBUG")]
        internal void SettTop(double x) { Top = x; }

        #endregion
    }
}
