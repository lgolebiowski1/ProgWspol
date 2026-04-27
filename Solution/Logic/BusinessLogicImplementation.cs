using System.Diagnostics;
using UnderneathLayerAPI = TP.ConcurrentProgramming.Data.DataAbstractAPI;

namespace TP.ConcurrentProgramming.BusinessLogic
{
    internal class BusinessLogicImplementation : BusinessLogicAbstractAPI
    {
        private bool _disposed = false;
        private readonly UnderneathLayerAPI _layerBelow;

        private const double _ballDiameter = 30.0;
        private double BallRadius => _ballDiameter / 2.0;

        #region ctor

        public BusinessLogicImplementation() : this(null) { }

        internal BusinessLogicImplementation(UnderneathLayerAPI? underneathLayer)
        {
            _layerBelow = underneathLayer ?? UnderneathLayerAPI.GetDataLayer();
        }

        #endregion

        #region BusinessLogicAbstractAPI

        public override double TableWidth => _layerBelow.TableWidth;
        public override double TableHeight => _layerBelow.TableHeight;
        public override double BallDiameter => _ballDiameter;

        public override void Start(int numberOfBalls, Action<IPosition, IBall> upperLayerHandler)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(BusinessLogicImplementation));
            if (upperLayerHandler == null)
                throw new ArgumentNullException(nameof(upperLayerHandler));

            _layerBelow.Start(numberOfBalls, (startingPosition, dataBall) =>
            {
                Ball logicBall = new Ball(dataBall, TableWidth, TableHeight, BallRadius);
                upperLayerHandler(new Position(startingPosition.x, startingPosition.y), logicBall);
            });
        }

        public override void Dispose()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(BusinessLogicImplementation));
            _layerBelow.Dispose();
            _disposed = true;
        }

        #endregion

        #region TestingInfrastructure

        [Conditional("DEBUG")]
        internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
        {
            returnInstanceDisposed(_disposed);
        }

        #endregion
    }
}
