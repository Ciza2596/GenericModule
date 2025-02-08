namespace CizaSceneModule
{
    public interface ILoadingTask
    {
        bool IsComplete { get; }

        void Execute();
    }
}