using UnityEngine;

namespace ReadWriteArc
{
    public class CharacterManagerCountTest : MonoBehaviour
    {
        [SerializeField]
        private int _createCount = 1000;


        private readonly CharacterManager _characterManager = new CharacterManager();

        private void OnEnable()
        {
            for (int i = 0; i < _createCount; i++)
                _characterManager.CreateCharacter();
        }

        private void OnDisable()
        {
            _characterManager.DestroyAllCharacters();
        }

        private void Update()
        {
            _characterManager.Tick(Time.deltaTime);
        }
    }
}