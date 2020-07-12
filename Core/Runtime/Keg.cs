namespace Kegstand
{
    public interface Keg
    {
        float MaxAmount { get; }
        float MinAmount { get; }
        float Amount { get; }
        void Increment(float delta);
        void Decrement(float decrement);
    }
}
