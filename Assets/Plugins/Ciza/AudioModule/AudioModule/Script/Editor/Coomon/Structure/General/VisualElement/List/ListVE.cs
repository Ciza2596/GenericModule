using System;
using System.Linq;
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

		#region Name

		protected virtual string[] HeadClasses => new[] { "list-head" };
		protected virtual string[] BodyClasses => new[] { "list-body" };
		protected virtual string[] FootClasses => new[] { "list-foot" };

		#endregion

		protected virtual Texture2D DropDownIcon => new TriangleDownIcon(ColorTheme.Type.TextLight).Texture;
		protected virtual Texture2D DropRightIcon => new TriangleRightIcon(ColorTheme.Type.TextLight).Texture;

		protected virtual Texture2D SearchIcon => new SearchIcon(ColorTheme.Type.TextLight).Texture;
		protected virtual Texture2D ClearSearchIcon => new CrossMarkIcon(ColorTheme.Type.TextLight).Texture;

		protected virtual Texture2D AddItemIcon => new DuplicateIcon(ColorTheme.Type.TextLight).Texture;


		[field: NonSerialized]
		protected SerializedProperty ListProperty { get; }

		protected string SearchingText
		{
			get => SessionState.GetString(SearchingTextKey, string.Empty);
			set => SessionState.SetString(SearchingTextKey, value);
		}

		protected bool IsSearch => SearchingText.CheckHasValue();

		protected virtual bool TryGetItemVE(SerializedProperty mapProperty, out ItemVE itemVE)
		{
			itemVE = _items.FirstOrDefault(item => (item.ItemProperty.serializedObject.targetObject == mapProperty.serializedObject.targetObject) && (item.ItemProperty.propertyPath == mapProperty.propertyPath));
			return itemVE != null;
		}


		// PUBLIC VARIABLE: ---------------------------------------------------------------------

		public virtual string Id => GetType().Name;
		public virtual string IdKey => Id + ".";

		public virtual string RootKey => IdKey + ListProperty.serializedObject.targetObject.GetInstanceID() + ".";
		public virtual string SearchingTextKey => RootKey + nameof(SearchingText);

		public VisualElement Body => this;

		public virtual bool IsAllowReordering => true;
		public virtual bool IsAllowDuplicate => true;
		public virtual bool IsAllowDelete => true;
		public virtual bool IsAllowContextMenu => true;
		public virtual bool IsAllowCopyPaste => true;

		public virtual bool IsAllowGroupCollapse => IsElementIsClass;
		public virtual bool IsAllowGroupExpand => IsElementIsClass;

		[field: NonSerialized]
		public virtual ItemSortManipulator ItemSortManipulator { get; private set; }

		[field: NonSerialized]
		public virtual Type ItemType { get; }

		[field: NonSerialized]
		public virtual bool IsElementIsClass { get; }


		// CONSTRUCTOR: ---------------------------------------------------------------------------

		[Preserve]
		public ListVE(SerializedProperty listProperty)
		{
			ListProperty = listProperty;
			ItemType = TypeUtils.GetGenericTypes(ListProperty)[0];
			IsElementIsClass = ItemType.CheckIsClassWithoutString();

			Add(_head);
			Add(_body);
			Add(_foot);
		}

		// PUBLIC METHODS: ------------------------------------------------------------------------

		public override void Refresh()
		{
			var mapsProperty = ListProperty;
			for (int i = 0; i < mapsProperty.arraySize; i++)
				SpawnMapItem(i, mapsProperty.GetArrayElementAtIndex(i));
			RemoveMapItems(mapsProperty.arraySize);
			RefreshSearchButton(IsSearch);
		}

		public virtual void InsertNewItem(int index) => InsertItem(index, TypeUtils.CreateInstance(ItemType));

		public virtual void InsertItem(int index, object value)
		{
			if (index < 0) return;

			ListProperty.serializedObject.Update();

			ListProperty.InsertArrayElementAtIndex(index);
			ListProperty.GetArrayElementAtIndex(index).SetValue(value);

			Refresh();
			RefreshIsExpandWhenInsert(index);
			_items[index].SetIsExpand(true);
		}

		public virtual void DuplicateItem(int index)
		{
			if (index < 0) return;

			ListProperty.serializedObject.Update();

			var source = ListProperty.GetArrayElementAtIndex(index).GetValue();
			if (source == null) return;

			var insertIndex = index + 1;
			ListProperty.InsertArrayElementAtIndex(insertIndex);
			var newObj = ListProperty.GetArrayElementAtIndex(insertIndex);

			CopyPasteUtils.Duplicate(newObj, source);
			SerializationUtils.ApplyUnregisteredSerialization(ListProperty.serializedObject);

			Refresh();
			RefreshIsExpandWhenInsert(insertIndex);
			_items[insertIndex].SetIsExpand(true);
		}

		public virtual void DeleteItem(int index)
		{
			if (index < 0) return;

			ListProperty.serializedObject.Update();

			ListProperty.DeleteArrayElementAtIndex(index);
			SerializationUtils.ApplyUnregisteredSerialization(ListProperty.serializedObject);

			Refresh();
		}

		public override void MoveItems(int sourceIndex, int destinationIndex)
		{
			MoveItems(ListProperty.serializedObject, ListProperty, sourceIndex, destinationIndex);
			Refresh();
		}

		public virtual void CollapseAll() => SetIsExpandAll(false);

		public virtual void ExpandAll() => SetIsExpandAll(true);


		// PROTECTED VIRTUAL METHODS: -------------------------------------------------------------

		protected override void DerivedInitialize()
		{
			foreach (var sheet in StyleSheetUtils.GetStyleSheets(USSPaths))
				styleSheets.Add(sheet);

			ItemSortManipulator = CreateSortManipulator();

			foreach (var c in HeadClasses)
				_head.AddToClassList(c);
			SetupHead();

			foreach (var c in BodyClasses)
				_body.AddToClassList(c);

			foreach (var c in FootClasses)
				_foot.AddToClassList(c);
			SetupFoot();
			Refresh();
		}

		#region Create VE

		protected virtual ItemSortManipulator CreateSortManipulator() => new ItemSortManipulator(this);

		protected virtual ItemVE CreateItem(SerializedProperty property)
		{
			var itemVE = new ItemVE(this, property);
			itemVE.Initialize();
			return itemVE;
		}

		#endregion

		#region Setup

		protected virtual void SetupHead()
		{
			if (IsAllowGroupCollapse)
			{
				var collapseAllButton = new Button(CollapseAll) { tooltip = "Collapse All" };
				collapseAllButton.Add(new Image() { image = DropRightIcon });
				_head.Add(collapseAllButton);
			}

			if (IsAllowGroupExpand)
			{
				var expandAllButton = new Button(ExpandAll) { tooltip = "Expand All" };
				expandAllButton.Add(new Image() { image = DropDownIcon });
				_head.Add(expandAllButton);
			}

			_searchTextField.Add(new Image() { image = SearchIcon });

			_clearSearchButton.clicked += () => { _searchTextField.value = string.Empty; };
			_clearSearchButton.Add(new Image() { image = ClearSearchIcon });
			_searchTextField.Add(_clearSearchButton);

			_searchTextField.tooltip = "Search";
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
			addButton.Add(new Label { text = "Add new item..." });
			addButton.clicked += () => { InsertNewItem(ListProperty.arraySize); };

			_foot.Add(addButton);
		}

		#endregion

		protected virtual void SetIsExpandAll(bool isExpand)
		{
			for (var i = 0; i < _body.childCount; i++)
				_items[i].SetIsExpand(isExpand);
		}

		protected virtual void RefreshSearchButton(bool isSearch) => _clearSearchButton.SetIsVisible(isSearch);

		protected virtual void SpawnMapItem(int index, SerializedProperty mapProperty, bool isAllowReordering = true, bool isAllowDuplicate = true, bool isAllowDelete = true, bool isAllowCopyPaste = true)
		{
			if (!TryGetItemVE(mapProperty, out var itemVE))
			{
				itemVE = CreateItem(mapProperty);
				_items.Add(itemVE);
				_body.Add(itemVE);
			}
			
			_items[index].Refresh(index, isAllowReordering, isAllowDuplicate, isAllowDelete, isAllowCopyPaste);
			var isVisible = !IsSearch || (IsSearch && _items[index].Title.ToLower().Contains(SearchingText.ToLower()));
			_items[index].SetIsVisible(isVisible);

			_items.Remove(itemVE);
			_items.Insert(index, itemVE);

			_body.Remove(itemVE);
			_body.Insert(index, itemVE);
		}

		protected virtual void RemoveMapItems(int finalIndex)
		{
			var itemsCount = _items.Count;
			for (var i = finalIndex; i < itemsCount; i++)
			{
				_items.RemoveAt(_items.Count - 1);
				_body.RemoveAt(_body.childCount - 1);
			}
		}

		protected virtual void RefreshIsExpandWhenInsert(int insertIndex)
		{
			if (insertIndex == ListProperty.arraySize - 1)
				return;

			for (int i = ListProperty.arraySize - 1; i > insertIndex; i--)
			{
				var previousIndex = i - 1;
				if (i - 1 < 0)
					continue;
				_items[i].SetIsExpand(_items[previousIndex].IsExpand);
			}
		}
	}
}