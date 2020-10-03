using System;
using Kegstand.Impl;

namespace Kegstand
{
    public partial class Simulator<TTimeValue, TClock> : Simulator 
    {
        public class Builder
        {
            public Simulator<TTimeValue, TClock> Build(TimedEventQueue<TTimeValue> queueImplementation, AmountVisitor<TTimeValue> visitorImplementation )
            {
                var simulator = new Simulator<TTimeValue, TClock>(queueImplementation, visitorImplementation);
                return simulator;
            }
        }
    }
}