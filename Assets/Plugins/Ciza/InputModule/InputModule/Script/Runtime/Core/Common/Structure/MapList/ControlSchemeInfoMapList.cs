using System;
using UnityEngine.Scripting;

namespace CizaInputModule
{
	[Serializable]
	public class ControlSchemeInfoMapList : MapList<ControlSchemeInfo>
	{
		// CONSTRUCTOR: ------------------------------------------------------------------------

		[Preserve]
		public ControlSchemeInfoMapList() { }

		// PUBLIC METHOD: ----------------------------------------------------------------------

		public new ControlSchemeInfoMapList Copy() =>
			Copy<ControlSchemeInfoMapList>();
	}
}
