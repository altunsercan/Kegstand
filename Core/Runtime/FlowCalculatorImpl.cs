namespace Kegstand
{
    public class FlowCalculatorImpl : FlowCalculator
    {
        public float CalculateAggregateFlow(Keg keg)
        {
            var tapList = keg.TapList;
            
            float delta = 0;
            foreach (Tap tap in tapList)
            {
                delta += tap.FlowAmount;
            }

            return delta;
        }
    }
}