using System.Collections.Generic;

namespace CizaInputModule
{
	public static class HashSetUtils
	{
		public static void AddRange<T>(this HashSet<T> objs, T[] addObjs)
		{
			foreach (var addObj in addObjs)
				objs.Add(addObj);
		}
	}
}