namespace CizaSaveLoadModule
{
	public interface IIo
	{
		bool CheckIsExist(string fullPath);

		string GetFullPath(string directoryPath, string fileOrDirectoryName);

		void CreateDirectory(string fullPath);
	}
}
