using Kegstand.Impl;
using NUnit.Framework;

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
        [TestCase(-1f)]
        public void TapShouldIncrementKeg(float flowAmount)
        {
            // Given
            Tap tap = new TapBase(flowAmount);
            Impl.KegBase keg = builder.StartWith(50f).Build();
            keg.AddTap(tap);
            
            // When
            keg.Update(10f);

            // Then
            Assert.AreEqual(50f+10f*flowAmount, keg.Amount);
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
            
            // When
            keg.Update(10f);

            // Then
            Assert.AreEqual(60f, keg.Amount);
        }
    }
}