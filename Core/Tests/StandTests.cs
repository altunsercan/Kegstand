using System.Collections.Generic;
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
            Keg keg = kegBuilder.Build();
            Keg keg2 = kegBuilder.Build();
            object uniqueObj = new object();
            object uniqueObj2 = new object();
            List<Stand.KegEntry> kegEntries = new List<Stand.KegEntry>()
            {
                new Stand.KegEntry(uniqueObj, keg),
                new Stand.KegEntry(uniqueObj2, keg2)
            };
                
            // When
            Stand stand = new Stand(kegEntries);

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
            List<Stand.KegEntry> kegEntries = new List<Stand.KegEntry>()
            {
                new Stand.KegEntry(uniqueObj, keg),
                new Stand.KegEntry(uniqueObj, keg2)
            };
            
            // When
            Stand stand = new Stand(kegEntries);
            
            // Then
            Assert.AreEqual(keg, stand.GetKeg(uniqueObj));
            Assert.AreNotEqual(keg2, stand.GetKeg(uniqueObj));
        }
        
    }
}