using System.Collections.Generic;
using Kegstand.Impl;
using UnityEngine;

namespace Kegstand.Unity
{
    public class KegComponent : MonoBehaviour, Keg, IWrapperComponent<Keg>
    {
        private Keg wrappedKeg;
        
        [SerializeField] public string Id;
        [SerializeField] private float maxAmount = 100f;
        [SerializeField] private float minAmount = 0f;
        [SerializeField] private float amount = 0f;

        void IWrapperComponent<Keg>.SetWrappedObject(Keg wrappedObject)
        {
            wrappedKeg = wrappedObject;
        }

        #region Wrapper Implementation
        public event KegEventsChangedDelegate EventsChanged
        {
            add => wrappedKeg.EventsChanged += value;
            remove => wrappedKeg.EventsChanged -= value;
        }
        public float MaxAmount => wrappedKeg?.MaxAmount??maxAmount;
        public float MinAmount => wrappedKeg?.MinAmount??minAmount;
        public float Amount => wrappedKeg?.Amount??amount;
        public float AggregateFlow => wrappedKeg.AggregateFlow;
        public IReadOnlyList<Tap> TapList => wrappedKeg.TapList;
        public void Increment(float delta) => wrappedKeg.Increment(delta);
        public void Decrement(float decrement) => wrappedKeg.Decrement(decrement);
        public int AppendCurrentEvents(List<TimedEvent> list) => wrappedKeg.AppendCurrentEvents(list);

        public void AddTap(Tap tap) => wrappedKeg.AddTap(tap);

        #endregion Wrapper Implementation


    }
}