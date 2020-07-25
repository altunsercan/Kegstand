using System.Collections.Generic;

namespace Kegstand
{
    public partial class Stand
    {
        public class Builder
        {
            private List<KegEntry> kegEntries = new List<KegEntry>();
            public Builder() { }

            public Builder AddKeg(object uniqueId, Keg keg)
            {
                kegEntries.Add(new Stand.KegEntry(uniqueId, keg));
                return this;
            }

            public Stand Build()
            {
                return new Stand(kegEntries);
            }
        }
    }
}