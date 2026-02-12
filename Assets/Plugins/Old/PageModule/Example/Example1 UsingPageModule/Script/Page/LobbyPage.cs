using CizaPageModule.Implement;
using CizaUniTask;
using UnityEngine;
using UnityEngine.UI;

namespace CizaPageModule.Example1
{
	public class LobbyPage : Page, IShowingAnimated, IShowingComplete, IHidingStart
	{
		//private variable
		[SerializeField]
		private Button _titleButton;

		[SerializeField]
		private BaseAnimPlayer _showingAnimPlayer;

		private PageModule _pageModule;

		//unity callback
		private void Awake()
		{
			var pageModuleExample = FindObjectOfType<PageModuleExample>();
			_pageModule = pageModuleExample.PageModule;
		}

		//PageModule callback
		public UniTask PlayShowingAnimationAsync() => _showingAnimPlayer.Play();
		public void ShowingComplete() => _titleButton.onClick.AddListener(OnTitleButtonClick);

		public void HidingStart() => _titleButton.onClick.RemoveListener(OnTitleButtonClick);

		//private method
		private void OnTitleButtonClick() =>
			_pageModule.HideAsync(nameof(LobbyPage), () => _pageModule.ShowAsync(nameof(TitlePage)));
	}
}
