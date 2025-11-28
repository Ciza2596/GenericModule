using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Scripting;

namespace CizaInputModule
{
	[Serializable]
	public class RumbleInfo : IRumbleInfo
	{
		// VARIABLE: -----------------------------------------------------------------------------

		[SerializeField]
		protected int _order;

		[SerializeField]
		protected float _duration;

		[Space]
		[SerializeField]
		protected ControlSchemeInfoMapList _controlSchemeInfoMapList;

		// PUBLIC VARIABLE: ---------------------------------------------------------------------

		public virtual int Order => _order;
		public virtual float Duration => _duration;

		public virtual bool TryGetControlSchemeInfo(string dataId, out IControlSchemeInfo controlSchemeInfo)
		{
			if (!_controlSchemeInfoMapList.TryGetValue(dataId, out var controlSchemeInfoImp))
			{
				controlSchemeInfo = null;
				return false;
			}

			controlSchemeInfo = controlSchemeInfoImp;
			return true;
		}


		// CONSTRUCTOR: ------------------------------------------------------------------------

		[Preserve]
		public RumbleInfo() : this(0, 0, new ControlSchemeInfoMapList()) { }

		[Preserve]
		public RumbleInfo(int order, float duration, ControlSchemeInfoMapList controlSchemeInfoMapList)
		{
			_order = order;
			_duration = duration;
			_controlSchemeInfoMapList = controlSchemeInfoMapList;
		}
	}
}