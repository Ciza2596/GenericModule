using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CizaAudioModule.Editor
{
	public class ItemVE : BItemVE
	{
		[NonSerialized]
		protected readonly VisualElement _head = new VisualElement();

		[NonSerialized]
		protected readonly Label _headReordering = new Label();

		[NonSerialized]
		protected VisualElement _headTitle;

		[NonSerialized]
		protected readonly Button _headDuplicate = new Button();

		[NonSerialized]
		protected readonly Button _headDelete = new Button();


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

		protected virtual Texture2D ReorderingIcon => new DragIcon(ColorTheme.Type.TextLight).Texture;
		protected virtual Texture2D DuplicateIcon => new DuplicateIcon(ColorTheme.Type.Light).Texture;
		protected virtual Texture2D DeleteIcon => new MinusIcon(ColorTheme.Type.Light).Texture;


		[field: NonSerialized]
		protected ListVE Root { get; }


		[field: NonSerialized]
		protected int Index { get; private set; }

		protected bool IsAllowReordering { get; set; } = true;
		protected bool IsAllowDuplicate { get; set; } = true;
		protected bool IsAllowDelete { get; set; } = true;

		protected bool IsAllowCopyPaste { get; private set; } = true;

		// PUBLIC VARIABLE: ---------------------------------------------------------------------

		[field: NonSerialized]
		public SerializedProperty ItemProperty { get; }

		public virtual string Title => $"Element {Index}";

		public virtual bool IsExpand
		{
			get => ItemProperty?.isExpanded ?? false;
			protected set
			{
				if (ItemProperty != null)
					ItemProperty.isExpanded = value;
			}
		}

		// CONSTRUCTOR: --------------------------------------------------------------------- 

		public ItemVE(ListVE root, SerializedProperty itemProperty) : base()
		{
			Root = root;
			ItemProperty = itemProperty;
			_headTitle = Root.IsElementIsClass ? new Button() : new PropertyField(ItemProperty);
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
			SetIsExpand(IsExpand);
		}

		public virtual void Refresh(int index, bool isAllowReordering = true, bool isAllowDuplicate = true, bool isAllowDelete = true, bool isAllowCopyPaste = true)
		{
			Index = index;
			IsAllowReordering = isAllowReordering;
			IsAllowDuplicate = isAllowDuplicate;
			IsAllowDelete = isAllowDelete;
			IsAllowCopyPaste = isAllowCopyPaste;

			RefreshHeadTitle();

			SetIsExpand(IsExpand);

			if (Root.IsAllowReordering)
			{
				GetActiveOpacity(_headReordering, IsAllowReordering, "Reordering");
				if (IsAllowReordering)
					_headReordering.AddManipulator(Root.ItemSortManipulator);
				else
					_headReordering.RemoveManipulator(Root.ItemSortManipulator);
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

				if (Root.IsElementIsClass && _headTitle is Button { } button)
					button.clicked += () => SetIsExpand(!IsExpand);
				else if (!Root.IsElementIsClass && _headTitle is PropertyField { } field)
					field.BindProperty(ItemProperty);

				if (Root.IsAllowContextMenu)
					_headTitle.AddManipulator(new ContextualMenuManipulator(OnOpenMenu));
				_head.Add(_headTitle);
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
			IsExpand = isExpand && Root.IsElementIsClass;
			_body.SetIsVisible(IsExpand);
			_headTitle.EnableInClassList(HeadTitleExpandedClass, IsExpand);
			if (IsExpand) RefreshBody();
		}

		protected virtual void DerivedInitialize()
		{
			SerializationUtils.CreateChildProperties(_body, ItemProperty, SerializationUtils.ChildrenMode.ShowLabelsInChildren, 0f);
		}

		protected virtual void RefreshBody() { }

		protected virtual void RefreshHeadTitle()
		{
			if (Root.IsElementIsClass && _headTitle is Button { } button)
				button.text = Title;
			else if (!Root.IsElementIsClass && _headTitle is PropertyField { } field)
				field.BindProperty(ItemProperty);
		}

		// EVENT CALLBACK: ---------------------------------------------------------------------

		protected virtual void OnOpenMenu(ContextualMenuPopulateEvent populateEvent)
		{
			populateEvent.menu.AppendSeparator();

			if (Root.IsAllowCopyPaste && IsAllowCopyPaste)
			{
				populateEvent.menu.AppendAction("Copy", _ => { CopyPasteUtils.Copy(ItemProperty.GetValue()); });

				populateEvent.menu.AppendAction("Paste", _ =>
				{
					var pasteIndex = Index + 1;
					if (CopyPasteUtils.TryPaste(Root.ItemType, out var copy))
						Root.InsertItem(pasteIndex, copy);
				}, _ => CopyPasteUtils.CheckCanPaste(Root.ItemType) ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
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