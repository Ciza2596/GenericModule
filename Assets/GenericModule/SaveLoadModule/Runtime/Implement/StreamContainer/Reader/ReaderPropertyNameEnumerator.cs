using System.Collections;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace CizaSaveLoadModule.Implement
{
	public class ReaderPropertyNameEnumerator : IEnumerable<string>
	{
		private DataType.IReader _reader;

		[Preserve]
		public ReaderPropertyNameEnumerator(DataType.IReader reader) =>
			_reader = reader;

		IEnumerator<string> IEnumerable<string>.GetEnumerator()
		{
			while (true)
			{
				// Allows us to repeat a property name or insert one of our own.
				if (_reader.OverridePropertyName != null)
				{
					var tempName = _reader.OverridePropertyName;
					_reader.SetOverridePropertyName(null);
					yield return tempName;
				}
				else
				{
					var propertyName = _reader.ReadPropertyName();
					if (propertyName is null)
						yield break;

					yield return propertyName;
				}
			}
		}

		public IEnumerator GetEnumerator() => GetEnumerator();
	}
}
