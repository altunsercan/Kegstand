using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Kegstand.Unity
{
    public class StandComponent : MonoBehaviour, Stand, IWrapperComponent<Stand>, IStandDefinitionProvider
    {
        [NotNull]
        private readonly List<Component> nonAllocComponentList = new List<Component>();
        
        [SerializeField] public bool AutoAddSiblingComponents;
        
        private Stand wrappedStand;
        
        public void SetWrappedObject(Stand wrappedObject)
        {
            wrappedStand = wrappedObject;
        }
        
        public StandDefinition GetStandDefinition()
        {
            var definition = new StandDefinition();
            if (AutoAddSiblingComponents)
            {
                DiscoverAndAddSiblingKegComponents(definition);
            }
            
            return definition;
        }

        private void DiscoverAndAddSiblingKegComponents(StandDefinition definition)
        {
            GetComponents(typeof(KegComponent), nonAllocComponentList);
            foreach (Component component in nonAllocComponentList)
            {
                if (!(component is KegComponent kegComponent)) continue;
                
                var entry = new KegEntry(kegComponent.Id, kegComponent);
                definition.Kegs.Add(entry);
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
        public IReadOnlyList<TapEntry> Taps => wrappedStand.Taps;

        public Keg GetKeg(object uniqueObj) => wrappedStand.GetKeg(uniqueObj);
        public Tap GetTap(object uniqueObj) => wrappedStand.GetTap(uniqueObj);

        #endregion Wrapper Implementation
    }
}