using NUnit.Framework;

namespace Kegstand.Tests
{
    public class StandTests
    {
        private KegBase.Builder<KegBase> kegBuilder;
        
        [SetUp]
        public void Setup()
        {
            var calculator = new FlowCalculatorImpl();
            kegBuilder = new KegBase.Builder<KegBase>(flowCalculator:calculator);
        }
        
        [Test]
        public void ShouldRegisterKegsWithUniqueId()
        {
            // Given
            Stand stand = new Stand();
            Keg keg = kegBuilder.Build();
            Keg keg2 = kegBuilder.Build();
            object uniqueObj = new object();
            object uniqueObj2 = new object();
            // When
            stand.AddKeg(uniqueObj, keg);
            stand.AddKeg(uniqueObj2, keg2);

            // Then
            Assert.AreEqual(keg, stand.GetKeg(uniqueObj));
            Assert.AreEqual(keg2, stand.GetKeg(uniqueObj2));
        }
        
        [Test]
        public void ShouldNotRegisterKegsWithoutUniqueId()
        {
            // Given
            Stand stand = new Stand();
            Keg keg = kegBuilder.Build();
            Keg keg2 = kegBuilder.Build();
            object uniqueObj = new object();
            // When
            stand.AddKeg(uniqueObj, keg);
            stand.AddKeg(uniqueObj, keg2);

            // Then
            Assert.AreEqual(keg, stand.GetKeg(uniqueObj));
            Assert.AreNotEqual(keg2, stand.GetKeg(uniqueObj));
        }
        
    }
}