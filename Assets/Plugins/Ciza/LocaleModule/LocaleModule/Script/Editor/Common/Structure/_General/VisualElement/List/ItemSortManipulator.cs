using UnityEngine.Scripting;

namespace CizaLocaleModule.Editor
{
	public class ItemSortManipulator : BSortManipulator<ItemVE>
	{
		// CONSTRUCTOR: --------------------------------------------------------------------- 
		
		[Preserve]
		public ItemSortManipulator(IListVE list) : base(list, false, true) { }
	}
}
