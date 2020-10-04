using System;
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
            dispatcher.KegFillChanged += fillChangedDelegate;

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

        [Test]
        public void ShouldProvideKegSpecificObservables()
        {
            // Given
            var visitor = Substitute.For<IAmountVisitor>();
            var keg = Substitute.For<Keg>();
            var dispatcher = new FillUpdateDispatcher();

            var observer = Substitute.For<IObserver<float>>();
            
            // When
            IObservable<float> kegFill = dispatcher.GetFillObservable(keg);
            kegFill.Subscribe(observer);
            
            dispatcher.DispatchUpdate(visitor);
            
            // Then
            observer.Received().OnNext(Arg.Any<float>());
        }
    }
}