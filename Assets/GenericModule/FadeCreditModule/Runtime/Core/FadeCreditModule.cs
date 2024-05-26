using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Scripting;
using Object = UnityEngine.Object;

namespace CizaFadeCreditModule
{
    public class FadeCreditModule
    {
        private readonly IFadeCreditModuleConfig _config;
        private readonly IAssetProvider _assetProvider;

        private readonly Dictionary<string, PrefabAsset> _prefabAssetMapByAddress = new Dictionary<string, PrefabAsset>();
        private readonly Dictionary<string, SpriteAsset> _spriteAssetMapByAddress = new Dictionary<string, SpriteAsset>();

        private readonly Dictionary<string, List<IRow>> _rowMapByAddress = new Dictionary<string, List<IRow>>();
        private readonly HashSet<IRow> _playingRow = new HashSet<IRow>();

        private readonly HashSet<IFadeCreditRowData> _playedRowDatas = new HashSet<IFadeCreditRowData>();

        private IFadeCreditController _controller;
        private IFadeCreditRowData[] _rowDatas;
        private float _time;

        private bool CantShow => !IsInitialized || !HasRowDatas || !IsLoaded;

        private bool CantHide => !IsInitialized || !HasRowDatas || !IsLoaded || !IsVisible || IsHiding;

        public bool IsInitialized { get; private set; }

        public bool IsLoading { get; private set; }
        public bool IsUnloading { get; private set; }

        public bool IsLoaded { get; private set; }

        public bool IsVisible => _controller.IsVisible;
        public bool IsHiding => _controller.IsHiding;

        public bool HasRowDatas => TryGetRowDatas(out var rowDatas);

        public float Time => IsVisible ? _time : 0;

        public bool TryGetRowDatas(out IFadeCreditRowData[] fadeCreditRowDatas)
        {
            if (_rowDatas == null)
            {
                fadeCreditRowDatas = Array.Empty<IFadeCreditRowData>();
                return false;
            }

            fadeCreditRowDatas = _rowDatas;
            return true;
        }

        public FadeCreditModule(IFadeCreditModuleConfig fadeCreditModuleConfig, IAssetProvider assetProvider)
        {
            _config = fadeCreditModuleConfig;
            _assetProvider = assetProvider;
        }

        public void Initialize(Transform parent = null)
        {
            if (IsInitialized || IsLoading || IsUnloading)
                return;

            var controllerGameObject = Object.Instantiate(_config.ControllerPrefab, parent);
            _controller = controllerGameObject.GetComponent<IFadeCreditController>();
            Assert.IsNotNull(_controller, "[FadeCreditModule::Initialize] FadeCreditController is not found.");
            _controller.HideImmediately();

            if (_config.IsDontDestroyOnLoad && parent == null)
                Object.DontDestroyOnLoad(controllerGameObject);

            IsInitialized = true;
        }

        public void Release()
        {
            if (!IsInitialized || IsLoading || IsUnloading)
                return;

            if (IsLoaded)
                UnloadAsset();

            SetRowDatas(null);

            _controller.Release();

            IsInitialized = false;
        }

        public void Tick(float deltaTime)
        {
            if (!IsInitialized || !IsVisible || IsHiding)
                return;

            _time += deltaTime;

            if (TryGetRowDatas(out var rowDatas))
            {
                foreach (var rowData in rowDatas)
                    if (Time >= rowData.Time && _playedRowDatas.Add(rowData))
                        Play(rowData);
            }

            foreach (var row in _playingRow.ToArray())
            {
                if (row.IsNeedHiding && !row.IsHiding)
                {
                    row.Hide();
                    continue;
                }

                if (!row.IsVisible)
                {
                    DeSpawnRow(row);
                    continue;
                }

                if (!row.IsShowing && !row.IsHiding)
                    row.Tick(deltaTime);
            }

            if (rowDatas.Length == _playedRowDatas.Count && _playingRow.Count == 0)
                Hide();
        }


        public void SetRowDatas(IFadeCreditRowData[] rowDatas)
        {
            if (!IsInitialized || IsLoading)
                return;

            if (TryGetRowDatas(out var currentRowDatas) && IsLoaded)
                UnloadAsset();

            _rowDatas = rowDatas;
        }

        public async UniTask LoadAssetAsync(CancellationToken cancellationToken)
        {
            if (!IsInitialized || IsLoading || IsUnloading)
                return;

            if (IsLoaded)
                UnloadAsset();

            if (!TryGetRowDatas(out var rowDatas))
                return;

            IsLoading = true;

            var uniTasks = new List<UniTask>();
            foreach (var rowData in rowDatas)
                uniTasks.Add(LoadAssetAsync(rowData, cancellationToken));
            await UniTask.WhenAll(uniTasks);

            IsLoaded = true;
            IsLoading = false;
        }

        public void UnloadAsset()
        {
            if (!IsInitialized || !IsLoaded)
                return;

            if (!TryGetRowDatas(out var rowDatas))
                return;

            if (IsVisible)
                HideImmediately();

            IsUnloading = true;
            IsLoaded = false;

            foreach (var rowData in rowDatas)
                UnloadAsset(rowData);

            IsUnloading = false;
        }

        public void Show()
        {
            if (CantShow)
                return;

            if (IsVisible)
                Hide();

            _time = 0;
            _controller.Show();
        }

        public void Hide()
        {
            if (CantHide)
                return;

            _controller.Hide();
            DestroyAll();
        }

        public void HideImmediately()
        {
            if (CantHide)
                return;

            _controller.HideImmediately();
            DestroyAll();
        }


        private async UniTask LoadAssetAsync(IFadeCreditRowData rowData, CancellationToken cancellationToken)
        {
            await m_LoadAssetAsync<GameObject, PrefabAsset>(rowData.PrefabAddress, _prefabAssetMapByAddress);

            if (rowData.RowKind.CheckIsEmpty())
                return;

            if (rowData.RowKind.CheckIsText())
                return;

            if (rowData.RowKind.CheckIsSprite())
            {
                await m_LoadAssetAsync<Sprite, SpriteAsset>(rowData.SpriteAddress, _spriteAssetMapByAddress);
                return;
            }

            async UniTask m_LoadAssetAsync<T, TAsset>(string m_address, Dictionary<string, TAsset> m_assetMapByAddress) where T : Object where TAsset : Asset
            {
                var obj = await _assetProvider.LoadAssetAsync<T>(m_address, cancellationToken);
                if (!m_assetMapByAddress.TryAdd(m_address, Activator.CreateInstance(typeof(TAsset), args: obj) as TAsset))
                    m_assetMapByAddress[m_address].AddCount();
            }
        }

        private void UnloadAsset(IFadeCreditRowData rowData)
        {
            m_UnloadAsset<GameObject, PrefabAsset>(rowData.PrefabAddress, _prefabAssetMapByAddress);

            if (rowData.RowKind.CheckIsEmpty())
                return;

            if (rowData.RowKind.CheckIsText())
                return;

            if (rowData.RowKind.CheckIsSprite())
            {
                m_UnloadAsset<Sprite, SpriteAsset>(rowData.SpriteAddress, _spriteAssetMapByAddress);
                return;
            }

            void m_UnloadAsset<T, TAsset>(string m_address, Dictionary<string, TAsset> m_assetMapByAddress) where T : Object where TAsset : Asset
            {
                if (!m_assetMapByAddress.TryGetValue(m_address, out var prefabAsset))
                    return;

                _assetProvider.UnloadAsset<T>(m_address);
                prefabAsset.ReduceCount();
                if (prefabAsset.Count <= 0)
                    m_assetMapByAddress.Remove(m_address);
            }
        }

        private void Play(IFadeCreditRowData rowData)
        {
            var row = SpawnRow(rowData.PrefabAddress);
            if (rowData.RowKind.CheckIsEmpty())
                row.PlayEmpty(_controller.Content, rowData.Position, rowData.Duration, rowData.Size);

            if (rowData.RowKind.CheckIsText())
                row.PlayText(_controller.Content, rowData.Position, rowData.Duration, rowData.Size, rowData.Text);

            if (rowData.RowKind.CheckIsSprite())
                row.PlaySprite(_controller.Content, rowData.Position, rowData.Duration, rowData.Size, _spriteAssetMapByAddress[rowData.SpriteAddress].Sprite);

            _playingRow.Add(row);
            row.Show();
        }


        private IRow SpawnRow(string address)
        {
            if (!_prefabAssetMapByAddress.TryGetValue(address, out var prefabAsset))
            {
                Debug.LogError($"[FadeCreditModule::SpawnRow] PrefabAsset is not found by: {address}.");
                return null;
            }

            _rowMapByAddress.TryAdd(address, new List<IRow>());

            if (_rowMapByAddress[address].Count <= 0)
            {
                var createdRow = Object.Instantiate(prefabAsset.Prefab, _controller.Pool).GetComponent<IRow>();
                createdRow.Initialize(address);
                createdRow.HideImmediately();
                _rowMapByAddress[address].Add(createdRow);
            }

            var row = _rowMapByAddress[address].First();
            _rowMapByAddress[address].Remove(row);

            return row;
        }

        private void DeSpawnRow(IRow row)
        {
            if (!_rowMapByAddress.ContainsKey(row.Address))
                return;

            _playingRow.Remove(row);

            row.Close(_controller.Pool);
            _rowMapByAddress[row.Address].Add(row);
        }

        private void DestroyAll()
        {
            foreach (var row in _playingRow.ToArray())
                DeSpawnRow(row);

            foreach (var rows in _rowMapByAddress.Values.ToArray())
                foreach (var row in rows.ToArray())
                {
                    rows.Remove(row);
                    row.Release();
                }

            _rowMapByAddress.Clear();
        }


        private class PrefabAsset : Asset
        {
            public GameObject Prefab { get; }

            [Preserve]
            public PrefabAsset(GameObject prefab) =>
                Prefab = prefab;
        }

        private class SpriteAsset : Asset
        {
            public Sprite Sprite { get; }

            [Preserve]
            public SpriteAsset(Sprite sprite) =>
                Sprite = sprite;
        }

        private abstract class Asset
        {
            public int Count { get; private set; } = 1;

            [Preserve]
            protected Asset() { }

            public void AddCount() =>
                Count++;

            public void ReduceCount() =>
                Count--;
        }
    }
}