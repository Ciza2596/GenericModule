namespace CizaAudioModule
{
    public static class StringExtension
    {
        public static string GetNotNull(this string str) =>
            StringUtils.GetNotNull(str);

        public static bool CheckHasValue(this string str) =>
            StringUtils.CheckHasValue(str);
        
    }
}