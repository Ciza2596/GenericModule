using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CizaAchievementModule.Implement
{
    public class AchievementModuleConfig : ScriptableObject, IAchievementModuleConfig
    {
        [SerializeField]
        private string[] _definedDataIds;


        public string[] DefinedDataIds => _definedDataIds != null ? _definedDataIds.ToArray() : Array.Empty<string>();
        
        public IDictionary<string, IAchievementInfo> CreateAchievementInfoMapByAchievementDataId() => throw new NotImplementedException();
    }
}