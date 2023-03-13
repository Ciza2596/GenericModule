using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace CizaSaveLoadModule.Example1
{
    [Serializable]
    public class ComponentCollectionData
    {
        [SerializeField] private TMP_InputField _saveLoadKeyInputField;
        [SerializeField] private Button _saveButton;
        [SerializeField] private Button _loadButton;

        [Space] [SerializeField] private TMP_InputField _setHpValueInputField;
        [SerializeField] private Button _setHpButton;

        [Space] [SerializeField] private TMP_InputField _setMpValueInputField;
        [SerializeField] private Button _setMpButton;

        [Space] [SerializeField] private TMP_Text _hpInfoText;
        [SerializeField] private TMP_Text _mpInfoText;


        public string SaveLoadKey => _saveLoadKeyInputField.text;
        public Button SaveButton => _saveButton;
        public Button LoadButton => _loadButton;

        public int SetHpValue => int.Parse(_setHpValueInputField.text);
        public Button SetHpButton => _setHpButton;

        public int SetMpValue => int.Parse(_setMpValueInputField.text);
        public Button SetMpButton => _setMpButton;


        public void SetHpInfo(int hp) => _hpInfoText.text = hp.ToString(CultureInfo.InvariantCulture);
        public void SetMpInfo(int mp) => _mpInfoText.text = mp.ToString(CultureInfo.InvariantCulture);
    }
}