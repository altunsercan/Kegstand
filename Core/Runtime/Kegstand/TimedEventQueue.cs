using System.Collections.Generic;

namespace Kegstand
{
    public interface TimedEventQueue
    {
        TimedEvent EnqueueNewEventToBuffer(Keg keg, float deltaTimeUnit, KegEvent eventType);
        int Count { get; }
    }
}