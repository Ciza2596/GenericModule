using UnityEngine;

namespace CizaFadeCreditModule.Implement
{
	[CreateAssetMenu(fileName = "FadeCreditModuleConfig", menuName = "Ciza/FadeCreditModule/FadeCreditModuleConfig")]
	public class FadeCreditModuleConfig : ScriptableObject, IFadeCreditModuleConfig
	{
		// VARIABLE: -----------------------------------------------------------------------------

		[SerializeField]
		private string _rootName;

		[Space]
		[SerializeField]
		private bool _isDontDestroyOnLoad;

		[SerializeField]
		private GameObject _controllerPrefab;

		// PUBLIC VARIABLE: ---------------------------------------------------------------------

		public virtual string RootName => _rootName;

		public virtual bool IsDontDestroyOnLoad => _isDontDestroyOnLoad;
		public virtual GameObject ControllerPrefab => _controllerPrefab;


		// CONSTRUCTOR: ------------------------------------------------------------------------

		public virtual void Reset()
		{
			_rootName = "[CreditModule]";

			_isDontDestroyOnLoad = true;
			_controllerPrefab = null;
		}
	}
}