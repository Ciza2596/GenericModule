using System.Collections.Generic;

namespace ViewModule
{
    public interface IViewDataOverview
    {
        public Dictionary<string, IView> GetViewTemplates();
    }
}