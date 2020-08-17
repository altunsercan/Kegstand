using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using UnityEngine.Assertions;

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
        [NotNull] public Keg Index { get; }
        
        [NotNull] private readonly TTimeValue time;
        [NotNull] public ref readonly TTimeValue Time => ref time;

        public KegEvent Type { get; }

        public TimedEvent(Keg index, TTimeValue time, KegEvent type)
        {
            Assert.IsNotNull(index);
            Assert.AreNotEqual(default(TTimeValue), time);

            Index = index;
            this.time = time;
            Type = type;
        }

        public bool IsPassed(Clock<TTimeValue> clock)
        {
            Assert.IsNotNull(clock);
            
            ref TTimeValue clockTime = ref clock.GetCurrentTimePassed();

            return clockTime.CompareTo(Time) >= 0;
        }
    }
}