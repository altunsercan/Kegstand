using System.Collections.Generic;

namespace Kegstand
{
    public interface Keg
    {
        float MaxAmount { get; }
        float MinAmount { get; }
        float Amount { get; }
        float AggregateFlow { get; }
        IReadOnlyList<Tap> TapList { get; }
        void Increment(float delta);
        void Decrement(float decrement);
    }
}
