using System.Collections.Generic;
using System.Linq;
using CizaCore;
using CizaUniTask;
using UnityEngine;

namespace CizaOptionModule
{
	public static class OptionModuleExtension
	{
		public static bool TryGetRandomOptionKey(this Option[] options, out string optionKey) =>
			options.TryGetRandomOptionKey(string.Empty, out optionKey);

		public static bool TryGetRandomOptionKey(this Option[] options, string withoutOptionKey, out string optionKey) =>
			options.TryGetRandomOptionKey(new[] { withoutOptionKey }, out optionKey);

		public static bool TryGetRandomOptionKey(this Option[] options, string[] withoutOptionKeys, out string optionKey)
		{
			var allOptionKeys = options.ToKeys().ToArrayWithoutSameItems(withoutOptionKeys);
			if (allOptionKeys.Length <= 0)
			{
				optionKey = string.Empty;
				return false;
			}

			optionKey = allOptionKeys[Random.Range(0, allOptionKeys.Length)];
			return true;
		}

		public static bool TryGetIsUnlockedOptions(this OptionModule optionModule, out Option[] options)
		{
			var isUnlockedOptions = new HashSet<Option>();

			foreach (var option in optionModule.GetAllOptions<Option>())
				if (option.IsUnlock)
					isUnlockedOptions.Add(option);

			options = isUnlockedOptions.ToArray();
			return options.Length > 0;
		}

		public static UniTask MovePageToLeftAsync(this OptionModule optionModule, int playerIndex, bool isImmediately = true)
		{
			if (optionModule.IsChangingPage)
				return UniTask.CompletedTask;

			return optionModule.TryMovePageToLeftAsync(playerIndex, isImmediately);
		}

		public static UniTask MovePageToRightAsync(this OptionModule optionModule, int playerIndex, bool isImmediately = true)
		{
			if (optionModule.IsChangingPage)
				return UniTask.CompletedTask;

			return optionModule.TryMovePageToRightAsync(playerIndex, isImmediately);
		}

		public static async UniTask HorizontalMovementAsync(this OptionModule optionModule, int playerIndex, Vector2 direction, bool isImmediately = true)
		{
			if (optionModule.IsChangingPage)
				return;

			if (direction.x > 0)
			{
				if (await optionModule.TryMoveOptionToRightAsync(playerIndex, isImmediately))
					return;
			}
			else if (direction.x < 0)
			{
				if (await optionModule.TryMoveOptionToLeftAsync(playerIndex, isImmediately))
					return;
			}
		}

		public static void VerticalMovement(this OptionModule optionModule, int playerIndex, Vector2 direction)
		{
			if (optionModule.IsChangingPage)
				return;

			if (direction.y > 0)
			{
				if (optionModule.TryMoveOptionToUp(playerIndex))
					return;
			}
			else if (direction.y < 0)
			{
				if (optionModule.TryMoveOptionToDown(playerIndex))
					return;
			}
		}

		public static async UniTask MovementAsync(this OptionModule optionModule, int playerIndex, Vector2 direction, bool isImmediately = true)
		{
			if (optionModule.IsChangingPage)
				return;

			if (direction.x > 0)
			{
				if (await optionModule.TryMoveOptionToRightAsync(playerIndex, isImmediately))
					return;
			}
			else if (direction.x < 0)
			{
				if (await optionModule.TryMoveOptionToLeftAsync(playerIndex, isImmediately))
					return;
			}

			if (direction.y > 0)
			{
				if (optionModule.TryMoveOptionToUp(playerIndex))
					return;
			}
			else if (direction.y < 0)
			{
				if (optionModule.TryMoveOptionToDown(playerIndex))
					return;
			}
		}
	}
}