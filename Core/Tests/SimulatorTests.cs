using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using Kegstand.Impl;
using NSubstitute;
using NSubstitute.Core;
using NSubstitute.Routing.AutoValues;
using NUnit.Framework;

namespace Kegstand.Tests
{
    public class SimulatorTests
    {
        private Impl.KegBase.Builder<Impl.KegBase> kegBuilder;
        
        [SetUp]
        public void Setup()
        {
            var calculator = new FlowCalculatorImpl();
            kegBuilder = new Impl.KegBase.Builder<Impl.KegBase>(flowCalculator:calculator);
        }
        
        [Test]
        [TestCase(20,3,4,12,5,67)]
        [TestCase(20,-5,4,-2,5,23)]
        public void ShouldMaintainTimeOrderedEvents(params float[] eventTimes)
        {
            // Given
            Simulator<TimeSpanClock> simulator = new Simulator<TimeSpanClock>();
            
            // When
            for (var index = 0; index < eventTimes.Length; index++)
            {
                simulator.AddEvent(new TimedEvent(Substitute.For<Keg>(), eventTimes[index], KegEvent.Filled));
            }            
            
            // Then
            Assert.That(simulator.Events, Is.Ordered.Ascending.By("Time"));                
        }

        [TestCase(1)]
        [TestCase(5)]
        public void ShouldBeAbleToRegisterStands(int standCount)
        {
            // Given
            Simulator<TimeSpanClock> simulator = new Simulator<TimeSpanClock>();
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
            Simulator<TimeSpanClock> simulator = new Simulator<TimeSpanClock>();
            
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
            Simulator<TimeSpanClock> simulator = new Simulator<TimeSpanClock>();
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
                        list.Add(new TimedEvent( keg,1f, KegEvent.Filled ));
                        return 1;
                    });
                return new KegEntry(key, keg);
            }
        }

        [Test]
        public void ShouldInvokeEventsWithElapsedTimers()
        {
            // Given
            Simulator<TimeSpanClock> simulator = new Simulator<TimeSpanClock>();
            simulator.AddEvent(new TimedEvent( Substitute.For<Keg>(), 2f, KegEvent.Filled ));
            simulator.AddEvent(new TimedEvent( Substitute.For<Keg>(), 5f, KegEvent.Filled ));
            simulator.AddEvent(new TimedEvent( Substitute.For<Keg>(), 7f, KegEvent.Filled ));

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

        [Test]
        public void ShouldRescheduleEventsOnKegEventChange()
        {
            // Given
            Simulator<TimeSpanClock> simulator = new Simulator<TimeSpanClock>();
            Stand stand = Substitute.For<Stand>();
            
            simulator.Register(stand);
            
            // When
            var changeList = new List<TimedEvent>();
            var fakeEvent = new TimedEvent(Substitute.For<Keg>(), 2124125f, KegEvent.Filled);
            changeList.Add(fakeEvent);

            var keg = Substitute.For<Keg>();
            var eventsChangedArgs = new KegEventsChangedArgs(keg, changeList);

            stand.EventsChanged += Raise.Event<KegEventsChangedDelegate>(eventsChangedArgs);

            // Then
            Assert.AreEqual(2124125f, simulator.Events
                .Where(evt=> evt.Type==KegEvent.Filled)
                .Select(evt=> evt.Time).First());
        }
    }
}