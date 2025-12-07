
using CizaUniTask;

namespace CizaPageModule
{
	public interface IInitializable
	{
		UniTask InitializeAsync(params object[] parameters);
	}
}
