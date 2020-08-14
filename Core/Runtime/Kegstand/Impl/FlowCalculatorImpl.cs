using System.Collections.Generic;
using UnityEngine.Assertions;

namespace Kegstand.Impl
{
    public class FlowCalculatorImpl : FlowCalculator
    {
        public float CalculateAggregateFlow(Keg keg)
        {
            Assert.IsNotNull(keg);
            
            IReadOnlyList<Tap> tapList = keg.TapList;
            
            float delta = 0;
            for (var index = 0; index < tapList.Count; index++)
            {
                Tap tap = tapList[index];
                if(tap == null) { continue; }
                delta += tap.FlowAmount;
            }

            return delta;
        }
    }
}