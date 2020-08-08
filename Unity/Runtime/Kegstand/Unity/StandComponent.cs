using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Kegstand.Unity
{
    public class StandComponent : MonoBehaviour, Stand, IWrapperComponent<Stand>
    {
        [NotNull]
        private readonly List<Component> nonAllocComponentList = new List<Component>();
        
        [SerializeField] public bool AutoAddSiblingComponents;
        
        private Stand wrappedStand;
        public void SetWrappedObject(Stand wrappedObject)
        {
            wrappedStand = wrappedObject;

            Initialize();
        }

        private void Initialize()
        {
            if (AutoAddSiblingComponents)
            {
                DiscoverAndAddSiblingKegComponents();
            }
        }

        private void DiscoverAndAddSiblingKegComponents()
        {
            GetComponents(typeof(KegComponent), nonAllocComponentList);
            foreach (Component component in nonAllocComponentList)
            {
                if (!(component is KegComponent kegComponent)) continue;
                
                var entry = new KegEntry(kegComponent.Id, kegComponent);
                AddKeg(entry);
            }
            nonAllocComponentList.Clear();
        }


        #region Wrapper Implementation
        public event KegEventsChangedDelegate EventsChanged
        {
            add => wrappedStand.EventsChanged += value;
            remove => wrappedStand.EventsChanged -= value;
        }
        public IReadOnlyList<KegEntry> Kegs => wrappedStand.Kegs;
        public void RegisterKegEntries(List<KegEntry> kegEntries) => wrappedStand.RegisterKegEntries(kegEntries);
        public void AddKeg(KegEntry kegEntry) => wrappedStand.AddKeg(kegEntry);
        public Keg GetKeg(object uniqueObj) => wrappedStand.GetKeg(uniqueObj);
        #endregion Wrapper Implementation
    }
}