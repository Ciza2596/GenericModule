using Cysharp.Threading.Tasks;
using UnityEngine;


public abstract class BaseAnimPlayer : MonoBehaviour
{
    public abstract UniTask Play();
}