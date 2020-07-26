﻿using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using NUnit.Framework;

namespace Kegstand.Tests
{
    public class SimulatorTests
    {
        private KegBase.Builder<KegBase> kegBuilder;
        
        [SetUp]
        public void Setup()
        {
            var calculator = new FlowCalculatorImpl();
            kegBuilder = new KegBase.Builder<KegBase>(flowCalculator:calculator);
        }
        
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
            Assert.That(simulator.Events, Is.Ordered.Ascending.By("Time"));                
        }

        [TestCase(1)]
        [TestCase(5)]
        public void ShouldBeAbleToRegisterStands(int standCount)
        {
            // Given
            Simulator simulator = new Simulator();
            List<Stand> testList = new List<Stand>();
            
            // When
            for (int i = 0; i < standCount; i++)
            {
                Stand stand = Substitute.For<Stand>();
                simulator.Register(stand);
                testList.Add(stand);
            }

            // Then
            Assert.AreNotEqual(0, simulator.Stands.Count());
            Assert.That(testList, Is.EquivalentTo(simulator.Stands));
        }


        [Test]
        public void ShouldNotRegisterSameStandMultipleTimes()
        {
            // Given
            Simulator simulator = new Simulator();
            
            // When
            Stand stand = Substitute.For<Stand>();
            simulator.Register(stand);
            simulator.Register(stand);
            
            // Then
            Assert.AreEqual(1, simulator.Stands.Count());
        }

        [Test]
        public void ShouldRegisterStandEvents()
        {
            // Given
            Simulator simulator = new Simulator();
            Stand stand = Substitute.For<Stand>();
            stand.Kegs.Returns((_)=>new List<KegEntry>(){ MakeTestKeg() });
            
            // When
            simulator.Register(stand);

            // Then
            var kegs = stand.Received(1).Kegs;
            
            Assert.AreEqual(1, simulator.Events.Count);


            KegEntry MakeTestKeg()
            {
                var key = new object();
                var keg = Substitute.For<Keg>();
                keg.AppendCurrentEvents(Arg.Any<List<TimedEvent>>())
                    .Returns((callInfo)=>
                    {
                        var list = callInfo.Arg<List<TimedEvent>>();
                        list.Add(new TimedEvent()
                        {
                            Index = 0, Time = 1f, Type = KegEvent.Filled
                        });
                        return 1;
                    });
                return new KegEntry(key, keg);
            }
        }

        [Test]
        public void ShouldInvokeEventsWithElapsedTimers()
        {
            // Given
            Simulator simulator = new Simulator();
            simulator.AddEvent(2f, 1);
            simulator.AddEvent(5f, 2);
            simulator.AddEvent(7f, 3);
            
            int total = 0;
            simulator.EventTriggered += OnEventTriggered;

            // When & Then

            simulator.Update(1f);
            Assert.AreEqual(0, total);
            simulator.Update(1f);
            Assert.AreEqual(1, total);
            simulator.Update(4f);
            Assert.AreEqual(2, total);
            simulator.Update(0.5f);
            Assert.AreEqual(2, total);
            simulator.Update(4f);
            Assert.AreEqual(3, total);
            
            Assert.AreEqual(0, simulator.Events.Count);

            void OnEventTriggered(TimedEvent evt)
            {
                total++;
            }
        }
    }
}