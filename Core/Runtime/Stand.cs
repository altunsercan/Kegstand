using System.Collections.Generic;

namespace Kegstand
{
    public class Stand
    {
        private readonly Dictionary<object, Keg> kegs = new Dictionary<object, Keg>();
        
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
}