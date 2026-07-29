using UnityEngine.Scripting;

namespace CizaAudioModule.Editor
{
	public class ItemSortManipulator : BSortManipulator<ItemVE>
	{
		// CONSTRUCTOR: --------------------------------------------------------------------- 
		
		[Preserve]
		public ItemSortManipulator(IListVE list) : base(list, false, true) { }
	}
}
