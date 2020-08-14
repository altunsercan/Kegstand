using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Kegstand.Impl;
using UnityEngine;
using UnityEngine.Assertions;

namespace Kegstand.Unity
{
    public class StandComponent : MonoBehaviour, Stand, IWrapperComponent<Stand>, IStandDefinitionProvider
    {
        public delegate void InitializedHandler(StandComponent stand);
        
        [NotNull]
        private readonly List<Component> nonAllocComponentList = new List<Component>();
        
        [SerializeField] public bool AutoAddSiblingComponents;

        private Stand wrappedStand;
        public event InitializedHandler Initialized;
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
                    (component)=>{
                        Assert.IsNotNull(component);
                        return new KegEntry(component.Id, component);
                    }
                    );
                ProcessSiblingComponent<TapComponent, TapEntry>(definition, definition.Taps, 
                    component =>
                    {
                        Assert.IsNotNull(component);
                        return new TapEntry(component.Id, component);
                    });
            }
            
            return definition;
        }
        
        private void ProcessSiblingComponent<T, U>(StandDefinition definition, [NotNull]List<U> listToAppend, [NotNull]Func<T, U> processAction) where T:Component
        {
            GetComponents(typeof(T), nonAllocComponentList);
            foreach (Component component in nonAllocComponentList)
            {
                if (!(component is T siblingComponent)) continue;
                U entry = processAction(siblingComponent);
                listToAppend.Add(entry);
            }
            nonAllocComponentList.Clear();
        }


        #region Wrapper Implementation

        public event KegEventsChangedDelegate EventsChanged
        {
            add
            {
                Assert.IsNotNull(wrappedStand);
                wrappedStand.EventsChanged += value;
            }
            remove
            {
                Assert.IsNotNull(wrappedStand);
                wrappedStand.EventsChanged -= value;
            }
        }

        public IReadOnlyList<KegEntry> Kegs
        {
            get
            {
                Assert.IsNotNull(wrappedStand);
                return wrappedStand.Kegs;
            }
        }

        public IReadOnlyList<TapEntry> Taps
        {
            get
            {
                Assert.IsNotNull(wrappedStand);
                return wrappedStand.Taps;
            }
        }

        public Keg GetKeg(object id)
        {
            Assert.IsNotNull(wrappedStand);
            return wrappedStand.GetKeg(id);
        }

        public Tap GetTap(object id)
        {
            Assert.IsNotNull(wrappedStand);
            return wrappedStand.GetTap(id);
        }

        #endregion Wrapper Implementation
    }
}