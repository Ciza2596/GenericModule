using System.Threading.Tasks;
using UnityEngine;

namespace AddressablesModule.Componet
{
    public interface IAsyncOperationHandleInfo
    {
        bool IsDone { get; }
        Task Task();
        Object Result { get; }
    }
}