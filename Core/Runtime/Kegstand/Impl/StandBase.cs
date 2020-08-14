using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine.Assertions;

namespace Kegstand.Impl
{
    public partial class StandBase : Stand
    {
        private readonly Dictionary<object, Keg> kegs = new Dictionary<object, Keg>();
        private readonly List<KegEntry> kegEntries = new List<KegEntry>();
        private readonly IReadOnlyList<KegEntry> readOnlyKegEntries;
        public IReadOnlyList<KegEntry> Kegs => readOnlyKegEntries;
        
        
        private readonly Dictionary<object, Tap> taps = new Dictionary<object, Tap>();
        private readonly List<TapEntry> tapEntries = new List<TapEntry>();
        private readonly IReadOnlyList<TapEntry> readOnlyTapEntries;
        public IReadOnlyList<TapEntry> Taps => readOnlyTapEntries;

        public event KegEventsChangedDelegate EventsChanged;
        
        [UsedImplicitly]
        public StandBase(List<KegEntry> kegEntries, List<TapEntry> tapEntries)
        {
            Assert.IsNotNull(kegEntries);
            Assert.IsNotNull(tapEntries);
            
            RegisterKegEntries(kegEntries);
            readOnlyKegEntries = kegEntries.AsReadOnly();
            
            RegisterTapEntries(tapEntries);
            readOnlyTapEntries = tapEntries.AsReadOnly();
        }

        private void RegisterKegEntries(List<KegEntry> kegEntriesToRegister)
        {
            foreach (KegEntry entry in kegEntriesToRegister)
                AddKeg(entry);
        }

        private void RegisterTapEntries(List<TapEntry> tapEntriesToRegister)
        {
            foreach (TapEntry tapEntry in tapEntriesToRegister)
                AddTap(tapEntry);
        }

        private void AddKeg(KegEntry kegEntry)
        {
            object key = kegEntry.Key;
            if (kegs.ContainsKey(key))
            {
                return;
            }

            Keg keg = kegEntry.Keg;
            kegs.Add(key, keg);
            keg.EventsChanged += DispatchKegEventChanged;
        }

        private void AddTap(in TapEntry tapEntry)
        {
            object key = tapEntry.Key;
            if (taps.ContainsKey(key))
            {
                return;
            }

            Tap tap = tapEntry.Tap;
            taps.Add(key, tap);
        }

        private void DispatchKegEventChanged(KegEventsChangedArgs changesArgs)
        {
            Assert.IsNotNull(changesArgs);
            EventsChanged?.Invoke(changesArgs);
        }

        public Keg GetKeg(object uniqueObj)
        {
            Keg keg = null;
            kegs.TryGetValue(uniqueObj, out keg);
            return keg;
        }

        public Tap GetTap(object uniqueObj)
        {
            Tap tap = null;
            taps.TryGetValue(uniqueObj, out tap);
            return tap;
        }
    }
}