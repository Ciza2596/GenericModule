namespace CizaTextModule
{
    public interface ITextMap
    {
        string Name { get; }

        bool IsEnable { get; }

        string Key { get; }

        string Text { get; }

        void SetText(string text);

        void SetKey(string key);
    }
}