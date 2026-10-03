using System;
using UnityEngine.Scripting;

namespace CizaAddressablesModule
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface)]
	public class TitleAttribute : Attribute, ISearchable
	{
		public int Order { get; }
		public string Title { get; }

		[Preserve]
		public TitleAttribute(string title) : this(0, title) { }

		[Preserve]
		public TitleAttribute(int order, string title)
		{
			Order = order;
			Title = title.Trim();
		}

		public override string ToString() =>
			Title;

		public string Text => Title;

		public int Priority => 10;
	}
}

