using Cysharp.Threading.Tasks;

namespace CizaPageModule
{
	public interface IShowingStart
	{
		UniTask OnShowingStart(params object[] parameters);
	}
}
