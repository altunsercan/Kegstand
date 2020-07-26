using System.Collections.Generic;
using UnityEngine;

namespace Kegstand
{
    public enum KegEvent
    {
        Filled,
        Emptied,
    }
    
    public class TimedEvent
    {
        public float Time { get; set; }
        public int Index { get; set; }
        public KegEvent Type { get; set; }
    }
    
    public class Simulator
    {
        public readonly IReadOnlyList<TimedEvent> Events;
        public readonly IReadOnlyList<Stand> Stands;

        private readonly List<TimedEvent> events = new List<TimedEvent>();
        private readonly List<Stand> stands = new List<Stand>();
        
        public Simulator()
        {
            Events = events.AsReadOnly();
            Stands = stands.AsReadOnly();
        }
        
        public void AddEvent(float time, int index)
        {
            events.Add(new TimedEvent(){Time = time, Index = index});
            events.Sort((x,y)=>(x.Time == y.Time)?0:(x.Time>y.Time)?1:-1);
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
                AddEvent(timedEvent.Time, timedEvent.Index);
            }
            
        }
    }
}