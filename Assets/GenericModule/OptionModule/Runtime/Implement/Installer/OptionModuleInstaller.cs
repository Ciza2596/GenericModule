using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CizaOptionModule.Implement
{
    public class OptionModuleInstaller
    {
        public static UniTask<OptionModule> InstallAsync(IOptionModuleConfig optionModuleConfig, Transform optionModulePageRootParentTransform, IOptionModulePageInfo[] optionPageInfos, IOptionInfo[] optionInfos, bool isColumnCircle, int pageIndex = 0, Vector2Int coordinate = default, bool isAutoChangePage = false, int optionDefaultPlayerIndex = 0, bool isAutoInitialize = true) =>
            InstallAsync(0, optionModuleConfig, optionModulePageRootParentTransform, optionPageInfos, optionInfos, isColumnCircle, pageIndex, coordinate, isAutoChangePage, optionDefaultPlayerIndex, isAutoInitialize);

        public static async UniTask<OptionModule> InstallAsync(int playerCount, IOptionModuleConfig optionModuleConfig, Transform optionModulePageRootParentTransform, IOptionModulePageInfo[] optionPageInfos, IOptionInfo[] optionInfos, bool isColumnCircle, int pageIndex = 0, Vector2Int coordinate = default, bool isAutoChangePage = false, int optionDefaultPlayerIndex = 0, bool isAutoInitialize = true)
        {
            var optionModule = new OptionModule(optionModuleConfig);
            if (isAutoInitialize)
                await optionModule.InitializeAsync(playerCount, optionModulePageRootParentTransform, optionPageInfos, optionInfos, isColumnCircle, pageIndex, coordinate, isAutoChangePage, optionDefaultPlayerIndex);
            return optionModule;
        }
    }
}