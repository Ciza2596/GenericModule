using System;
using CizaUniTask;

namespace CizaAudioModule
{
	public interface IVoiceAssetProvider : IAssetProvider
	{
		event Func<string, UniTask> OnChangedLocaleBeforeAsync;
		event Func<string, UniTask> OnChangedLocaleAsync;
	}
}
