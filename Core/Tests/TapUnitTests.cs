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

        [Test]
        [TestCase(1f)]
        [TestCase(-1f)]
        public void TapShouldIncrementKeg(float flowAmount)
        {
            // Given
            Tap tap = new TapBase(flowAmount);
            KegBase keg = new KegBase(100f, 0f, 50f);
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
            KegBase keg = new KegBase(100f, 0f, 50f);
            keg.AddTap(tap);
            keg.AddTap(tap);
            
            // When
            keg.Update(10f);

            // Then
            Assert.AreEqual(60f, keg.Amount);
        }
    }
}