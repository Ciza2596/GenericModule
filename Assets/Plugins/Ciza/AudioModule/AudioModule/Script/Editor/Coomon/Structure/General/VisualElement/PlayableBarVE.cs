using System;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

namespace CizaAudioModule.Editor
{
	public class PlayableBarVE : VisualElement
	{
		// VARIABLE: -----------------------------------------------------------------------------
		[NonSerialized]
		protected readonly Button _playButton = new Button();

		[NonSerialized]
		protected readonly Button _previousButton = new Button();

		[NonSerialized]
		protected readonly Button _nextButton = new Button();

		[NonSerialized]
		protected readonly Button _loopButton = new Button() { style = { fontSize = 16, unityFontStyleAndWeight = FontStyle.Bold, paddingTop = 4, paddingBottom = 2 } };

		protected readonly IntegerField _frameField = new IntegerField();

		protected virtual string PlayText => "\u25b6";
		protected virtual string PauseText => "\u2590\u2590<color=transparent>/</color>";
		protected virtual string PreviousText => "<size=10><b>|</b>\u25c0</size>";
		protected virtual string NextText => "<size=10>\u25b6<b>|</b></size>";
		protected virtual string LoopText => "↺";

		protected virtual string[] USSPaths => new[] { "PlayableBar" };

		protected virtual string[] PlayableBarClasses => new[] { "playablebar" };
		protected virtual string CheckedClass => "checked";

		protected virtual string PlayButtonTooltip => "Play";
		protected virtual string PauseButtonTooltip => "Pause";
		protected virtual string PreviousButtonTooltip => "Previous Frame";
		protected virtual string NextButtonTooltip => "Next Frame";
		protected virtual string LoopButtonTooltip => "Loop";

		public event Action<bool> OnSetIsPlaying;
		public event Action OnPrevious;
		public event Action OnNext;
		public event Action<bool> OnSetIsLoop;
		public event Action<int> OnSetFrame;

		public bool IsPlaying { get; protected set; }
		public bool IsLoop { get; protected set; }

		public int Frame { get; protected set; }


		// CONSTRUCTOR: --------------------------------------------------------------------- 

		[Preserve]
		public PlayableBarVE() { }

		// PUBLIC VARIABLE: ---------------------------------------------------------------------

		public virtual void Initialize(bool isShowFrame)
		{
			foreach (var styleSheet in StyleSheetUtils.GetStyleSheets(USSPaths))
				styleSheets.Add(styleSheet);

			foreach (var c in PlayableBarClasses)
				AddToClassList(c);

			_playButton.clicked += ToggleIsPlaying;
			_playButton.text = PlayText;
			_playButton.tooltip = PlayButtonTooltip;
			_playButton.AddToClassList(CheckedClass);
			_playButton.EnableInClassList(CheckedClass, IsPlaying);

			_previousButton.clicked += () => OnPrevious?.Invoke();
			_previousButton.text = PreviousText;
			_previousButton.tooltip = PreviousButtonTooltip;

			_nextButton.clicked += () => OnNext?.Invoke();
			_nextButton.text = NextText;
			_nextButton.tooltip = NextButtonTooltip;

			_loopButton.clicked += ToggleIsLoop;
			_loopButton.text = LoopText;
			_loopButton.tooltip = LoopButtonTooltip;
			_loopButton.AddToClassList(CheckedClass);
			_loopButton.EnableInClassList(CheckedClass, IsLoop);

			_frameField.RegisterValueChangedCallback(OnFrameValueChanged);

			Add(_previousButton);
			Add(_playButton);
			Add(_nextButton);
			Add(_loopButton);
			if (isShowFrame)
				Add(_frameField);
		}

		public virtual void ToggleIsPlaying() =>
			SetIsPlaying(!IsPlaying);

		public virtual void SetIsPlaying(bool isPlaying)
		{
			IsPlaying = isPlaying;
			_playButton.text = IsPlaying ? PauseText : PlayText;
			_playButton.style.fontSize = IsPlaying ? 10 : 12;
			_playButton.tooltip = IsPlaying ? PauseButtonTooltip : PlayButtonTooltip;
			_playButton.EnableInClassList(CheckedClass, IsPlaying);
			OnSetIsPlaying?.Invoke(IsPlaying);
		}

		public virtual void ToggleIsLoop() =>
			SetIsLoop(!IsLoop);

		public virtual void SetIsLoop(bool isLoop)
		{
			IsLoop = isLoop;
			_loopButton.EnableInClassList(CheckedClass, IsLoop);
			OnSetIsLoop?.Invoke(IsLoop);
		}

		public virtual void SetFrame(int frame) =>
			SetFrame(frame, true);


		public virtual void SetFrame(int frame, bool isTriggerCallback)
		{
			Frame = frame;
			_frameField.SetValueWithoutNotify(Frame);
			if (isTriggerCallback)
				OnSetFrame?.Invoke(Frame);
		}

		protected virtual void OnFrameValueChanged(ChangeEvent<int> @event) =>
			SetFrame(@event.newValue);
	}
}