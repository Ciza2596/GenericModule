using System;
using System.Collections.Generic;
using System.Linq;

namespace ReadWriteArc
{
    public class CharacterManager
    {
        private readonly Dictionary<string, ICharacter> _characterMapById = new Dictionary<string, ICharacter>();
        private readonly CharacterLogic _characterLogic = new CharacterLogic();

        public bool TryGetCharacterRead(string characterId, out ICharacterRead characterRead)
        {
            if (!_characterMapById.TryGetValue(characterId, out var character))
            {
                characterRead = null;
                return false;
            }

            characterRead = character;
            return true;
        }


        public string CreateCharacter()
        {
            var id = Guid.NewGuid().ToString();
            _characterMapById.Add(id, new Character(id));
            return _characterMapById[id].Id;
        }

        public void DestroyAllCharacters()
        {
            foreach (var id in _characterMapById.Keys.ToArray())
                DestroyCharacter(id);
        }

        public void DestroyCharacter(string characterId)
        {
            if (!_characterMapById.Remove(characterId, out var character))
                return;
        }


        public void Tick(float deltaTime)
        {
            foreach (var character in _characterMapById.Values.ToArray())
                character.Write(_characterLogic.Evaluate(deltaTime, character));
        }
    }
}