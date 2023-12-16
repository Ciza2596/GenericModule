using System.Linq;
using UnityEngine;

namespace CizaInputModule.Implement
{
	[CreateAssetMenu(fileName = "RumbleManagerConfig", menuName = "Ciza/InputModule/RumbleManagerConfig")]
	public class RumbleManagerConfig : ScriptableObject, IRumbleManagerConfig
	{
		[SerializeField]
		private RumbleInfo[] _rumbleInfos;

		public bool TryGetRumbleInfo(string dataId, out IRumbleInfo rumbleInfo)
		{
			if (_rumbleInfos == null)
			{
				rumbleInfo = null;
				return false;
			}

			rumbleInfo = _rumbleInfos.FirstOrDefault(rumbleInfo => rumbleInfo.DataId == dataId);
			return rumbleInfo != null;
		}
	}
}
