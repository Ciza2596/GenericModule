using System;
using CizaAsync;
using UnityEngine;

namespace CizaAudioModule
{
	public interface IVoiceAssetProvider : IAssetProvider
	{
		event Func<string, AsyncToken, Awaitable> OnChangedLocaleBeforeAsync;
		event Func<string, AsyncToken, Awaitable> OnChangedLocaleAsync;
	}
}