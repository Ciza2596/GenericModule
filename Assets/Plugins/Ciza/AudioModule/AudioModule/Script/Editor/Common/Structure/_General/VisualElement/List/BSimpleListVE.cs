using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

namespace CizaAudioModule.Editor
{
	public abstract class BSimpleListVE<TItem, TItemVE> : BListVE<TItemVE>, BBoxVE.IContent where TItemVE : BItemVE
	{
		// VARIABLE: -----------------------------------------------------------------------------

		protected readonly VisualElement _head = new VisualElement();
		protected readonly TextField _searchTextField = new TextField();
		protected readonly Button _clearSearchButton = new Button();
		protected readonly VisualElement _body = new VisualElement();
		protected readonly VisualElement _foot = new VisualElement();
		protected readonly List<int> _visibleItemIndexList = new List<int>();
		protected readonly List<int> _selectedItemIndexList = new List<int>();
		protected int _selectedItemIndexBaseline = -1;

		protected List<TItem> _items = new List<TItem>();
		protected readonly Func<TItem[]> _readItems;
		protected readonly Action<TItem[]> _writeItems;

		protected virtual string[] USSPaths => new[] { "List" };

		#region Class

		protected override string[] DropIndicatorClasses => new[] { "list-drop-indicator" };
		protected virtual string[] RootClasses => new[] { "list" };
		protected virtual string[] HeadClasses => new[] { "list-head" };
		protected virtual string[] BodyClasses => new[] { "list-body" };
		protected virtual string[] FootClasses => new[] { "list-foot" };

		#endregion

		protected virtual TItem[] ReadItems()
		{
			if (_readItems != null)
				return (_readItems() ?? Array.Empty<TItem>()).Select(NormalizeItem).ToArray();
			
			var json = EditorPrefs.GetString(DataKey, string.Empty);
			if (string.IsNullOrEmpty(json))
				return Array.Empty<TItem>();
			try
			{
				return (DeserializeItems(json) ?? Array.Empty<TItem>()).Select(NormalizeItem).ToArray();
			}
			catch (ArgumentException exception)
			{
				Debug.LogWarning($"Cannot read list from EditorPrefs '{DataKey}': {exception.Message}");
				return Array.Empty<TItem>();
			}
		}

		protected virtual void WriteItems(List<TItem> items)
		{
			if (_writeItems != null)
				_writeItems(items.ToArray());
			else
				EditorPrefs.SetString(DataKey, SerializeItems(items));
			if (IsInitialized)
				Refresh();
		}

		protected virtual string LegacyItemsFieldName => null;

		protected virtual TItem[] DeserializeItems(string json)
		{
			var items = JsonUtility.FromJson<StoredItems>(json)?.Items;
			if (items != null || string.IsNullOrEmpty(LegacyItemsFieldName))
				return items;

			// Old containers wrote one root field. Rename only that field,
			// leaving item values (including paths containing field names) intact.
			var pattern = @"\A(\s*\{\s*)""" + Regex.Escape(LegacyItemsFieldName) + @"""(\s*:)";
			var migratedJson = Regex.Replace(json, pattern, "$1\"Items\"$2");
			return JsonUtility.FromJson<StoredItems>(migratedJson)?.Items;
		}

		protected virtual string SerializeItems(List<TItem> items) =>
			JsonUtility.ToJson(new StoredItems { Items = items.ToArray() });

		[Serializable]
		protected class StoredItems
		{
			public TItem[] Items;
		}

		protected abstract TItem CreateDefaultItem();
		protected abstract TItemVE CreateItemVE();
		protected abstract int GetItemIndex(TItemVE item);
		protected abstract void RefreshItem(TItemVE item, int index, TItem value);
		protected abstract void RefreshItemSelectedStyle(TItemVE item);
		protected abstract string GetItemSearchText(TItem item);
		protected virtual TItem NormalizeItem(TItem item) => item;

		// PUBLIC VARIABLE: ---------------------------------------------------------------------

		public virtual string DataKey { get; }
		public virtual VisualElement Body => this;
		public virtual bool IsAllowSelection => true;
		public virtual bool IsAllowReordering => true;
		public virtual bool IsAllowDuplicate => true;
		public virtual bool IsAllowDelete => true;
		public virtual bool IsAllowContextMenu => true;
		public virtual bool IsAllowCopyPaste => true;
		public virtual bool IsSearch => !string.IsNullOrEmpty(_searchTextField.value);
		public virtual int Count => _items.Count;
		public virtual int[] SelectedItemIndexList => _selectedItemIndexList.ToArray();
		public virtual bool IsSelectAll => Count == _selectedItemIndexList.Count;
		public virtual bool IsUnselectAll => _selectedItemIndexList.Count == 0;

		public virtual TItem[] Items => _items.ToArray();

		// CONSTRUCTOR: ---------------------------------------------------------------------

		[Preserve]
		protected BSimpleListVE(string dataKey)
		{
			DataKey = dataKey;
		}

		protected BSimpleListVE(Func<TItem[]> readItems, Action<TItem[]> writeItems)
		{
			_readItems = readItems ?? throw new ArgumentNullException(nameof(readItems));
			_writeItems = writeItems ?? throw new ArgumentNullException(nameof(writeItems));
		}

		// LIFECYCLE METHOD: -------------------------------------------------------------------

		protected override void DerivedInitialize()
		{
			foreach (var sheet in StyleSheetUtils.GetStyleSheets(USSPaths))
				styleSheets.Add(sheet);
			foreach (var c in RootClasses)
				AddToClassList(c);
			foreach (var c in HeadClasses)
				_head.AddToClassList(c);
			foreach (var c in BodyClasses)
				_body.AddToClassList(c);
			foreach (var c in FootClasses)
				_foot.AddToClassList(c);

			_searchTextField.tooltip = "Search";
			_searchTextField.Add(new Image { image = ListVE.DEFAULT_SEARCH_ICON.Texture });
			_clearSearchButton.Add(new Image { image = ListVE.DEFAULT_CLEAR_SEARCH_ICON.Texture });
			_clearSearchButton.clicked += () => _searchTextField.value = string.Empty;
			_searchTextField.Add(_clearSearchButton);
			_searchTextField.RegisterValueChangedCallback(_ => Refresh());
			_head.Add(_searchTextField);
			var addButton = new Button(() => InsertItem(Count, CreateDefaultItem()));
			addButton.Add(new Image { image = ListVE.DEFAULT_ADD_ITEM_ICON.Texture });
			addButton.Add(new Label("Add new item..."));
			_foot.Add(addButton);

			Add(_head);
			Add(_body);
			Add(_foot);
			Refresh();
		}

		public override void Refresh()
		{
			if (!IsInitialized)
			{
				Initialize();
				return;
			}
			_items = ReadItems().ToList();
			_selectedItemIndexList.RemoveAll(i => i < 0 || i >= Count);
			if (_selectedItemIndexBaseline >= Count)
				_selectedItemIndexBaseline = -1;
			_visibleItemIndexList.Clear();
			for (var i = 0; i < Count; i++)
				if (!IsSearch || (GetItemSearchText(_items[i]) ?? string.Empty).Contains(_searchTextField.value, StringComparison.OrdinalIgnoreCase))
					_visibleItemIndexList.Add(i);

			var canReuse = _itemVEs.Count == _visibleItemIndexList.Count &&
				_itemVEs.Select(GetItemIndex).SequenceEqual(_visibleItemIndexList);
			if (!canReuse)
			{
				_itemVEs.Clear();
				_body.Clear();
			}
			for (var slot = 0; slot < _visibleItemIndexList.Count; slot++)
			{
				if (slot >= _itemVEs.Count)
				{
					var item = CreateItemVE();
					item.Initialize();
					_itemVEs.Add(item);
					_body.Add(item);
				}
				var index = _visibleItemIndexList[slot];
				RefreshItem(_itemVEs[slot], index, _items[index]);
			}
			_clearSearchButton.SetIsVisible(IsSearch);
			RefreshItemDragUI(-1, -1);
		}

		// PUBLIC METHOD: ----------------------------------------------------------------------

		public virtual void UpdateItem(int index, TItem value)
		{
			var values = ReadItems().ToList();
			if (index < 0 || index >= values.Count)
				return;
			values[index] = NormalizeItem(value);
			WriteItems(values);
		}

		public virtual void InsertItem(int index, TItem value) =>
			InsertItems(index, new[] { value });

		public virtual void InsertItems(int index, TItem[] values)
		{
			if (values == null || values.Length == 0)
				return;
			var items = ReadItems().ToList();
			index = Math.Clamp(index, 0, items.Count);
			items.InsertRange(index, values.Select(Clone));
			for (var i = 0; i < _selectedItemIndexList.Count; i++)
				if (_selectedItemIndexList[i] >= index)
					_selectedItemIndexList[i] += values.Length;
			if (_selectedItemIndexBaseline >= index)
				_selectedItemIndexBaseline += values.Length;
			WriteItems(items);
		}

		public virtual void DuplicateItem(int index)
		{
			var values = ReadItems();
			if (index >= 0 && index < values.Length)
				InsertItem(index + 1, values[index]);
		}

		public virtual void DeleteItem(int index)
		{
			var values = ReadItems().ToList();
			if (index < 0 || index >= values.Count)
				return;
			values.RemoveAt(index);
			_selectedItemIndexList.Remove(index);
			for (var i = 0; i < _selectedItemIndexList.Count; i++)
				if (_selectedItemIndexList[i] > index)
					_selectedItemIndexList[i]--;
			if (_selectedItemIndexBaseline == index)
				_selectedItemIndexBaseline = -1;
			else if (_selectedItemIndexBaseline > index)
				_selectedItemIndexBaseline--;
			WriteItems(values);
		}

		public override void MoveItem(int sourceIndex, int destinationIndex)
		{
			if (!IsAllowReordering || IsSearch)
				return;
			var values = ReadItems().ToList();
			if (sourceIndex < 0 || sourceIndex >= values.Count)
				return;
			destinationIndex = Math.Clamp(destinationIndex, 0, values.Count - 1);
			var selected = _selectedItemIndexList.Contains(sourceIndex)
				? _selectedItemIndexList.Where(i => i >= 0 && i < values.Count).Distinct().OrderBy(i => i).ToArray()
				: new[] { sourceIndex };
			var insertIndex = destinationIndex >= sourceIndex ? destinationIndex + 1 : destinationIndex;
			var blockIndex = Math.Clamp(insertIndex - selected.Count(i => i < insertIndex), 0, values.Count - selected.Length);
			var order = Enumerable.Range(0, values.Count).Where(i => !selected.Contains(i)).ToList();
			order.InsertRange(blockIndex, selected);
			for (var i = 0; i < _selectedItemIndexList.Count; i++)
				_selectedItemIndexList[i] = order.IndexOf(_selectedItemIndexList[i]);
			_selectedItemIndexBaseline = order.IndexOf(_selectedItemIndexBaseline);
			WriteItems(order.Select(i => values[i]).ToList());
		}

		public virtual void OnItemSelected(int index, EventModifiers modifiers)
		{
			if (!IsAllowSelection || index < 0 || index >= Count)
				return;
			if (modifiers.CheckIsShift())
			{
				if (_selectedItemIndexBaseline < 0)
					_selectedItemIndexBaseline = index;
				_selectedItemIndexList.Clear();
				var start = Math.Min(index, _selectedItemIndexBaseline);
				_selectedItemIndexList.AddRange(Enumerable.Range(start, Math.Abs(index - _selectedItemIndexBaseline) + 1));
			}
			else if (modifiers.CheckIsCtrl())
			{
				_selectedItemIndexBaseline = index;
				if (!_selectedItemIndexList.Remove(index))
					_selectedItemIndexList.Add(index);
			}
			else
			{
				var clear = _selectedItemIndexList.Count == 1 && _selectedItemIndexList[0] == index;
				_selectedItemIndexList.Clear();
				_selectedItemIndexBaseline = clear ? -1 : index;
				if (!clear)
					_selectedItemIndexList.Add(index);
			}
			RefreshSelectedColor();
		}

		public virtual void SelectAllItem()
		{
			if (!IsAllowSelection)
				return;
			_selectedItemIndexList.Clear();
			_selectedItemIndexList.AddRange(Enumerable.Range(0, Count));
			_selectedItemIndexBaseline = Count > 0 ? 0 : -1;
			RefreshSelectedColor();
		}

		public virtual void UnselectAllItem()
		{
			_selectedItemIndexList.Clear();
			_selectedItemIndexBaseline = -1;
			RefreshSelectedColor();
		}

		public virtual TItem[] GetSelectedItemsCopy()
		{
			var values = ReadItems();
			return _selectedItemIndexList.Where(i => i >= 0 && i < values.Length).OrderBy(i => i)
				.Select(i => Clone(values[i])).ToArray();
		}

		// PUBLIC METHOD: ----------------------------------------------------------------------

		public override int GetItemIndexOf(VisualElement item) =>
			item is TItemVE row && _itemVEs.Contains(row) ? GetItemIndex(row) : -1;

		public override bool TryGetClosestItemIndex(float cursorY, out int itemIndex)
		{
			itemIndex = -1;
			if (IsSearch || !IsAllowReordering)
				return false;
			return base.TryGetClosestItemIndex(cursorY, out itemIndex);
		}

		public override void RefreshItemDragUI(int sourceIndex, int targetIndex)
		{
			base.RefreshItemDragUI(sourceIndex, targetIndex);
			if (!_selectedItemIndexList.Contains(sourceIndex) || _selectedItemIndexList.Count <= 1)
				return;
			foreach (var item in _itemVEs)
				if (_selectedItemIndexList.Contains(GetItemIndex(item)))
					item.DisplayAsDrag();
		}

		public virtual BSortManipulator<TItemVE> CreateSortManipulator() =>
			new SimpleSortManipulator(this);

		// PROTECT METHOD: --------------------------------------------------------------------

		protected virtual void RefreshSelectedColor()
		{
			foreach (var item in _itemVEs)
				RefreshItemSelectedStyle(item);
		}

		protected abstract TItem Clone(TItem item);

		private class SimpleSortManipulator : BSortManipulator<TItemVE>
		{
			// CONSTRUCTOR: ---------------------------------------------------------------------

			public SimpleSortManipulator(IListVE list) : base(list, false, true) { }
		}
	}
}
