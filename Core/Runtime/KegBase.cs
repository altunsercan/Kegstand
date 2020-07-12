using System;
using UnityEngine;

namespace Kegstand
{
    public class KegBase : Keg
    {
        public float MaxAmount { get; private set; }
        public float MinAmount { get; private set; }
        public float Amount { get; private set; }

        public KegBase(float maxAmount, float minAmount, float startingAmount)
        {
            MaxAmount = maxAmount;
            MinAmount = minAmount;
            Amount = startingAmount;
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
    }
}