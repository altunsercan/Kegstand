namespace Kegstand
{
    public interface Tap
    {
        float FlowAmount { get; }

        void SetFlow(float f);
    }
}