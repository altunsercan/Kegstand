using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Kegstand.Impl
{
    internal class FillUpdateDispatcher
    {
        [NotNull] private readonly HashSet<Keg> trackedKegs = new HashSet<Keg>();
        [NotNull] private readonly Dictionary<Keg, FillObservable> fillObservables = new Dictionary<Keg, FillObservable>();

        public event KegFillChangedDelegate KegFillChanged;

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
            DispatchObservable(keg, amount);
            DispatchAll(keg, amount);
        }
        
        private void DispatchObservable(Keg keg, float amount)
        {
            if (fillObservables.TryGetValue(keg, out FillObservable observable))
            {
                observable.Dispatch(amount);
            }
        }
        
        private void DispatchAll(Keg keg, float amount)
        {
            KegFillChangedArgs args = new KegFillChangedArgs(keg, amount);
            KegFillChanged?.Invoke(args);
        }

        public void Untrack(Keg keg)
        {
            trackedKegs.Remove(keg);
            fillObservables.Remove(keg);
        }
        
        public IObservable<float> GetFillObservable([NotNull] Keg keg)
        {
            Track(keg);
            if (fillObservables.TryGetValue(keg, out var observable))
            {
                return observable;
            }

            return MakeFillObservable(keg);
        }

        private FillObservable MakeFillObservable(Keg keg)
        {
            var observable = new FillObservable();
            fillObservables.Add(keg, observable);
            return observable;
        }

        private class FillObservable : IObservable<float>
        {
            [NotNull, ItemNotNull] private readonly List<IObserver<float>> observers = new List<IObserver<float>>();
            public IDisposable Subscribe(IObserver<float> observer)
            {
                observers.Add(observer);
                return new SubscriptionDisposer(this, observer);
            }

            public void Dispatch(float fillAmount)
            {
                foreach (IObserver<float> observer in observers)
                {
                    observer.OnNext(fillAmount);
                }
            }

            private class SubscriptionDisposer : IDisposable
            {
                [NotNull] private readonly FillObservable fillObservable;
                [NotNull] private readonly IObserver<float> observer;

                public SubscriptionDisposer([NotNull] FillObservable fillObservable, [NotNull] IObserver<float> observer)
                {
                    this.fillObservable = fillObservable;
                    this.observer = observer;
                }
                public void Dispose()
                {
                    fillObservable.observers.Remove(observer);
                }
            }
        }
    }
}