using System.Threading.Tasks;
using UnityEngine;

namespace AddressableModule
{
    public interface IAsyncOperationHandleInfo
    {
        bool IsDone { get; }
        Task Task();
        Object Result { get; }
    }
}