using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CizaPageModule.Example1
{
    public abstract class BaseAnimPlayer : MonoBehaviour
    {
        public abstract UniTask Play();
    }
}