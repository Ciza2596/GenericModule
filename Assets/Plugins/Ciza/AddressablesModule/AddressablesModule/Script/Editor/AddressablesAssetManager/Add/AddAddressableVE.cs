using System;
using System.Linq;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

namespace CizaAddressablesModule.Editor
{
	public class AddAddressableVE : BItemVE
	{
		// CONST & STATIC: -----------------------------------------------------------------------

		public static readonly IIcon DEFAULT_REORDERING_ICON = new DragIcon(ColorTheme.Type.TextLight);
		public static readonly IIcon DEFAULT_DUPLICATE_ICON = new DuplicateIcon(ColorTheme.Type.TextLight);
		public static readonly IIcon DEFAULT_DELETE_ICON = new MinusIcon(ColorTheme.Type.TextLight);

		// VARIABLE: -----------------------------------------------------------------------------

		[NonSerialized]
		protected readonly VisualElement _head = new VisualElement();
		[NonSerialized]
		protected readonly Label _headReordering = new Label();
		[NonSerialized]
		protected readonly Button _headTitle = new Button();
		[NonSerialized]
		protected readonly VisualElement _body = new VisualElement();
		
		[NonSerialized]
		protected readonly TextField _dataIdField = new TextField("Data Id");
		[NonSerialized]
		protected readonly TextField _groupNameField = new TextField("Group Name");
		[NonSerialized]
		protected readonly EnumField _bundleModeField = new EnumField("Bundle Mode", default(BundledAssetGroupSchema.BundlePackingMode));
		[NonSerialized]
		protected readonly TextField _labelsField = new TextField("Labels");
		[NonSerialized]
		protected readonly TextField _addressTrimPrefixField = new TextField("Address Trim Prefix");
		[NonSerialized]
		protected readonly TextField _addressTrimSuffixField = new TextField("Address Trim Suffix");
		[NonSerialized]
		protected readonly TextField _addressAddPrefixField = new TextField("Address Add Prefix");
		[NonSerialized]
		protected readonly TextField _addressAddSuffixField = new TextField("Address Add Suffix");
		[NonSerialized]
		protected readonly FileFilterListVE _fileFilterListVE;
		[NonSerialized]
		protected readonly PathListVE _pathListVE;
		
		[NonSerialized]
		protected readonly Button _addButton = new Button() { text = "Add" };
		
		
		[NonSerialized]
		protected readonly Button _headDuplicate = new Button();
		[NonSerialized]
		protected readonly Button _headDelete = new Button();
		[NonSerialized]
		protected readonly BSortManipulator<AddAddressableVE> _sortManipulator;

		protected AddAddressableData _addAddressableData;
		protected bool _isBodyInitialized;
		protected bool _isInitialized;

		protected virtual string[] HeadClasses => new[] { "list-item-head" };
		protected virtual string[] HeadLeftClasses => new[] { "list-item-head-left" };
		protected virtual string[] HeadTitleClasses => new[] { "list-item-head-title" };
		protected virtual string HeadTitleExpandedClass => "list-item-head-title--expanded";
		protected virtual string[] HeadRightClasses => new[] { "list-item-head-right" };
		
		protected virtual string[] BodyClasses => new[] { "list-item-body", AlignLabel.UNITY_INSPECTOR_CLASS, AlignLabel.UNITY_INSPECTOR_ELEMENT_CLASS };
		protected virtual string[] SelectedItemClasses => new[] { "selected" };

		[field:NonSerialized]
		protected virtual AddAddressableListVE Root { get; }
		
		protected virtual string RootKey => $"AddAddressableDataList.{Index}";
		
		// PUBLIC VARIABLE: ---------------------------------------------------------------------

		public virtual int Index { get; protected set; }
		public virtual bool IsExpand { get; protected set; }

		// CONSTRUCTOR: ------------------------------------------------------------------------

		[Preserve]
		public AddAddressableVE(AddAddressableListVE root)
		{
			Root = root;
			_sortManipulator = Root.CreateSortManipulator();
			_fileFilterListVE = new FileFilterListVE(() => _addAddressableData?.FileFilters ?? Array.Empty<FileFilter>(), items => UpdateData(fileFilters: items));
			_pathListVE = new PathListVE(() => _addAddressableData?.Paths ?? Array.Empty<string>(), items => UpdateData(paths: items));
		}

		// LIFECYCLE METHOD: -------------------------------------------------------------------

		public override void Initialize()
		{
			if (_isInitialized)
				return;
			_isInitialized = true;
			base.Initialize();
			
			foreach (var c in HeadClasses)
				_head.AddToClassList(c);
			
			foreach (var c in HeadLeftClasses)
				_headReordering.AddToClassList(c);
			_headReordering.Add(new Image { image = DEFAULT_REORDERING_ICON.Texture });
			if (Root.IsAllowReordering)
			{
				_headReordering.AddManipulator(_sortManipulator);
				_head.Add(_headReordering);
			}

			foreach (var c in HeadTitleClasses)
				_headTitle.AddToClassList(c);
			
			_headTitle.clicked += () => SetIsExpand(!IsExpand);
			_head.Add(_headTitle);

			if (Root.IsAllowDuplicate)
				SetupButton(_headDuplicate, DEFAULT_DUPLICATE_ICON, "Duplicate", () => Root.DuplicateItem(Index));
			if (Root.IsAllowDelete)
				SetupButton(_headDelete, DEFAULT_DELETE_ICON, "Delete", () => Root.DeleteItem(Index));

			_headTitle.RegisterCallback<PointerDownEvent>(OnPointerDown, TrickleDown.TrickleDown);
			_headReordering.RegisterCallback<PointerDownEvent>(OnPointerDown);
			if (Root.IsAllowContextMenu)
			{
				_headTitle.AddManipulator(new ContextualMenuManipulator(OnOpenMenu));
				_headReordering.AddManipulator(new ContextualMenuManipulator(OnOpenMenu));
			}
			Add(_head);
			
			foreach (var c in BodyClasses)
				_body.AddToClassList(c);
			
			Add(_body);
			SetIsExpand(IsExpand);
		}

		public virtual void Refresh(int index, AddAddressableData data)
		{
			Index = index;
			_addAddressableData = data ?? new AddAddressableData();
			Refresh();
		}

		public override void Refresh()
		{
			_headTitle.text = _addAddressableData?.DataId ?? string.Empty;
			
			if (_isBodyInitialized)
				RefreshBody();
			_headReordering.SetEnabled(!Root.IsSearch);
			RefreshSelectedStyle();
		}

		// PUBLIC METHOD: ----------------------------------------------------------------------

		public virtual void RefreshSelectedStyle()
		{
			var isSelected = Root.SelectedItemIndexList.Contains(Index);
			foreach (var c in SelectedItemClasses)
				_head.EnableInClassList(c, isSelected);
		}

		// PROTECT METHOD: --------------------------------------------------------------------

		#region Setup

		protected virtual void SetupButton(Button button, IIcon icon, string tip, Action onClick)
		{
			foreach (var c in HeadRightClasses)
				button.AddToClassList(c);
			button.tooltip = tip;
			button.Add(new Image { image = icon.Texture, focusable = false });
			button.clicked += onClick;
			_head.Add(button);
		}

		#endregion

		public virtual void SetIsExpand(bool isExpand)
		{
			IsExpand = isExpand;
			if (IsExpand && !_isBodyInitialized)
			{
				CreateBodyContent();
				_isBodyInitialized = true;
				RefreshBody();
			}
			_body.SetIsVisible(IsExpand);
			_headTitle.EnableInClassList(HeadTitleExpandedClass, IsExpand);
		}
		
		protected virtual void CreateBodyContent()
		{
			SetupField(_dataIdField, _addAddressableData?.DataId ?? string.Empty);
			_body.Add(new SmallSpaceVE());
			
			
			var fileFilterListBoxVE = new BoxVE(RootKey + ".FileFilterListBox");
			fileFilterListBoxVE.Initialize("File Filter List", _fileFilterListVE);
			_body.Add(fileFilterListBoxVE);
			
			var pathListBoxVE = new BoxVE(RootKey + ".PathListBox");
			pathListBoxVE.Initialize("Path List", _pathListVE);
			_body.Add(pathListBoxVE);
			
			_body.Add(new SmallSpaceVE());
			SetupField(_groupNameField, _addAddressableData?.GroupName ?? string.Empty);
			
			_bundleModeField.AddToClassList(AlignLabel.UNITY_ALIGN_FIELD_CLASS);
			_bundleModeField.labelElement.style.paddingLeft = 0;
			_bundleModeField.SetValueWithoutNotify(_addAddressableData?.BundleMode ?? default);
			_bundleModeField.RegisterValueChangedCallback(_ => UpdateData());
			_body.Add(_bundleModeField);
			
			SetupField(_labelsField, _addAddressableData?.Labels ?? string.Empty);
			
			_body.Add(new SmallSpaceVE());
			SetupField(_addressTrimPrefixField, _addAddressableData?.AddressTrimPrefix ?? string.Empty);
			SetupField(_addressTrimSuffixField, _addAddressableData?.AddressTrimSuffix ?? string.Empty);
			_body.Add(new SmallSpaceVE());
			SetupField(_addressAddPrefixField, _addAddressableData?.AddressAddPrefix ?? string.Empty);
			SetupField(_addressAddSuffixField, _addAddressableData?.AddressAddSuffix ?? string.Empty);

			_body.Add(new SmallSpaceVE());
			_addButton.AddToClassList(AlignLabel.UNITY_ALIGN_FIELD_CLASS);
			_addButton.clicked += () => Root?.AddAddressable(_addAddressableData);
			_body.Add(_addButton);
		}

		protected virtual void SetupField(TextField field, string value)
		{
			field.AddToClassList(AlignLabel.UNITY_ALIGN_FIELD_CLASS);
			field.labelElement.style.paddingLeft = 0;
			field.SetValueWithoutNotify(value);
			field.RegisterValueChangedCallback(_ => UpdateData());
			_body.Add(field);
		}

		protected virtual void RefreshBody()
		{
			_dataIdField.SetValueWithoutNotify(_addAddressableData?.DataId ?? string.Empty);
			_groupNameField.SetValueWithoutNotify(_addAddressableData?.GroupName ?? string.Empty);
			_bundleModeField.SetValueWithoutNotify(_addAddressableData?.BundleMode ?? default);
			_labelsField.SetValueWithoutNotify(_addAddressableData?.Labels ?? string.Empty);
			_addressTrimPrefixField.SetValueWithoutNotify(_addAddressableData?.AddressTrimPrefix ?? string.Empty);
			_addressTrimSuffixField.SetValueWithoutNotify(_addAddressableData?.AddressTrimSuffix ?? string.Empty);
			_addressAddPrefixField.SetValueWithoutNotify(_addAddressableData?.AddressAddPrefix ?? string.Empty);
			_addressAddSuffixField.SetValueWithoutNotify(_addAddressableData?.AddressAddSuffix ?? string.Empty);
			_fileFilterListVE.Refresh();
			_pathListVE.Refresh();
		}

		protected virtual void UpdateData(FileFilter[] fileFilters = null, string[] paths = null)
		{
			var dataId = _dataIdField.value;
			fileFilters ??= _addAddressableData?.FileFilters;
			paths ??= _addAddressableData?.Paths;
			var groupName = _groupNameField.value;
			var bundleMode = (BundledAssetGroupSchema.BundlePackingMode)_bundleModeField.value;
			var labels = _labelsField.value;
			var addressTrimPrefix = _addressTrimPrefixField.value;
			var addressTrimSuffix = _addressTrimSuffixField.value;
			var addressAddPrefix = _addressAddPrefixField.value;
			var addressAddSuffix = _addressAddSuffixField.value;
			Root.UpdateItem(Index, new AddAddressableData(dataId, fileFilters, paths, groupName, bundleMode, labels, addressTrimPrefix, addressTrimSuffix, addressAddPrefix, addressAddSuffix));
		}


		// EVENT CALLBACK: ---------------------------------------------------------------------

		protected virtual void OnPointerDown(PointerDownEvent @event)
		{
			switch (@event.button)
			{
				case 1 when !Root.SelectedItemIndexList.Contains(Index):
					Root.OnItemSelected(Index, EventModifiers.None);
					break;
				case 0:
					Root.OnItemSelected(Index, @event.modifiers);
					break;
			}
		}

		protected virtual void OnOpenMenu(ContextualMenuPopulateEvent @event)
		{
			@event.menu.ClearItems();
			var selected = Root.SelectedItemIndexList;
			var selectionStatus = selected.Length > 0 ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled;
			@event.menu.AppendAction("Select All", _ => Root.SelectAllItem(), Root.IsAllowSelection && !Root.IsSelectAll ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
			@event.menu.AppendAction("Unselect All", _ => Root.UnselectAllItem(), Root.IsAllowSelection && !Root.IsUnselectAll ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
			
			if (Root.IsAllowCopyPaste)
			{
				@event.menu.AppendSeparator();
				@event.menu.AppendAction("Copy", _ => CopyPasteUtils.Copy(Root.GetSelectedItemsCopy()), selectionStatus);
				@event.menu.AppendAction("Paste", _ =>
				{
					if (CopyPasteUtils.TryPaste(typeof(AddAddressableData[]), out var copy) && copy is AddAddressableData[] items)
						Root.InsertItems(Index + 1, items);
				}, _ => CopyPasteUtils.CheckCanPaste(typeof(AddAddressableData[])) ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
			}
			if (Root.IsAllowDuplicate) 
				@event.menu.AppendAction("Duplicate", _ =>
				{
					foreach (var index in selected.OrderByDescending(i => i))
						Root.DuplicateItem(index);
				}, selectionStatus);
			
			if (Root.IsAllowDelete) 
				@event.menu.AppendAction("Delete", _ =>
				{
					Root.UnselectAllItem();
					foreach (var index in selected.OrderByDescending(i => i))
						Root.DeleteItem(index);
				}, selectionStatus);
			
			@event.StopPropagation();
		}
	}
}
