using System.Threading.Tasks;
using UnityEngine;

namespace SceneModule.Implement
{
    public class UnitySceneManagerLoadSceneAsync : ILoadSceneAsync
    {
        //private variable
        private readonly AsyncOperation _loadSceneAsync;


        //public variable
        public bool IsDone  => _loadSceneAsync.progress >= 0.90f;


        //public method
        public UnitySceneManagerLoadSceneAsync(AsyncOperation loadSceneAsync) =>
            _loadSceneAsync = loadSceneAsync;


        public void Activate() =>
            _loadSceneAsync.allowSceneActivation = true;
    }
}