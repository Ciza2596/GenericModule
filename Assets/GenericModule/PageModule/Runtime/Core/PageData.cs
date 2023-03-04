using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PageModule
{
    public class PageData
    {
        //public variable
        public Component Page { get; }
        public GameObject PageGameObject { get; }


        //constructor
        public PageData(Component page)
        {
            Page = page;
            PageGameObject = page.gameObject;
        }
        
        //public method
        public UniTask Initialize(params Object[] parameters)
        {
            return UniTask.CompletedTask;
        }
    }
}