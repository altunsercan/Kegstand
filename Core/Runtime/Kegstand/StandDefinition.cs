using System.Collections.Generic;
using JetBrains.Annotations;

namespace Kegstand
{
    public class StandDefinition
    {
        [NotNull] public readonly List<KegEntry> Kegs = new List<KegEntry>();
        [NotNull] public readonly List<TapEntry> Taps = new List<TapEntry>();
    }
}