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
            Clock clock = new TimeSpanClock(startTime);
            
            // When
            clock.Update(deltaSeconds: 1f);

            // Then
            ref TimeSpan current = ref clock.GetCurrentTimePassed();
            Assert.AreEqual(startTime+TimeSpan.FromSeconds(1f), current);
        }
    }
}