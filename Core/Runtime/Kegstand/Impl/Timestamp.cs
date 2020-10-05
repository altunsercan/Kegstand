using System;

namespace Kegstand.Impl
{
    public interface Timestamp
    {
        
    }

    public class Timestamp<TTimeValue> : Timestamp
    {
        private readonly TTimeValue timeValue;
        public ref readonly TTimeValue Time => ref timeValue;

        public Timestamp(TTimeValue time)
        {
            timeValue = time;
        }
    }
}