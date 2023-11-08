using UnityEngine;
using UnityEngine.Assertions;

namespace CizaOptionModule.Implement
{
	public abstract class OptionSubMono : MonoBehaviour, IOptionSubMon
	{
		protected Option _option;

		public virtual void Initialize(Option option)
		{
			_option = option;
			Assert.IsNotNull(_option, $"[{GetType().Name}::Awake] Option should be found.");
		}
	}
}
