using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CizaOptionModule.Implement
{
	public class OptionModuleInstaller
	{
		public static async UniTask<OptionModule> InstallAsync(IOptionModuleConfig optionModuleConfig, Transform optionModulePageRootParentTransform, IOptionModulePageInfo[] optionPageInfos, IOptionInfo[] optionInfos, bool isColumnCircle, int pageIndex = 0, Vector2Int coordinate = default, bool isAutoChangePage = false, bool isAutoInitialize = true, params object[] parameters)
		{
			var optionModule = new OptionModule(optionModuleConfig);
			if (isAutoInitialize)
				await optionModule.InitializeAsync(optionModulePageRootParentTransform, optionPageInfos, optionInfos, isColumnCircle, pageIndex, coordinate, isAutoChangePage, parameters);
			return optionModule;
		}
	}
}
