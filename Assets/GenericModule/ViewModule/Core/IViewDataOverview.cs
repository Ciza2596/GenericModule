using System.Collections.Generic;

namespace ViewModule
{
    public interface IViewDataOverview
    {
        public string GetViewParentTransformName();
        
        public Dictionary<string, IView> GetViewTemplates();
    }
}