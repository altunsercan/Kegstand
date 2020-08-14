﻿using System;
using System.Collections.Generic;
using System.Linq;
using Kegstand.Impl;
using NSubstitute;
using NUnit.Framework;

namespace Kegstand.Tests
{
    public class KegUnitTests
    {
        private Impl.KegBase.Builder<Impl.KegBase> kegBuilder; 
        [SetUp]
        public void Setup()
        {
            var calculator = new FlowCalculatorImpl();
            kegBuilder = new Impl.KegBase.Builder<Impl.KegBase>(flowCalculator: calculator);
        }
        
        [Test]
        [TestCase(100f, 10f, 10f, 20f)]
        [TestCase(100f,20f, 0f, 20f)]
        [TestCase(30f,20f, 20f, 30f)]
        public void CanIncrement(float maxAmount, float startingAmount, float increment, float result )
        {
            // Given
            Impl.KegBase keg = kegBuilder.Max(maxAmount).StartWith(startingAmount).Build();

            // When
            keg.Increment(increment);

            // Then
            Assert.AreEqual(keg.Amount, result);
        }
        
        [Test]
        public void CannotIncrementWithNegative()
        {
            // Given
            Impl.KegBase keg = kegBuilder.StartWith(50f).Build();

            // When & Then
            Assert.Throws<ArgumentException>(() => keg.Increment(-10f));
        }

        [Test]
        [TestCase(0f, 10f, 5f, 5f)]
        [TestCase(20f, 30f, 20f, 20f)]
        public void CanDecrementFluid(float minAmount, float startingAmount, float decrement, float result )
        {
            // Given
            Impl.KegBase keg = kegBuilder.Min(minAmount).StartWith(startingAmount).Build();

            // When
            keg.Decrement(decrement);

            // Then
            Assert.AreEqual(keg.Amount, result);
        }
        
        [Test]
        public void CannotDecrementWithNegative()
        {
            // Given
            Impl.KegBase keg = kegBuilder.StartWith(50f).Build(); 
            
            // When & Then
            Assert.Throws<ArgumentException>(() => keg.Decrement(-10f));
        }
        
        [Test]
        public void KegShouldMaintainTapFlowCache()
        {
            // Given
            var calculator = Substitute.For<FlowCalculator>();
            calculator.CalculateAggregateFlow(Arg.Any<Keg>()).Returns(10f);
            Impl.KegBase keg = kegBuilder.WithCalculator(calculator).Build();
            
            // When
            var value =keg.AggregateFlow;
            value = keg.AggregateFlow;
            
            // Then
            calculator.Received(1).CalculateAggregateFlow(Arg.Any<Keg>());
        }

        [Test]
        public void KegShouldDirtyAggregateFlowOnTapChange()
        {
            // Given
            var calculator = Substitute.For<FlowCalculator>();
            calculator.CalculateAggregateFlow(Arg.Any<Keg>()).Returns(10f);
            Impl.KegBase keg = kegBuilder.WithCalculator(calculator).Build();
                
            // When
            var value =keg.AggregateFlow;
            keg.AddTap(Substitute.For<Tap>());
            value = keg.AggregateFlow;
            
            // Then
            calculator.Received(2).CalculateAggregateFlow(Arg.Any<Keg>());
        }

        [Test]
        public void KegShouldGenerateFillEventsForPositiveFlow()
        {
            // Given
            var calculator = Substitute.For<FlowCalculator>();
            calculator.CalculateAggregateFlow(Arg.Any<Keg>()).Returns(1f);

            List<TimedEvent> list = new List<TimedEvent>(); 
            
            Impl.KegBase keg = kegBuilder.WithCalculator(calculator).Build();
            
            // When
            int eventCount = keg.AppendCurrentEvents(list);

            // Then
            Assert.Greater(eventCount, 0);
            Assert.AreEqual(eventCount, list.Count);
            Assert.That(list, Has.Some.Matches<TimedEvent>(evt=>evt.Type == KegEvent.Filled));
            Assert.That(list, Has.None.Matches<TimedEvent>(evt=>evt.Type == KegEvent.Emptied));
        }

        [Test]
        public void KegShouldGenerateEmptyEventsForNegativeFlow()
        {
            // Given
            var calculator = Substitute.For<FlowCalculator>();
            calculator.CalculateAggregateFlow(Arg.Any<Keg>()).Returns(-1f);

            List<TimedEvent> list = new List<TimedEvent>(); 
            
            Impl.KegBase keg = kegBuilder.WithCalculator(calculator).StartWith(50f).Build();
            
            // When
            int eventCount = keg.AppendCurrentEvents(list);

            // Then
            Assert.Greater(eventCount, 0);
            Assert.AreEqual(eventCount, list.Count);
            Assert.That(list, Has.Some.Matches<TimedEvent>(evt=>evt.Type == KegEvent.Emptied));
            Assert.That(list, Has.None.Matches<TimedEvent>(evt=>evt.Type == KegEvent.Filled));
        }


        [TestCase(1f, 100f)]
        [TestCase(-1f, 0f)]
        [TestCase(0f, 50f)]
        public void KegShouldNotGenerateFillEventAmountsAreAlreadyAtBoundary(float flow, float startingAmount)
        {
            // Given
            var calculator = Substitute.For<FlowCalculator>();
            calculator.CalculateAggregateFlow(Arg.Any<Keg>()).Returns(flow);

            List<TimedEvent> list = new List<TimedEvent>(); 
            
            Impl.KegBase keg = kegBuilder.WithCalculator(calculator).StartWith(startingAmount).Build();
            
            // When
            int eventCount = keg.AppendCurrentEvents(list);

            // Then
            Assert.AreEqual(0, eventCount);
            Assert.AreEqual(eventCount, list.Count);
        }

        [Test]
        public void KegShouldAnnounceKegEventsChanged()
        {
            // Given
            var flow = 1;
            KegEventsChangedArgs kegEventsChangedArgs = null;
            
            var calculator = Substitute.For<FlowCalculator>();
            calculator.CalculateAggregateFlow(Arg.Any<Keg>()).Returns((_)=>flow);
            
            Impl.KegBase keg = kegBuilder.WithCalculator(calculator).StartWith(0f).Build();

            // When
            var events = new List<TimedEvent>();
            keg.AppendCurrentEvents(events);
            float previousTime = events.Where(evt => evt.Type == KegEvent.Filled)
                .Select(evt=>evt.Time).FirstOrDefault();
            
            keg.EventsChanged += OnEventsChanged;
            
            flow = 2;
            keg.InvalidateFlowCache();
            var newEvents = new List<TimedEvent>();
            keg.AppendCurrentEvents(newEvents);
            float changedTime = kegEventsChangedArgs.Changes.First(evt => evt.Type == KegEvent.Filled).Time;

            // Then
            Assert.AreEqual(previousTime/2f, changedTime);            
            
            void OnEventsChanged(KegEventsChangedArgs args)
            {
                kegEventsChangedArgs = args;
            }
        }
    }
}
