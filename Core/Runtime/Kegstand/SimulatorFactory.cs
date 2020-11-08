using System;
using Kegstand.Impl;

namespace Kegstand
{
    public class SimulatorFactory
    {
        public static Simulator<TimeSpan> CreateDefault(Simulator<TimeSpan>.Builder builder = null)
        {
            builder = CreateDefaultBuilder(builder);
            return builder.Build();
        }

        public static Simulator<TimeSpan>.Builder CreateDefaultBuilder(Simulator<TimeSpan>.Builder builder = null)
        {
            builder = builder ?? new Simulator<TimeSpan>.Builder();

            builder.WithClock(new TimeSpanClock());
            builder.WithQueue(new TimedEventQueue<TimeSpan>(timeUnit => TimeSpan.FromSeconds(timeUnit)));
            builder.WithVisitor(new TimeSpanAmountVisitor(new Timestamp<TimeSpan>(TimeSpan.Zero)));
            return builder;
        }
    }
}