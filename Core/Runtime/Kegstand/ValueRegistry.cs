using System;

namespace Kegstand
{
    public struct ValueRegistry<TTimeValue>
        where TTimeValue : struct, IComparable<TTimeValue>
    {
        public static readonly ValueRegistry<TTimeValue> Invalid = default;

        public TTimeValue? Timestamp;
        public float Value;

        public bool IsInvalid => !Timestamp.HasValue && Math.Abs(Value - Invalid.Value) < float.Epsilon;
    }
}