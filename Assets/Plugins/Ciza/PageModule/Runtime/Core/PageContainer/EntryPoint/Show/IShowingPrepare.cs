using Cysharp.Threading.Tasks;

namespace CizaPageModule
{
	public interface IShowingPrepare
	{
		UniTask ShowingPrepareAsync(params object[] parameters);
	}
}
