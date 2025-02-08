using System.Collections.Generic;
using CizaPageModule.Implement;
using UnityEngine;
using UnityEngine.Assertions;

namespace CizaOptionModule.Implement
{
	[CreateAssetMenu(fileName = "OptionModuleConfig", menuName = "Ciza/OptionModule/OptionModuleConfig")]
	public class OptionModuleConfig : ScriptableObject, IOptionModuleConfig
	{
		[SerializeField]
		private string _pageRootName = "[OptionModuleRoot]";

		[SerializeField]
		private bool _isDontDestroyOnLoad = false;

		[Space]
		[SerializeField]
		private Page[] _pagePrefabs;

		public string PageRootName => _pageRootName;

		public bool IsDontDestroyOnLoad => _isDontDestroyOnLoad;

		public MonoBehaviour[] GetPagePrefabs()
		{
			var pagePrefabs = new List<MonoBehaviour>();

			foreach (var pagePrefab in _pagePrefabs)
			{
				Assert.IsNotNull(pagePrefab, "[OptionModuleConfig::GetPagePrefabs] Please check pagePrefabs. Lose a pagePrefab.");
				pagePrefabs.Add(pagePrefab);
			}

			return pagePrefabs.ToArray();
		}
	}
}
