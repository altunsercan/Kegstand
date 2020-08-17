using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Kegstand.Impl;
using UnityEngine.Assertions;

namespace Kegstand
{
    public interface Simulator
    {
        IReadOnlyList<Stand> Stands { get; }
        event Action<TimedEvent> EventTriggered;
        bool AddEvent(TimedEvent timedEvent);
        void Register(Stand stand);
        void Update(float deltaTime);
    }

    public interface TimedEventQueue : IList<TimedEvent>
    {
        TimedEvent EnqueueNewEventToBuffer(Keg keg, float deltaTimeUnit, KegEvent eventType);
    }

    [Obsolete("Temporarily created for refactoring. Will be updated after preparing unit testing")]
    public class TempTimedEventQueue<TTimeValue> : List<TimedEvent<TTimeValue>>, TimedEventQueue 
        where TTimeValue : IComparable<TTimeValue>
    {
        [NotNull]
        private readonly Func<float, TTimeValue> timeUnitConverter;

        public TempTimedEventQueue(Func<float, TTimeValue> timeUnitConverter)
        {
            Assert.IsNotNull(timeUnitConverter);
            this.timeUnitConverter = timeUnitConverter;
        }

        #region IList<TimedEvent>
        IEnumerator<TimedEvent> IEnumerable<TimedEvent>.GetEnumerator()
        {
            return this.Cast<TimedEvent>().GetEnumerator();
        }
        
        void ICollection<TimedEvent>.Add(TimedEvent item)
        {
            if (item is TimedEvent<TTimeValue> typedItem)
            {
                Add(typedItem);
            }
        }

        bool ICollection<TimedEvent>.Contains(TimedEvent item)
        {
            if (item is TimedEvent<TTimeValue> typedItem)
            {
                return Contains(typedItem);
            }
            return false;
        }

        public void CopyTo(TimedEvent[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        bool ICollection<TimedEvent>.Remove(TimedEvent item)
        {
            if (item is TimedEvent<TTimeValue> typedItem)
            {
                return Remove(typedItem);
            }
            return false;
        }

        bool ICollection<TimedEvent>.IsReadOnly => false;
        
        int IList<TimedEvent>.IndexOf(TimedEvent item)
        {
            throw new NotImplementedException();
        }

        void IList<TimedEvent>.Insert(int index, TimedEvent item)
        {
            throw new NotImplementedException();
        }

        TimedEvent IList<TimedEvent>.this[int index]
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
        #endregion  IList<TimedEvent>

        public TimedEvent EnqueueNewEventToBuffer(Keg keg, float deltaTimeUnit, KegEvent eventType)
        {
            var timedEvent = new TimedEvent<TTimeValue>(keg, timeUnitConverter(deltaTimeUnit), eventType);
            Add(timedEvent);
            return timedEvent;
        }
    }

    public class SimulatorFactory
    {
        public static Simulator<TimeSpan, TimeSpanClock> CreateDefault()
        {
            var amountVisitor = new TimeSpanAmountVisitor(new Timestamp<TimeSpan>(TimeSpan.Zero));
            return new Simulator<TimeSpan, TimeSpanClock>(amountVisitor, timeUnit=> TimeSpan.FromSeconds(timeUnit));
        }
    }
    
    public class Simulator<TTimeValue, TClock> : Simulator 
        where TClock : class, Clock<TTimeValue>, new()
        where TTimeValue : IComparable<TTimeValue>
    {
        [NotNull] private readonly TempTimedEventQueue<TTimeValue> timedEventsScratchList;
        [NotNull] private AmountVisitor<TTimeValue> amountVisitor;

        public readonly IReadOnlyList<TimedEvent<TTimeValue>> Events;
        public IReadOnlyList<Stand> Stands { get; }

        [NotNull] [ItemNotNull] private readonly List<TimedEvent<TTimeValue>> events = new List<TimedEvent<TTimeValue>>();
        [NotNull] private readonly List<Stand> stands = new List<Stand>();

        public event Action<TimedEvent> EventTriggered;


        [NotNull]
        private readonly TClock clock;

        public Simulator(AmountVisitor<TTimeValue> amountVisitor, Func<float, TTimeValue> unitTimeConverter)
        {
            Assert.IsNotNull(amountVisitor);
            
            timedEventsScratchList = new TempTimedEventQueue<TTimeValue>(unitTimeConverter);
            this.amountVisitor = amountVisitor;
            
            clock = new TClock();
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
            
            var i = 0;
            for (; i < events.Count; i++)
            {
                TimedEvent<TTimeValue> timedEvent = events[i];
                
                if (!timedEvent.IsPassed(clock)) { break; }
                
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