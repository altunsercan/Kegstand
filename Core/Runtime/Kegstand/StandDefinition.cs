using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Kegstand
{
    [ExcludeFromCodeCoverage] // Pure data class with no logic
    public class StandDefinition
    {
        [NotNull] public readonly List<KegEntry> Kegs = new List<KegEntry>();
        [NotNull] public readonly List<TapEntry> Taps = new List<TapEntry>();
    }
}