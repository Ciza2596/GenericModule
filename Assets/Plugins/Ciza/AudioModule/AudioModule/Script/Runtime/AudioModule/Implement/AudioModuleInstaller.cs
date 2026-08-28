using System;

namespace CizaAudioModule.Implement
{
	public static class AudioModuleInstaller
	{
		public static AudioPlayer Install(IAudioPlayerConfig audioPlayerConfig, IAssetProvider assetProvider, IVoiceAssetProvider voiceAssetProvider) =>
			Install<AudioPlayer>(audioPlayerConfig, assetProvider, voiceAssetProvider);

		public static TAudioPlayer Install<TAudioPlayer>(IAudioPlayerConfig audioPlayerConfig, IAssetProvider assetProvider, IVoiceAssetProvider voiceAssetProvider) where TAudioPlayer : AudioPlayer =>
			Activator.CreateInstance(typeof(TAudioPlayer), audioPlayerConfig, assetProvider, voiceAssetProvider) as TAudioPlayer;
	}
}