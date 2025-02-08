
namespace CizaSceneModule
{
    public interface ILoadSceneAsync
    {
        public bool IsDone { get; }
        
        public void Activate();
    }
}