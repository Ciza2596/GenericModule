namespace CizaSaveLoadModule.Example1
{
    public class SkillData
    {
        //public variable
        [CustomSerializable] public string Name { get; private set; } = "DefaultAttack";
        [CustomSerializable] public int MaxUseCount { get; } = 10;
        [CustomSerializable] public int UseCount { get; private set; } = 8;
    }
}