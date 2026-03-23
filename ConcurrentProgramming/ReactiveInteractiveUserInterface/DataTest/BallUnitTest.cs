//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

namespace TP.ConcurrentProgramming.Data.Test
{
  [TestClass]
  public class BallUnitTest
  {
    [TestMethod]
    public void ConstructorTestMethod()
    {
      Vector testinVector = new Vector(0.0, 0.0);
      Ball newInstance = new(testinVector, testinVector);
    }

    [TestMethod]
    public void MoveTestMethod()
    {
      Vector initialPosition = new(10.0, 10.0);
      Ball newInstance = new(initialPosition, new Vector(0.0, 0.0));
      IVector curentPosition = new Vector(0.0, 0.0);
      int numberOfCallBackCalled = 0;
      newInstance.NewPositionNotification += (sender, position) => { Assert.IsNotNull(sender); curentPosition = position; numberOfCallBackCalled++; };
      newInstance.Move(new Vector(0.0, 0.0));
      Assert.AreEqual<int>(1, numberOfCallBackCalled);
      Assert.AreEqual<IVector>(initialPosition, curentPosition);
    }

        [TestMethod]
        public void Ball_Move_RaisesEventWithUpdatedPosition()
        {
            Vector initial = new Vector(10.0, 20.0);
            Ball ball = new Ball(initial, new Vector(0.0, 0.0));
            IVector? reported = null;
            int calls = 0;
            ball.NewPositionNotification += (s, pos) => { reported = pos; calls++; };

            Vector delta = new Vector(1.5, -2.5);
            ball.Move(delta);

            Assert.AreEqual<int>(1, calls);
            Assert.IsNotNull(reported);
            Assert.AreEqual<double>(initial.x + delta.x, reported!.x);
            Assert.AreEqual<double>(initial.y + delta.y, reported!.y);
        }
    }
}