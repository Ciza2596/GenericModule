using UnityEngine;

namespace CizaPageModule
{
	public interface IPageModuleConfig
	{
		public string PageRootName        { get; }
		public bool   IsDontDestroyOnLoad { get; }

		public MonoBehaviour[] GetPagePrefabs();
	}
}
