using System.Collections.Generic;
using Kegstand.Impl;
using UnityEngine;
using UnityEngine.Assertions;

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
            add
            {
                Assert.IsNotNull(wrappedKeg);
                wrappedKeg.EventsChanged += value;
            }
            remove
            {
                Assert.IsNotNull(wrappedKeg);
                wrappedKeg.EventsChanged -= value;
            }
        }

        public float MaxAmount => wrappedKeg?.MaxAmount??maxAmount; // Pass through serialized value pre initialization
        public float MinAmount => wrappedKeg?.MinAmount??minAmount; // Pass through serialized value pre initialization
        public float Amount => wrappedKeg?.Amount??amount;  // Pass through serialized value pre initialization
        public float AggregateFlow
        {
            get
            {
                Assert.IsNotNull(wrappedKeg);
                return wrappedKeg.AggregateFlow;
            }
        }

        public IReadOnlyList<Tap> TapList
        {
            get
            {
                Assert.IsNotNull(wrappedKeg);
                return wrappedKeg.TapList;
            }
        }

        public void Increment(float delta)
        {
            Assert.IsNotNull(wrappedKeg);
            wrappedKeg.Increment(delta);
        }

        public void Decrement(float decrement)
        {
            Assert.IsNotNull(wrappedKeg);
            wrappedKeg.Decrement(decrement);
        }

        public int AppendCurrentEvents(TimedEventQueue queue)
        {
            Assert.IsNotNull(wrappedKeg);
            return wrappedKeg.AppendCurrentEvents(queue);
        }

        public void AddTap(Tap tap)
        {
            Assert.IsNotNull(wrappedKeg);
            wrappedKeg.AddTap(tap);
        }

        #endregion Wrapper Implementation


    }
}