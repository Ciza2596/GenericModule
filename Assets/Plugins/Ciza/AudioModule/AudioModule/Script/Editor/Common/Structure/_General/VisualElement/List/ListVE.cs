using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

namespace CizaAudioModule.Editor
{
	public class ListVE : BListVE<ItemVE>, BBoxVE.IContent
	{
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
		protected readonly VisualElement _foot = new VisualElement();

		protected virtual string[] USSPaths => new[] { "List" };

		#region Class

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

		protected virtual Texture2D DropDownIcon => new TriangleDownIcon(ColorTheme.Type.TextLight).Texture;
		protected virtual Texture2D DropRightIcon => new TriangleRightIcon(ColorTheme.Type.TextLight).Texture;

		protected virtual Texture2D SearchIcon => new SearchIcon(ColorTheme.Type.TextLight).Texture;
		protected virtual Texture2D ClearSearchIcon => new CrossMarkIcon(ColorTheme.Type.TextLight).Texture;

		protected virtual Texture2D AddItemIcon => new DuplicateIcon(ColorTheme.Type.TextLight).Texture;


		[field: NonSerialized]
		protected virtual SerializedProperty ListProperty { get; set; }

		protected virtual string SearchingText
		{
			get => SessionState.GetString(SearchingTextKey, string.Empty);
			set => SessionState.SetString(SearchingTextKey, value);
		}

		protected virtual bool IsSearch => SearchingText.CheckHasValue();

		// PUBLIC VARIABLE: ---------------------------------------------------------------------

		public virtual string DataId => GetType().Name;
		public virtual string DataIdKey => DataId + ".";

		public virtual string RootKey => DataIdKey + ListProperty.serializedObject.targetObject.GetInstanceID() + ".";
		public virtual string SearchingTextKey => RootKey + nameof(SearchingText);

		public virtual VisualElement Body => this;

		public virtual bool IsAllowReordering => true;
		public virtual bool IsAllowDisable => false;
		public virtual bool IsAllowDuplicate => true;
		public virtual bool IsAllowDelete => true;
		public virtual bool IsAllowContextMenu => true;
		public virtual bool IsAllowCopyPaste => true;

		public virtual bool IsAllowGroupCollapse => IsElementIsClass;
		public virtual bool IsAllowGroupExpand => IsElementIsClass;

		[field: NonSerialized]
		public virtual ItemSortManipulator SortManipulator { get; protected set; }

		[field: NonSerialized]
		public virtual SerializedProperty ItemsProperty { get; protected set; }

		public virtual int Count => ItemsProperty.arraySize;

		[field: NonSerialized]
		public virtual Type ItemType { get; protected set; }

		[field: NonSerialized]
		public virtual bool IsElementIsClass { get; protected set; }


		// CONSTRUCTOR: ---------------------------------------------------------------------------

		[Preserve]
		public ListVE(SerializedProperty listProperty)
		{
			ListProperty = listProperty;

			Add(_head);
			Add(_body);
			Add(_foot);
		}

		// PUBLIC METHODS: ------------------------------------------------------------------------

		public virtual void SetListProperty(SerializedProperty listProperty)
		{
			ListProperty = listProperty;
			ItemsProperty = CreateItemsProperty();
			DerivedInitializeItemType();
		}

		public virtual void SetIsShowHead(bool isShow)
		{
			if (isShow)
			{
				if (!Contains(_head))
					Add(_head);
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
			for (int i = 0; i < itemsProperty.arraySize; i++)
				SpawnItem(i, itemsProperty.GetArrayElementAtIndex(i));
			RemoveItems(itemsProperty.arraySize);
			RefreshSearchButton(IsSearch);
		}

		public virtual void InsertNewItemAtLast(Type type) =>
			InsertNewItem(Count, type);

		public virtual void InsertNewItem(int index, Type type) =>
			InsertItem(index, TypeUtils.CreateInstance(type));

		public virtual void InsertItem(int index, object value)
		{
			if (index < 0) return;

			ListProperty.serializedObject.Update();

			ItemsProperty.InsertArrayElementAtIndex(index);
			ItemsProperty.GetArrayElementAtIndex(index).SetValue(value);

			Refresh();
			RefreshIsExpandWhenInsert(index);
			_itemVEs[index].SetIsExpand(true);
		}

		public virtual void DuplicateItem(int index)
		{
			if (index < 0) return;

			ListProperty.serializedObject.Update();

			var source = ItemsProperty.GetArrayElementAtIndex(index).GetValue();
			if (source == null) return;

			var insertIndex = index + 1;
			ItemsProperty.InsertArrayElementAtIndex(insertIndex);
			var newObj = ItemsProperty.GetArrayElementAtIndex(insertIndex);

			CopyPasteUtils.Duplicate(newObj, source);
			SerializationUtils.ApplyUnregisteredSerialization(ListProperty.serializedObject);

			Refresh();
			RefreshIsExpandWhenInsert(insertIndex);
			_itemVEs[insertIndex].SetIsExpand(true);
		}

		public virtual void DeleteItem(int index)
		{
			if (index < 0) return;

			ListProperty.serializedObject.Update();

			ItemsProperty.DeleteArrayElementAtIndex(index);
			SerializationUtils.ApplyUnregisteredSerialization(ListProperty.serializedObject);

			Refresh();
		}

		public override void MoveItems(int sourceIndex, int destinationIndex)
		{
			destinationIndex = GetDestinationIndex(ItemsProperty, destinationIndex);
			var sourceIsExpand = _itemVEs[sourceIndex].IsExpand;
			if (destinationIndex > sourceIndex)
				for (int i = sourceIndex; i < destinationIndex; i++)
					_itemVEs[i].SetIsExpand(_itemVEs[i + 1].IsExpand);
			else if (destinationIndex < sourceIndex)
				for (int i = sourceIndex; i > destinationIndex; i--)
					_itemVEs[i].SetIsExpand(_itemVEs[i - 1].IsExpand);

			MoveItems(ItemsProperty, sourceIndex, destinationIndex);
			Refresh();
			_itemVEs[destinationIndex].SetIsExpand(sourceIsExpand);
		}

		public virtual void CollapseAll() =>
			SetIsExpandAll(false);

		public virtual void ExpandAll() =>
			SetIsExpandAll(true);


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

			foreach (var c in FootClasses)
				_foot.AddToClassList(c);
			SetupFoot();
		}

		protected virtual void DerivedInitializeItemType()
		{
			ItemType = TypeUtils.GetGenericTypes(ItemsProperty)[0];
			IsElementIsClass = ItemType.CheckIsClassWithoutString();
		}

		#region Create VE

		protected virtual SerializedProperty CreateItemsProperty() =>
			ListProperty;

		protected virtual ItemSortManipulator CreateSortManipulator() =>
			new ItemSortManipulator(this);

		protected virtual ItemVE CreateItem(SerializedProperty itemProperty)
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
				Refresh();
			});
			_head.Add(_searchTextField);
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
			for (var i = 0; i < _body.childCount; i++)
				_itemVEs[i].SetIsExpand(isExpand);
		}

		protected virtual void RefreshSearchButton(bool isSearch) =>
			_clearSearchButton.SetIsVisible(isSearch);

		protected virtual void SpawnItem(int index, SerializedProperty itemProperty, bool isAllowReordering = true, bool isAllowDisable = true, bool isAllowDuplicate = true, bool isAllowDelete = true, bool isAllowCopyPaste = true)
		{
			if (index >= _itemVEs.Count)
			{
				var itemVE = CreateItem(itemProperty);
				_itemVEs.Add(itemVE);
				_body.Add(itemVE);
			}

			_itemVEs[index].Refresh(index, itemProperty, isAllowReordering, isAllowDisable, isAllowDuplicate, isAllowDelete, isAllowCopyPaste);
			var isVisible = !IsSearch || (IsSearch && _itemVEs[index].Title.ToLower().Contains(SearchingText.ToLower()));
			_itemVEs[index].SetIsVisible(isVisible);
		}

		protected virtual void RemoveItems(int finalIndex)
		{
			var itemsCount = _itemVEs.Count;
			for (var i = finalIndex; i < itemsCount; i++)
			{
				_itemVEs.RemoveAt(_itemVEs.Count - 1);
				_body.RemoveAt(_body.childCount - 1);
			}
		}

		protected virtual void RefreshIsExpandWhenInsert(int insertIndex)
		{
			if (insertIndex == ItemsProperty.arraySize - 1)
				return;

			for (int i = ItemsProperty.arraySize - 1; i > insertIndex; i--)
			{
				var previousIndex = i - 1;
				if (i - 1 < 0)
					continue;
				_itemVEs[i].SetIsExpand(_itemVEs[previousIndex].IsExpand);
			}
		}
	}
}