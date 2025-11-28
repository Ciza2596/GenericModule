namespace CizaInputModule.Editor
{
	public static class StringUtils
	{
		public static bool CheckHasValue(string str) =>
			!string.IsNullOrEmpty(str) && !string.IsNullOrWhiteSpace(str);
	}
}