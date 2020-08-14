namespace Kegstand.Impl
{
    public class TapBase : Tap
    {
        public float FlowAmount { get; private set; }

        public TapBase(float startingFlowAmount = 0f)
        {
            FlowAmount = startingFlowAmount;
        }

        public void SetFlow(float amount)
        {
            FlowAmount = amount;
        }
    }
}