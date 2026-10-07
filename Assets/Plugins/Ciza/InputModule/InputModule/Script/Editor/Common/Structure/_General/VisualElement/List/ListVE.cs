using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

namespace CizaInputModule.Editor
{
	public class ListVE : BListVE<ItemVE>, BBoxVE.IContent
	{
		// CONST & STATIC: -----------------------------------------------------------------------

		public static readonly IIcon DEFAULT_DROP_DOWN_ICON = new TriangleDownIcon(ColorTheme.Type.TextLight);
		public static readonly IIcon DEFAULT_DROP_RIGHT_ICON = new TriangleRightIcon(ColorTheme.Type.TextLight);
		public static readonly IIcon DEFAULT_SEARCH_ICON = new SearchIcon(ColorTheme.Type.TextLight);
		public static readonly IIcon DEFAULT_CLEAR_SEARCH_ICON = new CrossMarkIcon(ColorTheme.Type.TextLight);
		public static readonly IIcon DEFAULT_ADD_ITEM_ICON = new DuplicateIcon(ColorTheme.Type.TextLight);

		// VARIABLE: -----------------------------------------------------------------------------

		#region Head

		[NonSerialized]
		protected readonly VisualElement _head = new VisualElement();

		[NonSerialized]
		protected readonly Button _clearSearchButton = new Button();

		[NonSerialized]
		protected readonly TextField _searchTextField = new TextField();

		#endregion

		[NonSerialized]
		protected readonly VisualElement _body = new VisualElement();

		[NonSerialized]
		protected readonly PageBarVE _pageBarVE = new PageBarVE();

		[NonSerialized]
		protected readonly VisualElement _foot = new VisualElement();

		[NonSerialized]
		protected readonly List<int> _filteredItemIndexList = new List<int>();

		[NonSerialized]
		protected readonly List<int> _visibleItemIndexList = new List<int>();

		[NonSerialized]
		protected readonly List<int> _selectedItemIndexList = new List<int>();

		[NonSerialized]
		protected int _selectedItemIndexBaseline = -1;

		protected virtual string[] USSPaths => new[] { "List" };

		#region Class

		protected override string[] DropIndicatorClasses => new[] { "list-drop-indicator" };
		protected virtual string[] RootClasses => new[] { "list" };
		protected virtual string[] HeadClasses => new[] { "list-head" };
		protected virtual string[] BodyClasses => new[] { "list-body" };
		protected virtual string[] FootClasses => new[] { "list-foot" };

		#endregion

		#region Tip

		protected virtual string CollapseAllTip => "Collapse All";
		protected virtual string ExpandAllTip => "Expand All";
		protected virtual string SearchTip => "Search";
		protected virtual string AddNewItemTip => "Add new item...";

		#endregion

		protected virtual Texture2D DropDownIcon => DEFAULT_DROP_DOWN_ICON.Texture;
		protected virtual Texture2D DropRightIcon => DEFAULT_DROP_RIGHT_ICON.Texture;

		protected virtual Texture2D SearchIcon => DEFAULT_SEARCH_ICON.Texture;
		protected virtual Texture2D ClearSearchIcon => DEFAULT_CLEAR_SEARCH_ICON.Texture;

		protected virtual Texture2D AddItemIcon => DEFAULT_ADD_ITEM_ICON.Texture;


		[field: NonSerialized]
		protected virtual SerializedProperty ListProperty { get; set; }

		protected virtual string SearchingText
		{
			get => SessionState.GetString(SearchingTextKey, string.Empty);
			set => SessionState.SetString(SearchingTextKey, value);
		}

		protected virtual bool IsSearch => SearchingText.CheckHasValue();

		protected virtual int CurrentPage
		{
			get => SessionState.GetInt(CurrentPageKey, 1);
			set => SessionState.SetInt(CurrentPageKey, value);
		}
		
		protected virtual int GetGlobalItemIndex(int visibleSlotIndex)
		{
			if (visibleSlotIndex < 0 || visibleSlotIndex >= _visibleItemIndexList.Count)
				return -1;

			return _visibleItemIndexList[visibleSlotIndex];
		}

		protected virtual int GetGlobalInsertIndex(int visibleSlotIndex)
		{
			if (visibleSlotIndex < 0 || visibleSlotIndex > _visibleItemIndexList.Count || _visibleItemIndexList.Count == 0)
				return -1;

			return visibleSlotIndex < _visibleItemIndexList.Count ? _visibleItemIndexList[visibleSlotIndex] : _visibleItemIndexList[^1] + 1;
		}

		protected virtual int GetVisibleSlotIndex(int globalItemIndex) =>
			_visibleItemIndexList.IndexOf(globalItemIndex);

		protected virtual int GetVisibleInsertSlotIndex(int globalInsertIndex)
		{
			var slotIndex = _visibleItemIndexList.IndexOf(globalInsertIndex);
			if (slotIndex >= 0)
				return slotIndex;

			return _visibleItemIndexList.Count > 0 && globalInsertIndex == _visibleItemIndexList[^1] + 1 ? _visibleItemIndexList.Count : -1;
		}
		
		protected virtual int[] GetDraggedSelectedItemIndices(int sourceIndex)
		{
			if (!_selectedItemIndexList.Contains(sourceIndex))
				return Array.Empty<int>();

			return _selectedItemIndexList.Where(i => i >= 0 && i < Count).Distinct().OrderBy(i => i).ToArray();
		}
		
		protected virtual int GetMovedIndex(int itemIndex, int sourceIndex, int destinationIndex)
		{
			if (itemIndex < 0)
				return itemIndex;
			if (itemIndex == sourceIndex)
				return destinationIndex;
			if (sourceIndex < destinationIndex && itemIndex > sourceIndex && itemIndex <= destinationIndex)
				return itemIndex - 1;
			if (destinationIndex < sourceIndex && itemIndex >= destinationIndex && itemIndex < sourceIndex)
				return itemIndex + 1;
			return itemIndex;
		}

		// PUBLIC VARIABLE: ---------------------------------------------------------------------

		public virtual bool IsAutoRefresh { get; }

		public virtual string DataId => GetType().Name;
		public virtual string DataIdKey => DataId + ".";

		public virtual string RootKey => DataIdKey + ListProperty.serializedObject.targetObject.GetInstanceID() + "." + ListProperty.propertyPath + ".";
		public virtual string SearchingTextKey => RootKey + nameof(SearchingText);
		
		public virtual string CurrentPageKey => RootKey + nameof(CurrentPage);

		public virtual VisualElement Body => this;
		
		public virtual bool IsPage => true;

		public virtual bool IsAllowSelection => true;
		public virtual bool IsAllowReordering => true;
		public virtual bool IsAllowDisable => false;
		public virtual bool IsAllowDuplicate => true;
		public virtual bool IsAllowDelete => true;
		public virtual bool IsAllowContextMenu => true;
		public virtual bool IsAllowCopyPaste => true;

		public virtual bool IsAllowGroupCollapse => IsElementClass;
		public virtual bool IsAllowGroupExpand => IsElementClass;

		[field: NonSerialized]
		public virtual ItemSortManipulator SortManipulator { get; protected set; }

		[field: NonSerialized]
		public virtual SerializedProperty ItemsProperty { get; protected set; }
		
		public virtual int Count => ItemsProperty.arraySize;
		
		public virtual int CountPerPage => 20;
		
		public virtual int CurrentPageCount { get; protected set; }
		
		public virtual int TotalPageCount => Count / CountPerPage + (Count % CountPerPage > 0 ? 1 : 0);

		[field: NonSerialized]
		public virtual Type ItemType { get; protected set; }

		[field: NonSerialized]
		public virtual bool IsElementClass { get; protected set; }


		public virtual int[] SelectedItemIndexList => _selectedItemIndexList.ToArray();
		public virtual bool IsSelectAll => Count == _selectedItemIndexList.Count;
		public virtual bool IsUnselectAll => _selectedItemIndexList.Count == 0;
		
		
		public virtual string GetItemTitle(int itemIndex, SerializedProperty itemProperty) => $"Element {itemIndex}";

		public override int GetItemIndexOf(VisualElement item)
		{
			var slotIndex = base.GetItemIndexOf(item);
			return GetGlobalItemIndex(slotIndex);
		}

		public override int ClosestItemIndex(float cursorY)
		{
			var slotIndex = base.ClosestItemIndex(cursorY);
			return GetGlobalInsertIndex(slotIndex);
		}

		// CONSTRUCTOR: ---------------------------------------------------------------------------

		[Preserve]
		public ListVE(SerializedProperty listProperty, bool isAutoRefresh)
		{
			ListProperty = listProperty;
			IsAutoRefresh = isAutoRefresh;

			Add(_pageBarVE);
			Add(_head);
			Add(_body);
			Add(_foot);
		}

		// PUBLIC METHODS: ------------------------------------------------------------------------

		public virtual void SetListProperty(SerializedProperty listProperty)
		{
			var isDifferentProperty = ListProperty == null ||
			                          ListProperty.serializedObject.targetObject != listProperty.serializedObject.targetObject ||
			                          ListProperty.propertyPath != listProperty.propertyPath;

			ListProperty = listProperty;
			RefreshItemsProperty();

			if (!IsInitialized || !isDifferentProperty)
				return;

			_searchTextField.SetValueWithoutNotify(SearchingText);
			ClearItemVEs();
		}

		public virtual void SetIsShowHead(bool isShow)
		{
			if (isShow)
			{
				if (!Contains(_head))
					Insert(0, _head);
			}
			else
			{
				if (Contains(_head))
					Remove(_head);
			}
		}

		public override void Refresh()
		{
			var itemsProperty = ItemsProperty;
			var index = 0;
			var isItemPropertyDirty = false;
			
			for (int i = 0; i < itemsProperty.arraySize; i++)
			{
				var itemProperty = itemsProperty.GetArrayElementAtIndex(index);
				if (itemProperty.GetValue() == null && IsElementClass)
					if (TypeUtils.TryCreateInstance(SerializationUtils.GetType(itemProperty, false), out var instance))
						itemProperty.SetValue(instance);
					else
					{
						itemsProperty.DeleteArrayElementAtIndex(index);
						continue;
					}

				isItemPropertyDirty |= CheckHasSetupSerializedProperty(index, itemProperty);
				index++;
			}

			if (isItemPropertyDirty)
				SerializationUtils.ApplyUnregisteredSerialization(itemsProperty.serializedObject);

			_filteredItemIndexList.Clear();
			for (var itemIndex = 0; itemIndex < itemsProperty.arraySize; itemIndex++)
			{
				var itemProperty = itemsProperty.GetArrayElementAtIndex(itemIndex);
				if (!IsSearch)
				{
					_filteredItemIndexList.Add(itemIndex);
					continue;
				}

				var itemTitle = GetItemTitle(itemIndex, itemProperty) ?? string.Empty;
				if (itemTitle.Contains(SearchingText, StringComparison.OrdinalIgnoreCase))
					_filteredItemIndexList.Add(itemIndex);
			}

			NormalizeSelection();
			var filteredCount = _filteredItemIndexList.Count;
			var countPerPage = Math.Max(1, CountPerPage);
			CurrentPageCount = IsPage ? Math.Max(1, (filteredCount + countPerPage - 1) / countPerPage) : 1;
			CurrentPage = Mathf.Clamp(CurrentPage, 1, CurrentPageCount);
			var startIndex = IsPage ? (CurrentPage - 1) * countPerPage : 0;
			var visibleCount = IsPage ? Math.Min(countPerPage, filteredCount - startIndex) : filteredCount;

			_visibleItemIndexList.Clear();
			for (var i = 0; i < visibleCount; i++)
				_visibleItemIndexList.Add(_filteredItemIndexList[startIndex + i]);

			RefreshVisibleItems();
			
			RefreshSearchButton(IsSearch);
			RefreshPageBar(CurrentPage, CurrentPageCount, IsPage && filteredCount > countPerPage);
		}

		public override void RefreshItemDragUI(int sourceIndex, int targetIndex)
		{
			var selectedIndices = GetDraggedSelectedItemIndices(sourceIndex);
			var sourceSlotIndex = selectedIndices.Length > 1 ? -1 : GetVisibleSlotIndex(sourceIndex);
			var targetSlotIndex = GetVisibleInsertSlotIndex(targetIndex);
			base.RefreshItemDragUI(sourceSlotIndex, targetSlotIndex);

			if (selectedIndices.Length <= 1)
				return;

			foreach (var selectedIndex in selectedIndices)
			{
				var selectedSlotIndex = GetVisibleSlotIndex(selectedIndex);
				if (selectedSlotIndex >= 0)
					_itemVEs[selectedSlotIndex].DisplayAsDrag();
			}
		}

		public virtual void OnItemSelected(int index, EventModifiers modifier)
		{
			if (!IsAllowSelection)
				return;

			if (modifier.CheckIsShift())
			{
				if (_selectedItemIndexList.Count == 0)
				{
					_selectedItemIndexBaseline = index;
					_selectedItemIndexList.Add(index);
				}
				else
				{
					var startIndex = Math.Min(index, _selectedItemIndexBaseline);
					var endIndex = Math.Max(index, _selectedItemIndexBaseline);
					_selectedItemIndexList.Clear();
					for (var i = startIndex; i <= endIndex; i++)
						_selectedItemIndexList.Add(i);
				}
			}
			else if (modifier.CheckIsCtrl())
			{
				_selectedItemIndexBaseline = index;
				if (!_selectedItemIndexList.Contains(index))
					_selectedItemIndexList.Add(index);
				else
					_selectedItemIndexList.Remove(index);
			}
			else
			{
				var isAllClear = _selectedItemIndexList.Count == 1 && _selectedItemIndexList[0] == index;
				_selectedItemIndexList.Clear();
				_selectedItemIndexBaseline = isAllClear ? -1 : index;
				if (!isAllClear)
					_selectedItemIndexList.Add(index);
			}

			RefreshSelectedColor();
		}

		public virtual void SelectAllItem()
		{
			_selectedItemIndexBaseline = 0;
			_selectedItemIndexList.Clear();
			_selectedItemIndexList.AddRange(Enumerable.Range(0, Count));

			RefreshSelectedColor();
		}

		public virtual void UnselectAllItem()
		{
			_selectedItemIndexBaseline = 0;
			_selectedItemIndexList.Clear();
			RefreshSelectedColor();
		}

		public virtual Array GetSelectedItemsCopy()
		{
			var indices = SelectedItemIndexList.Where(i => i >= 0 && i < Count).OrderBy(i => i).ToArray();
			var result = Array.CreateInstance(ItemType, indices.Length);

			for (var i = 0; i < indices.Length; i++)
				result.SetValue(ItemsProperty.GetArrayElementAtIndex(indices[i]).GetValue(), i);

			return result;
		}

		public virtual void InsertNewItemAtLast(Type type) =>
			InsertNewItem(Count, type);

		public virtual void InsertNewItem(int index, Type type)
		{
			if (TypeUtils.TryCreateInstance(type, out var value))
				InsertItem(index, value);
		}

		public virtual void InsertItem(int index, object value)
		{
			var values = Array.CreateInstance(ItemType, 1);
			values.SetValue(value, 0);
			InsertItems(index, values);
		}

		public virtual void InsertItems(int index, Array values)
		{
			if (values == null || values.Length == 0)
				return;

			index = Math.Clamp(index, 0, Count);
			var expandedStates = CaptureExpandedStates();

			ListProperty.serializedObject.Update();

			for (var i = 0; i < values.Length; i++)
			{
				var insertIndex = index + i;
				ItemsProperty.InsertArrayElementAtIndex(insertIndex);
				ItemsProperty.GetArrayElementAtIndex(insertIndex).SetValue(values.GetValue(i));
			}

			SerializationUtils.ApplyUnregisteredSerialization(ListProperty.serializedObject);
			RestoreExpandedStatesAfterInsert(expandedStates, index, values.Length, true);
			AdjustSelectionAfterInsert(index, values.Length);
			SetPageForItem(index + values.Length - 1);

			Refresh();
		}

		public virtual void DuplicateItem(int index)
		{
			if (index < 0 || index >= Count)
				return;

			var expandedStates = CaptureExpandedStates();

			ListProperty.serializedObject.Update();

			var source = ItemsProperty.GetArrayElementAtIndex(index).GetValue();
			var insertIndex = index + 1;
			ItemsProperty.InsertArrayElementAtIndex(insertIndex);
			var newObj = ItemsProperty.GetArrayElementAtIndex(insertIndex);

			SerializationUtils.Duplicate(newObj, source);

			SerializationUtils.ApplyUnregisteredSerialization(ListProperty.serializedObject);
			RestoreExpandedStatesAfterInsert(expandedStates, insertIndex, 1, true);
			AdjustSelectionAfterInsert(insertIndex, 1);
			SetPageForItem(insertIndex);
			Refresh();
		}

		public virtual void DeleteItem(int index)
		{
			if (index < 0 || index >= Count)
				return;

			var expandedStates = CaptureExpandedStates();

			ListProperty.serializedObject.Update();

			ItemsProperty.DeleteArrayElementAtIndex(index);
			SerializationUtils.ApplyUnregisteredSerialization(ListProperty.serializedObject);
			RestoreExpandedStatesAfterDelete(expandedStates, index);
			AdjustSelectionAfterDelete(index);

			if (IsAutoRefresh)
				Refresh();
		}

		public override void MoveItem(int sourceIndex, int destinationIndex)
		{
			if (IsSearch || sourceIndex < 0 || sourceIndex >= Count)
				return;

			destinationIndex = Math.Clamp(destinationIndex, 0, Count - 1);
			var selectedIndices = GetDraggedSelectedItemIndices(sourceIndex);
			if (selectedIndices.Length > 1)
			{
				MoveSelectedItems(sourceIndex, destinationIndex, selectedIndices);
				return;
			}

			if (sourceIndex == destinationIndex)
			{
				SetPageForItem(sourceIndex);
				Refresh();
				return;
			}

			var expandedStates = CaptureExpandedStates().ToList();
			var sourceIsExpand = expandedStates[sourceIndex];

			expandedStates.RemoveAt(sourceIndex);
			expandedStates.Insert(destinationIndex, sourceIsExpand);
			AdjustSelectionAfterMove(sourceIndex, destinationIndex);
			MoveItem(ItemsProperty, sourceIndex, destinationIndex);
			RestoreExpandedStates(expandedStates);
			SetPageForItem(destinationIndex);
			Refresh();
		}

		public virtual void CollapseAll() =>
			SetIsExpandAll(false);

		public virtual void ExpandAll() =>
			SetIsExpandAll(true);

		public virtual void SetPage(int page)
		{
			page = Mathf.Clamp(page, 1, Math.Max(1, CurrentPageCount));
			if (SortManipulator?.IsDragging == true)
			{
				RefreshItemDragUI(-1, -1);
				if (page != CurrentPage)
					SortManipulator.PreserveDragOnTargetRefresh();
			}
			
			if (page == CurrentPage)
			{
				_pageBarVE.SetPage(page, false);
				return;
			}
			
			CurrentPage = page;
			Refresh();
		}
		

		// PROTECTED VIRTUAL METHODS: -------------------------------------------------------------

		protected override void DerivedInitialize()
		{
			foreach (var sheet in StyleSheetUtils.GetStyleSheets(USSPaths))
				styleSheets.Add(sheet);

			SortManipulator = CreateSortManipulator();
			SetListProperty(ListProperty);

			foreach (var c in RootClasses)
				AddToClassList(c);

			foreach (var c in HeadClasses)
				_head.AddToClassList(c);
			SetupHead();

			foreach (var c in BodyClasses)
				_body.AddToClassList(c);

			SetupPageBar();

			foreach (var c in FootClasses)
				_foot.AddToClassList(c);
			SetupFoot();
		}

		protected virtual void RefreshItemsProperty()
		{
			ItemsProperty = CreateItemsProperty();
			ItemType = SerializationUtils.GetElementTypes(ItemsProperty)[0];
			IsElementClass = TypeUtils.CheckIsClassWithoutStringOrUnityObjSubclass(ItemType);
		}

		#region Create VE

		protected virtual SerializedProperty CreateItemsProperty() =>
			ListProperty;

		protected virtual ItemSortManipulator CreateSortManipulator() =>
			new ItemSortManipulator(this);

		protected virtual ItemVE CreateItemVE(SerializedProperty itemProperty)
		{
			var itemVE = new ItemVE(this, itemProperty);
			itemVE.Initialize();
			return itemVE;
		}

		#endregion

		#region Setup

		protected virtual void SetupHead()
		{
			if (IsAllowGroupCollapse)
			{
				var collapseAllButton = new Button(CollapseAll) { tooltip = CollapseAllTip };
				collapseAllButton.Add(new Image() { image = DropRightIcon });
				_head.Add(collapseAllButton);
			}

			if (IsAllowGroupExpand)
			{
				var expandAllButton = new Button(ExpandAll) { tooltip = ExpandAllTip };
				expandAllButton.Add(new Image() { image = DropDownIcon });
				_head.Add(expandAllButton);
			}

			_searchTextField.Add(new Image() { image = SearchIcon });

			_clearSearchButton.clicked += () => { _searchTextField.value = string.Empty; };
			_clearSearchButton.Add(new Image() { image = ClearSearchIcon });
			_searchTextField.Add(_clearSearchButton);

			_searchTextField.tooltip = SearchTip;
			_searchTextField.value = SearchingText;
			_searchTextField.RegisterValueChangedCallback(changeEvent =>
			{
				SearchingText = changeEvent.newValue;
				CurrentPage = 1;
				Refresh();
			});
			_head.Add(_searchTextField);
		}

		protected virtual void SetupPageBar()
		{
			_pageBarVE.Initialize(CurrentPage, CurrentPageCount, _ => SortManipulator.IsDragging);
			_pageBarVE.OnFirstPage += () => { SetPage(1); };
			_pageBarVE.OnPreviousPage += () => { SetPage(CurrentPage - 1); };
			_pageBarVE.OnNextPage += () => { SetPage(CurrentPage + 1); };
			_pageBarVE.OnLastPage += () => { SetPage(CurrentPageCount); };
			_pageBarVE.OnSetPage += SetPage;
		}

		protected virtual void SetupFoot()
		{
			var addButton = new Button();
			addButton.Add(new Image { image = AddItemIcon });
			addButton.Add(new Label { text = AddNewItemTip });
			addButton.clicked += () => { InsertNewItem(ItemsProperty.arraySize, ItemType); };

			_foot.Add(addButton);
		}

		#endregion

		protected virtual void SetIsExpandAll(bool isExpand)
		{
			for (var i = 0; i < Count; i++)
				ItemsProperty.GetArrayElementAtIndex(i).isExpanded = isExpand;

			foreach (var itemVE in _itemVEs)
				itemVE.SetIsExpand(isExpand);
		}

		protected virtual void RefreshSearchButton(bool isSearch) =>
			_clearSearchButton.SetIsVisible(isSearch);
		

		protected virtual void RefreshSelectedColor()
		{
			foreach (var itemVE in _itemVEs)
				itemVE.RefreshSelectedStyle();
		}

		protected virtual bool CheckHasSetupSerializedProperty(int itemIndex, SerializedProperty itemProperty) =>
			false;

		protected virtual void RefreshVisibleItems()
		{
			var canReuseItemVEs = _itemVEs.Count == _visibleItemIndexList.Count;
			for (var i = 0; canReuseItemVEs && i < _itemVEs.Count; i++)
				canReuseItemVEs = _itemVEs[i].Index == _visibleItemIndexList[i];

			if (!canReuseItemVEs)
				ClearItemVEs();

			for (var slotIndex = 0; slotIndex < _visibleItemIndexList.Count; slotIndex++)
			{
				var itemIndex = _visibleItemIndexList[slotIndex];
				var itemProperty = ItemsProperty.GetArrayElementAtIndex(itemIndex);
				if (slotIndex >= _itemVEs.Count)
				{
					var itemVE = CreateItemVE(itemProperty);
					_itemVEs.Add(itemVE);
					_body.Add(itemVE);
				}

				_itemVEs[slotIndex].Refresh(itemIndex, itemProperty, !IsSearch, true, true, true, true);
			}
		}

		protected virtual void ClearItemVEs()
		{
			_itemVEs.Clear();
			_body.Clear();
		}

		protected virtual void RefreshPageBar(int page, int pageCount, bool isVisible) =>
			_pageBarVE.Refresh(page, pageCount, isVisible);

		protected virtual bool[] CaptureExpandedStates()
		{
			var result = new bool[Count];
			for (var i = 0; i < Count; i++)
				result[i] = ItemsProperty.GetArrayElementAtIndex(i).isExpanded;
			return result;
		}

		protected virtual void RestoreExpandedStates(IEnumerable<bool> expandedStates)
		{
			var index = 0;
			foreach (var isExpanded in expandedStates)
			{
				if (index >= Count)
					break;
				ItemsProperty.GetArrayElementAtIndex(index).isExpanded = isExpanded;
				index++;
			}
		}

		protected virtual void RestoreExpandedStatesAfterInsert(bool[] oldStates, int insertIndex, int insertCount, bool insertedIsExpanded)
		{
			for (var i = 0; i < Count; i++)
			{
				if (i < insertIndex)
					ItemsProperty.GetArrayElementAtIndex(i).isExpanded = oldStates[i];
				else if (i < insertIndex + insertCount)
					ItemsProperty.GetArrayElementAtIndex(i).isExpanded = insertedIsExpanded;
				else
					ItemsProperty.GetArrayElementAtIndex(i).isExpanded = oldStates[i - insertCount];
			}
		}

		protected virtual void RestoreExpandedStatesAfterDelete(bool[] oldStates, int deletedIndex)
		{
			for (var i = 0; i < Count; i++)
				ItemsProperty.GetArrayElementAtIndex(i).isExpanded = oldStates[i < deletedIndex ? i : i + 1];
		}

		protected virtual void SetPageForItem(int itemIndex)
		{
			if (!IsPage || IsSearch || itemIndex < 0)
				return;
			CurrentPage = itemIndex / Math.Max(1, CountPerPage) + 1;
		}

		protected virtual void NormalizeSelection()
		{
			_selectedItemIndexList.RemoveAll(i => i < 0 || i >= Count);
			if (_selectedItemIndexBaseline < 0 || _selectedItemIndexBaseline >= Count)
				_selectedItemIndexBaseline = -1;
		}

		protected virtual void AdjustSelectionAfterInsert(int insertIndex, int insertCount)
		{
			for (var i = 0; i < _selectedItemIndexList.Count; i++)
				if (_selectedItemIndexList[i] >= insertIndex)
					_selectedItemIndexList[i] += insertCount;

			if (_selectedItemIndexBaseline >= insertIndex)
				_selectedItemIndexBaseline += insertCount;
		}

		protected virtual void AdjustSelectionAfterDelete(int deletedIndex)
		{
			for (var i = _selectedItemIndexList.Count - 1; i >= 0; i--)
			{
				if (_selectedItemIndexList[i] == deletedIndex)
					_selectedItemIndexList.RemoveAt(i);
				else if (_selectedItemIndexList[i] > deletedIndex)
					_selectedItemIndexList[i]--;
			}

			if (_selectedItemIndexBaseline == deletedIndex)
				_selectedItemIndexBaseline = -1;
			else if (_selectedItemIndexBaseline > deletedIndex)
				_selectedItemIndexBaseline--;
		}

		protected virtual void AdjustSelectionAfterMove(int sourceIndex, int destinationIndex)
		{
			for (var i = 0; i < _selectedItemIndexList.Count; i++)
				_selectedItemIndexList[i] = GetMovedIndex(_selectedItemIndexList[i], sourceIndex, destinationIndex);
			_selectedItemIndexBaseline = GetMovedIndex(_selectedItemIndexBaseline, sourceIndex, destinationIndex);
		}

		protected virtual void MoveSelectedItems(int sourceIndex, int destinationIndex, int[] selectedIndices)
		{
			var insertIndex = destinationIndex >= sourceIndex ? destinationIndex + 1 : destinationIndex;
			var selectedIndexSet = new HashSet<int>(selectedIndices);
			var destinationBlockIndex = Math.Clamp(insertIndex - selectedIndices.Count(i => i < insertIndex), 0, Count - selectedIndices.Length);
			var desiredOrder = Enumerable.Range(0, Count).Where(i => !selectedIndexSet.Contains(i)).ToList();
			desiredOrder.InsertRange(destinationBlockIndex, selectedIndices);

			if (desiredOrder.Select((itemIndex, index) => itemIndex == index).All(isSame => isSame))
			{
				SetPageForItem(selectedIndices[0]);
				Refresh();
				return;
			}

			var expandedStates = CaptureExpandedStates();
			var currentOrder = Enumerable.Range(0, Count).ToList();
			ItemsProperty.serializedObject.Update();

			for (var destination = 0; destination < desiredOrder.Count; destination++)
			{
				var source = currentOrder.IndexOf(desiredOrder[destination]);
				if (source == destination)
					continue;

				ItemsProperty.MoveArrayElement(source, destination);
				var movedItemIndex = currentOrder[source];
				currentOrder.RemoveAt(source);
				currentOrder.Insert(destination, movedItemIndex);
			}

			SerializationUtils.ApplyUnregisteredSerialization(ItemsProperty.serializedObject);
			RestoreExpandedStates(desiredOrder.Select(itemIndex => expandedStates[itemIndex]));

			_selectedItemIndexList.Clear();
			for (var i = 0; i < selectedIndices.Length; i++)
				_selectedItemIndexList.Add(destinationBlockIndex + i);

			_selectedItemIndexBaseline = _selectedItemIndexBaseline >= 0 ? desiredOrder.IndexOf(_selectedItemIndexBaseline) : -1;
			SetPageForItem(_selectedItemIndexList[0]);
			Refresh();
		}
	}
}
