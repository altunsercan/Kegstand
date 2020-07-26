﻿using System;
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
         
            List<TimedEvent> kegEvents = new List<TimedEvent>();
            stands.Add(stand);
            var kegs = stand.Kegs;
            foreach (KegEntry kegEntry in kegs)
            {
                kegEntry.Keg.AppendCurrentEvents(kegEvents);
            }

            foreach (TimedEvent timedEvent in kegEvents)
            {
                events.Add(timedEvent);
            }
            events.Sort(SortEventsByTimeComparison);
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
        
        private static int SortEventsByTimeComparison(TimedEvent x, TimedEvent y)
        {
            return (x.Time == y.Time) ? 0 : (x.Time > y.Time) ? 1 : -1;
        }
    }
}