using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Assertions;

namespace Kegstand.Impl
{
    public delegate void KegEventsChangedDelegate([NotNull] KegEventsChangedArgs changes);
    public delegate void KegFillChangedDelegate([NotNull] KegFillChangedArgs args);
    
    public partial class KegBase : Keg
    {
        public event KegEventsChangedDelegate EventsChanged;
        
        private FlowCalculator flowCalculator;
        public float MaxAmount { get; private set; }
        public float MinAmount { get; private set; }

        private Timestamp timestamp;
        private float amount;
        public float Amount(IAmountVisitor amountVisitor)
        {
            Assert.IsNotNull(amountVisitor);
            float amountCalculated = amountVisitor.CalculateCurrentAmount(amount, AggregateFlow, timestamp);
            return Mathf.Clamp(amountCalculated, MinAmount, MaxAmount);
        }

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
            
            amount = Mathf.Min(amount + delta, MaxAmount);
        }

        public void Decrement(float delta)
        {
            if (delta < 0f)
            {
                throw new ArgumentException("Argument should not be less than zero", nameof(delta));
            }
            
            amount = Mathf.Max(amount - delta, MinAmount);
        }

        public int AppendCurrentEvents(IAmountVisitor amountVisitor, TimedEventQueue queue)
        {
            Assert.IsNotNull(queue);
            Assert.IsNotNull(amountVisitor);
            
            if (isDirtyCurrentEvents)
            {
                isDirtyCurrentEvents = false;
                CreateCurrentEvents(amountVisitor, currentEvents, queue);
            }
            
            var args = new KegEventsChangedArgs(this, currentEvents.AsReadOnly());
            EventsChanged?.Invoke(args) ;
            return currentEvents.Count;
        }

        private void CreateCurrentEvents(
            [NotNull] IAmountVisitor amountVisitor,
            [NotNull] List<TimedEvent> timedEvents,
            [NotNull] TimedEventQueue queue)
        {
            timedEvents.Clear();
            TimedEvent timedEvent;
            if(CreateFillEvent(amountVisitor, queue, out timedEvent))
            {
                timedEvents.Add(timedEvent);
            }
            
            if (CreateEmptyEvent(amountVisitor, queue, out timedEvent))
            {
                timedEvents.Add(timedEvent);
            }
        }

        private bool CreateEmptyEvent(
            [NotNull] IAmountVisitor amountVisitor,
            [NotNull]TimedEventQueue queue,
            out TimedEvent timedEvent)
        {
            timedEvent = null;
            if (AggregateFlow >= 0 || Mathf.Approximately(Amount(amountVisitor), MinAmount)) { return false; }
            float timeToEmpty = (Amount(amountVisitor) - MinAmount) / -AggregateFlow;
            timedEvent = queue.EnqueueNewEventToBuffer(this, timeToEmpty, KegEvent.Emptied);
            return true;
        }

        private bool CreateFillEvent(
            [NotNull] IAmountVisitor amountVisitor,
            [NotNull] TimedEventQueue queue,
            out TimedEvent timedEvent)
        {
            timedEvent = null;
            if (AggregateFlow <= 0 || Mathf.Approximately(Amount(amountVisitor), MaxAmount)) { return false; }
            
            float timeToFill = (MaxAmount - Amount(amountVisitor)) / AggregateFlow;
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

        [Obsolete("Created for testing should be refactored to remove")]
        public void InvalidateFlowCache()
        {
            isDirtyAggregateFlow = true;
            isDirtyCurrentEvents = true;
        }
    }
}