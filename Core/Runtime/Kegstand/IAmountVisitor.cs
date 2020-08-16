using Kegstand.Impl;

namespace Kegstand
{
    public interface IAmountVisitor
    {
        float CalculateCurrentAmount(float recordedAmount, float currentFlow, Timestamp recordedTimestamp);
    }
}