namespace CizaLocaleModule
{
    public static class StringUtils
    {
        public static string GetNotNull(string str) =>
            str ?? string.Empty;

        public static bool CheckHasValue(string str) =>
            !string.IsNullOrEmpty(str) && !string.IsNullOrWhiteSpace(str);
    }
}