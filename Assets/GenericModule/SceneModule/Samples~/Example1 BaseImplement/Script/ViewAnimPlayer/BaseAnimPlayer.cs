using System.Threading.Tasks;
using UnityEngine;

namespace SceneModule.Example1
{
    public abstract class BaseAnimPlayer : MonoBehaviour
    {
        public abstract Task Play();
    }
}
