using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Kegstand.Impl;
using UnityEngine.Assertions;

namespace Kegstand
{
    public interface Simulator
    {
        IReadOnlyList<Stand> Stands { get; }
        event Action<TimedEvent> EventTriggered;
        event Action ClockTicked;
        bool AddEvent(TimedEvent timedEvent);
        void Register(Stand stand);
        void Update(float deltaTime);
        IDisposable ObserveKegFill(Keg keg, IObserver<float> kegValueBar);
    }

    public partial class Simulator<TTimeValue, TClock> : Simulator 
        where TClock : class, Clock<TTimeValue>, new()
        where TTimeValue : IComparable<TTimeValue>
    {
        [NotNull] private readonly TimedEventQueue<TTimeValue> timedEventsScratchList;
        [NotNull] private AmountVisitor<TTimeValue> amountVisitor;
        [NotNull] private FillUpdateDispatcher fillUpdateDispatcher;

        public readonly IReadOnlyList<TimedEvent<TTimeValue>> Events;
        public IReadOnlyList<Stand> Stands { get; }

        [NotNull] [ItemNotNull] private readonly List<TimedEvent<TTimeValue>> events = new List<TimedEvent<TTimeValue>>();
        [NotNull] private readonly List<Stand> stands = new List<Stand>();

        public event Action<TimedEvent> EventTriggered;
        public event Action ClockTicked;

        [NotNull]
        private readonly Clock<TTimeValue> clock;

        private Simulator(
            Clock<TTimeValue> clock,
            TimedEventQueue<TTimeValue> eventQueue,
            AmountVisitor<TTimeValue> amountVisitor)
        {
            Assert.IsNotNull(clock);
            Assert.IsNotNull(eventQueue);
            Assert.IsNotNull(amountVisitor);

            timedEventsScratchList = eventQueue;
            this.amountVisitor = amountVisitor;

            this.clock = clock;
            Events = events.AsReadOnly();
            Stands = stands.AsReadOnly();
        }

        public bool AddEvent(TimedEvent timedEvent)
        {
            if (!(timedEvent is TimedEvent<TTimeValue> validTimeEvent)) return false;
            
            AddEvent(validTimeEvent);
            return true;
        }
        
        private void AddEvent(TimedEvent<TTimeValue> timedEvent)
        {
            events.Add(timedEvent);
            events.Sort(SortEventsByTimeComparison);
        }

        public void Register(Stand stand)
        {
            Assert.IsNotNull(stand);
            
            if (stands.Contains(stand))
            {
                return;
            }
         
            stands.Add(stand);
            var kegs = stand.Kegs;
            for (var kegIndex = 0; kegIndex < kegs.Count; kegIndex++)
            {
                KegEntry kegEntry = kegs[kegIndex];
                Keg keg = kegEntry.Keg;
                keg.AppendCurrentEvents(amountVisitor, timedEventsScratchList);
            }

            // TODO: Move sorting logic to the Queue
            events.AddRange(timedEventsScratchList);
            events.Sort(SortEventsByTimeComparison);
            timedEventsScratchList.Clear();
            
            stand.EventsChanged += OnKegEventsChanged;
        }

        public void Update(float deltaTime)
        {
            clock.Update(deltaTime); 
            
            ProcessEvents();

            fillUpdateDispatcher.DispatchUpdate(amountVisitor);
            
            ClockTicked?.Invoke();
        }

        private void ProcessEvents()
        {
            var i = 0;
            for (; i < events.Count; i++)
            {
                TimedEvent<TTimeValue> timedEvent = events[i];

                if (!timedEvent.IsPassed(clock))
                {
                    break;
                }

                EventTriggered?.Invoke(timedEvent);
            }

            events.RemoveRange(0, i);
        }

        private void OnKegEventsChanged([NotNull] KegEventsChangedArgs changeArgs)
        {
            Keg keg = changeArgs.Keg;
            IReadOnlyList<TimedEvent> changes = changeArgs.Changes;
            
            for (var changeIndex = 0; changeIndex < changes.Count; changeIndex++)
            {
                TimedEvent evt = changeArgs.Changes[changeIndex];
                if (!(evt is TimedEvent<TTimeValue> changedEvt)) { continue; }
                
                var isNewEvent = true;
                for (var index = 0; isNewEvent && (index < events.Count); index++)
                {
                    isNewEvent = !ReplaceEventInIndexIfMatched(keg, index, changedEvt);
                }

                if (isNewEvent)
                {
                    events.Add(changedEvt);
                }
            }
        }
        
        public IDisposable ObserveKegFill(Keg keg, IObserver<float> kegValueBar) => fillUpdateDispatcher.GetFillObservable(keg).Subscribe(kegValueBar);

        private bool ReplaceEventInIndexIfMatched([NotNull] Keg keg, int index, [NotNull] TimedEvent<TTimeValue> changedEvt)
        {
            TimedEvent existingEvt = events[index];
            
            if (existingEvt.Index == keg && existingEvt.Type == changedEvt.Type)
            {
                events[index] = changedEvt;
                return true;
            }

            return false;
        }

        private static int SortEventsByTimeComparison([NotNull]TimedEvent<TTimeValue> x, [NotNull]TimedEvent<TTimeValue> y)
        {
            return x.Time.CompareTo(y.Time);
        }
    }
}