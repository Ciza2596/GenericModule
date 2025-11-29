using System;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace CizaAudioModule.Editor
{
	public class ItemVE : BItemVE
	{
		// CONST & STATIC: -----------------------------------------------------------------------

		public static readonly IIcon DEFAULT_REORDERING_ICON = new DragIcon(ColorTheme.Type.TextLight);
		public static readonly IIcon DEFAULT_DISABLE_ICON = new CancelIcon(ColorTheme.Type.TextLight);
		public static readonly IIcon DEFAULT_DUPLICATE_ICON = new DuplicateIcon(ColorTheme.Type.TextLight);
		public static readonly IIcon DEFAULT_DELETE_ICON = new MinusIcon(ColorTheme.Type.TextLight);

		// VARIABLE: -----------------------------------------------------------------------------

		[NonSerialized]
		protected readonly VisualElement _head = new VisualElement();

		[NonSerialized]
		protected readonly Label _headReordering = new Label();

		[NonSerialized]
		protected readonly VisualElement _headTitle;

		[NonSerialized]
		protected readonly Button _headDisable = new Button();

		[NonSerialized]
		protected readonly Button _headDuplicate = new Button();

		[NonSerialized]
		protected readonly Button _headDelete = new Button();

		[NonSerialized]
		protected Object _target;

		[NonSerialized]
		protected string _itemPath;

		[NonSerialized]
		protected Type _itemType;

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
		protected virtual string SelectedItemClass => "selected";


		protected virtual Texture2D ReorderingIcon => DEFAULT_REORDERING_ICON.Texture;
		protected virtual Texture2D DisableIcon => DEFAULT_DISABLE_ICON.Texture;
		protected virtual Texture2D DuplicateIcon => DEFAULT_DUPLICATE_ICON.Texture;
		protected virtual Texture2D DeleteIcon => DEFAULT_DELETE_ICON.Texture;


		[field: NonSerialized]
		protected virtual ListVE Root { get; }


		protected virtual bool IsAllowReordering { get; set; } = true;
		protected virtual bool IsAllowDisable { get; set; } = true;
		protected virtual bool IsAllowDuplicate { get; set; } = true;
		protected virtual bool IsAllowDelete { get; set; } = true;

		protected virtual bool IsAllowCopyPaste { get; set; } = true;

		// PUBLIC VARIABLE: ---------------------------------------------------------------------

		[field: NonSerialized]
		public virtual int Index { get; protected set; }

		[field: NonSerialized]
		public virtual SerializedProperty ItemProperty { get; protected set; }

		public virtual string Title => $"Element {Index}";

		public virtual bool IsEnable { get; protected set; }

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

		[Preserve]
		public ItemVE(ListVE root, SerializedProperty itemProperty) : base()
		{
			Root = root;
			SetItemProperty(itemProperty, false);
			_headTitle = CreateHeadTitle();
			Add(_head);
			Add(_body);
		}


		// LIFECIRCLE METHOD: ------------------------------------------------------------------

		public override void Initialize()
		{
			base.Initialize();

			foreach (var c in HeadClasses)
				_head.AddToClassList(c);
			_head.AddToClassList(SelectedItemClass);
			_headTitle.AddToClassList(HeadTitleExpandedClass);

			foreach (var c in BodyClasses)
				_body.AddToClassList(c);

			SetupHead();
			SetupDrop();
			DerivedInitialize();
			SetIsExpand(IsExpand);
		}

		public override void Refresh() =>
			Refresh(Index, ItemProperty, IsAllowReordering, IsAllowDisable, IsAllowDuplicate, IsAllowDelete, IsAllowCopyPaste);

		public virtual void Refresh(int index, SerializedProperty itemProperty, bool isAllowReordering, bool isAllowDisable, bool isAllowDuplicate, bool isAllowDelete, bool isAllowCopyPaste)
		{
			Index = index;
			SetItemProperty(itemProperty, true);
			IsAllowReordering = isAllowReordering;
			IsAllowDisable = isAllowDisable;
			IsAllowDuplicate = isAllowDuplicate;
			IsAllowDelete = isAllowDelete;
			IsAllowCopyPaste = isAllowCopyPaste;

			RefreshHeadTitle();

			SetIsExpand(IsExpand);

			if (Root.IsAllowReordering)
			{
				GetActiveOpacity(_headReordering, IsAllowReordering, "Reordering");
				if (IsAllowReordering)
					_headReordering.AddManipulator(Root.SortManipulator);
				else
					_headReordering.RemoveManipulator(Root.SortManipulator);
			}

			if (Root.IsAllowDisable)
			{
				_headTitle.style.opacity = IsEnable ? 1f : 0.25f;
				_headDisable.SetIsVisible(IsAllowDisable && !IsEnable);

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

			RefreshSelectedStyle();
		}

		public virtual void RefreshSelectedStyle() =>
			_head.EnableInClassList(SelectedItemClass, Root.SelectedItemIndexList.Contains(Index));

		// PROTECT METHOD: --------------------------------------------------------------------

		#region Setup

		#region Setup Head

		protected virtual VisualElement CreateHeadTitle() =>
			Root.IsElementClass ? new Button() : new PropertyField(ItemProperty);

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

				if (Root.IsElementClass && _headTitle is Button { } button)
					button.clicked += () => SetIsExpand(!IsExpand);
				else if (!Root.IsElementClass && _headTitle is BindableElement field)
					field.BindProperty(ItemProperty);

				if (Root.IsAllowContextMenu)
					_headTitle.AddManipulator(new ContextualMenuManipulator(OnOpenMenu));

				_headTitle.RegisterCallback<PointerDownEvent>(@event =>
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
				});
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

		protected virtual void SetItemProperty(SerializedProperty itemProperty, bool isRefreshBodyContent)
		{
			var target = itemProperty.serializedObject.targetObject;
			var itemPath = itemProperty.propertyPath;
			var itemType = itemProperty.GetValue()?.GetType();

			var isDifTarget = target != _target;
			var isDifItemPath = itemPath != _itemPath;
			var isDifItemType = itemType != _itemType;

			if (isDifTarget || isDifItemPath || isDifItemType)
			{
				ItemProperty = itemProperty;
				_target = target;
				_itemPath = itemPath;
				_itemType = itemType;
				if (isRefreshBodyContent)
				{
					_body.Clear();
					CreateBodyContent();
				}
			}
		}

		public virtual void SetIsExpand(bool isExpand)
		{
			IsExpand = isExpand && Root.IsElementClass && _body.childCount > 0;
			_body.SetIsVisible(IsExpand);
			_headTitle.EnableInClassList(HeadTitleExpandedClass, IsExpand);
		}

		protected virtual void DerivedInitialize()
		{
			CreateBodyContent();
		}

		protected virtual void CreateBodyContent()
		{
			SerializationUtils.CreateChildProperties(_body, ItemProperty, SerializationUtils.ChildrenKinds.ShowLabelsInChildren, 0f, onChangeValue: OnRefreshBody);
		}

		protected virtual void OnRefreshBody(SerializedPropertyChangeEvent @event)
		{
			RefreshHeadTitle();
		}

		protected virtual void RefreshHeadTitle()
		{
			if (Root.IsElementClass && _headTitle is Button button)
				button.text = Title;
			else if (!Root.IsElementClass)
			{
				_headTitle.Unbind();
				_headTitle.Bind(ItemProperty.serializedObject);
			}
		}

		// EVENT CALLBACK: ---------------------------------------------------------------------

		protected virtual void OnOpenMenu(ContextualMenuPopulateEvent populateEvent)
		{
			populateEvent.menu.ClearItems();

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
				populateEvent.menu.AppendAction("Select All", _ => { Root.SelectAllItem(); }, _ => Root.IsAllowSelection && !Root.IsSelectAll ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
				populateEvent.menu.AppendAction("Unselect All", _ => { Root.UnselectAllItem(); }, _ => Root.IsAllowSelection && !Root.IsUnselectAll ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);

				populateEvent.menu.AppendSeparator();

				populateEvent.menu.AppendAction("Copy", _ => { CopyPasteUtils.Copy(Root.GetSelectedItemsCopy()); }, _ => Root.SelectedItemIndexList.Length > 0 ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);

				var arrayType = Array.CreateInstance(Root.ItemType, 0).GetType();
				populateEvent.menu.AppendAction("Paste", _ =>
				{
					if (!CopyPasteUtils.TryPaste(arrayType, out var copy) || copy is not Array array)
						return;
					var pasteIndex = Index + 1;
					foreach (var item in array)
					{
						Root.InsertItem(pasteIndex, item);
						pasteIndex++;
					}
				}, _ => CopyPasteUtils.CheckCanPaste(arrayType) ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);

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