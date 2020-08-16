using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Assertions;

namespace Kegstand.Impl
{
    public delegate void KegEventsChangedDelegate([NotNull] KegEventsChangedArgs changes);

    public partial class KegBase : Keg
    {
        public event KegEventsChangedDelegate EventsChanged;
        
        private FlowCalculator flowCalculator;
        public float MaxAmount { get; private set; }
        public float MinAmount { get; private set; }
        public float Amount { get; private set; }

        private bool isDirtyAggregateFlow = true;
        private float cachedAggregateFlow;

        private bool isDirtyCurrentEvents = true;
        [NotNull]
        private readonly List<TimedEvent> currentEvents = new List<TimedEvent>(); 
        
        public float AggregateFlow
        {
            get
            {
                if (isDirtyAggregateFlow)
                {
                    isDirtyAggregateFlow = false;
                    cachedAggregateFlow = flowCalculator.CalculateAggregateFlow(this);
                }
                return cachedAggregateFlow;
            }
        } //private set; }

        public IReadOnlyList<Tap> TapList { get; } // Initialized in constructor
        [NotNull] private readonly List<Tap> tapList = new List<Tap>();

        public KegBase()
        {
            TapList = tapList.AsReadOnly();
        }

        public void Increment(float delta)
        {
            if (delta < 0f)
            {
                throw new ArgumentException("Argument should not be less than zero", nameof(delta));
            }
            
            Amount = Mathf.Min(Amount + delta, MaxAmount);
        }

        public void Decrement(float delta)
        {
            if (delta < 0f)
            {
                throw new ArgumentException("Argument should not be less than zero", nameof(delta));
            }
            
            Amount = Mathf.Max(Amount - delta, MinAmount);
        }

        public int AppendCurrentEvents(TimedEventQueue queue)
        {
            Assert.IsNotNull(queue);
            
            if (isDirtyCurrentEvents)
            {
                isDirtyCurrentEvents = false;
                CreateCurrentEvents(currentEvents, queue);
            }
            
            var args = new KegEventsChangedArgs(this, currentEvents.AsReadOnly());
            EventsChanged?.Invoke(args) ;
            return currentEvents.Count;
        }

        private void CreateCurrentEvents([NotNull] List<TimedEvent> timedEvents, [NotNull]TimedEventQueue queue)
        {
            timedEvents.Clear();
            TimedEvent timedEvent;
            if(CreateFillEvent(queue, out timedEvent))
            {
                timedEvents.Add(timedEvent);
            }
            
            if (CreateEmptyEvent(queue, out timedEvent))
            {
                timedEvents.Add(timedEvent);
            }
        }

        private bool CreateEmptyEvent([NotNull]TimedEventQueue queue, out TimedEvent timedEvent)
        {
            timedEvent = null;
            if (AggregateFlow >= 0 || Mathf.Approximately(Amount, MinAmount)) { return false; }
            float timeToEmpty = (Amount - MinAmount) / -AggregateFlow;
            timedEvent = queue.EnqueueNewEventToBuffer(this, timeToEmpty, KegEvent.Emptied);
            return true;
        }

        private bool CreateFillEvent([NotNull]TimedEventQueue queue, out TimedEvent timedEvent)
        {
            timedEvent = null;
            if (AggregateFlow <= 0 || Mathf.Approximately(Amount, MaxAmount)) { return false; }
            
            float timeToFill = (MaxAmount - Amount) / AggregateFlow;
            timedEvent = queue.EnqueueNewEventToBuffer(this, timeToFill, KegEvent.Filled);
            return true;
        }

        public void AddTap(Tap tap)
        {
            if (tapList.Contains(tap))
            {
                return;
            }
            tapList.Add(tap);
            isDirtyAggregateFlow = true;
            isDirtyCurrentEvents = true;
        }

        public void Update(float deltaTime)
        {
            var delta = flowCalculator.CalculateAggregateFlow(this);
            if (delta > 0)
            {
                Increment(delta*deltaTime);
            }
            else
            {
                Decrement(-delta*deltaTime);
            }
        }

        [Obsolete("Created for testing should be refactored to remove")]
        public void InvalidateFlowCache()
        {
            isDirtyAggregateFlow = true;
            isDirtyCurrentEvents = true;
        }
    }
}