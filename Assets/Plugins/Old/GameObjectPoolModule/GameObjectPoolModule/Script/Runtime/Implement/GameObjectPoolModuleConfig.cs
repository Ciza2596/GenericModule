using UnityEngine;

namespace CizaGameObjectPoolModule.Implement
{
	[CreateAssetMenu(fileName = "GameObjectPoolModuleConfig", menuName = "Ciza/GameObjectPoolModule/GameObjectPoolModuleConfig")]
	public class GameObjectPoolModuleConfig : ScriptableObject, IGameObjectPoolModuleConfig
	{
		// VARIABLE: -----------------------------------------------------------------------------

		[SerializeField]
		protected string _poolRootName;

		[Space]
		[SerializeField]
		protected string _poolPrefix;

		[SerializeField]
		protected string _poolSuffix;

		// PUBLIC VARIABLE: ---------------------------------------------------------------------

		public virtual string PoolRootName => _poolRootName;
		public virtual string PoolPrefix => _poolPrefix;
		public virtual string PoolSuffix => _poolSuffix;

		// CONSTRUCTOR: ------------------------------------------------------------------------

		public virtual void Reset()
		{
			_poolRootName = "[GameObjectModule]";

			_poolPrefix = "[";
			_poolSuffix = "s]";
		}
	}
}