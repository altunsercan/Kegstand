﻿using JetBrains.Annotations;

namespace Kegstand.Impl
{
    public partial class KegBase
    {
        public class Builder<T> where T : KegBase, new()
        {
            private FlowCalculator flowCalculator;
            
            private float minAmount;
            private float maxAmount;
            private float startAmount;
            public Builder(float minAmount = 0f, float maxAmount = 100f, FlowCalculator flowCalculator = null)
            {
                this.flowCalculator = flowCalculator;
                
                this.maxAmount = maxAmount;
                this.minAmount = minAmount;
            }
            
            public Builder<T> Max(float max)
            {
                this.maxAmount = max;
                return this;
            }

            public Builder<T> Min(float min)
            {
                this.minAmount = min;
                return this;
            }

            public Builder<T> StartWith(float startsWith)
            {
                this.startAmount = startsWith;
                return this;
            }

            public Builder<T> WithCalculator(FlowCalculator calculator)
            {
                flowCalculator = calculator;
                return this;
            }

            public T Build()
            {
                var keg = new T();
                keg.flowCalculator = flowCalculator;
                keg.amount = startAmount;
                keg.MinAmount = minAmount;
                keg.MaxAmount = maxAmount;
                return keg;
            }
        }
    }
}