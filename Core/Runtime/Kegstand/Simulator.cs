using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine.Assertions;

namespace Kegstand
{
    public interface Simulator
    {
        IReadOnlyList<Stand> Stands { get; }
        event Action<TimedEvent> EventTriggered;
        void AddEvent(TimedEvent timedEvent);
        void Register(Stand stand);
        void Update(float deltaTime);
    }

    public class Simulator<TClock> : Simulator where TClock : Clock, new()
    {
        [NotNull] private static readonly List<TimedEvent> TimedEventsScratchList = new List<TimedEvent>();
        
        public readonly IReadOnlyList<TimedEvent> Events;
        public IReadOnlyList<Stand> Stands { get; }

        [NotNull] private readonly List<TimedEvent> events = new List<TimedEvent>();
        [NotNull] private readonly List<Stand> stands = new List<Stand>();
        
        public event Action<TimedEvent> EventTriggered;

        private readonly TClock clockObj;
        private float clock;
        
        public Simulator()
        {
            clockObj = new TClock();
            Events = events.AsReadOnly();
            Stands = stands.AsReadOnly();
        }

        public void AddEvent(TimedEvent timedEvent)
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
                keg.AppendCurrentEvents(TimedEventsScratchList);
            }

            events.AddRange(TimedEventsScratchList);
            events.Sort(SortEventsByTimeComparison);
            TimedEventsScratchList.Clear();
            
            stand.EventsChanged += OnKegEventsChanged;
        }

        public void Update(float deltaTime)
        {
            clock += deltaTime;
            var i = 0;
            for (; i < events.Count; i++)
            {
                TimedEvent timedEvent = events[i];
                if(timedEvent==null) { return; }
                if (timedEvent.Time > clock) { break; }
                
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
                TimedEvent changedEvt = changeArgs.Changes[changeIndex];
                if (changedEvt == null) { continue; }
                
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

        private bool ReplaceEventInIndexIfMatched([NotNull] Keg keg, int index, [NotNull] TimedEvent changedEvt)
        {
            TimedEvent existingEvt = events[index];

            if (existingEvt == null) { return false; }

            if (existingEvt.Index == keg && existingEvt.Type == changedEvt.Type)
            {
                events[index] = changedEvt;
                return true;
            }

            return false;
        }

        private static int SortEventsByTimeComparison(TimedEvent x, TimedEvent y)
        {
            if (x == null && y == null) { return 0; }
            if (x == null) { return -1; }
            if (y == null) { return 1; }
            
            return (Math.Abs(x.Time - y.Time) < float.Epsilon) ? 0 : (x.Time > y.Time) ? 1 : -1;
        }
    }
}