using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kegstand
{
    public class KegBase : Keg
    {
        public float MaxAmount { get; private set; }
        public float MinAmount { get; private set; }
        public float Amount { get; private set; }

        List<Tap> tapList;
        public KegBase(float maxAmount, float minAmount, float startingAmount)
        {
            MaxAmount = maxAmount;
            MinAmount = minAmount;
            Amount = startingAmount;

            tapList = new List<Tap>();
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

        public void AddTap(Tap tap)
        {
            if (tapList.Contains(tap))
            {
                return;
            }
            tapList.Add(tap);
        }

        public void Update(float deltaTime)
        {
            float delta = 0;
            foreach (Tap tap in tapList)
            {
                delta += tap.FlowAmount;
            }

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