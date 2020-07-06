using System;
using Kegstand;
using NUnit.Framework;

namespace Tests
{
    public class KegUnitTests
    {
        [Test]
        [TestCase(100f, 10f, 10f, 20f)]
        [TestCase(100f,20f, 0f, 20f)]
        [TestCase(30f,20f, 20f, 30f)]
        public void CanIncrement(float maxAmount, float startingAmount, float increment, float result )
        {
            // Given
            KegBase keg = new KegBase( maxAmount,0f, startingAmount );

            // When
            keg.Increment(increment);

            // Then
            Assert.AreEqual(keg.Amount, result);
        }
        
        [Test]
        public void CannotIncrementWithNegative()
        {
            // Given
            KegBase keg = new KegBase( 100f,0f, 50f );

            // When & Then
            Assert.Throws<ArgumentException>(() => keg.Increment(-10f));
        }
        
        
        [Test]
        [TestCase(0f, 10f, 5f, 5f)]
        [TestCase(20f, 30f, 20f, 20f)]
        public void CanDecrementFluid(float minAmount, float startingAmount, float decrement, float result )
        {
            // Given
            KegBase keg = new KegBase( 100f, minAmount, startingAmount );

            // When
            keg.Decrement(decrement);

            // Then
            Assert.AreEqual(keg.Amount, result);
        }
        
        [Test]
        public void CannotDecrementWithNegative()
        {
            // Given
            KegBase keg = new KegBase( 100f,0f, 50f );

            // When & Then
            Assert.Throws<ArgumentException>(() => keg.Decrement(-10f));
        }


    }
}
