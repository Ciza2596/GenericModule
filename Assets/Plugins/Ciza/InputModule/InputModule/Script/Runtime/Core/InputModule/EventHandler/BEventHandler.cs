using UnityEngine.Scripting;

namespace CizaInputModule
{
	public abstract class BEventHandler
	{
		// CONSTRUCTOR: ------------------------------------------------------------------------

		[Preserve]
		protected BEventHandler() { }

		// LIFECYCLE METHOD: ------------------------------------------------------------------

		public abstract void Register(InputModule inputModule);

		public abstract void Unregister(InputModule inputModule);
	}
}