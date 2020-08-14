namespace Kegstand
{
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
}