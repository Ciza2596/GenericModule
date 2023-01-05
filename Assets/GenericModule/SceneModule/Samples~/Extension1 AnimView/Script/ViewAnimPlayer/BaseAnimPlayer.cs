using System.Threading.Tasks;
using UnityEngine;


public abstract class BaseAnimPlayer : MonoBehaviour
{
    public abstract Task Play();
}