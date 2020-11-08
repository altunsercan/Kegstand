using System;
using Kegstand.Impl;
using NUnit.Framework;
using UnityEngine;

namespace Kegstand.Tests
{
    public class TapUnitTests
    {
        /*
         * TDD: TO DO List
         * Flow caching
         * On/Off tap
         * 
         * 
         */

        private Impl.KegBase.Builder<Impl.KegBase> builder;
        [SetUp]
        public void Setup()
        {
            var calculator = new FlowCalculatorImpl();

            builder = new Impl.KegBase.Builder<Impl.KegBase>();
            builder.WithCalculator(calculator);
        }
        
        [Test]
        [TestCase(1f)]
        [TestCase(10f)]
        [TestCase(-1f)]
        [TestCase(-10f)]
        public void TapShouldIncrementKeg(float flowAmount)
        {
            // Given
            Tap tap = new TapBase(flowAmount);
            Impl.KegBase keg = builder.StartWith(50f).Build();
            keg.AddTap(tap);
            
            TimeSpanAmountVisitor visitor = new TimeSpanAmountVisitor(new Timestamp<TimeSpan>(TimeSpan.FromSeconds(10f)));
            
            // When & Then
            //keg.Update(10f);

            // 
            Assert.AreEqual(Mathf.Clamp(50f+10f*flowAmount, keg.MinAmount, keg.MaxAmount), keg.Amount(visitor));
        }


        [Test]
        public void TapShouldBeAddedOnlyOnce()
        {
            // Given
            Tap tap = new TapBase();
            tap.SetFlow(1f);
            Impl.KegBase keg = builder.StartWith(50f).Build();
            keg.AddTap(tap);
            keg.AddTap(tap);
            
            TimeSpanAmountVisitor visitor = new TimeSpanAmountVisitor(new Timestamp<TimeSpan>(TimeSpan.FromSeconds(10f)));
            
            // When & Then
            // keg.Update(10f);

            //
            Assert.AreEqual(60f, keg.Amount(visitor));
        }
        
    }
}