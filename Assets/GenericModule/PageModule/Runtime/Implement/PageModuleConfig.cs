using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace CizaPageModule.Implement
{
	[CreateAssetMenu(fileName = "PageModuleConfig", menuName = "Ciza/PageModule/PageModuleConfig")]
	public class PageModuleConfig : ScriptableObject, IPageModuleConfig
	{
		[SerializeField]
		private string _pageGameObjectRootName = "[PageRoot]";

		[SerializeField]
		private bool _isDontDestroyOnLoad = false;

		[Space]
		[SerializeField]
		private Page[] _pagePrefabs;

		public string PageGameObjectRootName => _pageGameObjectRootName;

		public bool IsDontDestroyOnLoad => _isDontDestroyOnLoad;

		public MonoBehaviour[] GetPagePrefabs()
		{
			var pagePrefabs = new List<MonoBehaviour>();

			foreach (var pagePrefab in _pagePrefabs)
			{
				Assert.IsNotNull(pagePrefab, "[PageModuleConfig::GetPagePrefabs] Please check pagePrefabs. Lose a pagePrefab.");
				pagePrefabs.Add(pagePrefab);
			}

			return pagePrefabs.ToArray();
		}
	}
}
