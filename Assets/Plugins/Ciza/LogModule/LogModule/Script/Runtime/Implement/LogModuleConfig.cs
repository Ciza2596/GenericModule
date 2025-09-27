using UnityEngine;

namespace CizaLogModule.Implement
{
	[CreateAssetMenu(fileName = "LogModuleConfig", menuName = "Ciza/LogModule/LogModuleConfig")]
	public class LogModuleConfig : ScriptableObject, ILogModuleConfig
	{
		// VARIABLE: -----------------------------------------------------------------------------

		[SerializeField]
		protected bool _isPrintDebug;

		[SerializeField]
		protected bool _isPrintInfo;

		[SerializeField]
		protected bool _isPrintWarn;

		[SerializeField]
		protected bool _isPrintError;

		// PUBLIC VARIABLE: ---------------------------------------------------------------------

		public virtual bool IsPrintDebug => _isPrintDebug;
		public virtual bool IsPrintInfo => _isPrintInfo;
		public virtual bool IsPrintWarn => _isPrintWarn;
		public virtual bool IsPrintError => _isPrintError;


		// CONSTRUCTOR: ------------------------------------------------------------------------

		public virtual void Reset()
		{
			_isPrintDebug = false;
			_isPrintInfo = false;
			_isPrintWarn = false;
			_isPrintError = false;
		}
	}
}