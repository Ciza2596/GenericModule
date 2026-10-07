using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace CizaInputModule.Editor
{
	public class ImportMenuWindow : EditorWindow
	{
		// CONST & STATIC: -----------------------------------------------------------------------

		protected static ImportMenuWindow Window;
		protected static EditorApplication.CallbackFunction _pendingOpen;
		protected static VisualElement _pendingContent;
		protected static int _openVersion;

		[InitializeOnLoadMethod]
		protected static void InitializeWindowLifecycle()
		{
			AssemblyReloadEvents.beforeAssemblyReload += CloseAll;
			EditorApplication.delayCall += CloseOrphanedWindows;
		}

		protected static void CloseOrphanedWindows()
		{
			foreach (var window in Resources.FindObjectsOfTypeAll<ImportMenuWindow>())
				if (window._content == null)
					window.CloseWindow();
		}

		protected static void CancelPendingOpen()
		{
			_openVersion++;
			if (_pendingOpen != null)
				EditorApplication.delayCall -= _pendingOpen;
			_pendingOpen = null;
			_pendingContent = null;
		}

		protected static void CloseAll()
		{
			CancelPendingOpen();
			foreach (var window in Resources.FindObjectsOfTypeAll<ImportMenuWindow>())
				window.CloseWindow();
			Window = null;
		}

		// VARIABLE: -----------------------------------------------------------------------------

		[NonSerialized]
		protected VisualElement _content;

		[NonSerialized]
		protected ScrollView _contentViewport;

		[NonSerialized]
		protected VisualElement _trigger;

		[NonSerialized]
		protected EditorWindow _triggerOwner;

		[NonSerialized]
		protected Rect _triggerRectInOwner;

		[NonSerialized]
		protected float _triggerRightInset;

		[NonSerialized]
		protected bool _hasTriggerRectInOwner;

		[NonSerialized]
		protected float _contentHeight;

		[NonSerialized]
		protected Rect _dropDownPosition;

		[NonSerialized]
		protected bool _hasDropDownPosition;

		[NonSerialized]
		protected bool _isWindowUpdateQueued;

		[NonSerialized]
		protected bool _isClosing;

		// PUBLIC METHOD: ------------------------------------------------------------------------

		public static bool CheckIsOpen(VisualElement content) =>
			content != null && (_pendingContent == content || (Window != null && !Window._isClosing && Window._content == content));

		public static bool TryGetActivatorRect(VisualElement trigger, out Rect activatorRect)
		{
			if (trigger?.panel == null)
			{
				activatorRect = default;
				return false;
			}

			foreach (var owner in Resources.FindObjectsOfTypeAll<EditorWindow>())
			{
				if (!owner.rootVisualElement.Contains(trigger))
					continue;

				activatorRect = trigger.worldBound;
				if (!TryGetScreenRect(owner, out var ownerRect))
					return false;
				activatorRect.position += ownerRect.position;
				return true;
			}

			activatorRect = default;
			return false;
		}

		public static void Open(VisualElement trigger, Rect activatorRect, VisualElement content)
		{
			if (trigger?.panel == null || content == null)
				return;

			CloseAll();
			var version = _openVersion;
			_pendingContent = content;
			_pendingOpen = () =>
			{
				if (version != _openVersion)
					return;
				_pendingOpen = null;
				_pendingContent = null;
				if (TryGetActivatorRect(trigger, out var currentActivatorRect))
				{
					Show(currentActivatorRect, content);
					Window._trigger = trigger;
					Window.CacheTriggerOwner();
					Window.CacheTriggerRect();
					EditorApplication.update += Window.QueueWindowUpdate;
				}
			};
			EditorApplication.delayCall += _pendingOpen;
		}

		protected static void Show(Rect activatorRect, VisualElement content)
		{
			Window = CreateInstance<ImportMenuWindow>();
			Window._content = content;
			Window._contentHeight = 1f;
			Window._dropDownPosition = new Rect(activatorRect.xMin, activatorRect.yMax, activatorRect.width, Window._contentHeight);

			Window.minSize = Vector2.one;
			Window.maxSize = new Vector2(100000f, 100000f);
			Window.position = Window._dropDownPosition;
			Window.ShowPopup();
			SetPosition(Window, Window._dropDownPosition);
			Window._hasDropDownPosition = true;
			Window.ResizeToContent();
		}

		public static void Close(VisualElement content)
		{
			if (content == null)
				return;
			if (_pendingContent == content)
				CancelPendingOpen();
			foreach (var window in Resources.FindObjectsOfTypeAll<ImportMenuWindow>())
				if (window._content == content)
					window.OnRequestClose();
		}

		// PROTECT METHOD: -----------------------------------------------------------------------

		protected virtual void CreateGUI()
		{
			if (_content == null || _isClosing)
				return;

			_content.RemoveFromHierarchy();
			_content.style.flexGrow = 0;
			_content.style.flexShrink = 0;
			_content.UnregisterCallback<GeometryChangedEvent>(OnContentGeometryChanged);
			_content.RegisterCallback<GeometryChangedEvent>(OnContentGeometryChanged);
			if (_contentViewport == null)
			{
				_contentViewport = new ScrollView(ScrollViewMode.Vertical);
				_contentViewport.style.flexGrow = 1;
				_contentViewport.style.minHeight = 0;
				rootVisualElement.Add(_contentViewport);
			}

			_contentViewport.Add(_content);

			if (_content is BImportMenuVE importMenu)
			{
				importMenu.OnRequestClose -= OnRequestClose;
				importMenu.OnRequestClose += OnRequestClose;
			}
		}

		protected virtual void StopContentCallbacks()
		{
			EditorApplication.update -= QueueWindowUpdate;
			EditorApplication.delayCall -= UpdateWindowAfterInspectors;
			_isWindowUpdateQueued = false;
			_trigger = null;
			_triggerOwner = null;
			_hasTriggerRectInOwner = false;
			_hasDropDownPosition = false;
			_content?.UnregisterCallback<GeometryChangedEvent>(OnContentGeometryChanged);

			if (_content is BImportMenuVE importMenu)
				importMenu.OnRequestClose -= OnRequestClose;
		}

		protected virtual void OnDestroy()
		{
			EditorApplication.delayCall -= CloseWindow;
			StopContentCallbacks();
			_content = null;
			if (Window == this)
				Window = null;
		}

		protected virtual void OnRequestClose()
		{
			if (_isClosing)
				return;
			_isClosing = true;
			StopContentCallbacks();
			EditorApplication.delayCall += CloseWindow;
		}

		protected virtual void CloseWindow()
		{
			EditorApplication.delayCall -= CloseWindow;
			if (this == null)
				return;
			_isClosing = true;
			StopContentCallbacks();
			base.Close();
		}

		protected virtual void OnContentGeometryChanged(GeometryChangedEvent geometryChangedEvent)
		{
			if (_isClosing || geometryChangedEvent.target != _content)
				return;

			var height = Mathf.Ceil(geometryChangedEvent.newRect.height);
			if (Mathf.Approximately(_contentHeight, height))
				return;
			_contentHeight = height;
			QueueWindowUpdate();
		}

		protected virtual void QueueWindowUpdate()
		{
			if (this == null || _isClosing || !_hasDropDownPosition || _isWindowUpdateQueued)
				return;
			_isWindowUpdateQueued = true;
			EditorApplication.delayCall += UpdateWindowAfterInspectors;
		}

		protected virtual void UpdateWindowAfterInspectors()
		{
			_isWindowUpdateQueued = false;
			if (this == null || _isClosing || !_hasDropDownPosition)
				return;

			UpdateActivatorRect();
			ResizeToContent();
		}

		protected virtual void UpdateActivatorRect()
		{
			if (this == null || _isClosing || !_hasDropDownPosition)
				return;

			var isTriggerAttached = _trigger?.panel != null;
			if (isTriggerAttached && (_triggerOwner == null || !_triggerOwner.rootVisualElement.Contains(_trigger)))
				CacheTriggerOwner();
			if (_triggerOwner == null)
				return;

			if (!TryGetScreenRect(_triggerOwner, out var ownerRect))
				return;

			Rect activatorRect;
			if (isTriggerAttached && _triggerOwner.rootVisualElement.Contains(_trigger))
			{
				activatorRect = _trigger.worldBound;
				CacheTriggerRect(ownerRect, activatorRect);
			}
			else
			{
				if (!_hasTriggerRectInOwner)
					return;

				activatorRect = _triggerRectInOwner;
				activatorRect.width = ownerRect.width - activatorRect.xMin - _triggerRightInset;
			}

			activatorRect.position += ownerRect.position;
			if (activatorRect.width <= 0f || !float.IsFinite(activatorRect.width) || !float.IsFinite(activatorRect.xMin) || !float.IsFinite(activatorRect.yMax))
				return;

			if (Mathf.Approximately(_dropDownPosition.x, activatorRect.xMin) && Mathf.Approximately(_dropDownPosition.y, activatorRect.yMax) && Mathf.Approximately(_dropDownPosition.width, activatorRect.width))
				return;

			_dropDownPosition.position = new Vector2(activatorRect.xMin, activatorRect.yMax);
			_dropDownPosition.width = activatorRect.width;
		}

		protected virtual void CacheTriggerOwner()
		{
			_triggerOwner = null;
			foreach (var owner in Resources.FindObjectsOfTypeAll<EditorWindow>())
				if (owner.rootVisualElement.Contains(_trigger))
				{
					_triggerOwner = owner;
					break;
				}
		}

		protected virtual void CacheTriggerRect()
		{
			if (_trigger?.panel == null || _triggerOwner == null || !_triggerOwner.rootVisualElement.Contains(_trigger))
				return;
			if (!TryGetScreenRect(_triggerOwner, out var ownerRect))
				return;

			CacheTriggerRect(ownerRect, _trigger.worldBound);
		}

		protected virtual void CacheTriggerRect(Rect ownerRect, Rect triggerRect)
		{
			if (triggerRect.width <= 0f || triggerRect.height <= 0f || !float.IsFinite(triggerRect.xMin) || !float.IsFinite(triggerRect.yMin) || !float.IsFinite(triggerRect.xMax))
				return;

			_triggerRectInOwner = triggerRect;
			_triggerRightInset = Mathf.Max(0f, ownerRect.width - triggerRect.xMax);
			_hasTriggerRectInOwner = true;
		}

		protected virtual void ResizeToContent()
		{
			if (this == null || _isClosing || !_hasDropDownPosition || _contentHeight <= 0f || float.IsNaN(_contentHeight) || float.IsInfinity(_contentHeight))
				return;

			_dropDownPosition.height = Mathf.Ceil(_contentHeight);
			var targetPosition = _dropDownPosition;
			var visibleRect = GetVisibleRect(this, targetPosition);
			targetPosition.height = Mathf.Min(targetPosition.height, Mathf.Max(1f, visibleRect.yMax - targetPosition.y));
			var pixelsPerPoint = _triggerOwner != null ? _triggerOwner.rootVisualElement.scaledPixelsPerPoint : rootVisualElement.scaledPixelsPerPoint;
			targetPosition.x = Mathf.Round(targetPosition.x * pixelsPerPoint) / pixelsPerPoint;
			targetPosition.y = Mathf.Round(targetPosition.y * pixelsPerPoint) / pixelsPerPoint;
			targetPosition.width = Mathf.Round(targetPosition.width * pixelsPerPoint) / pixelsPerPoint;
			SetPosition(this, targetPosition);
		}

		#region Native Window

		protected static Rect GetVisibleRect(EditorWindow window, Rect rect)
		{
			var view = TypeUtils.GetFieldInfo(typeof(EditorWindow), "m_Parent").GetValue(window);
			var container = TypeUtils.GetPropertyInfo(view.GetType(), "window").GetValue(view);
			var containerType = TypeUtils.GetType(typeof(EditorWindow).Assembly, "UnityEditor.ContainerWindow");
			var fitMethod = TypeUtils.GetMethodInfo(containerType, "FitRectToScreen", typeof(Rect), typeof(Vector2), typeof(bool), containerType);
			return (Rect)fitMethod.Invoke(null, new object[] { rect, rect.position, true, container });
		}

		protected static bool TryGetScreenRect(EditorWindow window, out Rect rect)
		{
			var view = TypeUtils.GetFieldInfo(typeof(EditorWindow), "m_Parent").GetValue(window) as UnityEngine.Object;
			if (view == null)
			{
				rect = default;
				return false;
			}

			rect = (Rect)TypeUtils.GetPropertyInfo(view.GetType(), "screenPosition").GetValue(view);
			return true;
		}

		protected static void SetPosition(EditorWindow window, Rect rect)
		{
			var view = TypeUtils.GetFieldInfo(typeof(EditorWindow), "m_Parent").GetValue(window) as UnityEngine.Object;
			if (view == null)
				return;
			var container = TypeUtils.GetPropertyInfo(view.GetType(), "window").GetValue(view) as UnityEngine.Object;
			if (container == null)
				return;
			var positionProperty = TypeUtils.GetPropertyInfo(container.GetType(), "position");
			if ((Rect)positionProperty.GetValue(container) == rect)
				return;

			positionProperty.SetValue(container, rect);
			TypeUtils.GetMethodInfo(container.GetType(), "OnResize").Invoke(container, null);
			TypeUtils.GetFieldInfo(typeof(EditorWindow), "m_Pos").SetValue(window, rect);
		}

		#endregion
	}
}