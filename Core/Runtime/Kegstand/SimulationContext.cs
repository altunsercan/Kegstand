using System;

namespace Kegstand
{
    public class SimulationContext<TTimeValue>
        where TTimeValue : struct, IComparable<TTimeValue>
    {
        public readonly ValueRegistar<TTimeValue> Values;
        public TTimeValue ClockTime { get; private set; }

        public SimulationContext()
        {
            Values = new ValueRegistar<TTimeValue>(this);
        }
    }
}