namespace Kegstand
{
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