using System;

namespace Kegstand.Impl
{
    public class TimeSpanClock : Clock
    {
        private TimeSpan currentTime;

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