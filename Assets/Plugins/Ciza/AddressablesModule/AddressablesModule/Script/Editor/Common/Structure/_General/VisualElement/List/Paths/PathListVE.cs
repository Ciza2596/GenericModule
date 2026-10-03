using System;
using UnityEngine.Scripting;

namespace CizaAddressablesModule.Editor
{
	public class PathListVE : BSimpleListVE<string, PathVE>
	{
		// VARIABLE: -----------------------------------------------------------------------------

		protected override string CreateDefaultItem() =>
			string.Empty;
		protected override PathVE CreateItemVE() =>
			new PathVE(this);
		protected override int GetItemIndex(PathVE item) =>
			item.Index;
		protected override void RefreshItem(PathVE item, int index, string value) =>
			item.Refresh(index, value);
		protected override void RefreshItemSelectedStyle(PathVE item) =>
			item.RefreshSelectedStyle();
		protected override string GetItemSearchText(string item) =>
			item;
		protected override string NormalizeItem(string item) =>
			item ?? string.Empty;
		protected override string Clone(string item) =>
			NormalizeItem(item);
		protected override string LegacyItemsFieldName => "_paths";

		// CONSTRUCTOR: ------------------------------------------------------------------------

		[Preserve]
		public PathListVE(string pathsKey) : base(pathsKey ?? "CizaAddressablesModule.ImportMenu.Paths") { }

		[Preserve]
		public PathListVE(Func<string[]> readItems, Action<string[]> writeItems) : base(readItems, writeItems) { }

	}
}
