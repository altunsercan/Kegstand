using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine.Assertions;

namespace Kegstand
{
    public interface Stand
    {
        void RegisterKegEntries(List<KegEntry> kegEntries);
        void AddKeg(object uniqueObj, Keg keg);
        object GetKeg(object uniqueObj);
    }

    public partial class StandBase : Stand
    {
        private readonly Dictionary<object, Keg> kegs = new Dictionary<object, Keg>();

        public StandBase(List<KegEntry> kegEntries)
        {
            Assert.IsNotNull(kegEntries);
            RegisterKegEntries(kegEntries);
        }

        public void RegisterKegEntries(List<KegEntry> kegEntries)
        {
            foreach (KegEntry entry in kegEntries)
            {
                AddKeg(entry.Key, entry.Keg);
            }
        }

        public void AddKeg(object uniqueObj, Keg keg)
        {
            if (kegs.ContainsKey(uniqueObj))
            {
                return;
            }
            kegs.Add(uniqueObj, keg);
        }

        public object GetKeg(object uniqueObj)
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