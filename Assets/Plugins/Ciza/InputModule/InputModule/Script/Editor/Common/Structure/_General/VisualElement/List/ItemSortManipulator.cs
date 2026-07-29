using UnityEngine.Scripting;

namespace CizaInputModule.Editor
{
	public class ItemSortManipulator : BSortManipulator<ItemVE>
	{
		// CONSTRUCTOR: --------------------------------------------------------------------- 
		
		[Preserve]
		public ItemSortManipulator(IListVE list) : base(list, false, true) { }
	}
}