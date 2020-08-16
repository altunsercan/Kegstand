using System;
using System.Diagnostics.CodeAnalysis;

namespace Kegstand
{

    public interface TimedEvent
    {
        Keg Index { get; }
        KegEvent Type { get; }
    }
    
    [ExcludeFromCodeCoverage] // Pure data class with no logic
    public class TimedEvent<TTimeValue> : TimedEvent where TTimeValue : IComparable<TTimeValue>
    {
        public Keg Index { get; }
        // TODO: Use ref return
        public TTimeValue Time { get; }
        public KegEvent Type { get; }

        public TimedEvent(Keg index, TTimeValue time, KegEvent type)
        {
            Index = index;
            Time = time;
            Type = type;
        }

        public bool IsPassed(Clock<TTimeValue> clock)
        {
            ref TTimeValue clockTime = ref clock.GetCurrentTimePassed();

            return clockTime.CompareTo(Time) >= 0;
        }
    }
}