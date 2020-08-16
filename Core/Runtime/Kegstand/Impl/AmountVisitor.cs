using System;
using JetBrains.Annotations;
using UnityEngine.Assertions;

namespace Kegstand.Impl
{
    public class AmountVisitor: IAmountVisitor
    {
        [NotNull]
        private Timestamp<TimeSpan> timestamp;

        public AmountVisitor(Timestamp<TimeSpan> timestamp)
        {
            Assert.IsNotNull(timestamp);
            
            this.timestamp = timestamp;
        }

        public float CalculateCurrentAmount(float recordedAmount, float currentFlow, Timestamp recordedTimestamp)
        {
            Assert.IsNotNull(recordedTimestamp);
            
            if (Math.Abs(currentFlow) < float.Epsilon)
            {
                return recordedAmount;
            }

            if (!(recordedTimestamp is Timestamp<TimeSpan> typedTimeStamp))
            {
                throw new ArgumentException($"Invalid typed {nameof(Timestamp)} used as {nameof(recordedTimestamp)}", nameof(recordedTimestamp));
            }

            TimeSpan deltaTime = typedTimeStamp.Time - timestamp.Time;
            return recordedAmount + currentFlow * deltaTime.Seconds;
        }
    }
}