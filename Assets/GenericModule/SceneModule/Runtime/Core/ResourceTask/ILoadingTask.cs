namespace SceneModule
{
    public interface ILoadingTask
    {
        public bool IsComplete { get; }

        public void Execute();
    }
}