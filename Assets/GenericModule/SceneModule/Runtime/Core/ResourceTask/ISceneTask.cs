namespace SceneModule
{
    public interface ISceneTask
    {
        public bool IsCompleteLoading { get; }

        public void Load();

        public void Release();
    }
}