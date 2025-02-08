namespace CizaInputModule
{
    public interface IControlSchemeInfo
    {
        string DataId { get; }

        float LowFrequency { get; }
        float HighFrequency { get; }
    }
}