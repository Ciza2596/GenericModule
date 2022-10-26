

namespace ViewModule
{
    public static class ViewModuleExtension
    {

        public static void ShowViewByFade(this ViewModule viewModule, string fadeViewName, string viewName, params object[] items)
        {
            var length = items.Length + 1;
            var varItems = new object[length];
            varItems[0] = viewName;

            for (int i = 1; i < length; i++)
                varItems[i] = items[i - 1];
            
            viewModule.ShowView(fadeViewName, varItems);
        }

    }
}
