using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

namespace CizaAudioModule.Editor
{
	public class BoxVE : BBoxVE
	{
		// CONST & STATIC: -----------------------------------------------------------------------

		public static readonly IIcon DEFAULT_TRIANGLE_DOWN_ICON = new TriangleDownIcon(ColorTheme.Type.TextLight);
		public static readonly IIcon DEFAULT_TRIANGLE_RIGHT_ICON = new TriangleRightIcon(ColorTheme.Type.TextLight);

		// VARIABLE: -----------------------------------------------------------------------------

		[NonSerialized]
		protected readonly string _boxId;

		[NonSerialized]
		protected readonly Image _headImage = new Image();

		protected virtual string IsExpandKey => _boxId + "." + nameof(IsExpand);

		protected override string[] USSPaths => new[] { "Box" };

		protected override string[] RootClasses => new[] { "box-root" };
		protected override string[] HeadClasses => new[] { "box-head" };
		protected override string[] BodyClasses => new[] { "box-body" };
		protected virtual string ActiveBodyClass => "box-active";

		protected virtual string[] TitleLabelClasses => new[] { "box-head-titlelabel" };

		public virtual bool IsAllowContextMenu { get; set; } = true;
		public virtual bool IsAllowCopyPaste { get; set; } = true;

		protected virtual Texture2D TriangleDownIcon => DEFAULT_TRIANGLE_DOWN_ICON.Texture;
		protected virtual Texture2D TriangleRightIcon => DEFAULT_TRIANGLE_RIGHT_ICON.Texture;

		[field: NonSerialized]
		protected SerializedProperty Property { get; }

		protected virtual Type Type => SerializationUtils.GetType(Property, false);

		// EVENT: ---------------------------------------------------------------------------------

		public event Action<bool> OnExpand;

		// PUBLIC VARIABLE: ---------------------------------------------------------------------

		public virtual bool IsExpand
		{
			get => Property?.isExpanded ?? EditorPrefs.GetBool(IsExpandKey, true);
			protected set
			{
				if (Property != null)
					Property.isExpanded = value;
				EditorPrefs.SetBool(IsExpandKey, value);
				OnExpand?.Invoke(value);
			}
		}

		// CONSTRUCTOR: --------------------------------------------------------------------- 

		[Preserve]
		public BoxVE(SerializedProperty property)
		{
			Property = property;
		}

		[Preserve]
		public BoxVE(string boxId)
		{
			_boxId = boxId;
			IsAllowCopyPaste = false;
		}

		// PUBLIC METHOD: ----------------------------------------------------------------------

		public override void Refresh()
		{
			_headImage.image = IsExpand ? TriangleDownIcon : TriangleRightIcon;
			_body.EnableInClassList(ActiveBodyClass, IsExpand);
			if (!IsExpand) return;
			Content?.Refresh();
		}

		// PROTECT METHOD: --------------------------------------------------------------------

		protected override void DerivedInitialize(string title, IContent content, VisualElement headAdditional)
		{
			base.DerivedInitialize(title, content, headAdditional);

			AddHeadLeftContent(_headImage);

			var titleLabel = new Label(Title);
			foreach (var titleLabelClass in TitleLabelClasses)
				titleLabel.AddToClassList(titleLabelClass);
			AddHeadLeftContent(titleLabel);

			if (headAdditional != null)
				AddHeadRightContent(headAdditional);

			if (IsAllowContextMenu)
				AddHeadManipulator(new ContextualMenuManipulator(OnOpenMenu));

			_body.AddToClassList(ActiveBodyClass);
			IsExpand = IsExpand;
		}

		protected override void OnHeadClick()
		{
			IsExpand = !IsExpand;
			Refresh();
		}

		protected virtual void OnOpenMenu(ContextualMenuPopulateEvent populateEvent)
		{
			if (IsAllowCopyPaste)
			{
				populateEvent.menu.AppendAction("Copy", _ => { CopyPasteUtils.Copy(Property?.GetValue()); });

				populateEvent.menu.AppendAction("Paste", _ =>
				{
					if (CopyPasteUtils.TryPaste(Type, out var copy))
					{
						Property?.SetValue(copy);
						Content?.Refresh();
					}
				}, _ => CopyPasteUtils.CheckCanPaste(Type) ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
				populateEvent.menu.AppendSeparator();
			}
		}
	}
}