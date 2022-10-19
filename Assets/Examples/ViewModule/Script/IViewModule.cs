
namespace ViewModule.Example
{
    public interface IViewModule
    {
        public T GetViewComponent<T>(string viewName);

        public bool GetIsVisibleView(string viewName);
        public bool GetIsShowing(string viewName);
        public bool GetIsHiding(string viewName);


        public void LoadView(string viewName, params object[] items);
        public void LoadAllViews();


        public void ReleaseView(string viewName);
        public void ReleaseAllViews();


        public void ShowView(string viewName, params object[] items);
        public void HideView(string viewName);
        
    }
}