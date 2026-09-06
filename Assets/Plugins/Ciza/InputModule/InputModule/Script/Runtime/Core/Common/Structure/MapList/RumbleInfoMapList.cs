using System;
using UnityEngine.Scripting;

namespace CizaInputModule
{
	[Serializable]
	public class RumbleInfoMapList : MapList<RumbleInfo>
	{
		// CONSTRUCTOR: ------------------------------------------------------------------------

		[Preserve]
		public RumbleInfoMapList() { }

		// PUBLIC METHOD: ----------------------------------------------------------------------

		public new RumbleInfoMapList Copy() =>
			Copy<RumbleInfoMapList>();
	}
}
