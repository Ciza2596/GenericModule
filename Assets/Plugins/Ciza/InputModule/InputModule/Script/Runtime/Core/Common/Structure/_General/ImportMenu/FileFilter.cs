using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace CizaInputModule
{
	[Serializable]
	public class FileFilter : IZomeraphyPanel
	{

		[SerializeField]
		private string _extension;

		[SerializeField]
		private string _namePrefix;

		[SerializeField]
		private string _nameSuffix;
		
		public virtual string Extension => $".{_extension}";
		public virtual string NamePrefix => _namePrefix;
		public virtual string NameSuffix => _nameSuffix;

		[Preserve]
		public FileFilter() { }

		[Preserve]
		public FileFilter(string namePrefix, string nameSuffix, string extension)
		{
			_namePrefix = namePrefix ?? string.Empty;
			_nameSuffix = nameSuffix ?? string.Empty;
			_extension = extension ?? string.Empty;
		}
	}
}