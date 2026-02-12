using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Scripting;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace CizaInputModule
{
	public class VirtualMouseContainer
	{
		protected readonly Dictionary<int, IVirtualMouse> _virtualMouseMapByPlayerIndex = new Dictionary<int, IVirtualMouse>();
		protected readonly IVirtualMouseContainerConfig _config;

		protected Canvas _canvas;

		// EVENT: ---------------------------------------------------------------------------------

		// PlayerIndex, CurrentActionMapDataId, Returns Path
		public event Func<int, string, string> GetMoveActionPath;

		// PlayerIndex, CurrentActionMapDataId, Returns Path
		public event Func<int, string, string> GetLeftButtonActionPath;

		// PlayerIndex, CurrentActionMapDataId, Returns Path
		public event Func<int, string, string> GetRightButtonActionPath;

		// PlayerIndex, CurrentActionMapDataId, Returns Path
		public event Func<int, string, string> GetScrollActionPath;

		// PUBLIC VARIABLE: ---------------------------------------------------------------------
		public virtual bool IsInitialized => _canvas != null;

		public virtual bool CanEnableVirtualMouse { get; protected set; }

		public virtual int[] PlayerIndexList => _virtualMouseMapByPlayerIndex.Keys.ToArray();

		public virtual bool TryGetVirtualMouseReadModel(int playerIndex, out IVirtualMouseReadModel virtualMouseReadModel)
		{
			if (!TryGetVirtualMouse(playerIndex, out var virtualMouse))
			{
				virtualMouseReadModel = null;
				return false;
			}

			virtualMouseReadModel = virtualMouse;
			return true;
		}


		// CONSTRUCTOR: ------------------------------------------------------------------------

		[Preserve]
		public VirtualMouseContainer(IVirtualMouseContainerConfig config)
		{
			_config = config;
		}

		// LIFECYCLE METHOD: ------------------------------------------------------------------

		public virtual void Initialize(Transform parent)
		{
			if (IsInitialized)
				return;

			if (_config.CanvasPrefab == null)
			{
				Debug.LogWarning($"[CizaInputModule.VirtualMouseContainer::Initialize] No virtual mouse canvas prefab.");
				return;
			}

			var canvas = Object.Instantiate(_config.CanvasPrefab, parent).GetComponent<Canvas>();

			var canvasScaler = canvas.gameObject.GetComponent<CanvasScaler>();
			if (canvasScaler.uiScaleMode != CanvasScaler.ScaleMode.ScaleWithScreenSize)
				canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
			canvasScaler.referenceResolution = _config.ReferenceResolution;

			_canvas = canvas;
		}

		public virtual void Release()
		{
			if (!IsInitialized)
				return;

			DestroyAll();

			var canvas = _canvas;
			_canvas = null;
			Object.DestroyImmediate(canvas.gameObject);
		}

		// PUBLIC METHOD: ----------------------------------------------------------------------

		public virtual void SetCanEnableVirtualMouse(bool canEnableVirtualMouse)
		{
			if (!IsInitialized)
				return;

			CanEnableVirtualMouse = canEnableVirtualMouse;
			if (!CanEnableVirtualMouse)
				DisableAll();
		}

		public virtual void Create(PlayerInput playerInput)
		{
			if (!IsInitialized)
				return;

			if (playerInput == null)
				return;

			var playerIndex = playerInput.playerIndex;
			if (_virtualMouseMapByPlayerIndex.ContainsKey(playerIndex))
				Destroy(playerIndex);

			if (!_config.TryGetInfo(playerIndex, out var info))
			{
				Debug.LogWarning($"[CizaInputModule.VirtualMouseContainer::AddVirtualMouse] VirtualMouseInfo was not found for PlayerIndex: {playerIndex}");
				return;
			}

			var virtualMouse = Activator.CreateInstance(info.VirtualMouseType, info, _canvas, playerInput, _config.ReferenceResolution) as IVirtualMouse;
			virtualMouse.Initialize(_config.MoveSensitivity, _config.ScrollSensitivity, _config.IsScreenPaddingByRatio, _config.ScreenPadding);
			virtualMouse.GetMoveActionPath += GetMoveActionPathImp;
			virtualMouse.GetLeftButtonActionPath += GetLeftButtonActionPathImp;
			virtualMouse.GetRightButtonActionPath += GetRightButtonActionPathImp;
			virtualMouse.GetScrollActionPath += GetScrollActionPathImp;
			_virtualMouseMapByPlayerIndex.Add(playerIndex, virtualMouse);
		}


		public virtual void DestroyAll()
		{
			if (!IsInitialized)
				return;

			foreach (var playerIndex in PlayerIndexList)
				Destroy(playerIndex);
		}

		public virtual void Destroy(int playerIndex)
		{
			if (!IsInitialized)
				return;

			if (_virtualMouseMapByPlayerIndex.TryGetValue(playerIndex, out var virtualMouse))
				virtualMouse.Release();

			_virtualMouseMapByPlayerIndex.Remove(playerIndex);
		}


		public virtual void Enable(int playerIndex)
		{
			if (!IsInitialized || !TryGetVirtualMouse(playerIndex, out var virtualMouse))
				return;

			virtualMouse.Enable();
		}

		public virtual void EnableAll()
		{
			if (!IsInitialized)
				return;

			foreach (var playerIndex in PlayerIndexList)
				Enable(playerIndex);
		}

		public virtual void Disable(int playerIndex)
		{
			if (!IsInitialized || !TryGetVirtualMouse(playerIndex, out var virtualMouse))
				return;

			virtualMouse.Disable();
		}

		public virtual void DisableAll()
		{
			if (!IsInitialized)
				return;

			foreach (var playerIndex in PlayerIndexList)
				Disable(playerIndex);
		}

		public virtual void SetMoveSensitivity(int playerIndex, float moveSensitivity)
		{
			if (!IsInitialized || !TryGetVirtualMouse(playerIndex, out var virtualMouse))
				return;

			virtualMouse.SetMoveSensitivity(moveSensitivity);
		}

		public virtual void SetMoveSensitivity(float moveSensitivity)
		{
			if (!IsInitialized)
				return;

			foreach (var playerIndex in PlayerIndexList)
				SetMoveSensitivity(playerIndex, moveSensitivity);
		}

		public virtual void SetScrollSensitivity(int playerIndex, float scrollSensitivity)
		{
			if (!IsInitialized || !TryGetVirtualMouse(playerIndex, out var virtualMouse))
				return;

			virtualMouse.SetScrollSensitivity(scrollSensitivity);
		}

		public virtual void SetScrollSensitivity(float scrollSensitivity)
		{
			if (!IsInitialized)
				return;

			foreach (var playerIndex in PlayerIndexList)
				SetScrollSensitivity(playerIndex, scrollSensitivity);
		}

		public virtual void SetScreenPadding(int playerIndex, bool isByRatio, RectOffset padding)
		{
			if (!IsInitialized || !TryGetVirtualMouse(playerIndex, out var virtualMouse))
				return;

			virtualMouse.SetScreenPadding(isByRatio, padding);
		}

		public virtual void SetScreenPadding(bool isByRatio, RectOffset padding)
		{
			if (!IsInitialized)
				return;

			foreach (var playerIndex in PlayerIndexList)
				SetScreenPadding(playerIndex, isByRatio, padding);
		}

		public virtual void SetScreenPadding(int playerIndex, RectOffset padding)
		{
			if (!IsInitialized || !TryGetVirtualMouse(playerIndex, out var virtualMouse))
				return;

			virtualMouse.SetScreenPadding(virtualMouse.IsScreenPaddingByRatio, padding);
		}

		public virtual void SetScreenPadding(RectOffset padding)
		{
			if (!IsInitialized)
				return;

			foreach (var playerIndex in PlayerIndexList)
				SetScreenPadding(playerIndex, padding);
		}


		public virtual void SyncHardwareMouseToVirtualMousePosition(int playerIndex)
		{
			if (!IsInitialized || !InputSystemUtils.TryGetHardwareMouse(out var hardwareMouse) || !_virtualMouseMapByPlayerIndex.TryGetValue(playerIndex, out var virtualMouse) || !virtualMouse.IsEnable)
				return;

			var virtualMousePosition = virtualMouse.Mouse.position.ReadValue();
			hardwareMouse.WarpCursorPosition(virtualMousePosition);
		}

		public virtual void SyncVirtualMouseToHardwareMousePosition(int playerIndex)
		{
			if (!IsInitialized || !InputSystemUtils.TryGetHardwareMouse(out var hardwareMouse) || !_virtualMouseMapByPlayerIndex.TryGetValue(playerIndex, out var virtualMouse) || !virtualMouse.IsEnable)
				return;

			var hardwareMousePosition = hardwareMouse.position.ReadValue();
			virtualMouse.AnchorVirtualMouseToPosition(hardwareMousePosition);
		}


		// PROTECT METHOD: --------------------------------------------------------------------

		protected virtual bool TryGetVirtualMouse(int playerIndex, out IVirtualMouse virtualMouse) =>
			_virtualMouseMapByPlayerIndex.TryGetValue(playerIndex, out virtualMouse);

		protected virtual string GetMoveActionPathImp(int playerIndex, string currentActionMapDataId) =>
			GetMoveActionPath?.Invoke(playerIndex, currentActionMapDataId);

		protected virtual string GetLeftButtonActionPathImp(int playerIndex, string currentActionMapDataId) =>
			GetLeftButtonActionPath?.Invoke(playerIndex, currentActionMapDataId);

		protected virtual string GetRightButtonActionPathImp(int playerIndex, string currentActionMapDataId) =>
			GetRightButtonActionPath?.Invoke(playerIndex, currentActionMapDataId);

		protected virtual string GetScrollActionPathImp(int playerIndex, string currentActionMapDataId) =>
			GetScrollActionPath?.Invoke(playerIndex, currentActionMapDataId);
	}
}