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

            public Builder CopyDefinition(StandDefinition definition)
            {
                foreach (KegEntry kegEntry in definition.Kegs)
                    AddKeg(kegEntry.Key, kegEntry.Keg);
            
                foreach (TapEntry tapEntry in definition.Taps)
                    AddTap(tapEntry.Key, tapEntry.Tap);

                return this;
            }

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