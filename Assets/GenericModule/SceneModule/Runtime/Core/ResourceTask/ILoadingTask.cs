namespace CizaSceneModule
{
    public interface ILoadingTask
    {
        public bool IsComplete { get; }

        public void Execute();
    }
}