using System;
using System.Collections.Generic;
using System.Linq;
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

        #region IList<TimedEvent>
        IEnumerator<TimedEvent> IEnumerable<TimedEvent>.GetEnumerator()
        {
            return this.Cast<TimedEvent>().GetEnumerator();
        }
        
        void ICollection<TimedEvent>.Add(TimedEvent item)
        {
            if (item is TimedEvent<TTimeValue> typedItem)
            {
                Add(typedItem);
            }
        }

        bool ICollection<TimedEvent>.Contains(TimedEvent item)
        {
            if (item is TimedEvent<TTimeValue> typedItem)
            {
                return Contains(typedItem);
            }
            return false;
        }

        public void CopyTo(TimedEvent[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        bool ICollection<TimedEvent>.Remove(TimedEvent item)
        {
            if (item is TimedEvent<TTimeValue> typedItem)
            {
                return Remove(typedItem);
            }
            return false;
        }

        bool ICollection<TimedEvent>.IsReadOnly => false;
        
        int IList<TimedEvent>.IndexOf(TimedEvent item)
        {
            throw new NotImplementedException();
        }

        void IList<TimedEvent>.Insert(int index, TimedEvent item)
        {
            throw new NotImplementedException();
        }

        TimedEvent IList<TimedEvent>.this[int index]
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
        #endregion  IList<TimedEvent>

        public TimedEvent EnqueueNewEventToBuffer(Keg keg, float deltaTimeUnit, KegEvent eventType)
        {
            var timedEvent = new TimedEvent<TTimeValue>(keg, timeUnitConverter(deltaTimeUnit), eventType);
            Add(timedEvent);
            return timedEvent;
        }
    }
}