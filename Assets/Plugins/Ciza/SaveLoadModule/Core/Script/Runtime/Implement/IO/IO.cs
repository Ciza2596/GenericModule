using System.IO;
using UnityEngine.Scripting;

namespace CizaSaveLoadModule.Implement
{
	[Preserve]
	public class Io : IIo
	{
		public bool CheckIsExist(string fullPath) =>
			File.Exists(fullPath);

		// Takes a directory path and a file or directory name and combines them into a single path.
		public string GetFullPath(string directoryPath, string fileOrDirectoryName) =>
			Path.Combine(directoryPath, fileOrDirectoryName);

		public void CreateDirectory(string fullPath)
		{
			var directoryName = Path.GetDirectoryName(fullPath);
			if (string.IsNullOrEmpty(directoryName) || string.IsNullOrWhiteSpace(directoryName))
				return;

			if (Directory.Exists(directoryName))
				return;

			Directory.CreateDirectory(directoryName);
		}

		public void DeleteFile(string fullPath)
		{
			var fileInfo = new FileInfo(fullPath);
			fileInfo.Delete();
		}
	}
}
