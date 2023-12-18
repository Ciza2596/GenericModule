namespace CizaTextModule
{
    public interface ITextMap
    {
        string Name { get; }

        bool IsEnable { get; }

        string Key { get; }

        void SetText(string text);

        void SetKey(string key);
    }
}