using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class UniTaskExample : MonoBehaviour
{
    [SerializeField] private float _delaySeconds = 1;

    private CancellationTokenSource _cancellationTokenSource;


    [Button]
    private async void Show()
    {
        Debug.Log("ShowStart.");

        await PlayAnim();

        Debug.Log("ShowEnd.");
    }


    [Button]
    private void Cancel()
    {
        _cancellationTokenSource.Cancel();
    }

    private async UniTask PlayAnim()
    {
        if (_cancellationTokenSource != null)
            _cancellationTokenSource.Cancel();

        _cancellationTokenSource = new CancellationTokenSource();

        Debug.Log("PlayAnimStart.");
        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_delaySeconds), cancellationToken: _cancellationTokenSource.Token);
            Debug.Log("PlayAnimEnd.");
        }
        catch
        {
            // ignored
        }
    }
}