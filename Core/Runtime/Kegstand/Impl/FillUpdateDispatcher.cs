using System.Collections.Generic;
using JetBrains.Annotations;

namespace Kegstand.Impl
{
    internal class FillUpdateDispatcher
    {
        [NotNull] private readonly HashSet<Keg> trackedKegs = new HashSet<Keg>();
        
        public event KegFillChangedDelegate FillChanged;

        public void Track(Keg keg)
        {
            trackedKegs.Add(keg);
        }
        
        public virtual void DispatchUpdate(IAmountVisitor visitor)
        {
            foreach (Keg keg in trackedKegs)
            {
                UpdateTracked(keg, visitor);
            }
        }

        private void UpdateTracked(Keg keg, IAmountVisitor visitor)
        {
            float amount = keg.Amount(visitor);
            
            KegFillChangedArgs args = new KegFillChangedArgs(keg, amount);
            FillChanged?.Invoke(args);
        }

        public void Untrack(Keg keg)
        {
            trackedKegs.Remove(keg);
        }
    }
}