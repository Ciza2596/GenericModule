using UnityEngine;

namespace CizaInputModule.Implement
{
	[CreateAssetMenu(fileName = "RumbleManagerConfig", menuName = "Ciza/InputModule/RumbleManagerConfig")]
	public class RumbleManagerConfig : ScriptableObject, IRumbleManagerConfig
	{
		[SerializeField]
		protected RumbleInfoMapList _rumbleInfoMapList;

		public virtual string[] AllDataIds => _rumbleInfoMapList.Keys;


		public virtual bool TryGetRumbleInfo(string dataId, out IRumbleInfo rumbleInfo)
		{
			if (!_rumbleInfoMapList.TryGetValue(dataId, out var rumbleInfoImp))
			{
				rumbleInfo = null;
				return false;
			}

			rumbleInfo = rumbleInfoImp;
			return true;
		}


		public virtual void Reset()
		{
			_rumbleInfoMapList = new RumbleInfoMapList();
		}
	}
}