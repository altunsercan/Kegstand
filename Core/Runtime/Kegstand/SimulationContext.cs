using System;

namespace Kegstand
{
    public class SimulationContext<TTimeValue>
        where TTimeValue : struct, IComparable<TTimeValue>
    {
        private readonly Clock<TTimeValue> clock;
        public readonly ValueRegistar<TTimeValue> Values;
        public TTimeValue ClockTime => clock.GetCurrentTimePassed(); 

        public SimulationContext(Clock<TTimeValue> clock)
        {
            this.clock = clock;
            Values = new ValueRegistar<TTimeValue>(this);
        }
    }
}