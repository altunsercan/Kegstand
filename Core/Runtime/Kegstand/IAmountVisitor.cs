using Kegstand.Impl;

namespace Kegstand
{
    public interface IAmountVisitor
    {
        void SetCurrentTimestamp(Timestamp current);
        float CalculateCurrentAmount(float recordedAmount, float currentFlow, Timestamp recordedTimestamp);
    }
}