namespace CizaAudioModule
{
    public struct Pair<TValue>
    {
        public readonly string Key;

        public readonly TValue Value;

        public Pair(string key, TValue value)
        {
            Key = key;
            Value = value;
        }
    }
}