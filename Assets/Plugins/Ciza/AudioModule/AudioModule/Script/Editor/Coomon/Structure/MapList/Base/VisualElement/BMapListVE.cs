using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

namespace CizaAudioModule.Editor.MapListVisual
{
	public abstract class BMapListVE : BListVE<BMapItemVE>, BBoxVE.IContent
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

		protected virtual string MapsPath => "_maps";

		#region Name

		protected virtual string Name => "MapList";

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
		protected SerializedProperty MapListProperty { get; }

		protected virtual string SearchingText
		{
			get => SessionState.GetString(SearchingTextKey, string.Empty);
			set => SessionState.SetString(SearchingTextKey, value);
		}

		protected virtual bool IsSearch => SearchingText.CheckHasValue();

		protected virtual bool TryGetItemVE(SerializedProperty mapProperty, out BMapItemVE mapItemVE)
		{
			mapItemVE = _items.FirstOrDefault(item => (item.MapProperty.serializedObject.targetObject == mapProperty.serializedObject.targetObject) && (item.MapProperty.propertyPath == mapProperty.propertyPath));
			return mapItemVE != null;
		}

		// PUBLIC VARIABLE: ---------------------------------------------------------------------

		public virtual string Id => GetType().Name;
		public virtual string IdKey => Id + ".";

		public virtual string RootKey => IdKey + MapListProperty.serializedObject.targetObject.GetInstanceID() + ".";
		public virtual string SearchingTextKey => RootKey + nameof(SearchingText);


		public VisualElement Body => this;

		public virtual bool IsAllowReordering => true;
		public virtual bool IsAllowDisable => true;
		public virtual bool IsAllowDuplicate => true;
		public virtual bool IsAllowDelete => true;
		public virtual bool IsAllowContextMenu => true;
		public virtual bool IsAllowCopyPaste => true;

		public virtual bool IsAllowGroupCollapse => true;
		public virtual bool IsAllowGroupExpand => true;

		[field: NonSerialized]
		public MapItemSortManipulator MapItemSortManipulator { get; private set; }

		[field: NonSerialized]
		public SerializedProperty MapsProperty { get; private set; }

		public virtual Type MapType { get; }


		// CONSTRUCTOR: ---------------------------------------------------------------------------

		[Preserve]
		protected BMapListVE(SerializedProperty mapListProperty)
		{
			MapListProperty = mapListProperty;
			MapType = TypeUtils.GetGenericTypes(MapListProperty)[0];

			Add(_head);
			Add(_body);
			Add(_foot);
		}

		// PUBLIC METHODS: ------------------------------------------------------------------------

		public override void Refresh()
		{
			var mapsProperty = MapsProperty;
			for (int i = 0; i < mapsProperty.arraySize; i++)
				SpawnMapItem(i, mapsProperty.GetArrayElementAtIndex(i));
			RemoveMapItems(mapsProperty.arraySize);
			RefreshSearchButton(IsSearch);
		}

		public virtual void InsertNewItem(int index) => InsertItem(index, TypeUtils.CreateInstance(MapType));

		public virtual void InsertItem(int index, object value)
		{
			if (index < 0) return;

			MapListProperty.serializedObject.Update();

			MapsProperty.InsertArrayElementAtIndex(index);
			MapsProperty.GetArrayElementAtIndex(index).SetValue(value);

			Refresh();
			RefreshIsExpandWhenInsert(index);
			_items[index].SetIsExpand(true);
		}

		public virtual void DuplicateItem(int index)
		{
			if (index < 0) return;

			MapListProperty.serializedObject.Update();

			var source = MapsProperty.GetArrayElementAtIndex(index).GetValue();
			if (source == null) return;

			var insertIndex = index + 1;
			MapsProperty.InsertArrayElementAtIndex(insertIndex);
			var newObj = MapsProperty.GetArrayElementAtIndex(insertIndex);

			CopyPasteUtils.Duplicate(newObj, source);
			SerializationUtils.ApplyUnregisteredSerialization(MapListProperty.serializedObject);

			Refresh();
			RefreshIsExpandWhenInsert(index);
			_items[insertIndex].SetIsExpand(true);
		}

		public virtual void DeleteItem(int index)
		{
			if (index < 0) return;

			MapListProperty.serializedObject.Update();

			MapsProperty.DeleteArrayElementAtIndex(index);
			SerializationUtils.ApplyUnregisteredSerialization(MapListProperty.serializedObject);

			Refresh();
		}

		public override void MoveItems(int sourceIndex, int destinationIndex)
		{
			MoveItems(MapListProperty.serializedObject, MapsProperty, sourceIndex, destinationIndex);
			Refresh();
		}

		public virtual void CollapseAll() => SetIsExpandAll(false);

		public virtual void ExpandAll() => SetIsExpandAll(true);


		// PROTECTED VIRTUAL METHODS: -------------------------------------------------------------

		protected override void DerivedInitialize()
		{
			name = Name;

			foreach (var sheet in StyleSheetUtils.GetStyleSheets(USSPaths))
				styleSheets.Add(sheet);

			MapsProperty = MapListProperty.FindPropertyRelative(MapsPath);
			MapItemSortManipulator = CreateSortManipulator();

			foreach (var c in HeadClasses)
				_head.AddToClassList(c);
			SetupHead();

			foreach (var c in BodyClasses)
				_body.AddToClassList(c);

			foreach (var c in FootClasses)
				_foot.AddToClassList(c);
			SetupFoot();
		}

		#region Create VE

		protected virtual MapItemSortManipulator CreateSortManipulator() => new MapItemSortManipulator(this);

		protected abstract BMapItemVE CreateMapItem(SerializedProperty mapProperty);

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
			addButton.clicked += () => { InsertNewItem(MapsProperty.arraySize); };

			_foot.Add(addButton);
		}

		#endregion

		protected virtual void SetIsExpandAll(bool isExpand)
		{
			for (var i = 0; i < _body.childCount; i++)
				_items[i].SetIsExpand(isExpand);
		}

		protected virtual void RefreshSearchButton(bool isSearch)
		{
			_clearSearchButton.style.display = isSearch ? DisplayStyle.Flex : DisplayStyle.None;
		}

		protected virtual void SpawnMapItem(int index, SerializedProperty mapProperty, bool isAllowReordering = true, bool isAllowDisable = true, bool isAllowDuplicate = true, bool isAllowDelete = true, bool isAllowCopyPaste = true)
		{
			if (!TryGetItemVE(mapProperty, out var itemVE))
			{
				itemVE = CreateMapItem(mapProperty);
				_items.Add(itemVE);
				_body.Add(itemVE);
			}

			_items.Remove(itemVE);
			_items.Insert(index, itemVE);

			_body.Remove(itemVE);
			_body.Insert(index, itemVE);

			_items[index].Refresh(index, isAllowReordering, isAllowDisable, isAllowDuplicate, isAllowDelete, isAllowCopyPaste);
			var isVisible = !IsSearch || (IsSearch && _items[index].Key.ToLower().Contains(SearchingText.ToLower()));
			_items[index].SetIsVisible(isVisible);
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
			if (insertIndex == MapsProperty.arraySize - 1)
				return;

			for (int i = MapsProperty.arraySize - 1; i > insertIndex; i--)
			{
				var previousIndex = i - 1;
				if (i - 1 < 0)
					continue;
				_items[i].SetIsExpand(_items[previousIndex].IsExpand);
			}
		}
	}
}