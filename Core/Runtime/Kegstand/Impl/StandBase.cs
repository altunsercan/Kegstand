using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine.Assertions;

namespace Kegstand.Impl
{
    public partial class StandBase : Stand
    {
        [NotNull] private readonly Dictionary<object, Keg> kegs = new Dictionary<object, Keg>();
        public IReadOnlyList<KegEntry> Kegs { get; }
        
        
        [NotNull] private readonly Dictionary<object, Tap> taps = new Dictionary<object, Tap>();
        private readonly IReadOnlyList<TapEntry> readOnlyTapEntries;
        public IReadOnlyList<TapEntry> Taps { get; }

        public event KegEventsChangedDelegate EventsChanged;
        
        [UsedImplicitly]
        public StandBase(List<KegEntry> kegEntries, List<TapEntry> tapEntries)
        {
            Assert.IsNotNull(kegEntries);
            Assert.IsNotNull(tapEntries);

            RegisterKegEntries(kegEntries);
            Kegs = kegEntries.AsReadOnly();
            
            RegisterTapEntries(tapEntries);
            Taps = tapEntries.AsReadOnly();
        }

        private void RegisterKegEntries([NotNull] List<KegEntry> kegEntriesToRegister)
        {
            foreach (KegEntry entry in kegEntriesToRegister)
                AddKeg(entry);
        }

        private void RegisterTapEntries([NotNull] List<TapEntry> tapEntriesToRegister)
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

        public Keg GetKeg(object id)
        {
            Assert.IsNotNull(id);

            kegs.TryGetValue(id, out Keg keg);
            return keg;
        }

        public Tap GetTap(object id)
        {
            Assert.IsNotNull(id);
            
            taps.TryGetValue(id, out Tap tap);
            return tap;
        }
    }
}