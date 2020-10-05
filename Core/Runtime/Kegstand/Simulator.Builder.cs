using System;
using Kegstand.Impl;

namespace Kegstand
{
    public partial class Simulator<TTimeValue, TClock>  
    {
        public class Builder
        {
            private FillUpdateDispatcher fillUpdateDispatcher;

            internal Builder WithFillDispatcher(FillUpdateDispatcher dispatcher)
            {
                fillUpdateDispatcher = dispatcher;
                return this;
            }
            
            public Simulator<TTimeValue, TClock> Build(Clock<TTimeValue> clock, TimedEventQueue<TTimeValue> queueImplementation, AmountVisitor<TTimeValue> visitorImplementation )
            {
                var simulator = new Simulator<TTimeValue, TClock>(clock, queueImplementation, visitorImplementation);
                simulator.fillUpdateDispatcher = fillUpdateDispatcher ?? new FillUpdateDispatcher();
                
                return simulator;
            }
        }
    }
}