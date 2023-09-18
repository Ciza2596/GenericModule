namespace CizaSceneModule
{
	public interface IInitializingTask
	{
		bool IsComplete { get; }

		void Execute();
	}
}
