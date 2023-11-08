using System;
using CizaCore;
using UnityEngine;

namespace CizaOptionModule.Implement
{
	public class OptionView : MonoBehaviour, IOptionView
	{
		[SerializeField]
		private CollectionSettings _collectionSettings;

		[Serializable]
		private class CollectionSettings : OptionSettings<Option>
		{
			[SerializeField]
			private ColumnInfoImp _columnInfo;

			[SerializeField]
			private RowInfoImp _rowInfo;

			public IColumnInfo ColumnInfo => _columnInfo;
			public IRowInfo    RowInfo    => _rowInfo;
		}

		public IOptionColumn[] OptionColumns      => _collectionSettings.OptionColumns;
		public Option[]        Options            => _collectionSettings.Options;
		public Option[]        OptionsIncludeNull => _collectionSettings.OptionsIncludeNull;
		public IColumnInfo     ColumnInfo         => _collectionSettings.ColumnInfo;
		public IRowInfo        RowInfo            => _collectionSettings.RowInfo;

		public void UnSelectAll()
		{
			foreach (var option in Options)
				option.Unselect();
		}

		[Serializable]
		private class ColumnInfoImp : IColumnInfo
		{
			[SerializeField]
			private bool _isColumnCircle;

			[SerializeField]
			private bool _isNotMoveWhenNullOrDisableInColumn;

			[Space]
			[SerializeField]
			private bool _isAutoChangeRowToLeft;

			[SerializeField]
			private bool _isAutoChangeRowToRight;

			public bool IsColumnCircle                     => _isColumnCircle;
			public bool IsNotMoveWhenNullOrDisableInColumn => _isNotMoveWhenNullOrDisableInColumn;
			public bool IsAutoChangeRowToLeft              => _isAutoChangeRowToLeft;
			public bool IsAutoChangeRowToRight             => _isAutoChangeRowToRight;
		}

		[Serializable]
		private class RowInfoImp : IRowInfo
		{
			[SerializeField]
			private bool _isRowCircle;

			[SerializeField]
			private bool _isNotMoveWhenNullOrDisableInRow;

			[Space]
			[SerializeField]
			private bool _isAutoChangeColumnToUp;

			[SerializeField]
			private bool _isAutoChangeColumnToDown;

			public bool IsRowCircle                     => _isRowCircle;
			public bool IsNotMoveWhenNullOrDisableInRow => _isNotMoveWhenNullOrDisableInRow;
			public bool IsAutoChangeColumnToUp          => _isAutoChangeColumnToUp;
			public bool IsAutoChangeColumnToDown        => _isAutoChangeColumnToDown;
		}
	}
}
