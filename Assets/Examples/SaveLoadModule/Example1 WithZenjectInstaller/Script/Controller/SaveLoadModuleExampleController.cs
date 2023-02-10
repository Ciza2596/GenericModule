using UnityEngine;
using Zenject;


namespace SaveLoadModule.Example1
{
    public class SaveLoadModuleExampleController : IInitializable
    {
        //private variable
        private readonly SaveLoadModule _saveLoadModule;
        private readonly ComponentCollectionData _componentCollectionData;

        private PlayerData _playerData = new PlayerData();


        //constructor
        public SaveLoadModuleExampleController(SaveLoadModule saveLoadModule,
            ComponentCollectionData componentCollectionData)
        {
            _saveLoadModule = saveLoadModule;
            _componentCollectionData = componentCollectionData;
        }

        //zenject callback
        public void Initialize()
        {
            _componentCollectionData.SaveButton.onClick.AddListener(OnSaveButtonClick);
            _componentCollectionData.LoadButton.onClick.AddListener(OnLoadButtonClick);


            _componentCollectionData.SetHpButton.onClick.AddListener(OnSetHpButtonClick);

            _componentCollectionData.SetMpButton.onClick.AddListener(OnSetMpButtonClick);
            
            UpdatePlayerInfo();
        }


        //private method
        private void OnSaveButtonClick()
        {
            var saveLoadKey = _componentCollectionData.SaveLoadKey;
            _saveLoadModule.Save(saveLoadKey, _playerData);
        }

        private void OnLoadButtonClick()
        {
            // var saveLoadKey = _componentCollectionData.SaveLoadKey;
            // var playerData = _saveLoadModule.TryLoad<PlayerData>(saveLoadKey);
            //
            // if (playerData is null)
            // {
            //     Debug.Log("[SaveLoadModuleExampleController::OnLoadButtonClick] PlayerData is null.");
            //     return;
            // }
            //
            // _playerData = playerData;
            // UpdatePlayerInfo();
        }


        private void OnSetHpButtonClick()
        {
            var hp = _componentCollectionData.SetHpValue;
            _playerData.SetHp(hp);
            UpdatePlayerInfo();
        }

        private void OnSetMpButtonClick()
        {
            var mp = _componentCollectionData.SetMpValue;
            _playerData.SetMp(mp);
            UpdatePlayerInfo();
        }

        private void UpdatePlayerInfo()
        {
            var hp = _playerData.Hp;
            _componentCollectionData.SetHpInfo(hp);

            var mp = _playerData.Mp;
            _componentCollectionData.SetMpInfo(mp);
        }
    }
}