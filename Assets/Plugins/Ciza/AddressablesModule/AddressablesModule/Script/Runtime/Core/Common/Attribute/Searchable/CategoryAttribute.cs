using System;
using System.Text;
using UnityEngine.Scripting;

namespace CizaAddressablesModule
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
	public class CategoryAttribute : Attribute, ISearchable
	{
		private static readonly char[] SEPARATOR = { '/' };

		public int Order { get; }

		public string Name { get; }
		public string[] Path { get; }

		[Preserve]
		public CategoryAttribute(string title) : this(0, title) { }

		[Preserve]
		public CategoryAttribute(int order, string category)
		{
			Order = order;
			var categories = category.Split(SEPARATOR);
			Name = categories[^1];

			Path = new string[categories.Length - 1];
			for (int i = 0; i < categories.Length - 1; ++i)
				Path[i] = categories[i];
		}

		public override string ToString() =>
			ToString("/");

		public string ToString(string separator)
		{
			var builder = new StringBuilder();
			foreach (var path in Path)
				builder.Append(path).Append(separator);

			builder.Append(Name);
			return builder.ToString();
		}

		public string Text => ToString(" ");
		public int Priority => 8;
	}
}

