using System.Collections.Generic;
using UnityEngine;

namespace ViewModule
{
    [CreateAssetMenu(fileName = "ViewDataOverView", menuName = "ViewModule/ViewDataOverview")]
    public class ViewDataOverview : ScriptableObject
    {
        [SerializeField] private List<GameObject> _viewPrefabs;


        public ViewDataOverview() { }

        public void InitViewDataOverview(List<GameObject> viewPrefabs) =>
            _viewPrefabs = viewPrefabs;

        public List<GameObject> GetViewPrefabs() => new List<GameObject>(_viewPrefabs);
    }
}