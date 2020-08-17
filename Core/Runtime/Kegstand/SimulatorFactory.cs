using System;
using Kegstand.Impl;

namespace Kegstand
{
    public class SimulatorFactory
    {
        public static Simulator<TimeSpan, TimeSpanClock> CreateDefault()
        {
            var eventQueue = new TimedEventQueue<TimeSpan>(timeUnit => TimeSpan.FromSeconds(timeUnit));
            var amountVisitor = new TimeSpanAmountVisitor(new Timestamp<TimeSpan>(TimeSpan.Zero));
            return new Simulator<TimeSpan, TimeSpanClock>(eventQueue, amountVisitor);
        }
    }
}