using System;
using UnityEngine;

namespace CizaInputModule
{
	[Serializable]
	public class VirtualMouseContainerConfig : IVirtualMouseContainerConfig
	{
		[SerializeField]
		private Vector2Int _referenceResolution = new Vector2Int(1920, 1080);

		[Space]
		[SerializeField]
		private float _moveSensitivity = 700f;

		[SerializeField]
		private float _scrollSensitivity = 0.2f;

		[Space]
		[SerializeField]
		private ScreenPaddingEnabler _hasScreenPadding;

		[Space]
		[Space]
		[SerializeField]
		private GameObject _virtualMouseCanvasPrefab;

		[Space]
		[SerializeField]
		private VirtualMouseInfoMapList _virtualMouseInfoMapList;

		public virtual Vector2Int ReferenceResolution => _referenceResolution;

		public virtual float MoveSensitivity => _moveSensitivity;
		public virtual float ScrollSensitivity => _scrollSensitivity;

		public virtual bool IsScreenPaddingByRatio => !_hasScreenPadding.TryGetValue(out var screenPadding) || screenPadding.IsByRatio;
		public virtual RectOffset ScreenPadding => _hasScreenPadding.TryGetValue(out var screenPadding) ? screenPadding?.Padding ?? new RectOffset() : new RectOffset();


		public virtual GameObject CanvasPrefab => _virtualMouseCanvasPrefab;

		public virtual bool TryGetInfo(int playerIndex, out IVirtualMouseInfo info)
		{
			if (!_virtualMouseInfoMapList.TryGetValue(playerIndex.ToString(), out var infoImp))
			{
				info = null;
				return false;
			}

			info = infoImp;
			return true;
		}
	}
}