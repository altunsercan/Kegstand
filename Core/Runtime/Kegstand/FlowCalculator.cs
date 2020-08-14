using JetBrains.Annotations;

namespace Kegstand
{
    public interface FlowCalculator
    {
        [Pure]
        float CalculateAggregateFlow(Keg keg);
    }
}