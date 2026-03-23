//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

namespace TP.ConcurrentProgramming.BusinessLogic.Test
{
  [TestClass]
  public class BallUnitTest
  {
    [TestMethod]
    public void MoveTestMethod()
    {
      DataBallFixture dataBallFixture = new DataBallFixture();
      Ball newInstance = new(dataBallFixture);
      int numberOfCallBackCalled = 0;
      newInstance.NewPositionNotification += (sender, position) => { Assert.IsNotNull(sender); Assert.IsNotNull(position); numberOfCallBackCalled++; };
      dataBallFixture.Move();
      Assert.AreEqual<int>(1, numberOfCallBackCalled);
    }

    #region testing instrumentation

    private class DataBallFixture : Data.IBall
    {
      public Data.IVector Velocity { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

      public event EventHandler<Data.IVector>? NewPositionNotification;

      internal void Move()
      {
        NewPositionNotification?.Invoke(this, new VectorFixture(0.0, 0.0));
      }
    }

    private class VectorFixture : Data.IVector
    {
      internal VectorFixture(double X, double Y)
      {
        x = X; y = Y;
      }

      public double x { get; init; }
      public double y { get; init; }
    }

        [TestMethod]
        public void BusinessBall_Forwards_DataEvent_As_Position()
        {
            var dataBall = new TestDataBall();
            var businessBall = new TP.ConcurrentProgramming.BusinessLogic.Ball(dataBall);
            TP.ConcurrentProgramming.BusinessLogic.IPosition? reported = null;
            int calls = 0;
            businessBall.NewPositionNotification += (s, p) => { reported = p; calls++; };

            dataBall.Raise(new TestVector(2.5, 7.5));

            Assert.AreEqual<int>(1, calls);
            Assert.IsNotNull(reported);
            Assert.AreEqual<double>(2.5, reported!.x);
            Assert.AreEqual<double>(7.5, reported!.y);
        }

        private class TestDataBall : Data.IBall
        {
            public Data.IVector Velocity { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public event EventHandler<Data.IVector>? NewPositionNotification;
            public void Raise(Data.IVector v) => NewPositionNotification?.Invoke(this, v);
        }
        private record TestVector(double x, double y) : Data.IVector;

        #endregion testing instrumentation
    }
}