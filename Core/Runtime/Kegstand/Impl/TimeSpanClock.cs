using System;

namespace Kegstand.Impl
{
    public class TimeSpanClock : Clock<TimeSpan>
    {
        private TimeSpan currentTime;

        public TimeSpanClock()
        {
            currentTime = TimeSpan.Zero;
        }

        public TimeSpanClock(TimeSpan startTime)
        {
            currentTime = startTime;
        }

        public void Update(float deltaSeconds)
        {
            ref TimeSpan time = ref currentTime;

            time = time.Add(TimeSpan.FromSeconds(deltaSeconds));
        }

        public ref TimeSpan GetCurrentTimePassed()
        {
            return ref currentTime;
        }
    }
}