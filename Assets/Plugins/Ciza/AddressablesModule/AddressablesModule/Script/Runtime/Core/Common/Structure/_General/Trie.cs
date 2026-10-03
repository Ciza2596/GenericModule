using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CizaAddressablesModule
{
	public class Trie<T> : IEnumerable<Trie<T>>
	{
		public readonly string Id;
		public T Data { get; }

		public Trie<T> Parent { get; private set; }
		public Dictionary<string, Trie<T>> Children { get; }

		// CONSTRUCTOR: ------------------------------------------------------------------------

		public Trie(string id, T data)
		{
			Id = id;
			Data = data;

			Children = new Dictionary<string, Trie<T>>();
		}

		private Trie() : this(string.Empty, default) { }

		// PUBLIC METHOD: ----------------------------------------------------------------------

		public virtual Trie<T> AddChild(Trie<T> item)
		{
			item.Parent?.Children.Remove(item.Id);
			item.Parent = this;

			if (!Children.TryAdd(item.Id, item))
				return null;

			return Children[item.Id];
		}

		public static Trie<T> Create() =>
			new Trie<T>();

		public override string ToString()
		{
			var sb = new StringBuilder();
			BuildString(sb, this, 0);
			return sb.ToString();
		}

		public static string BuildString(Trie<T> trie)
		{
			var sb = new StringBuilder();
			BuildString(sb, trie, 0);
			return sb.ToString();
		}

		public IEnumerator<Trie<T>> GetEnumerator() =>
			Children.Values.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() =>
			GetEnumerator();


		// PRIVATE METHOD: --------------------------------------------------------------------- 

		private static void BuildString(StringBuilder sb, Trie<T> node, int depth)
		{
			sb.AppendLine(node.Id.PadLeft(node.Id.Length + depth));

			foreach (var child in node)
				BuildString(sb, child, depth + 1);
		}
	}
}

