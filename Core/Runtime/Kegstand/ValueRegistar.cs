using System;
using System.Collections.Generic;

namespace Kegstand
{
    public class ValueRegistar<TTimeValue>
        where TTimeValue : struct, IComparable<TTimeValue>
    {
        private readonly SimulationContext<TTimeValue> context;

        private int lastValueIndex = -1;
        private ValueRegistry<TTimeValue>[] values = new ValueRegistry<TTimeValue>[16];
        private readonly Dictionary<Keg, int> kegToValueIndex = new Dictionary<Keg, int>();
        
        public readonly RefAccessor AsRef;
        
        public ValueRegistar(SimulationContext<TTimeValue> context)
        {
            this.context = context;
            AsRef = new RefAccessor(this);
        }

        public ValueRegistry<TTimeValue> this[Keg keg]
        {
            get => kegToValueIndex.TryGetValue(keg, out int index) ? values[index] : ValueRegistry<TTimeValue>.Invalid;
            set
            {
                value.Timestamp = value.Timestamp??context.ClockTime;

                int index = ExpandOrGetValueSetIndex(keg);
                AssignIndex(index, keg, value);
            }
        }

        private void AssignIndex( int index, Keg keg, ValueRegistry<TTimeValue> value)
        {
            values[index] = value;
            kegToValueIndex[keg] = index;
        }

        private int ExpandOrGetValueSetIndex(Keg keg)
        {
            int index;
            if (kegToValueIndex.TryGetValue(keg, out int indexFound))
            {
                index = indexFound;
            }
            else
            {
                if (lastValueIndex + 1 == values.Length)
                {
                    var temp = new ValueRegistry<TTimeValue>[values.Length * 2];
                    Array.Copy(values, temp, values.Length);
                    values = temp;
                }

                lastValueIndex++;
                index = lastValueIndex;
            }

            return index;
        }

        public class RefAccessor
        {
            private readonly ValueRegistar<TTimeValue> registar;

            internal RefAccessor(ValueRegistar<TTimeValue> registar)
            {
                this.registar = registar;
            }
            
            public ref ValueRegistry<TTimeValue> this[Keg keg]
            {
                get
                {
                    int index;
                    if (registar.kegToValueIndex.TryGetValue(keg, out index))
                    {
                        return ref registar.values[index];
                    }
                    
                    index = registar.ExpandOrGetValueSetIndex(keg);
                    registar.AssignIndex(index, keg, ValueRegistry<TTimeValue>.Invalid);
                    return ref registar.values[index];
                }
            }
        }
    }
}