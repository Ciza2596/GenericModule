namespace CizaAchievementModule
{
    public static class StringExtension
    {
        public static bool HasValue(this string str) =>
            !string.IsNullOrEmpty(str) && !string.IsNullOrWhiteSpace(str);
    }
}