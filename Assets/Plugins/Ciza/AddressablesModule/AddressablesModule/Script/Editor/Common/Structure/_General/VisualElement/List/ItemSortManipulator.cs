using UnityEngine.Scripting;

namespace CizaAddressablesModule.Editor
{
	public class ItemSortManipulator : BSortManipulator<ItemVE>
	{
		// CONSTRUCTOR: --------------------------------------------------------------------- 
		
		[Preserve]
		public ItemSortManipulator(IListVE list) : base(list, false, true) { }
	}
}