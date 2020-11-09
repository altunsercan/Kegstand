using System;
using Kegstand.Impl;
using NSubstitute;
using NUnit.Framework;
using Assert = UnityEngine.Assertions.Assert;

namespace Kegstand.Tests
{
    public class ContextTests
    {
        private TestClock clock;
        private SimulationContext<TimeSpan> context;
        
        [SetUp]
        public void SetupTest()
        {
            clock = new TestClock();
            context = new SimulationContext<TimeSpan>(clock);
        }
        

        [TestCase(1f, 10f)]
        [TestCase(-3f, 20f)]
        public void ContextShouldRegisterKegValuesWithCurrentClock(float value, float secondsOnClock)
        {
            // Given
            var keg = Substitute.For<Keg>();
            clock.CurrentTime = TimeSpan.FromSeconds(secondsOnClock);

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
            
            // When

            // Then
            Assert.AreEqual(0f, context.Values[keg].Value);
            Assert.IsTrue(context.Values[keg].IsInvalid);
        }

        [Test]
        public void ContextShouldReflectClockTimestamp()
        {
            // Given
            clock.CurrentTime = TimeSpan.FromSeconds(10f);
            
            // When

            // Then
            Assert.AreEqual(TimeSpan.FromSeconds(10f), context.ClockTime);
        }

        // NSubstitute bug with ref return required manual mocking
        private class TestClock : Clock<TimeSpan>
        {
            public TimeSpan CurrentTime;
            public void Update(float deltaSeconds)
            {
                throw new NotImplementedException();
            }

            public ref TimeSpan GetCurrentTimePassed()
            {
                return ref CurrentTime;
            }
        }
        
    }
}