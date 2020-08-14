using System.Collections.Generic;
using JetBrains.Annotations;
using Kegstand.Impl;

namespace Kegstand
{
    public interface Stand
    {
        event KegEventsChangedDelegate EventsChanged;
        [NotNull] IReadOnlyList<KegEntry> Kegs { get; }
        [NotNull] IReadOnlyList<TapEntry> Taps { get; }
        Keg GetKeg(object uniqueObj);
        Tap GetTap(object uniqueObj);
    }
}