using UnityEngine;

namespace Kegstand.Unity
{
    public class TapComponent : MonoBehaviour, Tap, IWrapperComponent<Tap>
    {
        private Tap wrappedTap;
        
        [SerializeField] public string Id;
        public void SetWrappedObject(Tap wrappedObject) => wrappedTap = wrappedObject;

        #region Wrapper Implementation
        public float FlowAmount => wrappedTap.FlowAmount;
        public void SetFlow(float f) => wrappedTap.SetFlow(f);
        #endregion Wrapper Implementation

    }
}