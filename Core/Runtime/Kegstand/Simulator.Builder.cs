using System;
using Kegstand.Impl;
using UnityEngine.Assertions;

namespace Kegstand
{
    public partial class Simulator<TTimeValue>  
    {
        public class Builder
        {
            private FillUpdateDispatcher fillUpdateDispatcher;
            private Clock<TTimeValue> clock;
            private TimedEventQueue<TTimeValue> queue;
            private AmountVisitor<TTimeValue> visitor;

            internal Builder WithFillDispatcher(FillUpdateDispatcher dispatcher)
            {
                fillUpdateDispatcher = dispatcher;
                return this;
            }

            public Builder WithQueue(TimedEventQueue<TTimeValue> queueImplementation)
            {
                this.queue = queueImplementation;
                return this;
            }
            
            public Builder WithClock(Clock<TTimeValue> clock)
            {
                this.clock = clock;
                return this;
            }

            public Builder WithVisitor(AmountVisitor<TTimeValue> visitor)
            {
                this.visitor = visitor;
                return this;
            }

            public Simulator<TTimeValue> Build()
            {
                Assert.IsNotNull(queue);
                Assert.IsNotNull(visitor);
                Assert.IsNotNull(clock);
                
                var simulator = new Simulator<TTimeValue>(clock, queue, visitor);
                simulator.fillUpdateDispatcher = fillUpdateDispatcher ?? new FillUpdateDispatcher();
                
                return simulator;
            }
        }
    }
}