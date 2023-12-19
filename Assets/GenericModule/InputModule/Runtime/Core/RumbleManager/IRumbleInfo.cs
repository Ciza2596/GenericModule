namespace CizaInputModule
{
    public interface IRumbleInfo
    {
        string DataId { get; }
        
        float Duration { get; }

        bool TryGetControlSchemeInfo(string dataId, out IControlSchemeInfo controlSchemeInfo);
    }
}