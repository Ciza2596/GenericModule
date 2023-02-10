using System;
using UnityEngine;

namespace SaveLoadModule.Test
{
    [Serializable]
    public class PlayerData
    {
        //private variable
        [SerializeField] private float _hp;
        [ES3Serializable] private SkillData _skillData = new SkillData();


        //public variable
        public float Hp => _hp;

        
        //public method
        public void SetHp(float hp) => _hp = hp;
    }
}