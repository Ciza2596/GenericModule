using System;
using UnityEditor;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

namespace CizaLocaleModule.Editor
{
	public abstract class BBoxVE : VisualElement
	{
		// VARIABLE: -----------------------------------------------------------------------------

		[NonSerialized]
		protected readonly VisualElement _head = new VisualElement();

		[NonSerialized]
		protected readonly VisualElement _headLeftAlignment = new FlexibleSpaceVE() { style = { flexDirection = FlexDirection.Row } };

		[NonSerialized]
		protected readonly VisualElement _headRightAlignment = new VisualElement() { style = { flexDirection = FlexDirection.RowReverse, flexShrink = 0 } };

		[NonSerialized]
		protected readonly VisualElement _body = new VisualElement();

		[NonSerialized]
		protected readonly VisualElement _foot = new VisualElement();

		[field: NonSerialized]
		protected string Title { get; private set; }

		[field: NonSerialized]
		protected IContent Content { get; private set; }


		protected virtual string[] USSPaths => Array.Empty<string>();
		protected virtual string[] RootClasses => Array.Empty<string>();
		protected virtual string[] HeadClasses => Array.Empty<string>();
		protected virtual string[] BodyClasses => Array.Empty<string>();
		protected virtual string[] FootClasses => Array.Empty<string>();

		// PUBLIC VARIABLE: ---------------------------------------------------------------------

		[field: NonSerialized]
		public virtual bool IsInitialized { get; protected set; }


		// CONSTRUCTOR: --------------------------------------------------------------------- 

		[Preserve]
		protected BBoxVE()
		{
			Add(_head);
			Add(_body);
			Add(_foot);
		}

		// PUBLIC METHOD: ----------------------------------------------------------------------

		public void Initialize(string title, IContent content)
		{
			if (IsInitialized) return;
			IsInitialized = true;
			DerivedInitialize(title, content);
			Refresh();
		}

		public abstract void Refresh();

		// PROTECT METHOD: --------------------------------------------------------------------

		protected virtual void DerivedInitialize(string title, IContent content)
		{
			Title = title;

			_head.Add(_headLeftAlignment);
			_head.Add(_headRightAlignment);

			Content = content;
			_body.Add(Content?.Body);

			foreach (var styleSheet in StyleSheetUtils.GetStyleSheets(USSPaths))
				styleSheets.Add(styleSheet);

			foreach (var rootClass in RootClasses)
				AddToClassList(rootClass);

			foreach (var headClass in HeadClasses)
				_head.AddToClassList(headClass);

			foreach (var bodyClass in BodyClasses)
				_body.AddToClassList(bodyClass);

			foreach (var footClass in FootClasses)
				_foot.AddToClassList(footClass);

			_head.AddManipulator(new Clickable(OnHeadClick));
		}

		protected void AddHeadManipulator(IManipulator manipulator) => _head.AddManipulator(manipulator);
		protected void AddHeadLeftContent(VisualElement content) => _headLeftAlignment.Add(content);
		protected void AddHeadRightContent(VisualElement content) => _headRightAlignment.Add(content);

		protected abstract void OnHeadClick();

		public interface IContent
		{
			VisualElement Body { get; }

			void Refresh();
		}

		public class ContentVE : VisualElement, IContent
		{
			public VisualElement Body => this;
			public virtual void Refresh() { }
		}

		public class PropertyContentVE : ContentVE
		{
			public SerializedProperty Property { get; }

			[Preserve]
			public PropertyContentVE(SerializedProperty property) =>
				Property = property;

			public override void Refresh()
			{
				Clear();
				SerializationUtils.CreateChildProperties(this, Property, SerializationUtils.ChildrenKinds.ShowLabelsInChildren, 0);
			}
		}
	}
}