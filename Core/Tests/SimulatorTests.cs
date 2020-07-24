using NUnit.Framework;

namespace Kegstand.Tests
{
    public class SimulatorTests
    {
        [Test]
        [TestCase(20,3,4,12,5,67)]
        [TestCase(20,-5,4,-2,5,23)]
        public void ShouldMaintainTimeOrderedEvents(params float[] eventTimes)
        {
            // Given
            Simulator simulator = new Simulator();
            
            // When
            for (var index = 0; index < eventTimes.Length; index++)
            {
                float t = eventTimes[index];
                simulator.AddEvent(t, index);
            }            
            
            // Then
            Assert.That(simulator.Events, Is.Ordered.By("Time"));                
        }
    }
}