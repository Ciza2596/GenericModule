using UnityEngine;

namespace CizaTextModule
{
	public abstract class TextMap : MonoBehaviour, ITextMap
	{
		// VARIABLE: -----------------------------------------------------------------------------

		[SerializeField]
		protected bool _isEnable = true;

		[Space]
		[SerializeField]
		protected bool _isFollowLocalKey = true;

		[TextArea(6, 10)]
		[SerializeField]
		protected string _localKey;

		protected string _key;

		// PUBLIC VARIABLE: ---------------------------------------------------------------------

		public virtual string Name => name;

		public virtual bool IsEnable => _isEnable;

		public virtual string Key
		{
			get
			{
				var key = _isFollowLocalKey ? _localKey : _key;
				return key.CheckHasValue() ? key : string.Empty;
			}
		}

		public abstract string Text { get; }

		// PUBLIC METHOD: ----------------------------------------------------------------------

		public virtual void SetKey(string key) =>
			_key = key;

		public abstract void SetText(string text);
	}
}