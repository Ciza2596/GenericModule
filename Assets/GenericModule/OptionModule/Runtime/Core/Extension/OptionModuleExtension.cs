using System.Linq;
using CizaCore;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CizaOptionModule
{
    public static class OptionModuleExtension
    {
        public static bool TryGetRandomOptionKeyWith(this OptionModule optionModule, string withoutOptionKey, out string optionKey) =>
            optionModule.TryGetRandomOptionKeyWith(new[] { withoutOptionKey }, out optionKey);

        public static bool TryGetRandomOptionKeyWith(this OptionModule optionModule, string[] withoutOptionKeys, out string optionKey)
        {
            var allOptionKeys = optionModule.GetAllOptions<Option>().ToKeys().ToArrayWithoutSomeItems(withoutOptionKeys);
            if (allOptionKeys.Length <= 0)
            {
                optionKey = string.Empty;
                return false;
            }

            optionKey = allOptionKeys[Random.Range(0, allOptionKeys.Length)];
            return true;
        }

        public static UniTask MovePageToLeftAsync(this OptionModule optionModule, int playerIndex)
        {
            if (optionModule.IsChangingPage)
                return UniTask.CompletedTask;

            return optionModule.TryMovePageToLeftAsync(playerIndex);
        }

        public static UniTask MovePageToRightAsync(this OptionModule optionModule, int playerIndex)
        {
            if (optionModule.IsChangingPage)
                return UniTask.CompletedTask;

            return optionModule.TryMovePageToRightAsync(playerIndex);
        }

        public static async UniTask HorizontalMovementAsync(this OptionModule optionModule, int playerIndex, Vector2 direction)
        {
            if (optionModule.IsChangingPage)
                return;

            if (direction.x > 0)
            {
                if (await optionModule.TryMoveOptionToRightAsync(playerIndex))
                    return;
            }
            else if (direction.x < 0)
            {
                if (await optionModule.TryMoveOptionToLeftAsync(playerIndex))
                    return;
            }
        }

        public static async UniTask VerticalMovementAsync(this OptionModule optionModule, int playerIndex, Vector2 direction)
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

        public static async UniTask MovementAsync(this OptionModule optionModule, int playerIndex, Vector2 direction)
        {
            if (optionModule.IsChangingPage)
                return;

            if (direction.x > 0)
            {
                if (await optionModule.TryMoveOptionToRightAsync(playerIndex))
                    return;
            }
            else if (direction.x < 0)
            {
                if (await optionModule.TryMoveOptionToLeftAsync(playerIndex))
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