namespace Kegstand.Impl
{
    public interface IAmountVisitor
    {
        float Visit(float amount, Timestamp timestamp);
    }
}