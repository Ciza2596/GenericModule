using System.Linq;
using CizaFadeCreditModule.Implement;
using UnityEngine;

namespace CizaFadeCreditModule.Example
{
    public class FadeCreditModuleExample : MonoBehaviour
    {
        [SerializeField]
        private FadeCreditRowData[] _rowDatas;


        [Space]
        [SerializeField]
        private FadeCreditModuleConfig _fadeCreditModuleConfig;

        [SerializeField]
        private AssetProvider _assetProvider;

        private FadeCreditModule _fadeCreditModule;

        private async void OnEnable()
        {
            _fadeCreditModule = new FadeCreditModule(_fadeCreditModuleConfig, _assetProvider);
            _fadeCreditModule.Initialize(null);
            _fadeCreditModule.SetRowDatas(_rowDatas.ToArray());
            await _fadeCreditModule.LoadAssetAsync(default);
            _fadeCreditModule.Show();
        }

        private void OnDisable()
        {
            _fadeCreditModule.Release();
        }


        private void Update()
        {
            _fadeCreditModule?.Tick(Time.deltaTime);
        }
    }
}