using UnityEngine.Scripting;
using UnityEngine.UIElements;

namespace CizaAudioModule.Editor
{
	public abstract class BItemVE : VisualElement
	{
		// CONSTRUCTOR: --------------------------------------------------------------------- 

		[Preserve]
		protected BItemVE() { }

		// PUBLIC METHOD: ----------------------------------------------------------------------

		public virtual void Initialize() { }

		public virtual void Refresh() { }

		public virtual void DisplayAsNormal()
		{
			style.opacity = 1f;
		}

		public virtual void DisplayAsDrag() =>
			style.opacity = 0.25f;
	}
}