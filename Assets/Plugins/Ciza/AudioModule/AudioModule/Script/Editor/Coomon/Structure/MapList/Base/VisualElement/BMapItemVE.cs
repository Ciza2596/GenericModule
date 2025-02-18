using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace CizaAudioModule.Editor.MapListVisual
{
	public abstract class BMapItemVE : BItemVE
	{
		// VARIABLE: -----------------------------------------------------------------------------

		#region Head

		[NonSerialized]
		protected readonly VisualElement _head = new VisualElement();

		[NonSerialized]
		protected readonly Label _headReordering = new Label();

		[NonSerialized]
		protected readonly Button _headTitle = new Button();

		[NonSerialized]
		protected readonly Button _headDisable = new Button();

		[NonSerialized]
		protected readonly Button _headDuplicate = new Button();

		[NonSerialized]
		protected readonly Button _headDelete = new Button();

		#endregion

		[NonSerialized]
		protected readonly VisualElement _body = new VisualElement();

		protected override string[] DropAboveClasses => new[] { "list-item-drop-above" };
		protected override string[] DropBelowClasses => new[] { "list-item-drop-below" };

		protected virtual string[] HeadClasses => new[] { "list-item-head" };
		protected virtual string[] HeadLeftClasses => new[] { "list-item-head-left" };
		protected virtual string[] HeadTitleClasses => new[] { "list-item-head-title" };
		protected virtual string HeadTitleExpandedClass => "list-item-head-title--expanded";
		protected virtual string[] HeadRightClasses => new[] { "list-item-head-right" };
		protected virtual string[] BodyClasses => new[] { "list-item-body" };

		protected virtual string KeyPath => "_key";
		protected virtual string IsEnablePath => "_isEnable";
		protected virtual string ValuePath => "_value";
		protected virtual Texture2D ReorderingIcon => new DragIcon(ColorTheme.Type.TextLight).Texture;
		protected virtual Texture2D DisableIcon => new CancelIcon(ColorTheme.Type.Light).Texture;
		protected virtual Texture2D DuplicateIcon => new DuplicateIcon(ColorTheme.Type.Light).Texture;
		protected virtual Texture2D DeleteIcon => new MinusIcon(ColorTheme.Type.Light).Texture;


		[field: NonSerialized]
		protected BMapListVE Root { get; }

		[field: NonSerialized]
		protected SerializedProperty MapProperty { get; }

		protected SerializedProperty KeyProperty => MapProperty.FindPropertyRelative(KeyPath);
		protected SerializedProperty IsEnableProperty => MapProperty.FindPropertyRelative(IsEnablePath);
		protected SerializedProperty ValueProperty => MapProperty.FindPropertyRelative(ValuePath);

		[field: NonSerialized]
		protected int Index { get; private set; }

		protected bool IsAllowReordering { get; set; } = true;
		protected bool IsAllowDisable { get; set; } = true;
		protected bool IsAllowDuplicate { get; set; } = true;
		protected bool IsAllowDelete { get; set; } = true;

		protected bool IsAllowCopyPaste { get; private set; } = true;

		// PUBLIC VARIABLE: ---------------------------------------------------------------------

		public virtual string Key
		{
			get => KeyProperty.stringValue;

			set
			{
				KeyProperty.stringValue = value;
				SerializationUtils.ApplyUnregisteredSerialization(MapProperty.serializedObject);
			}
		}

		public virtual bool IsEnable
		{
			get => IsEnableProperty.boolValue;

			set
			{
				IsEnableProperty.boolValue = value;
				SerializationUtils.ApplyUnregisteredSerialization(MapProperty.serializedObject);
			}
		}

		protected virtual bool IsExpand
		{
			get => MapProperty?.isExpanded ?? false;
			private set
			{
				if (MapProperty != null)
					MapProperty.isExpanded = value;
			}
		}

		// CONSTRUCTOR: --------------------------------------------------------------------- 

		protected BMapItemVE(BMapListVE root, SerializedProperty mapProperty) : base()
		{
			Root = root;
			MapProperty = mapProperty;
			Add(_head);
			Add(_body);
		}


		// LIFECIRCLE METHOD: ------------------------------------------------------------------

		public override void Initialize()
		{
			base.Initialize();

			foreach (var c in HeadClasses)
				_head.AddToClassList(c);
			_headTitle.AddToClassList(HeadTitleExpandedClass);

			foreach (var c in BodyClasses)
				_body.AddToClassList(c);

			SetupHead();
			SetupDrop();
			DerivedInitialize();
		}

		public virtual void Refresh(int index, bool isAllowReordering = true, bool isAllowDisable = true, bool isAllowDuplicate = true, bool isAllowDelete = true, bool isAllowCopyPaste = true)
		{
			Index = index;
			IsAllowReordering = isAllowReordering;
			IsAllowDisable = isAllowDisable;
			IsAllowDuplicate = isAllowDuplicate;
			IsAllowDelete = isAllowDelete;
			IsAllowCopyPaste = isAllowCopyPaste;

			RefreshHeadTitle();

			SetIsExpand(MapProperty?.isExpanded ?? false);

			if (Root.IsAllowReordering)
			{
				GetActiveOpacity(_headReordering, IsAllowReordering, "Reordering");
				if (IsAllowReordering)
					_headReordering.AddManipulator(Root.MapItemSortManipulator);
				else
					_headReordering.RemoveManipulator(Root.MapItemSortManipulator);
			}

			if (Root.IsAllowDisable)
			{
				_headTitle.style.opacity = IsEnable ? 1f : 0.25f;
				_headDisable.style.display = (IsAllowDisable && IsEnable) || !IsAllowDisable ? DisplayStyle.None : DisplayStyle.Flex;

				GetActiveOpacity(_headDisable, IsAllowDisable, "Disable");
			}

			if (Root.IsAllowDuplicate)
			{
				GetActiveOpacity(_headDuplicate, IsAllowDuplicate, "Duplicate");
			}

			if (Root.IsAllowDelete)
			{
				GetActiveOpacity(_headDelete, IsAllowDelete, "Delete");
			}
		}

		// PROTECT METHOD: --------------------------------------------------------------------

		#region Setup

		#region Setup Head

		protected virtual void SetupHead()
		{
			if (Root.IsAllowReordering)
			{
				foreach (var c in HeadLeftClasses)
					_headReordering.AddToClassList(c);
				if (Root.IsAllowContextMenu)
					_headReordering.AddManipulator(new ContextualMenuManipulator(OnOpenMenu));

				var image = new Image { image = ReorderingIcon };
				_headReordering.Add(image);
				_head.Add(_headReordering);
			}

			{
				foreach (var c in HeadTitleClasses)
					_headTitle.AddToClassList(c);
				_headTitle.clicked += () => SetIsExpand(!IsExpand);

				if (Root.IsAllowContextMenu)
					_headTitle.AddManipulator(new ContextualMenuManipulator(OnOpenMenu));
				_head.Add(_headTitle);
			}

			if (Root.IsAllowDisable)
			{
				foreach (var c in HeadRightClasses)
					_headDisable.AddToClassList(c);
				_headDisable.clicked += () =>
				{
					IsEnable = true;
					Root.Refresh();
				};
				_headDisable.Add(new Image { image = DisableIcon, focusable = false });
				_head.Add(_headDisable);
			}

			if (Root.IsAllowDuplicate)
			{
				foreach (var c in HeadRightClasses)
					_headDuplicate.AddToClassList(c);
				_headDuplicate.clicked += () => { Root.DuplicateItem(Index); };
				_headDuplicate.Add(new Image { image = DuplicateIcon, focusable = false });
				_head.Add(_headDuplicate);
			}

			if (Root.IsAllowDelete)
			{
				foreach (var c in HeadRightClasses)
					_headDelete.AddToClassList(c);
				_headDelete.clicked += () => { Root.DeleteItem(Index); };
				_headDelete.Add(new Image { image = DeleteIcon, focusable = false });
				_head.Add(_headDelete);
			}
		}

		#endregion

		protected virtual void SetupDrop()
		{
			Insert(0, _dropAbove);
			Insert(3, _dropBelow);
		}

		#endregion

		public virtual void SetIsExpand(bool isExpand)
		{
			IsExpand = isExpand;
			_body.SetIsVisible(IsExpand);
			_headTitle.EnableInClassList(HeadTitleExpandedClass, isExpand);
			if (isExpand) RefreshBody();
		}

		protected abstract void DerivedInitialize();

		protected virtual void RefreshBody() { }

		protected virtual void RefreshHeadTitle()
		{
			_headTitle.text = Key;
			SerializationUtils.ApplyUnregisteredSerialization(MapProperty.serializedObject);
		}

		// EVENT CALLBACK: ---------------------------------------------------------------------

		protected virtual void OnOpenMenu(ContextualMenuPopulateEvent populateEvent)
		{
			if (Root.IsAllowDisable && IsAllowDisable)
			{
				populateEvent.menu.AppendAction(IsEnable ? "Disable" : "Enable", _ =>
				{
					IsEnable = !IsEnable;
					Root.Refresh();
				});
			}

			populateEvent.menu.AppendSeparator();

			if (Root.IsAllowCopyPaste && IsAllowCopyPaste)
			{
				populateEvent.menu.AppendAction("Copy", _ => { CopyPasteUtils.Copy(MapProperty.GetValue()); });

				populateEvent.menu.AppendAction("Paste", _ =>
				{
					var pasteIndex = Index + 1;
					if (CopyPasteUtils.TryPaste(Root.MapType, out var copy))
						Root.InsertItem(pasteIndex, copy);
				}, _ => CopyPasteUtils.CheckCanPaste(Root.MapType) ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
				populateEvent.menu.AppendSeparator();
			}

			if (Root.IsAllowGroupCollapse)
				populateEvent.menu.AppendAction("Collapse All", _ => Root.CollapseAll());

			if (Root.IsAllowGroupExpand)
				populateEvent.menu.AppendAction("Expand All", _ => Root.ExpandAll());
		}

		protected virtual void GetActiveOpacity(VisualElement visualElement, bool isActive, string tooltip)
		{
			visualElement.enabledSelf = isActive;
			visualElement.tooltip = isActive ? tooltip : string.Empty;
			visualElement.style.opacity = isActive ? 1 : 0;
		}
	}
}