using System;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

namespace CizaAudioModule.Editor
{
	public abstract class BBoxVE : VisualElement
	{
		[NonSerialized]
		private readonly VisualElement _head = new VisualElement();
		
		[NonSerialized]
		private readonly VisualElement _headLeftAlignment = new FlexibleSpaceVE() { style = { flexDirection = FlexDirection.Row } };
		[NonSerialized]
		private readonly VisualElement _headRightAlignment = new VisualElement() { style = { flexDirection = FlexDirection.RowReverse } };

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

		[field: NonSerialized]
		public bool IsInitialized { get; private set; }


		[Preserve]
		protected BBoxVE()
		{
			Add(_head);
			Add(_body);
			Add(_foot);
		}

		public void Initialize(string title, IContent content)
		{
			if (IsInitialized) return;
			IsInitialized = true;
			DerivedInitialize(title, content);
			Refresh();
		}

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

		public abstract void Refresh();

		protected void AddHeadMainpulator(IManipulator manipulator) => _head.AddManipulator(manipulator);
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
	}
}