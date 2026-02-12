using System;
using System.Collections.Generic;
using System.Threading;
using CizaUniTask;

namespace CizaEventModule
{
	internal class AsyncEventDelegateContainer : BaseEventDelegateContainer
	{
		public async UniTask Invoke<T>(T eventData, CancellationToken cancellationToken)
		{
			var invocationTasks = new List<UniTask>();

			foreach (var invocation in EventDelegate.GetInvocationList())
				invocationTasks.Add(((Func<T, CancellationToken, UniTask>)invocation).Invoke(eventData, cancellationToken));

			await UniTask.WhenAll(invocationTasks);
		}
	}
}
