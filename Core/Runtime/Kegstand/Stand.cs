using System.Collections.Generic;
using Kegstand.Impl;

namespace Kegstand
{
    public interface Stand
    {
        event KegEventsChangedDelegate EventsChanged;
        IReadOnlyList<KegEntry> Kegs { get; }
        IReadOnlyList<TapEntry> Taps { get; }
        Keg GetKeg(object uniqueObj);
        Tap GetTap(object uniqueObj);
    }
}