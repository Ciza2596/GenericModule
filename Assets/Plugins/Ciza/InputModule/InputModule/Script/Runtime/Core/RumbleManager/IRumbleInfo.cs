namespace CizaInputModule
{
    public interface IRumbleInfo
    {
        int Order { get; }
        float Duration { get; }

        bool TryGetControlSchemeInfo(string dataId, out IControlSchemeInfo controlSchemeInfo);
    }
}