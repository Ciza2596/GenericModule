using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Scripting;

namespace CizaPopupModule.Implement
{
	[CreateAssetMenu(fileName = "PopupModuleConfig", menuName = "Ciza/PopupModule/PopupModuleConfig")]
	public class PopupModuleConfig : ScriptableObject, IPopupModuleConfig
	{
		// VARIABLE: -----------------------------------------------------------------------------

		[SerializeField]
		protected string _rootName;

		[SerializeField]
		protected bool _isDontDestroyOnLoad;

		[Space]
		[SerializeField]
		protected PopupPrefabMap[] _popupPrefabMaps;

		// PUBLIC VARIABLE: ---------------------------------------------------------------------

		public string RootName => _rootName;
		public bool IsDontDestroyOnLoad => _isDontDestroyOnLoad;

		public bool TryGetPopupPrefab(string dataId, out GameObject popupPrefab)
		{
			if (_popupPrefabMaps == null || _popupPrefabMaps.Length == 0)
			{
				popupPrefab = null;
				return false;
			}

			var popupMap = _popupPrefabMaps.FirstOrDefault(popupMap => popupMap.DataId == dataId);
			if (popupMap != null && popupMap.Prefab != null)
			{
				popupPrefab = popupMap.Prefab;
				return true;
			}

			popupPrefab = null;
			return false;
		}


		// CONSTRUCTOR: ------------------------------------------------------------------------

		public virtual void Reset()
		{
			_rootName = "[PopupModule]";
			_isDontDestroyOnLoad = true;

			_popupPrefabMaps = Array.Empty<PopupPrefabMap>();
		}


		[Serializable]
		public class PopupPrefabMap
		{
			// VARIABLE: -----------------------------------------------------------------------------

			[SerializeField]
			protected string _dataId;

			[SerializeField]
			protected GameObject _prefab;

			// PUBLIC VARIABLE: ---------------------------------------------------------------------

			public virtual string DataId => _dataId;
			public virtual GameObject Prefab => _prefab;

			// CONSTRUCTOR: ------------------------------------------------------------------------

			[Preserve]
			public PopupPrefabMap() : this(string.Empty, null) { }

			[Preserve]
			public PopupPrefabMap(string dataId, GameObject prefab)
			{
				_dataId = dataId;
				_prefab = prefab;
			}
		}
	}
}