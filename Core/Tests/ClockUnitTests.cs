using System;
using Kegstand.Impl;
using NSubstitute;
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
        [TestCase(5f, 2f)]
        [TestCase(5f, 0f)]
        [TestCase(0f, 2f)]
        public void ShouldCalculateKegAmountBasedOnClock(float timePassed, float flow)
        {
            // Given
            IAmountVisitor visitor = new TimeSpanAmountVisitor(new Timestamp<TimeSpan>(TimeSpan.FromSeconds(timePassed)));
            Timestamp lastRecordedTimestamp = new Timestamp<TimeSpan>(TimeSpan.FromSeconds(0f));
            
            float currentAmount = 0;
            
            // When
            var calculatedAmount = visitor.CalculateCurrentAmount(currentAmount, flow, lastRecordedTimestamp);

            // Then
            Assert.AreEqual((timePassed * flow), calculatedAmount);   
        }
        
        
        [Test]
        public void ShouldNotCalculateKegAmountWithInvalidTimestamp()
        {
            // Given
            IAmountVisitor visitor = new TimeSpanAmountVisitor(new Timestamp<TimeSpan>(TimeSpan.FromSeconds(5f)));
            Timestamp lastRecordedTimestamp = Substitute.For<Timestamp>();
            
            float currentAmount = 0;
            
            // When & Then
            Assert.Throws<ArgumentException>(() =>  visitor.CalculateCurrentAmount(currentAmount, 1f, lastRecordedTimestamp));
        }
    }
}