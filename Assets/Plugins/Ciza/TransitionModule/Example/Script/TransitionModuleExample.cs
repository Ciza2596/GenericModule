using CizaTransitionModule.Implement;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace CizaTransitionModule.Example
{
    public class TransitionModuleExample : MonoBehaviour
    {
        [SerializeField]
        private Button _goToAButton;

        [SerializeField]
        private GameObject _viewAPrefab;

        [Space]
        [SerializeField]
        private Button _goToBButton;

        [SerializeField]
        private GameObject _viewBPrefab;


        [Space]
        [SerializeField]
        private TransitionModuleConfig _transitionModuleConfig;

        private TransitionModule _transitionModule;


        private async void Awake()
        {
            _transitionModule = new TransitionModule(_transitionModuleConfig);
            await _transitionModule.InitializeAsync(transform);

            _goToAButton.onClick.AddListener(GoToAButton);
            _goToBButton.onClick.AddListener(GoToBButton);
        }

        private void GoToAButton() =>
            GoToA();

        private void GoToBButton() =>
            GoToB();

        private void GoToA()
        {
            _transitionModule.TransitAsync(new Presenter(_viewAPrefab));
        }

        private void GoToB()
        {
            _transitionModule.TransitAsync(new Presenter(_viewBPrefab));
        }


        private class Presenter : IPresenter
        {
            private readonly GameObject _viewPrefab;

            private GameObject _view;

            public Presenter(GameObject viewPrefab) =>
                _viewPrefab = viewPrefab;

            public UniTask InitializeAsync()
            {
                Debug.Log($"InitializeAsync viewPrefab: {_viewPrefab.name}.");
                _view = Instantiate(_viewPrefab);

                return UniTask.CompletedTask;
            }

            public void Complete()
            {
                Debug.Log($"Complete viewPrefab: {_viewPrefab.name}.");
            }

            public UniTask ReleaseAsync()
            {
                Debug.Log($"ReleaseAsync viewPrefab: {_viewPrefab.name}.");

                var view = _view;
                _view = null;
                Destroy(view);

                return UniTask.CompletedTask;
            }
        }
    }
}