using System;
using System.Collections.Generic;

namespace Kegstand
{
    public class Simulator
    {
        public readonly IReadOnlyList<TimedEvent> Events;
        public readonly IReadOnlyList<Stand> Stands;
        
        private readonly List<TimedEvent> events = new List<TimedEvent>();
        private readonly List<Stand> stands = new List<Stand>();
        
        public event Action<TimedEvent> EventTriggered;

        private float clock = 0f;

        public Simulator()
        {
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
            if (stands.Contains(stand))
            {
                return;
            }
         
            stands.Add(stand);
            List<TimedEvent> kegEvents = new List<TimedEvent>();
            var kegs = stand.Kegs;
            foreach (KegEntry kegEntry in kegs)
            {
                var keg = kegEntry.Keg;
                keg.AppendCurrentEvents(kegEvents);
            }

            foreach (TimedEvent timedEvent in kegEvents)
            {
                events.Add(timedEvent);
            }
            events.Sort(SortEventsByTimeComparison);
            
            stand.EventsChanged+= OnKegEventsChanged;
        }

        public void Update(float deltaTime)
        {
            clock += deltaTime;
            var i = 0;
            for (; i < events.Count; i++)
            {
                TimedEvent timedEvent = events[i];
                if (timedEvent.Time > clock) { break; }
                
                EventTriggered?.Invoke(timedEvent);
            }
            
            events.RemoveRange(0, i);
        }

        private void OnKegEventsChanged(KegEventsChangedArgs changeArgs)
        {
            Keg keg = changeArgs.Keg;
            for (var changeIndex = 0; changeIndex < changeArgs.Changes.Count; changeIndex++)
            {
                TimedEvent changedEvt = changeArgs.Changes[changeIndex];

                bool isNewEvent = true;
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

        private bool ReplaceEventInIndexIfMatched(Keg keg, int index, TimedEvent changedEvt)
        {
            TimedEvent existingEvt = events[index];
            if (existingEvt.Index == keg && existingEvt.Type == changedEvt.Type)
            {
                events[index] = changedEvt;
                return true;
            }

            return false;
        }

        private static int SortEventsByTimeComparison(TimedEvent x, TimedEvent y)
        {
            return (x.Time == y.Time) ? 0 : (x.Time > y.Time) ? 1 : -1;
        }
        
        private class KegEventsChangeObserver : IObserver<KegEventsChangedArgs>
        {
            private Simulator simulator;

            public KegEventsChangeObserver(Simulator simulator)
            {
                this.simulator = simulator;
            }

            public void OnNext(KegEventsChangedArgs value)
            {
                // Auto-mocked NSubstittue will return null
                if (value == null){ return; }
                
                simulator.OnKegEventsChanged(value);
            }
            
            public void OnError(Exception error)
            {
                throw error;
            }
            
            public void OnCompleted()
            {
                // Ignore because stream can be completed when a stand is unregistered
            }
        }
    }
}