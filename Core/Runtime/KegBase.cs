using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kegstand
{
    public class KegEventsChangedArgs
    {
        public IReadOnlyList<TimedEvent> Changes { get; set; }
    }
    
    
    public partial class KegBase : Keg
    {
        public delegate void EventsChangedDelegate(Keg keg, KegEventsChangedArgs changes);

        public event EventsChangedDelegate EventsChanged;
        
        private FlowCalculator flowCalculator;
        public float MaxAmount { get; private set; }
        public float MinAmount { get; private set; }
        public float Amount { get; private set; }

        private bool isDirtyAggregateFlow = true;
        private float cachedAggregateFlow;

        private bool isDirtyCurrentEvents = true;
        private List<TimedEvent> currentEvents = new List<TimedEvent>(); 
        
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

        public IReadOnlyList<Tap> TapList { get; private set; }
        List<Tap> tapList;


        public KegBase()
        {
            tapList = new List<Tap>();
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

        public int AppendCurrentEvents(List<TimedEvent> list)
        {
            if (isDirtyCurrentEvents)
            {
                isDirtyCurrentEvents = false;
                CreateCurrentEvents(currentEvents);

            }
            
            
            var args = new KegEventsChangedArgs();
            args.Changes = currentEvents.AsReadOnly();
            EventsChanged?.Invoke(this, args) ;
            
            list.AddRange(currentEvents);
            return currentEvents.Count;
        }

        private void CreateCurrentEvents(List<TimedEvent> timedEvents)
        {
            timedEvents.Clear();
            TimedEvent timedEvent = null;
            if (AggregateFlow > 0 && !Mathf.Approximately(Amount,MaxAmount))
            {
                float timeToFill = (MaxAmount - Amount) / AggregateFlow;
                timedEvent = new TimedEvent()
                {
                    Index = this,
                    Time = timeToFill,
                    Type = KegEvent.Filled
                };    
            }else if (AggregateFlow < 0 && !Mathf.Approximately(Amount, MinAmount))
            {
                float timeToEmpty = (Amount - MinAmount) / -AggregateFlow;
                timedEvent = new TimedEvent()
                {
                    Index = this,
                    Time = timeToEmpty,
                    Type = KegEvent.Emptied
                };  
            }

            if (timedEvent != null)
            {
                timedEvents.Add(timedEvent);
            }
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