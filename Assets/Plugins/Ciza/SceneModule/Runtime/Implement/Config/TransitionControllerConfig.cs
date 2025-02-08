using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace CizaSceneModule.Implement
{
	[CreateAssetMenu(fileName = "TransitionControllerConfig", menuName = "Ciza/SceneModule/TransitionControllerConfig")]
	public class TransitionControllerConfig : ScriptableObject, ITransitionControllerConfig
	{
		[SerializeField]
		private GameObject _viewParentPrefab;

		[SerializeField]
		private ViewPrefabMap[] _transitionInViewPrefabMaps;

		[SerializeField]
		private ViewPrefabMap[] _loadingViewPrefabMaps;

		[SerializeField]
		private ViewPrefabMap[] _transitionOutViewPrefabMaps;

		private bool _isViewName;

		public void SetIsViewName(bool isViewName) => _isViewName = isViewName;

		public GameObject GetViewParentPrefab() => _viewParentPrefab;

		public GameObject GetTransitionInViewPrefab(string viewNameOrTag)
		{
			var viewPrefab = _isViewName ? GetViewPrefabByViewName(_transitionInViewPrefabMaps, viewNameOrTag) : GetViewPrefabByViewTag(_transitionInViewPrefabMaps, viewNameOrTag);
			return viewPrefab;
		}

		public GameObject GetLoadingViewPrefab(string viewNameOrTag)
		{
			var viewPrefab = _isViewName ? GetViewPrefabByViewName(_loadingViewPrefabMaps, viewNameOrTag) : GetViewPrefabByViewTag(_loadingViewPrefabMaps, viewNameOrTag);
			return viewPrefab;
		}

		public GameObject GetTransitionOutPrefab(string viewNameOrTag)
		{
			var viewPrefab = _isViewName ? GetViewPrefabByViewName(_transitionOutViewPrefabMaps, viewNameOrTag) : GetViewPrefabByViewTag(_transitionOutViewPrefabMaps, viewNameOrTag);
			return viewPrefab;
		}

		private GameObject GetViewPrefabByViewName(ViewPrefabMap[] viewMaps, string viewName)
		{
			var viewMap = viewMaps.First(viewMap => viewMap.ViewName == viewName);
			Assert.IsNotNull(viewMap, $"[TransitionControllerConfig::GetViewPrefabByViewName] Not find prefab with viewName: {viewName}");

			var viewPrefab = viewMap.ViewPrefab;
			return viewPrefab;
		}

		private GameObject GetViewPrefabByViewTag(ViewPrefabMap[] viewMaps, string viewTag)
		{
			var viewMap = viewMaps.First(viewMap => viewMap.ViewTag.Contains(viewTag));
			Assert.IsNotNull(viewMap, $"[TransitionControllerConfig::GetViewPrefabByViewTag] Not find prefab with viewTag: {viewTag}");

			var viewPrefab = viewMap.ViewPrefab;
			return viewPrefab;
		}
	}
}
