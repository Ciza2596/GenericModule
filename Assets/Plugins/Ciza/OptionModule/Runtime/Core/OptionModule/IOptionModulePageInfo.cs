using UnityEngine;

namespace CizaOptionModule
{
	public interface IOptionModulePageInfo
	{
		string PageIndexString { get; }

		GameObject OptionViewPrefab { get; }

		string[] OptionKeys { get; }
	}
}
