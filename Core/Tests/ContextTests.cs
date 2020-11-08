using System;
using NSubstitute;
using NUnit.Framework;
using Assert = UnityEngine.Assertions.Assert;

namespace Kegstand.Tests
{
    public class ContextTests
    {

        [TestCase(1f, 10f)]
        [TestCase(-3f, 20f)]
        public void ContextShouldRegisterKegValuesWithCurrentClock(float value, float secondsOnClock)
        {
            // Given
            var keg = Substitute.For<Keg>();
            var context = new SimulationContext<TimeSpan>();
            context.SetClockTime(TimeSpan.FromSeconds(secondsOnClock));
            
            // When
            context.Values[keg] = new ValueRegistry<TimeSpan>(){ Value = value };

            // Then
            Assert.AreEqual(value, context.Values[keg].Value);
            Assert.AreEqual(TimeSpan.FromSeconds(secondsOnClock), context.Values[keg].Timestamp);
        }

        [Test]
        public void ContextShouldReturnZeroForNotRegisteredValue()
        {
            // Given
            var keg = Substitute.For<Keg>();
            var context = new SimulationContext<TimeSpan>();
            
            // When

            // Then
            Assert.AreEqual(0f, context.Values[keg].Value);
            Assert.IsTrue(context.Values[keg].IsInvalid);
        }

        [Test]
        public void ContextShouldKeepClockTimestamp()
        {
            // Given
            var context = new SimulationContext<TimeSpan>();
            
            // When
            context.SetClockTime(TimeSpan.FromSeconds(10f));

            // Then
            Assert.AreEqual(TimeSpan.FromSeconds(10f), context.ClockTime);
        }

    }
}