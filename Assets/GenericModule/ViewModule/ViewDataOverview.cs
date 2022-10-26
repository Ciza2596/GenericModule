using System;
using System.Collections.Generic;
using UnityEngine;

namespace ViewModule
{
    [CreateAssetMenu(fileName = "ViewDataOverView", menuName = "ViewModule/ViewDataOverview")]
    public class ViewDataOverview : ScriptableObject, IViewDataOverview
    {
        //private variable
        [SerializeField] private string _viewParentTransformName = "ViewParent";
        [Space] [SerializeField] private List<Map> _viewPrefabMaps;

        //public method
        public string GetViewParentTransformName() => _viewParentTransformName;


        public Dictionary<string, IView> GetViewTemplates()
        {
            var viewTemplates = new Dictionary<string, IView>();

            foreach (var viewPrefabMap in _viewPrefabMaps)
                viewTemplates.Add(viewPrefabMap.Key, viewPrefabMap.ViewPrefabs.GetComponent<IView>());


            return viewTemplates;
        }


        [Serializable]
        private class Map
        {
            public string Key;
            public GameObject ViewPrefabs;
        }
    }
}