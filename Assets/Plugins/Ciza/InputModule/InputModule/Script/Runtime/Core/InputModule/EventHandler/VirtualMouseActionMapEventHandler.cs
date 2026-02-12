using UnityEngine.Scripting;

namespace CizaInputModule
{
	public class VirtualMouseActionMapEventHandler : BEventHandler
	{
		// CONST & STATIC: -----------------------------------------------------------------------

		public const string MOVE_ACTION_DATA_ID = "LeftMove";
		public const string CONFIRM_ACTION_DATA_ID = "Confirm";
		public const string CANCEL_ACTION_DATA_ID = "Cancel";
		public const string SCROLL_ACTION_DATA_ID = "Scroll";

		// CONSTRUCTOR: ------------------------------------------------------------------------

		[Preserve]
		public VirtualMouseActionMapEventHandler() : base() { }

		// LIFECYCLE METHOD: ------------------------------------------------------------------

		public override void Register(InputModule inputModule)
		{
			inputModule.GetVirtualMouseMoveActionPath += GetVirtualMouseMoveActionPath;
			inputModule.GetVirtualMouseLeftButtonActionPath += GetVirtualMouseLeftButtonActionPath;
			inputModule.GetVirtualMouseRightButtonActionPath += GetVirtualMouseRightButtonActionPath;
			inputModule.GetVirtualMouseScrollActionPath += GetVirtualMouseScrollActionPath;
		}

		public override void Unregister(InputModule inputModule)
		{
			inputModule.GetVirtualMouseMoveActionPath -= GetVirtualMouseMoveActionPath;
			inputModule.GetVirtualMouseLeftButtonActionPath -= GetVirtualMouseLeftButtonActionPath;
			inputModule.GetVirtualMouseRightButtonActionPath -= GetVirtualMouseRightButtonActionPath;
			inputModule.GetVirtualMouseScrollActionPath -= GetVirtualMouseScrollActionPath;
		}


		// PROTECT METHOD: --------------------------------------------------------------------

		protected virtual string GetVirtualMouseMoveActionPath(int playerIndex, string currentActionMapDataId) =>
			GetDefaultActionPath(currentActionMapDataId, MOVE_ACTION_DATA_ID);

		protected virtual string GetVirtualMouseLeftButtonActionPath(int playerIndex, string currentActionMapDataId) =>
			GetDefaultActionPath(currentActionMapDataId, CONFIRM_ACTION_DATA_ID);

		protected virtual string GetVirtualMouseRightButtonActionPath(int playerIndex, string currentActionMapDataId) =>
			GetDefaultActionPath(currentActionMapDataId, CANCEL_ACTION_DATA_ID);

		protected virtual string GetVirtualMouseScrollActionPath(int playerIndex, string currentActionMapDataId) =>
			GetDefaultActionPath(currentActionMapDataId, SCROLL_ACTION_DATA_ID);


		protected virtual string GetDefaultActionPath(string currentActionMapDataId, string actionDataId) =>
			$"{currentActionMapDataId}/{actionDataId}";
	}
}