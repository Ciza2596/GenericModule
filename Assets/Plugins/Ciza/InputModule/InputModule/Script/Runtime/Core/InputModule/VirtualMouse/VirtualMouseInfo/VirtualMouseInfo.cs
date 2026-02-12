using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace CizaInputModule
{
	[Serializable]
	public class VirtualMouseInfo : IVirtualMouseInfo
	{
		// VARIABLE: -----------------------------------------------------------------------------

		[SerializeField]
		private int _playerIndex;

		[SerializeField]
		private GameObject _virtualMousePrefab;

		// PUBLIC VARIABLE: ---------------------------------------------------------------------

		public virtual int PlayerIndex => _playerIndex;

		public virtual Type VirtualMouseType => typeof(VirtualMouse);
		public virtual GameObject BodyPrefab => _virtualMousePrefab;

		[Preserve]
		public VirtualMouseInfo() : this(0, null) { }

		[Preserve]
		public VirtualMouseInfo(int playerIndex, GameObject virtualMousePrefab)
		{
			_playerIndex = playerIndex;
			_virtualMousePrefab = virtualMousePrefab;
		}
	}
}