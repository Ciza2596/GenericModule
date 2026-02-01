using System;
using System.Threading;
using CizaUniTask;

namespace CizaAudioModule
{
	public interface IVoiceAssetProvider : IAssetProvider
	{
		event Func<string, CancellationToken, UniTask> OnChangedLocaleBeforeAsync;
		event Func<string, CancellationToken, UniTask> OnChangedLocaleAsync;
	}
}