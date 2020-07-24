using System;
using NSubstitute;
using NUnit.Framework;

namespace Kegstand.Tests
{
    public class KegUnitTests
    {
        private KegBase.Builder<KegBase> kegBuilder; 
        [SetUp]
        public void Setup()
        {
            var calculator = new FlowCalculatorImpl();
            kegBuilder = new KegBase.Builder<KegBase>(flowCalculator: calculator);
        }
        
        [Test]
        [TestCase(100f, 10f, 10f, 20f)]
        [TestCase(100f,20f, 0f, 20f)]
        [TestCase(30f,20f, 20f, 30f)]
        public void CanIncrement(float maxAmount, float startingAmount, float increment, float result )
        {
            // Given
            KegBase keg = kegBuilder.Max(maxAmount).StartWith(startingAmount).Build();

            // When
            keg.Increment(increment);

            // Then
            Assert.AreEqual(keg.Amount, result);
        }
        
        [Test]
        public void CannotIncrementWithNegative()
        {
            // Given
            KegBase keg = kegBuilder.StartWith(50f).Build();

            // When & Then
            Assert.Throws<ArgumentException>(() => keg.Increment(-10f));
        }

        [Test]
        [TestCase(0f, 10f, 5f, 5f)]
        [TestCase(20f, 30f, 20f, 20f)]
        public void CanDecrementFluid(float minAmount, float startingAmount, float decrement, float result )
        {
            // Given
            KegBase keg = kegBuilder.Min(minAmount).StartWith(startingAmount).Build();

            // When
            keg.Decrement(decrement);

            // Then
            Assert.AreEqual(keg.Amount, result);
        }
        
        [Test]
        public void CannotDecrementWithNegative()
        {
            // Given
            KegBase keg = kegBuilder.StartWith(50f).Build(); 
            
            // When & Then
            Assert.Throws<ArgumentException>(() => keg.Decrement(-10f));
        }
        
        [Test]
        public void KegShouldMaintainTapFlowCache()
        {
            // Given
            var calculator = Substitute.For<FlowCalculator>();
            calculator.CalculateAggregateFlow(Arg.Any<Keg>()).Returns(10f);
            KegBase keg = kegBuilder.WithCalculator(calculator).Build();
            
            // When
            var value =keg.AggregateFlow;
            value = keg.AggregateFlow;
            
            // Then
            calculator.Received(1).CalculateAggregateFlow(Arg.Any<Keg>());
        }

        [Test]
        public void KegShouldDirtyAggregateFlowOnTapChange()
        {
            // Given
            var calculator = Substitute.For<FlowCalculator>();
            calculator.CalculateAggregateFlow(Arg.Any<Keg>()).Returns(10f);
            KegBase keg = kegBuilder.WithCalculator(calculator).Build();
                
            // When
            var value =keg.AggregateFlow;
            keg.AddTap(Substitute.For<Tap>());
            value = keg.AggregateFlow;
            
            // Then
            calculator.Received(2).CalculateAggregateFlow(Arg.Any<Keg>());
        }
        
    }
}
