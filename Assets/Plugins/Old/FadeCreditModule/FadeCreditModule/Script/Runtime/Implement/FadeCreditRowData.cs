using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace CizaFadeCreditModule.Implement
{
    [Serializable]
    public class FadeCreditRowData : IFadeCreditRowData
    {
        [SerializeField]
        private float _time;

        [SerializeField]
        private int _viewOrder;

        [Space]
        [SerializeField]
        private string _prefabAddress;

        [Space]
        [SerializeField]
        private Vector2 _position;

        [SerializeField]
        private float _duration;

        [SerializeField]
        private Vector2 _size = new Vector2(100, 100);

        [Space]
        [SerializeField]
        private RowKinds _rowKind;

        [Space]
        [SerializeField]
        private string _text;

        [SerializeField]
        private string _spriteAddress;


        [Preserve]
        public FadeCreditRowData() { }

        [Preserve]
        public FadeCreditRowData(float time, int viewOrder, string prefabAddress, Vector2 position, float duration, Vector2 size) : this(time, viewOrder, prefabAddress, position, duration, size, RowKinds.Empty, string.Empty, string.Empty) { }


        [Preserve]
        public FadeCreditRowData(float time, int viewOrder, string prefabAddress, Vector2 position, float duration, Vector2 size, RowKinds rowKind, string text, string spriteAddress)
        {
            _time = time;
            _viewOrder = viewOrder;

            _prefabAddress = prefabAddress;

            _position = position;
            _duration = duration;
            _size = size;

            _rowKind = rowKind;

            _text = text;
            _spriteAddress = spriteAddress;
        }

        public float Time => _time;
        public int ViewOrder => _viewOrder;

        public string PrefabAddress => _prefabAddress;

        public Vector2 Position => _position;
        public float Duration => _duration;
        public Vector2 Size => _size;

        public RowKinds RowKind => _rowKind;

        public string Text => _text;
        public string SpriteAddress => _spriteAddress;
    }
}