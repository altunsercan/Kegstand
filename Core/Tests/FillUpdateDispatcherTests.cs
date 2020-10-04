using Kegstand.Impl;
using NSubstitute;
using NUnit.Framework;

namespace Kegstand.Tests
{
    public class FillUpdateDispatcherTests
    {

        [Test]
        public void ShouldUpdateOnlyTrackedKegs()
        {
            // Given
            var testFillAmount = 12345;
            
            var visitor = Substitute.For<IAmountVisitor>();
            var keg = Substitute.For<Keg>();
            keg.Amount(Arg.Any<IAmountVisitor>()).Returns(testFillAmount);
            
            var dispatcher = new FillUpdateDispatcher();

            var fillChangedDelegate = Substitute.For<KegFillChangedDelegate>();
            dispatcher.FillChanged += fillChangedDelegate;

            // When
            dispatcher.Track(keg);
            dispatcher.DispatchUpdate(visitor);
            dispatcher.DispatchUpdate(visitor);
            dispatcher.Untrack(keg);
            dispatcher.DispatchUpdate(visitor);

            // Then
            fillChangedDelegate.Received(2).Invoke(Arg.Is<KegFillChangedArgs>(arg=>arg.FillAmount == testFillAmount));
            keg.Received(2).Amount(visitor);
        }
    }
}