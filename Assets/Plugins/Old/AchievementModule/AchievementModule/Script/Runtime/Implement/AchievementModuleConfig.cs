using System;
using System.Linq;
using UnityEngine;

namespace CizaAchievementModule.Implement
{
	[CreateAssetMenu(fileName = "AchievementModuleConfig", menuName = "Ciza/AchievementModule/AchievementModuleConfig")]
	public class AchievementModuleConfig : ScriptableObject, IAchievementModuleConfig
	{
		// VARIABLE: -----------------------------------------------------------------------------

		[SerializeField]
		protected StatInfo[] _statInfos;

		[SerializeField]
		protected AchievementInfo[] _achievementInfos;

		// PUBLIC VARIABLE: ---------------------------------------------------------------------

		public virtual Type StatType => typeof(Stat);

		public virtual IStatInfo[] StatInfos => _statInfos != null ? _statInfos.Cast<IStatInfo>().ToArray() : Array.Empty<IStatInfo>();
		public virtual IAchievementInfo[] AchievementInfos => _achievementInfos != null ? _achievementInfos.Cast<IAchievementInfo>().ToArray() : Array.Empty<IAchievementInfo>();

		// CONSTRUCTOR: ------------------------------------------------------------------------

		public virtual void Reset()
		{
			_statInfos = Array.Empty<StatInfo>();
			_achievementInfos = Array.Empty<AchievementInfo>();
		}
	}
}