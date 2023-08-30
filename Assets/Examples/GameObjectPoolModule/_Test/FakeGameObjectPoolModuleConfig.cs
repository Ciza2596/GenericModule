using CizaGameObjectPoolModule;

public class FakeGameObjectPoolModuleConfig : IGameObjectPoolModuleConfig
{
    //public variable
    public string PoolRootName { get; } = "[GameObjectModule]";
    public string PoolPrefix { get; } = "[";
    public string PoolSuffix { get; } = "s]";
}