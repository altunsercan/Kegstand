using System.Collections.Generic;
using UnityEngine;

namespace Kegstand.Unity
{
    public class StandComponent : MonoBehaviour, Stand, IWrapperComponent<Stand>
    {
        private Stand wrappedStand;
        public event KegEventsChangedDelegate EventsChanged
        {
            add => wrappedStand.EventsChanged += value;
            remove => wrappedStand.EventsChanged -= value;
        }

        public IReadOnlyList<KegEntry> Kegs => wrappedStand.Kegs;

        public void RegisterKegEntries(List<KegEntry> kegEntries) => wrappedStand.RegisterKegEntries(kegEntries);

        public void AddKeg(KegEntry kegEntry) => wrappedStand.AddKeg(kegEntry);

        public Keg GetKeg(object uniqueObj) => wrappedStand.GetKeg(uniqueObj);

        public void SetWrappedObject(Stand wrappedObject) => wrappedStand = wrappedObject;
    }
}