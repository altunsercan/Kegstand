using System.Collections.Generic;

namespace Kegstand
{
    public partial class StandBase
    {
        public class Builder 
        {
            private List<KegEntry> kegEntries = new List<KegEntry>();
            public Builder() { }

            public Builder AddKeg(object uniqueId, Keg keg)
            {
                kegEntries.Add(new KegEntry(uniqueId, keg));
                return this;
            }

            public Stand Build()
            {
                return new StandBase(kegEntries);
            }
        }
    }
}