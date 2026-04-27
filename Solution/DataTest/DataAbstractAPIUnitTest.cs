namespace TP.ConcurrentProgramming.Data.Test
{
    [TestClass]
    public class DataAbstractAPIUnitTest
    {
        [TestMethod]
        public void GetDataLayer_ReturnsFreshInstance()
        {
            DataAbstractAPI instance = DataAbstractAPI.GetDataLayer();
            Assert.IsNotNull(instance);
            instance.Dispose();
        }
    }
}
