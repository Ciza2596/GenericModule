using Cysharp.Threading.Tasks;

namespace CizaPageModule
{
	public interface IInitializable
	{
		UniTask Initialize(params object[] parameters);
	}
}
