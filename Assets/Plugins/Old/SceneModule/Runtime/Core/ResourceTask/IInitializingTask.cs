namespace CizaSceneModule
{
	public interface IInitializingTask
	{
		bool HasTask { get; }

		bool IsComplete { get; }

		void Execute();
	}
}
