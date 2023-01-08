using System.Threading.Tasks;
using UnityEngine;

namespace AddressablesModule.Componet
{
    public interface IAsyncOperationHandleInfo
    {
        Task Task();
        Object Result { get; }
    }
}