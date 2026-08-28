using System;
using UnityEngine;

namespace CizaInputModule
{
	public static class InputModuleInstaller
	{
		public static InputModule Install(IInputModuleConfig config, bool isAutoInitialize = true, BEventHandler[] eventHandlers = null, Transform parent = null) =>
			Install<InputModule>(config, isAutoInitialize, eventHandlers, parent);

		public static TInputModule Install<TInputModule>(IInputModuleConfig config, bool isAutoInitialize = true, BEventHandler[] eventHandlers = null, Transform parent = null) where TInputModule : InputModule
		{
			var inputModule = Activator.CreateInstance(typeof(TInputModule), config) as TInputModule;
			if (isAutoInitialize)
				inputModule.Initialize(eventHandlers ?? Array.Empty<BEventHandler>(), parent);
			return inputModule;
		}
	}
}