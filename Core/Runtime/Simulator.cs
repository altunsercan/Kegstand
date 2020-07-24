using System.Collections.Generic;
using UnityEngine;

namespace Kegstand
{
    public class TimedEvent
    {
        public float Time { get; set; }
        public int Index { get; set; }
    }
    
    public class Simulator
    {
        
        public IEnumerable<TimedEvent> Events { get; private set; }
        private readonly List<TimedEvent> events = new List<TimedEvent>();
        public Simulator()
        {
            Events = events;
        }
        
        public void AddEvent(float time, int index)
        {
            events.Add(new TimedEvent(){Time = time, Index = index});
            events.Sort((x,y)=>(x.Time == y.Time)?0:(x.Time>y.Time)?1:-1);
        }
    }
}