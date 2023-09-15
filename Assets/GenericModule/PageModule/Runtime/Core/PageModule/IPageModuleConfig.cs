using UnityEngine;

namespace CizaPageModule
{
	public interface IPageModuleConfig
	{
		public string PageGameObjectRootName { get; }
		public bool   IsDontDestroyOnLoad    { get; }

		public MonoBehaviour[] GetPagePrefabs();
	}
}
