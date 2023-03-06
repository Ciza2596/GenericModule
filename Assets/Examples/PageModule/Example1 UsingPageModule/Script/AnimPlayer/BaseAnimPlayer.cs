using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PageModule.Example1
{
    public abstract class BaseAnimPlayer : MonoBehaviour
    {
        public abstract UniTask Play();
    }
}