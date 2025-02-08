using System;
using Cysharp.Threading.Tasks;

namespace CizaAudioModule
{
	public interface IVoiceAssetProvider : IAssetProvider
	{
		event Func<string, UniTask> OnChangedLocaleBeforeAsync;
		event Func<string, UniTask> OnChangedLocaleAsync;
	}
}
