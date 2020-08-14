namespace Kegstand
{
    public class TimedEvent
    {
        public Keg Index { get; set; }
        public float Time { get; set; }
        public KegEvent Type { get; set; }
        
        public TimedEvent(){}

        public TimedEvent(Keg index, float time, KegEvent type)
        {
            Index = index;
            Time = time;
            Type = Type;
        }
        
    }
}