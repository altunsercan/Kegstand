using System;
using NSubstitute;
using NUnit.Framework;

namespace Kegstand.Tests
{
    public class KegUnitTests
    {
        private FlowCalculator calculator;
        [SetUp]
        public void Setup()
        {
            calculator = new FlowCalculatorImpl();
        }
        
        [Test]
        [TestCase(100f, 10f, 10f, 20f)]
        [TestCase(100f,20f, 0f, 20f)]
        [TestCase(30f,20f, 20f, 30f)]
        public void CanIncrement(float maxAmount, float startingAmount, float increment, float result )
        {
            // Given
            KegBase keg = new KegBase(calculator, maxAmount,0f, startingAmount );

            // When
            keg.Increment(increment);

            // Then
            Assert.AreEqual(keg.Amount, result);
        }
        
        [Test]
        public void CannotIncrementWithNegative()
        {
            // Given
            KegBase keg = new KegBase( calculator, 100f,0f, 50f );

            // When & Then
            Assert.Throws<ArgumentException>(() => keg.Increment(-10f));
        }

        [Test]
        [TestCase(0f, 10f, 5f, 5f)]
        [TestCase(20f, 30f, 20f, 20f)]
        public void CanDecrementFluid(float minAmount, float startingAmount, float decrement, float result )
        {
            // Given
            KegBase keg = new KegBase( calculator, 100f, minAmount, startingAmount );

            // When
            keg.Decrement(decrement);

            // Then
            Assert.AreEqual(keg.Amount, result);
        }
        
        [Test]
        public void CannotDecrementWithNegative()
        {
            // Given
            KegBase keg = new KegBase( calculator, 100f,0f, 50f );

            // When & Then
            Assert.Throws<ArgumentException>(() => keg.Decrement(-10f));
        }
        
        [Test]
        public void KegShouldMaintainTapFlowCache()
        {
            // Given
            calculator = Substitute.For<FlowCalculator>();
            calculator.CalculateAggregateFlow(Arg.Any<Keg>()).Returns(10f);
            KegBase keg = new KegBase(calculator, 100f, 0f, 50f);
            
            // When
            var value =keg.AggregateFlow;
            value = keg.AggregateFlow;
            
            // Then
            calculator.Received(1).CalculateAggregateFlow(default);
        }
        
    }
}
