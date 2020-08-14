using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Kegstand.Impl;

namespace Kegstand
{
    public interface Keg
    {
        event KegEventsChangedDelegate EventsChanged;
        float MaxAmount { get; }
        float MinAmount { get; }
        float Amount { get; }
        float AggregateFlow { get; }
        [NotNull] IReadOnlyList<Tap> TapList { get; }
        void Increment(float delta);
        void Decrement(float decrement);
        [Obsolete("Method will be moved out out of public interface")]
        int AppendCurrentEvents(List<TimedEvent> list);
        [Obsolete("Method will be moved out out of public interface")]
        void AddTap(Tap tap);
    }
}
