using System;
using Kegstand.Impl;
using NUnit.Framework;

namespace Kegstand.Tests
{
    public class ClockUnitTests
    {

        [Test]
        public void ShouldProgressOnUpdate()
        {
            // Given
            TimeSpan startTime = TimeSpan.Zero;
            Clock<TimeSpan> clock = new TimeSpanClock(startTime);
            
            // When
            clock.Update(deltaSeconds: 1f);

            // Then
            ref TimeSpan current = ref clock.GetCurrentTimePassed();
            Assert.AreEqual(startTime+TimeSpan.FromSeconds(1f), current);
        }

        [Test]
        public void ShouldCalculateKegAmountBasedOnClock()
        {
            // Given
            IAmountVisitor visitor = new AmountVisitor(new Timestamp<TimeSpan>(TimeSpan.FromSeconds(0f)));
            Timestamp currentTimestamp = new Timestamp<TimeSpan>(TimeSpan.FromSeconds(5f));
            
            float currentAmount = 0;
            float flow = 2f;
            
            // When
            var calculatedAmount = visitor.CalculateCurrentAmount(currentAmount, flow, currentTimestamp);

            // Then
            Assert.AreEqual(10f, calculatedAmount);   
        }
    }
}