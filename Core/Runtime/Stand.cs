using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine.Assertions;

namespace Kegstand
{
    public class Stand
    {
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

        
        private readonly Dictionary<object, Keg> kegs = new Dictionary<object, Keg>();

        public Stand(List<KegEntry> kegEntries)
        {
            Assert.IsNotNull(kegEntries);
            RegisterKegEntries(kegEntries);
        }

        private void RegisterKegEntries(List<KegEntry> kegEntries)
        {
            foreach (KegEntry entry in kegEntries)
            {
                AddKeg(entry.Key, entry.Keg);
            }
        }

        private void AddKeg(object uniqueObj, Keg keg)
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
}