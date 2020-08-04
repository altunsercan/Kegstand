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
        void RegisterKegEntries(List<KegEntry> kegEntries);
        void AddKeg(KegEntry kegEntry);
        Keg GetKeg(object uniqueObj);
    }

    public partial class StandBase : Stand
    {
        private readonly Dictionary<object, Keg> kegs = new Dictionary<object, Keg>();
        private readonly List<KegEntry> kegEntries = new List<KegEntry>();
        private readonly IReadOnlyList<KegEntry> readOnlyKegEntries;
     
        public IReadOnlyList<KegEntry> Kegs => readOnlyKegEntries;
        
        public event KegEventsChangedDelegate EventsChanged;
        
        public StandBase(List<KegEntry> kegEntries)
        {
            Assert.IsNotNull(kegEntries);
            RegisterKegEntries(kegEntries);
            readOnlyKegEntries = kegEntries.AsReadOnly();
        }

        public void RegisterKegEntries(List<KegEntry> kegEntries)
        {
            foreach (KegEntry entry in kegEntries)
            {
                AddKeg(entry);
            }
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
    }

    public struct KegEntry
    {
        public readonly object Key;
        public readonly Keg Keg;

        public KegEntry(object key, Keg keg)
        {
            Key = key;
            Keg = keg;
        }
    }
}