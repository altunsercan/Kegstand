﻿using System.Collections.Generic;
using System.Linq;
using NSubstitute;
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
            var kegs = stand.Kegs;
            Assert.That(kegs, 
                Has.Some.Matches<KegEntry>(entry=>entry.Key == uniqueObj)
                    .And.Some.Matches<KegEntry>(entry=>entry.Key == uniqueObj2));
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

        [Test]
        public void ShouldRegisterTapsWithUniqueId()
        {
            // Given
            Tap tap = Substitute.For<Tap>();
            Tap tap2 = Substitute.For<Tap>();
            object uniqueObj = new object();
            object uniqueObj2 = new object();

            standBuilder.AddTap(uniqueObj, tap);
            standBuilder.AddTap(uniqueObj2, tap2);
            
            // When
            Stand stand = standBuilder.Build();
            IReadOnlyList<TapEntry> taps = stand.Taps;
            
            // Then
            Assert.AreEqual(tap, stand.GetTap(uniqueObj));
            Assert.AreEqual(tap2, stand.GetTap(uniqueObj2));
            Assert.That(taps, 
                Has.Some.Matches<TapEntry>(entry=>entry.Key == uniqueObj)
                    .And.Some.Matches<TapEntry>(entry=>entry.Key == uniqueObj2));
        }
        
        
    }
}