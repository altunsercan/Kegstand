using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kegstand
{
    public partial class KegBase : Keg
    {
        private FlowCalculator flowCalculator;
        public float MaxAmount { get; private set; }
        public float MinAmount { get; private set; }
        public float Amount { get; private set; }

        private bool isDirtyAggregateFlow = true;
        private float cachedAggregateFlow;
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
        
        /*
        public KegBase(FlowCalculator flowCalculator, float maxAmount, float minAmount, float startingAmount)
        {
            this.flowCalculator = flowCalculator;
            
            MaxAmount = maxAmount;
            MinAmount = minAmount;
            Amount = startingAmount;

            tapList = new List<Tap>();
            TapList = tapList.AsReadOnly();
        }*/


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

        public void AddTap(Tap tap)
        {
            if (tapList.Contains(tap))
            {
                return;
            }
            tapList.Add(tap);
            isDirtyAggregateFlow = true;
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
    }
}