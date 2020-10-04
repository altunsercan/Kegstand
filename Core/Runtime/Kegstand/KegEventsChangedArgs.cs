using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine.Assertions;

namespace Kegstand
{
    public class KegEventsChangedArgs
    {
        [NotNull] public readonly Keg Keg;
        [NotNull] public readonly IReadOnlyList<TimedEvent> Changes;

        public KegEventsChangedArgs(Keg keg, IReadOnlyList<TimedEvent> changes)
        {
            Assert.IsNotNull(keg);
            Assert.IsNotNull(changes);
            
            Keg = keg;
            Changes = changes;
        }
    }

    public class KegFillChangedArgs
    {
        [NotNull] public readonly Keg Keg;
        [NotNull] public readonly float FillAmount;
        
        public KegFillChangedArgs(Keg keg, float fillAmount)
        {
            Keg = keg;
            FillAmount = fillAmount;
        }
        
    }
}