using System;
using Cysharp.Threading.Tasks;

namespace CizaAudioPlayerModule
{
	public interface IVoiceAssetProvider : CizaAudioModule.IAssetProvider
	{
		event Func<string, UniTask> OnChangedLocaleBefore;
		event Func<string, UniTask> OnChangedLocale;
	}
}
