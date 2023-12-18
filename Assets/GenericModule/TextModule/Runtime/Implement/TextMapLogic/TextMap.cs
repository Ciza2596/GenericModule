using UnityEngine;

namespace CizaTextModule.Implement
{
    public abstract class TextMap : MonoBehaviour, ITextMap
    {
        [SerializeField]
        private bool _isEnable = true;

        [Space]
        [SerializeField]
        private bool _isFollowLocalKey = true;

        [TextArea]
        [SerializeField]
        private string _localKey;

        private string _key;

        public string Name => name;

        public bool IsEnable => _isEnable;

        public string Key
        {
            get
            {
                var key = _isFollowLocalKey ? _localKey : _key;
                return !string.IsNullOrEmpty(key) ? key : string.Empty;
            }
        }

        public void SetKey(string key) =>
            _key = key;

        public abstract void SetText(string text);
    }
}