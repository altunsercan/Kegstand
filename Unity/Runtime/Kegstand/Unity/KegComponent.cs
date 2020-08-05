using System.Collections.Generic;
using UnityEngine;

namespace Kegstand.Unity
{
    public class KegComponent : MonoBehaviour, Keg, IWrapperComponent<Keg>
    {
        private Keg wrappedKeg;
        
        [SerializeField] public string Id;

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
        public float MaxAmount => wrappedKeg.MaxAmount;
        public float MinAmount => wrappedKeg.MinAmount;
        public float Amount => wrappedKeg.Amount;
        public float AggregateFlow => wrappedKeg.AggregateFlow;
        public IReadOnlyList<Tap> TapList => wrappedKeg.TapList;
        public void Increment(float delta) => wrappedKeg.Increment(delta);
        public void Decrement(float decrement) => wrappedKeg.Decrement(decrement);
        public int AppendCurrentEvents(List<TimedEvent> list) => wrappedKeg.AppendCurrentEvents(list);
        #endregion Wrapper Implementation
        

    }
}