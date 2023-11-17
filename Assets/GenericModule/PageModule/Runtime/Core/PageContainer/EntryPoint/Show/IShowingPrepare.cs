using Cysharp.Threading.Tasks;

namespace CizaPageModule
{
	public interface IShowingPrepare
	{
		UniTask OnShowingPrepareAsync(params object[] parameters);
	}
}
