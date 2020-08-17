using System.Collections.Generic;

namespace Kegstand
{
    public interface TimedEventQueue : IList<TimedEvent>
    {
        TimedEvent EnqueueNewEventToBuffer(Keg keg, float deltaTimeUnit, KegEvent eventType);
    }
}