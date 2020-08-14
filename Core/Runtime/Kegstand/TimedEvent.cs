using System.Diagnostics.CodeAnalysis;

namespace Kegstand
{
    
    [ExcludeFromCodeCoverage] // Pure data class with no logic
    public class TimedEvent
    {
        public Keg Index { get; }
        public float Time { get; }
        public KegEvent Type { get; }

        public TimedEvent(Keg index, float time, KegEvent type)
        {
            Index = index;
            Time = time;
            Type = type;
        }
        
    }
}