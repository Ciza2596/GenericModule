using System;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

namespace CizaInputModule.Editor
{
    public class PageBarVE : VisualElement
	{
		// VARIABLE: -----------------------------------------------------------------------------

		[NonSerialized]
		protected readonly Button _firstButton = new Button();

		[NonSerialized]
		protected readonly Button _previousButton = new Button();

		[NonSerialized]
		protected readonly Button _nextButton = new Button();

		[NonSerialized]
		protected readonly Button _lastButton = new Button();

		[NonSerialized]
		protected readonly IntegerField _pageField = new IntegerField();
		
		[NonSerialized]
		protected readonly Label _pageCountLabel = new Label();

		protected virtual string FirstText => "|←";
		protected virtual string PreviousText => "←";
		protected virtual string NextText => "→";
		protected virtual string LastText => "→|";
		
		protected virtual string[] USSPaths => new[] { "PageBar" };

		protected virtual string[] PageBarClasses => new[] { "pagebar" };

		protected virtual string FirstButtonTooltip => "First Page";
		protected virtual string PreviousButtonTooltip => "Previous Page";
		protected virtual string NextButtonTooltip => "Next Page";
		protected virtual string LastButtonTooltip => "Last Page";
		
		protected virtual string PageCountFormat => "/ {0}";

		// EVENT: ---------------------------------------------------------------------------------
		
		public virtual event Action OnFirstPage;
		public virtual event Action OnPreviousPage;
		public virtual event Action OnNextPage;
		public virtual event Action OnLastPage;
		public virtual event Action<int> OnSetPage;
		
		protected virtual Func<VisualElement, bool> CheckCanAutoTurnPage { get; set; }
		
		// PUBLIC VARIABLE: ---------------------------------------------------------------------

		public virtual int Page { get; protected set; }
		public virtual int PageCount { get; protected set; } = 1;
		
		// CONSTRUCTOR: --------------------------------------------------------------------- 
		
		[Preserve]
		public PageBarVE() { }

		// LIFECYCLE METHOD: ------------------------------------------------------------------
		
		public virtual void Initialize(int page, int pageCount, Func<VisualElement, bool> checkCanAutoTurnPage)
		{
			CheckCanAutoTurnPage = checkCanAutoTurnPage;
			
			this.SetIsVisible(false);

			foreach (var styleSheet in StyleSheetUtils.GetStyleSheets(USSPaths))
				styleSheets.Add(styleSheet);
			
			foreach (var c in PageBarClasses)
				AddToClassList(c);

			_firstButton.clicked += FirstPage;
			_firstButton.text = FirstText;
			_firstButton.tooltip = FirstButtonTooltip;
			_firstButton.AddManipulator(CreatePageButtonHoverManipulator(FirstPage));

			_previousButton.clicked += PreviousPage;
			_previousButton.text = PreviousText;
			_previousButton.tooltip = PreviousButtonTooltip;
			_previousButton.AddManipulator(CreatePageButtonHoverManipulator(PreviousPage));

			_nextButton.clicked += NextPage;
			_nextButton.text = NextText;
			_nextButton.tooltip = NextButtonTooltip;
			_nextButton.AddManipulator(CreatePageButtonHoverManipulator(NextPage));

			_lastButton.clicked += LastPage;
			_lastButton.text = LastText;
			_lastButton.tooltip = LastButtonTooltip;
			_lastButton.AddManipulator(CreatePageButtonHoverManipulator(LastPage));

			_pageField.RegisterValueChangedCallback(OnPageValueChanged);
			_pageField.isDelayed = true;
			SetPageCount(pageCount);
			SetPage(page, false);

			Add(_firstButton);
			Add(_previousButton);
			Add(_pageField);
			Add(_pageCountLabel);
			Add(_nextButton);
			Add(_lastButton);
		}

		// PUBLIC METHOD: ----------------------------------------------------------------------
		
		public virtual void SetPage(int page, bool isTriggerCallback)
		{
			Page = Mathf.Clamp(page, 1, PageCount);
			_pageField.SetValueWithoutNotify(Page);
			RefreshButtons();
			
			if (isTriggerCallback)
				OnSetPage?.Invoke(Page);
		}

		public virtual void Refresh(int page, int pageCount, bool isVisible)
		{
			SetPageCount(pageCount);
			SetPage(page, false);
			this.SetIsVisible(isVisible);
		}

		// PROTECT METHOD: --------------------------------------------------------------------
		
		protected virtual void SetPageCount(int pageCount)
		{
			PageCount = Math.Max(1, pageCount);
			_pageCountLabel.text = string.Format(PageCountFormat, PageCount);
		}
		
		protected virtual void FirstPage() => 
			OnFirstPage?.Invoke();

		protected virtual void PreviousPage() => 
			OnPreviousPage?.Invoke();
		
		protected virtual void NextPage() => 
			OnNextPage?.Invoke();
		
		protected virtual void LastPage() => 
			OnLastPage?.Invoke();

		protected virtual void OnPageValueChanged(ChangeEvent<int> @event) =>
			SetPage(@event.newValue, true);

		protected virtual void RefreshButtons()
		{
			_firstButton.SetEnabled(Page > 1);
			_previousButton.SetEnabled(Page > 1);
			_nextButton.SetEnabled(Page < PageCount);
			_lastButton.SetEnabled(Page < PageCount);
		}
		
		protected virtual PageButtonHoverManipulator CreatePageButtonHoverManipulator(Action onHover) =>
			new PageButtonHoverManipulator(this, onHover, CheckCanAutoTurnPage);
    }
}
