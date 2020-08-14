using System.Collections.Generic;

namespace Kegstand
{
    public class KegEventsChangedArgs
    {
        public Keg Keg { get; set; }
        public IReadOnlyList<TimedEvent> Changes { get; set; }
    }
}