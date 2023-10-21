using System;
using Cysharp.Threading.Tasks;

namespace CizaAudioModule
{
	public interface IVoiceAssetProvider : IAssetProvider
	{
		event Func<string, UniTask> OnChangedLocaleBefore;
		event Func<string, UniTask> OnChangedLocale;
	}
}
