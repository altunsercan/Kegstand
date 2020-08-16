using System;
using JetBrains.Annotations;
using UnityEngine.Assertions;

namespace Kegstand.Impl
{
    public abstract class AmountVisitor<TTimeValue>: IAmountVisitor
        where TTimeValue : IComparable<TTimeValue>
    {
        [NotNull]
        private Timestamp<TTimeValue> timestamp;

        public AmountVisitor(Timestamp<TTimeValue> timestamp)
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

            if (!(recordedTimestamp is Timestamp<TTimeValue> typedTimeStamp))
            {
                throw new ArgumentException($"Invalid typed {nameof(Timestamp)} expected {nameof(TTimeValue)}", nameof(recordedTimestamp));
            }
            
            TTimeValue currentTime = typedTimeStamp.Time;
            TTimeValue anchorTime = timestamp.Time;
            
            var deltaTime = CalculateDeltaSeconds( ref currentTime, ref anchorTime);
            return recordedAmount + currentFlow * deltaTime;
        }

        protected abstract float CalculateDeltaSeconds(ref TTimeValue timeToCheck, ref TTimeValue anchorTime);
    }

    public class TimeSpanAmountVisitor : AmountVisitor<TimeSpan>
    {
        public TimeSpanAmountVisitor(Timestamp<TimeSpan> timestamp) : base(timestamp)
        {
        }

        protected override float CalculateDeltaSeconds(ref TimeSpan timeToCheck, ref TimeSpan anchorTime)
        {
            return (float)(timeToCheck.TotalSeconds - anchorTime.TotalSeconds);
        }
    }
        
}