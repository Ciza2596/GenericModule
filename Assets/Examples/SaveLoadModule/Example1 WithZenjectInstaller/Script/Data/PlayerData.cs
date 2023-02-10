namespace SaveLoadModule.Example1
{
    public class PlayerData
    {
        //private variable
        [CustomSerializable] private int _hp = 100;
        [CustomSerializable]
        private SkillData _skillData = new SkillData();


        //public variable
        public int Hp => _hp;

        [CustomSerializable] public int Mp { get; private set; } = 50;
        
        
        //public method
        public void SetHp(int hp) => _hp = hp;

        public void SetMp(int mp) => Mp = mp;
    }
}