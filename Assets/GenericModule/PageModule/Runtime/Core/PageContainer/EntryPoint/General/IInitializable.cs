using Cysharp.Threading.Tasks;

namespace CizaPageModule
{
	public interface IInitializable
	{
		UniTask InitializeAsync(params object[] parameters);
	}
}
