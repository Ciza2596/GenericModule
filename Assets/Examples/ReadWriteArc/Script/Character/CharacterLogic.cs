using System;
using UnityEngine.Scripting;

namespace ReadWriteArc
{
    public class CharacterLogic
    {
        private readonly CharacterRead _characterRead = new CharacterRead();

        public ICharacterRead Evaluate(float deltaTime, ICharacterRead inputCharacterRead) =>
            _characterRead.Write(inputCharacterRead.Id, !inputCharacterRead.IsGround, !inputCharacterRead.IsWall, !inputCharacterRead.IsHurt, !inputCharacterRead.IsSmashHurt, inputCharacterRead.CurrentAnim);


        [Serializable]
        private class CharacterRead : ICharacterRead
        {
            public string Id { get; private set; }
            public bool IsGround { get; private set; }
            public bool IsWall { get; private set; }
            public bool IsHurt { get; private set; }
            public bool IsSmashHurt { get; private set; }
            public string CurrentAnim { get; private set; }

            [Preserve]
            public ICharacterRead Write(string id, bool isGround, bool isWall, bool isHurt, bool isSmashHurt, string currentAnim)
            {
                Id = id;
                IsGround = isGround;
                IsWall = isWall;
                IsHurt = isHurt;
                IsSmashHurt = isSmashHurt;
                CurrentAnim = currentAnim;

                return this;
            }
        }
    }
}