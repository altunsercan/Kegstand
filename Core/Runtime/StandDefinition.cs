using System.Collections.Generic;

namespace Kegstand
{
    public class StandDefinition
    {
        public readonly List<KegEntry> Kegs = new List<KegEntry>();
        public readonly List<TapEntry> Taps = new List<TapEntry>();
    }
}