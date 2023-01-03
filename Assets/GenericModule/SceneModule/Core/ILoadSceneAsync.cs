namespace SceneModule
{
    public interface ILoadSceneAsync
    {
        public float Progress { get; }

        public void Activate();
    }
}