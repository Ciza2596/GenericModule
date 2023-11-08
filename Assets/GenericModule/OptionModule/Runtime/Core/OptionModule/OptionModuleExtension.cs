using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CizaOptionModule
{
	public static class OptionModuleExtension
	{
		public static async void MovePageToLeft(this OptionModule optionModule)
		{
			if (optionModule.IsChangingPage)
				return;

			await optionModule.TryMovePageToLeftAsync();
		}

		public static async void MovePageToRight(this OptionModule optionModule)
		{
			if (optionModule.IsChangingPage)
				return;

			await optionModule.TryMovePageToRightAsync();
		}

		public static async UniTask MovementAsync(this OptionModule optionModule, Vector2 direction)
		{
			if (optionModule.IsChangingPage)
				return;

			if (direction.x > 0)
			{
				if (await optionModule.TryMoveOptionToRightAsync())
					return;
			}
			else if (direction.x < 0)
			{
				if (await optionModule.TryMoveOptionToLeftAsync())
					return;
			}

			if (direction.y > 0)
			{
				if (optionModule.TryMoveOptionToUp())
					return;
			}
			else if (direction.y < 0)
			{
				if (optionModule.TryMoveOptionToDown())
					return;
			}
		}
	}
}
