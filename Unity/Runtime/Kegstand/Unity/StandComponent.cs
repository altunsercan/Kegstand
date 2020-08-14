using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Kegstand.Impl;
using UnityEngine;

namespace Kegstand.Unity
{
    public class StandComponent : MonoBehaviour, Stand, IWrapperComponent<Stand>, IStandDefinitionProvider
    {
        public delegate void InitialalizedHandler(StandComponent stand);
        
        [NotNull]
        private readonly List<Component> nonAllocComponentList = new List<Component>();
        
        [SerializeField] public bool AutoAddSiblingComponents;

        private Stand wrappedStand;
        public event InitialalizedHandler Initialized;
        public bool IsInitialized { get; private set; }


        public void SetWrappedObject(Stand wrappedObject)
        {
            wrappedStand = wrappedObject;

            IsInitialized = true;
            Initialized?.Invoke(this);
        }

        StandDefinition IStandDefinitionProvider.GetStandDefinition()
        {
            var definition = new StandDefinition();
            if (AutoAddSiblingComponents)
            {
                ProcessSiblingComponent<KegComponent, KegEntry>(definition, definition.Kegs,
                    component => new KegEntry(component.Id, component));
                ProcessSiblingComponent<TapComponent, TapEntry>(definition, definition.Taps, 
                    component => new TapEntry(component.Id, component));
            }
            
            return definition;
        }
        
        private void ProcessSiblingComponent<T, U>(StandDefinition definition, List<U> listToAppend, Func<T, U> processAction) where T:Component
        {
            GetComponents(typeof(T), nonAllocComponentList);
            foreach (Component component in nonAllocComponentList)
            {
                if (!(component is T siblingComponent)) continue;
                var entry = processAction(siblingComponent);
                listToAppend.Add(entry);
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