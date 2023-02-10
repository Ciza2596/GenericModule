using System;


namespace SaveLoadModule.Test
{
    [Serializable]
    public class SkillData
    {
        //public variable
        [ES3Serializable] public string Name { get; private set; } = "DefaultAttack";
        [ES3Serializable] public int MaxUseCount { get; } = 10;
        [ES3Serializable] public int UseCount { get; private set; } = 8;
    }
}