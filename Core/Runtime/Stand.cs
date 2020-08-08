using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine.Assertions;

namespace Kegstand
{
    public interface Stand
    {
        event KegEventsChangedDelegate EventsChanged;
        IReadOnlyList<KegEntry> Kegs { get; }
        IReadOnlyList<TapEntry> Taps { get; }
        void RegisterKegEntries(List<KegEntry> kegEntries);
        void RegisterTapEntries(List<TapEntry> tapEntries);
        void AddKeg(KegEntry kegEntry);
        Keg GetKeg(object uniqueObj);
        Tap GetTap(object uniqueObj);
    }

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
        
        public StandBase(List<KegEntry> kegEntries, List<TapEntry> tapEntries)
        {
            Assert.IsNotNull(kegEntries);
            Assert.IsNotNull(tapEntries);
            
            RegisterKegEntries(kegEntries);
            readOnlyKegEntries = kegEntries.AsReadOnly();
            
            RegisterTapEntries(tapEntries);
            readOnlyTapEntries = tapEntries.AsReadOnly();
        }

        public void RegisterKegEntries(List<KegEntry> kegEntries)
        {
            foreach (KegEntry entry in kegEntries)
                AddKeg(entry);
        }

        public void RegisterTapEntries(List<TapEntry> tapEntries)
        {
            foreach (TapEntry tapEntry in tapEntries)
                AddTap(tapEntry);
        }

        public void AddKeg(KegEntry kegEntry)
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

    public readonly struct KegEntry
    {
        public readonly object Key;
        public readonly Keg Keg;

        public KegEntry(object key, Keg keg)
        {
            Key = key;
            Keg = keg;
        }
    }
    
    public readonly struct TapEntry
    {
        public readonly object Key;
        public readonly Tap Tap;

        public TapEntry(object key, Tap tap)
        {
            Key = key;
            Tap = tap;
        }
    }
}