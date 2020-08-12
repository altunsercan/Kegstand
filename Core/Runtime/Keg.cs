using System;
using System.Collections.Generic;

namespace Kegstand
{
    public interface Keg
    {
        event KegEventsChangedDelegate EventsChanged;
        float MaxAmount { get; }
        float MinAmount { get; }
        float Amount { get; }
        float AggregateFlow { get; }
        IReadOnlyList<Tap> TapList { get; }
        void Increment(float delta);
        void Decrement(float decrement);
        int AppendCurrentEvents(List<TimedEvent> list);
        void AddTap(Tap tap);
    }
}
