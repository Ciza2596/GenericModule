using System.Collections.Generic;

namespace CizaInputModule
{
    public static class HashSetExtension
    {
        public static void AddRange(this HashSet<string> strs, string[] addStrs)
        {
            foreach (var addStr in addStrs)
                strs.Add(addStr);
        }
    }
}