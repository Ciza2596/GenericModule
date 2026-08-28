using System;
using UnityEngine;

namespace CizaAudioModule
{
	public static class AudioModuleInstaller
	{
		public static AudioPlayer Install(IAudioPlayerConfig config, IAssetProvider assetProvider, IVoiceAssetProvider voiceAssetProvider, bool isAutoInitialize = true, Transform parent = null) =>
			Install<AudioPlayer>(config, assetProvider, voiceAssetProvider, isAutoInitialize, parent);

		public static TAudioPlayer Install<TAudioPlayer>(IAudioPlayerConfig config, IAssetProvider assetProvider, IVoiceAssetProvider voiceAssetProvider, bool isAutoInitialize = true, Transform parent = null) where TAudioPlayer : AudioPlayer
		{
			var audioPlayer = Activator.CreateInstance(typeof(TAudioPlayer), config, assetProvider, voiceAssetProvider) as TAudioPlayer;
			if (isAutoInitialize)
				audioPlayer.Initialize(parent);
			return audioPlayer;
		}
	}
}