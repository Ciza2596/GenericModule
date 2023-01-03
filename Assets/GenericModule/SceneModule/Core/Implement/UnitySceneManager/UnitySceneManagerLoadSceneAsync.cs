using System.Threading.Tasks;
using UnityEngine;

namespace SceneModule.Implement
{
    public class UnitySceneManagerLoadSceneAsync : ILoadSceneAsync
    {
        //private variable
        private readonly AsyncOperation _loadSceneAsync;


        //public variable
        public float Progress => _loadSceneAsync.progress;
        public Task Task { get; }


        //public method
        public UnitySceneManagerLoadSceneAsync(AsyncOperation loadSceneAsync) =>
            _loadSceneAsync = loadSceneAsync;


        public void Activate() =>
            _loadSceneAsync.allowSceneActivation = true;
    }
}