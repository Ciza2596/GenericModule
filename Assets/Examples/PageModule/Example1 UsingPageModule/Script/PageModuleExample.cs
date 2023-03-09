using PageModule.Implement;
using UnityEngine;

namespace PageModule.Example1
{
    public class PageModuleExample : MonoBehaviour
    {
        //private variable
        [SerializeField] private PageModuleConfig _pageModuleConfig;
        
        //public variable
        public  PageModule PageModule { get; private set; }

        
        //unity callback
        private async void Awake()
        {
            PageModule = new PageModule(_pageModuleConfig);
            PageModule.CreateAll();
            
            await PageModule.Show<TitlePage>();
        }
    }
}
