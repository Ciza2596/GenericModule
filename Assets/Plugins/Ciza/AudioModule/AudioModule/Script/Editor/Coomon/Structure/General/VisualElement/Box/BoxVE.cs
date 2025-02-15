using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

namespace CizaAudioModule.Editor
{
	public class BoxVE : BBoxVE
	{
		[NonSerialized]
		protected readonly string _boxId;

		[NonSerialized]
		protected readonly Image _headImage = new Image();

		[NonSerialized]
		protected readonly VisualElement _headAdditional;

		protected virtual string IsExpandKey => _boxId + "." + nameof(IsExpand);

		protected override string[] USSPaths => new[] { "Box" };

		protected override string[] RootClasses => new[] { "box-root" };
		protected override string[] HeadClasses => new[] { "box-head" };
		protected override string[] BodyClasses => new[] { "box-body" };
		protected virtual string ActiveBodyClass => "box-active";

		public virtual bool IsAllowContextMenu { get; set; } = true;
		public virtual bool IsAllowCopyPaste { get; set; } = true;

		protected virtual Texture2D TriangleDownIcon => new TriangleDownIcon(ColorTheme.Type.TextLight).Texture;
		protected virtual Texture2D TriangleRightIcon => new TriangleRightIcon(ColorTheme.Type.TextLight).Texture;

		[field: NonSerialized]
		protected SerializedProperty Property { get; }

		protected Type Type => TypeUtils.GetType(Property, false);

		public event Action<bool> OnExpand;

		public virtual bool IsExpand
		{
			get => Property?.isExpanded ?? EditorPrefs.GetBool(IsExpandKey, false);
			protected set
			{
				if (Property != null)
					Property.isExpanded = value;
				EditorPrefs.SetBool(IsExpandKey, value);
				OnExpand?.Invoke(value);
			}
		}

		[Preserve]
		public BoxVE(SerializedProperty property, VisualElement headAdditional = null)
		{
			Property = property;
			_headAdditional = headAdditional;
		}

		[Preserve]
		public BoxVE(string boxId, VisualElement headAdditional = null)
		{
			_boxId = boxId;
			_headAdditional = headAdditional;
		}

		protected override void DerivedInitialize(string title, IContent content)
		{
			base.DerivedInitialize(title, content);

			AddHeadLeftContent(_headImage);
			AddHeadLeftContent(new Label(Title));

			if (_headAdditional != null)
				AddHeadRightContent(_headAdditional);

			if (IsAllowContextMenu)
				AddHeadMainpulator(new ContextualMenuManipulator(OnOpenMenu));

			_body.AddToClassList(ActiveBodyClass);
			IsExpand = IsExpand;
		}

		public override void Refresh()
		{
			_headImage.image = IsExpand ? TriangleDownIcon : TriangleRightIcon;

			_body.EnableInClassList(ActiveBodyClass, IsExpand);

			if (!IsExpand) return;

			Content?.Refresh();
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