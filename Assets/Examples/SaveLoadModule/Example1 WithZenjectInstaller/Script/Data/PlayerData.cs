namespace SaveLoadModule.Example1
{
    public class PlayerData
    {
        //private variable
        [CustomSerializable] private float _hp = 100;
        [CustomSerializable]
        private SkillData _skillData = new SkillData();


        //public variable
        public float Hp => _hp;

        [CustomSerializable] public float Mp { get; private set; } = 50;
        
        
        //public method
        public void SetHp(float hp) => _hp = hp;

        public void SetMp(float mp) => Mp = mp;
    }
}