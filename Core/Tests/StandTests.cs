using NUnit.Framework;

namespace Kegstand.Tests
{
    public class StandTests
    {
        private KegBase.Builder<KegBase> kegBuilder;
        private StandBase.Builder standBuilder;
        
        [SetUp]
        public void Setup()
        {
            var calculator = new FlowCalculatorImpl();
            kegBuilder = new KegBase.Builder<KegBase>(flowCalculator:calculator);
            
            standBuilder = new StandBase.Builder();
        }
        
        [Test]
        public void ShouldRegisterKegsWithUniqueId()
        {
            // Given
            Keg keg = kegBuilder.Build();
            Keg keg2 = kegBuilder.Build();
            object uniqueObj = new object();
            object uniqueObj2 = new object();
            
            standBuilder.AddKeg(uniqueObj, keg);
            standBuilder.AddKeg(uniqueObj2, keg2);
                
            // When
            Stand stand = standBuilder.Build();

            // Then
            Assert.AreEqual(keg, stand.GetKeg(uniqueObj));
            Assert.AreEqual(keg2, stand.GetKeg(uniqueObj2));
        }
        
        [Test]
        public void ShouldNotRegisterKegsWithoutUniqueId()
        {
            // Given
            Keg keg = kegBuilder.Build();
            Keg keg2 = kegBuilder.Build();
            object uniqueObj = new object();
            
            standBuilder.AddKeg(uniqueObj, keg);
            standBuilder.AddKeg(uniqueObj, keg2);
            
            // When
            Stand stand = standBuilder.Build();
            
            // Then
            Assert.AreEqual(keg, stand.GetKeg(uniqueObj));
            Assert.AreNotEqual(keg2, stand.GetKeg(uniqueObj));
        }
        
        
        
    }
}