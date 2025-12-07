using System;
using CizaUniTask;
using UnityEngine;

namespace CizaOptionModule.Implement
{
	public class OptionModuleInstaller
	{
		public static UniTask<TOptionModule> InstallAsync<TOptionModule>(IOptionModuleConfig optionModuleConfig, Transform optionModulePageRootParentTransform, IOptionModulePageInfo[] optionPageInfos, IOptionInfo[] optionInfos, bool isColumnCircle, int pageIndex = 0, Vector2Int coordinate = default, bool isAutoChangePage = false, int optionDefaultPlayerIndex = 0, bool isAutoInitialize = true) where TOptionModule : OptionModule =>
			InstallAsync<TOptionModule>(0, optionModuleConfig, optionModulePageRootParentTransform, optionPageInfos, optionInfos, isColumnCircle, pageIndex, coordinate, isAutoChangePage, optionDefaultPlayerIndex, isAutoInitialize);

		public static async UniTask<TOptionModule> InstallAsync<TOptionModule>(int playerCount, IOptionModuleConfig optionModuleConfig, Transform optionModulePageRootParentTransform, IOptionModulePageInfo[] optionPageInfos, IOptionInfo[] optionInfos, bool isColumnCircle, int pageIndex = 0, Vector2Int coordinate = default, bool isAutoChangePage = false, int optionDefaultPlayerIndex = 0, bool isAutoInitialize = true) where TOptionModule : OptionModule
		{
			var optionModule = Activator.CreateInstance(typeof(TOptionModule), optionModuleConfig) as TOptionModule;
			if (isAutoInitialize)
				await optionModule.InitializeAsync(playerCount, optionModulePageRootParentTransform, optionPageInfos, optionInfos, isColumnCircle, pageIndex, coordinate, isAutoChangePage, optionDefaultPlayerIndex);
			return optionModule;
		}
	}
}