using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

namespace CizaInputModule.Editor
{
	public class PathVE : BItemVE
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
		protected readonly VisualElement _headTitle = new VisualElement();
		[NonSerialized]
		protected readonly TextField _pathField = new TextField();
		[NonSerialized]
		protected readonly Button _headDuplicate = new Button();
		[NonSerialized]
		protected readonly Button _headDelete = new Button();
		[NonSerialized]
		protected readonly BSortManipulator<PathVE> _sortManipulator;

		protected string _path;
		
		protected bool _isInitialized;

		protected virtual string[] HeadClasses => new[] { "list-item-head" };
		protected virtual string[] HeadLeftClasses => new[] { "list-item-head-left" };
		protected virtual string[] HeadTitleClasses => new[] { "list-item-head-title" };
		protected virtual string[] HeadRightClasses => new[] { "list-item-head-right" };
		protected virtual string[] SelectedItemClasses => new[] { "selected" };

		[field:NonSerialized]
		protected virtual PathListVE Root { get; }

		// PUBLIC VARIABLE: ---------------------------------------------------------------------

		public virtual int Index { get; protected set; }

		// CONSTRUCTOR: ------------------------------------------------------------------------

		[Preserve]
		public PathVE(PathListVE root)
		{
			Root = root;
			_sortManipulator = Root.CreateSortManipulator();
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
			_headTitle.style.flexDirection = FlexDirection.Row;
			_headTitle.style.minWidth = 0;
			SetupField(_pathField, "Path");
			_headTitle.Add(_pathField);
			_head.Add(_headTitle);

			SetupPinButton();

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
		}

		public virtual void Refresh(int index, string path)
		{
			Index = index;
			_path = path ?? string.Empty;
			Refresh();
		}

		public override void Refresh()
		{
			_pathField.SetValueWithoutNotify(_path);
			_headTitle.tooltip = _path;
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

		protected virtual void SetupField(TextField field, string tip)
		{
			field.tooltip = tip;
			field.style.flexGrow = 1;
			field.style.flexBasis = 0;
			field.style.minWidth = 0;
			field.SetMargin(0, 0, 1, 1);
			field.RegisterValueChangedCallback(_ => Root.UpdateItem(Index, _pathField.value));
		}

		protected virtual void SetupButton(Button button, IIcon icon, string tip, Action onClick)
		{
			foreach (var c in HeadRightClasses)
				button.AddToClassList(c);
			button.tooltip = tip;
			button.Add(new Image { image = icon.Texture, focusable = false });
			button.clicked += onClick;
			_head.Add(button);
		}

		protected virtual void SetupPinButton()
		{
			var pinButton = new Button() { text = "\u29bf", style =
			{
				width = 16, 
				height = 16, 
				backgroundColor = new Color(0.168f, 0.168f, 0.168f),
				position = Position.Absolute,
				unityFontStyleAndWeight = FontStyle.Bold,
				fontSize = 10,
			} };
			pinButton.SetMargin(2);
			pinButton.SetBorder(0);
			pinButton.clicked += OnPinButtonClicked;
			_head.Add(pinButton);
			pinButton.SetAnchoredPosition(AnchorKinds.Right, new Vector2(48, 0));
		}

		#endregion

		// EVENT CALLBACK: ---------------------------------------------------------------------

		protected virtual void OnPinButtonClicked()
		{
			if (string.IsNullOrEmpty(_path) || !AssetDatabase.IsValidFolder(_path))
				return;
			
			var folder = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(_path);
			
			if (folder != null)
				AssetDatabase.OpenAsset(folder);
		}

		protected virtual void OnPointerDown(PointerDownEvent evt)
		{
			if (evt.button == 1 && !Root.SelectedItemIndexList.Contains(Index))
				Root.OnItemSelected(Index, EventModifiers.None);
			else if (evt.button == 0)
				Root.OnItemSelected(Index, evt.modifiers);
		}

		protected virtual void OnOpenMenu(ContextualMenuPopulateEvent evt)
		{
			evt.menu.ClearItems();
			var selected = Root.SelectedItemIndexList;
			var selectionStatus = selected.Length > 0 ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled;
			evt.menu.AppendAction("Select All", _ => Root.SelectAllItem(),
				Root.IsAllowSelection && !Root.IsSelectAll ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
			evt.menu.AppendAction("Unselect All", _ => Root.UnselectAllItem(),
				Root.IsAllowSelection && !Root.IsUnselectAll ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
			if (Root.IsAllowCopyPaste)
			{
				evt.menu.AppendSeparator();
				evt.menu.AppendAction("Copy", _ => CopyPasteUtils.Copy(Root.GetSelectedItemsCopy()), selectionStatus);
				evt.menu.AppendAction("Paste", _ =>
				{
					if (CopyPasteUtils.TryPaste(typeof(string[]), out var copy) && copy is string[] paths)
						Root.InsertItems(Index + 1, paths);
				}, _ => CopyPasteUtils.CheckCanPaste(typeof(string[])) ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
			}
			if (Root.IsAllowDuplicate)
				evt.menu.AppendAction("Duplicate", _ =>
				{
					foreach (var index in selected.OrderByDescending(i => i))
						Root.DuplicateItem(index);
				}, selectionStatus);
			if (Root.IsAllowDelete)
				evt.menu.AppendAction("Delete", _ =>
				{
					Root.UnselectAllItem();
					foreach (var index in selected.OrderByDescending(i => i))
						Root.DeleteItem(index);
				}, selectionStatus);
			evt.StopPropagation();
		}
	}
}