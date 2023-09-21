using Cysharp.Threading.Tasks;

namespace CizaPageModule
{
	public interface IShowingStart
	{
		UniTask OnShowingStartAsync(params object[] parameters);
	}
}
