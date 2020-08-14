using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Kegstand.Unity
{
    public class TapComponent : MonoBehaviour, Tap, IWrapperComponent<Tap>
    {
        private Tap wrappedTap;
        
        [SerializeField] public string Id;
        [SerializeField] public List<KegComponent> ConnectedKegs;
        [SerializeField] private float flowAmount; // Only initial amount
        
        public void SetWrappedObject(Tap wrappedObject) => wrappedTap = wrappedObject;

        #region Wrapper Implementation
        public float FlowAmount => wrappedTap?.FlowAmount??flowAmount;
        public void SetFlow(float f)
        {
            Assert.IsNotNull(wrappedTap);
            wrappedTap.SetFlow(f);
        }

        #endregion Wrapper Implementation

    }
}