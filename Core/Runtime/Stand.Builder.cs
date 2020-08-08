using System.Collections.Generic;

namespace Kegstand
{
    public partial class StandBase
    {
        public class Builder 
        {
            private List<KegEntry> kegEntries = new List<KegEntry>();
            private List<TapEntry> tapEntries = new List<TapEntry>();
            public Builder() { }

            public Builder AddKeg(object uniqueId, Keg keg)
            {
                kegEntries.Add(new KegEntry(uniqueId, keg));
                return this;
            }
            
            public Builder AddTap(object uniqueId, Tap tap)
            {
                tapEntries.Add(new TapEntry(uniqueId, tap));
                return this;
            }
            
            public Stand Build()
            {
                return new StandBase(kegEntries, tapEntries);
            }
        }
    }
}