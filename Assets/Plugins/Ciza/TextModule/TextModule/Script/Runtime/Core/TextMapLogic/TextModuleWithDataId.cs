using UnityEngine.Scripting;

namespace CizaTextModule
{
	public class TextModuleWithDataId : TextModule
	{
		// PUBLIC VARIABLE: ---------------------------------------------------------------------

		public virtual string DataId { get; }
		public virtual string KeyPattern { get; }

		// CONSTRUCTOR: ------------------------------------------------------------------------

		[Preserve]
		public TextModuleWithDataId(string dataId, string keyPattern, ITextModuleConfig config) : base(config)
		{
			DataId = dataId;
			KeyPattern = keyPattern;
		}
	}
}