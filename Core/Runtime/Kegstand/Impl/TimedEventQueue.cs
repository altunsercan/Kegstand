using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine.Assertions;

namespace Kegstand.Impl
{
    public class TimedEventQueue<TTimeValue> : List<TimedEvent<TTimeValue>>, TimedEventQueue 
        where TTimeValue : IComparable<TTimeValue>
    {
        [NotNull]
        private readonly Func<float, TTimeValue> timeUnitConverter;

        public TimedEventQueue(Func<float, TTimeValue> timeUnitConverter)
        {
            Assert.IsNotNull(timeUnitConverter);
            this.timeUnitConverter = timeUnitConverter;
        }

        public TimedEvent EnqueueNewEventToBuffer(Keg keg, float deltaTimeUnit, KegEvent eventType)
        {
            var timedEvent = new TimedEvent<TTimeValue>(keg, timeUnitConverter(deltaTimeUnit), eventType);
            Add(timedEvent);
            return timedEvent;
        }
    }
}