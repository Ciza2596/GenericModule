using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace CizaAddressablesModule.Editor
{
	public class FileFilterListVE : BSimpleListVE<FileFilter, FileFilterVE>
	{
		// VARIABLE: -----------------------------------------------------------------------------

		protected override FileFilter CreateDefaultItem() =>
			new FileFilter();
		protected override FileFilterVE CreateItemVE() =>
			new FileFilterVE(this);
		protected override int GetItemIndex(FileFilterVE item) =>
			item.Index;
		protected override void RefreshItem(FileFilterVE item, int index, FileFilter value) =>
			item.Refresh(index, value);
		protected override void RefreshItemSelectedStyle(FileFilterVE item) =>
			item.RefreshSelectedStyle();
		protected override string GetItemSearchText(FileFilter item) =>
			GetFilterTitle(item);
		protected override FileFilter NormalizeItem(FileFilter item) =>
			item ?? new FileFilter();
		protected override FileFilter Clone(FileFilter item) =>
			item == null ? new FileFilter() : JsonUtility.FromJson<FileFilter>(JsonUtility.ToJson(item));
		protected override string LegacyItemsFieldName => "_fileFilters";


		// PUBLIC VARIABLE: ---------------------------------------------------------------------

		public virtual string GetFilterTitle(FileFilter filter)
		{
			if (filter == null)
				return string.Empty;
			var fileName = string.IsNullOrEmpty(filter.NamePrefix) && string.IsNullOrEmpty(filter.NameSuffix) ? "*" : filter.NamePrefix + "..." + filter.NameSuffix;
			return fileName + (filter.Extension == "." ? ".*" : filter.Extension);
		}

		// CONSTRUCTOR: ------------------------------------------------------------------------

		[Preserve]
		public FileFilterListVE(string fileFiltersKey) : base(fileFiltersKey ?? "CizaAddressablesModule.ImportMenu.FileFilters") { }

		[Preserve]
		public FileFilterListVE(Func<FileFilter[]> readItems, Action<FileFilter[]> writeItems) : base(readItems, writeItems) { }

		// PUBLIC METHOD: ----------------------------------------------------------------------

		public virtual void UpdateItem(int index, string prefix, string suffix, string extension) =>
			base.UpdateItem(index, new FileFilter(prefix, suffix, extension));
	}
}
